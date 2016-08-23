using System;
using System.Threading.Tasks;
using Olives.WebJob.Repositories;

namespace Olives.WebJob
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Executing background tasks of Olives system");
            ExecuteBackgroundTasks().Wait();
        }

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