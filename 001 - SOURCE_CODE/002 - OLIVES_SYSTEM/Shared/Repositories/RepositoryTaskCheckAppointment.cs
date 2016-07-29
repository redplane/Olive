using System;
using System.Data.Entity.Migrations;
using System.Threading;
using System.Threading.Tasks;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using System.Linq;
using System.Data.Entity;
using System.Diagnostics;

namespace Shared.Repositories
{
    public class RepositoryTaskCheckAppointment : IRepositoryTaskCheckAppointment
    {
        /// <summary>
        /// Initialize/update a task monitoring.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public async Task<TaskCheckAppointment> InitializeTaskCheckAppointment(TaskCheckAppointment initializer)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Begin a transaction.
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Task monitoringTask = null;

                    // As the time passed. Start the task.
                    if (DateTime.UtcNow > initializer.StartTime)
                    {
                        // Initialize the task.
                        monitoringTask = new Task(() => AppointmentCheckingBackgroundTask(initializer.AppointmentId), cancellationToken);

                        // Initialize the task into sql database.
                        initializer.TaskId = monitoringTask.Id;

                        // Save the task first.
                        context.TaskCheckAppointments.AddOrUpdate(initializer);

                        // Save it to database first.
                        await context.SaveChangesAsync();

                        // Add task to monitor list.
                        ContinuationTaskHelper.Instance.InitializeAppointmentMonitoringTask(initializer.AppointmentId, new TaskDetail(monitoringTask.Id, TaskType.AppointmentMonitor, cancellationTokenSource));
                        
                        // Commit the transaction.
                        transaction.Commit();

                        // Start the task.
                        monitoringTask.Start();
                        
                        Debug.WriteLine($"Appointment [Id: {initializer.AppointmentId}] should be run on Task [Id: {monitoringTask.Id}] at {initializer.StartTime.ToString("F")}");
                        return initializer;
                    }

                    // Task is started in the future. Calculate the timespan when task should be started.
                    var taskTimeSpan = initializer.StartTime - DateTime.UtcNow;

                    // Start the task.
                    monitoringTask = Task.Delay(taskTimeSpan, cancellationToken).ContinueWith((x) => AppointmentCheckingBackgroundTask(initializer.AppointmentId), cancellationToken);

                    // Store the task id first.
                    initializer.TaskId = monitoringTask.Id;

                    // Initialize into sql task.
                    context.TaskCheckAppointments.AddOrUpdate(initializer);

                    // Save the task first.
                    await context.SaveChangesAsync();

                    // Add task to monitor list.
                    ContinuationTaskHelper.Instance.InitializeAppointmentMonitoringTask(initializer.AppointmentId, new TaskDetail(monitoringTask.Id, TaskType.AppointmentMonitor, cancellationTokenSource));

                    Debug.WriteLine($"Appointment [Id: {initializer.AppointmentId}] should be run on Task [Id: {monitoringTask.Id}] at {initializer.StartTime.ToString("F")}");

