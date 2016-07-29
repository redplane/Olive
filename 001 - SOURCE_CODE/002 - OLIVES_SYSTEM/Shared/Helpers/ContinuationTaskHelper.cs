using System.Collections.Generic;
using Shared.Models;

namespace Shared.Helpers
{
    public class ContinuationTaskHelper
    {
        #region Properties

        /// <summary>
        /// Static instance which provides access to all public functions inside class.
        /// </summary>
        private static ContinuationTaskHelper _instance;

        /// <summary>
        /// Retrieve the static instance of class. 
        /// A new instance will be initialized as it is not available.
        /// </summary>
        public static ContinuationTaskHelper Instance
        {
            get { return _instance ?? (_instance = new ContinuationTaskHelper()); }
        }

        /// <summary>
        /// List of task with detailed information.
        /// </summary>
        public IDictionary<int, TaskDetail> AppointmentMonitoringTasks { get; set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Initialize an instance with default settings.
        /// </summary>
        public ContinuationTaskHelper()
        {
            // Initialize a list of detailed tasks.
            AppointmentMonitoringTasks = new Dictionary<int, TaskDetail>();
        }

        #endregion

        #region Methods

        public void InitializeAppointmentMonitoringTask(int appointmentId, TaskDetail detail)
        {
            // Retrieve the previous monitoring task.
            var monitoringTask = AppointmentMonitoringTasks.ContainsKey(appointmentId) ? AppointmentMonitoringTasks[appointmentId] : null;

            // Monitoring task hasn't been initialized before.
            if (monitoringTask == null)
            {
                AppointmentMonitoringTasks[appointmentId] = detail;
                return;
            }

            // It has been initialized before, stop the previous.
            monitoringTask.CancellationTokenSource.Cancel();
            
            // Remove the previous.
            AppointmentMonitoringTasks.Remove(appointmentId);

            // Add the new one.
            AppointmentMonitoringTasks.Add(appointmentId, detail);
        }

        public void DeleteMonitoringTask(int appointment)
        {
            try
            {
                lock (AppointmentMonitoringTasks)
                {
                    AppointmentMonitoringTasks.Remove(appointment);
                }
            }
            catch
            {
                
            }
        }

        public TaskDetail FindAppointmentMonitoringTaskDetail(int appointment)
        {
            if (AppointmentMonitoringTasks.ContainsKey(appointment))
                return null;

            return AppointmentMonitoringTasks[appointment];
        }

        #endregion
    }
}