using System;
using System.Threading.Tasks;
using Olives.Job.Repositories;

namespace Olives.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Executing background tasks of Olives system");
            ExecuteBackgroundTasks().Wait();
        }

        /// <summary>
        /// Background task for handling olives database.
        /// </summary>
        /// <returns></returns>
        private static async Task ExecuteBackgroundTasks()
        {
            var repositoryOlives = new RepositoryOlives();

            // Clean up all expired account tokens.
            await repositoryOlives.FindAndCleanAllExpiredAccountTokens();

            // Clean up all expired accounts.
            await repositoryOlives.FindAndCleanAllExpiredAccounts();

            // Clean up all junk files.
            await repositoryOlives.FindAndCleanJunkFile();

            // Clean up all expired appointments.
            await repositoryOlives.FindAndHandleExpiredAppointments();
        }
    }
}