                    // Commit the transaction.
                    return initializer;
                }
                catch (Exception exception)
                {
                    // As exception happens, rollback the transaction first.
                    transaction.Rollback();

                    // Continue throwing error.
                    throw;
                }
            }
        }

        /// <summary>
        /// Check the database for waiting task and resume 'em all.
        /// This function should be used when web application starts up.
        /// </summary>
        /// <returns></returns>
        public async Task ResumeCheckAppointment()
        {
            // First, clean all pending tasks.
            CleanInCompleteTasks();

            // Database context initialization.
            var context = new OlivesHealthEntities();

            // Make a list of task.
            await context.TaskCheckAppointments.ForEachAsync(x =>
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;

                Task monitoringTask = null;

                // As the time passed. Start the task.
                if (DateTime.UtcNow > x.StartTime)
                {
                    // Initialize the task.
                    monitoringTask = new Task(() => AppointmentCheckingBackgroundTask(x.AppointmentId),
                        cancellationToken);

                    // Initialize the task into sql database.
                    x.TaskId = monitoringTask.Id;

                    // Add task to monitor list.
                    ContinuationTaskHelper.Instance.InitializeAppointmentMonitoringTask(x.AppointmentId,
                        new TaskDetail(monitoringTask.Id, TaskType.AppointmentMonitor, cancellationTokenSource));

                    // Start the task.
                    monitoringTask.Start();
                }

                // Task is started in the future. Calculate the timespan when task should be started.
                var taskTimeSpan = x.StartTime - DateTime.UtcNow;

                // Start the task.
                monitoringTask =
                    Task.Delay(taskTimeSpan, cancellationToken)
                        .ContinueWith((t) => AppointmentCheckingBackgroundTask(x.AppointmentId),
                            cancellationToken);

                // Store the task id first.
                x.TaskId = monitoringTask.Id;

                // Add task to monitor list.
                ContinuationTaskHelper.Instance.InitializeAppointmentMonitoringTask(x.AppointmentId,
                    new TaskDetail(monitoringTask.Id, TaskType.AppointmentMonitor, cancellationTokenSource));

            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appointmentId"></param>
        private async void AppointmentCheckingBackgroundTask(int appointmentId)
        {
            // Change the status of appointments.
            var result = await MonitorAppointment(appointmentId);

            // Find the monitoring task of appointment.
            var monitoringTask = ContinuationTaskHelper.Instance.FindAppointmentMonitoringTaskDetail(appointmentId);

            // Task cannot be found.
            if (monitoringTask == null)
                return;

            // Cancel the task first.
            monitoringTask.CancellationTokenSource.Cancel(false);

            Debug.WriteLine($"Appointment [Id: {appointmentId}] ran on Task [Id: {monitoringTask.Id}] at {DateTime.UtcNow.ToString("F")}");

            // Remove the monitoring task.
            ContinuationTaskHelper.Instance.DeleteMonitoringTask(appointmentId);
        }

        /// <summary>
        /// Check the appointment and modify its state.
        /// </summary>
        private async Task<bool> MonitorAppointment(int appointmentId)
        {
            // Database context initialization.
            var context = new OlivesHealthEntities();

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    #region Appointments modifications

                    // Take all appointments.
                    IQueryable<Appointment> appointments = context.Appointments;

                    // Find the appointment by using id.
                    appointments = appointments.Where(x => x.Id == appointmentId);

                    // Find the appointment with statuses ignore.
                    appointments = appointments.Where(x => x.Status != (byte)StatusAppointment.Expired);
                    appointments = appointments.Where(x => x.Status != (byte)StatusAppointment.Cancelled);
                    appointments = appointments.Where(x => x.Status != (byte)StatusAppointment.Done);

                    await appointments.ForEachAsync(x =>
                    {
                        // Change active appointment to done when time comes.
                        if (x.Status == (byte)StatusAppointment.Active)
                        {
                            x.Status = (byte)StatusAppointment.Done;
                            return;
                        }

                        x.Status = (byte)StatusAppointment.Expired;
                    });

                    #endregion

                    #region Appointment monitoring task deletion

                    // Take all tasks.
                    IQueryable<TaskCheckAppointment> appointmentMonitoringTasks = context.TaskCheckAppointments;

                    // Find the task by using id.
                    appointmentMonitoringTasks = appointmentMonitoringTasks.Where(x => x.AppointmentId == appointmentId);

                    // Remove all task of appointment.
                    context.TaskCheckAppointments.RemoveRange(appointmentMonitoringTasks);

                    #endregion

                    // Save the changes.
                    await context.SaveChangesAsync();

                    // Commit the transaction.
                    transaction.Commit();

                    return true;
                }
                catch (Exception exception)
                {
                    // Rollback the transaction.
                    transaction.Rollback();

                    return false;
                }
            }
        }

        private void CleanInCompleteTasks()
        {
            // Lock the list to prevent another threads invocation.
            lock (ContinuationTaskHelper.Instance.AppointmentMonitoringTasks)
            {
                var keys = ContinuationTaskHelper.Instance.AppointmentMonitoringTasks.Keys;

                if (keys.Count < 0)
                    return;

                foreach (var key in keys)
                {
                    try
                    {
                        // Cancel all pending tasks.
                        ContinuationTaskHelper.Instance.AppointmentMonitoringTasks[key].CancellationTokenSource.Cancel();
                    }
                    catch
                    {
                        continue;
                    }
                }

                // Clear the list.
                ContinuationTaskHelper.Instance.AppointmentMonitoringTasks.Clear();
            }
        }
    }
}