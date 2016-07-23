using System;
using System.Collections.Generic;
using Autofac;
using Olives.WebJob.Interfaces;
using Olives.WebJob.Repositories;

namespace Olives.WebJob
{
    internal class Program
    {
        /// <summary>
        ///     Container which contains injections registered in Main function.
        /// </summary>
        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<RepositoryOlives>().As<IRepositoryOlives>();
            Container = builder.Build();

            // Run cleaning up tasks.
            CleanInvalidAccountTokensAsync();
            HandleInvalidAppointmentsAsync();
            CleanJunkFilesAsync();
        }

        /// <summary>
        ///     This function is for searching and cleaning invalid account tokens.
        /// </summary>
        private static async void CleanInvalidAccountTokensAsync()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                // Retrieve the instance of IRepositoryOlive.
                var repositoryOlives = scope.Resolve<IRepositoryOlives>();

                // Handle all invalid appointment.
                var records = await repositoryOlives.RemoveAllExpiredActivationCode();
                Console.WriteLine("Removed {0} record{1}", records, records > 0 ? "s" : "");
            }
        }

        /// <summary>
        ///     This function is for searching and cleaning invalid appointment.
        /// </summary>
        private static async void HandleInvalidAppointmentsAsync()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                // Retrieve the instance of IRepositoryOlive.
                var repositoryOlives = scope.Resolve<IRepositoryOlives>();

                // Handle all invalid appointment.
                var records = await repositoryOlives.HandleExpiredAppointmentsAsync();
                Console.WriteLine("Removed {0} record{1}", records, records > 0 ? "s" : "");
            }
        }

        /// <summary>
        ///     This function is for cleaning user junk file.
        /// </summary>
        private static async void CleanJunkFilesAsync()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                // Retrieve the instance of IRepositoryOlive.
                var repositoryOlives = scope.Resolve<IRepositoryOlives>();

                // Initialize a list of exception to find whether exception occurs while junk files are removed or not.
                var exceptions = new List<Exception>();

                // Handle all invalid appointment.
                var records = await repositoryOlives.CleanJunkFilesAsync(exceptions);
                Console.WriteLine("Removed {0} record{1}", records, records > 0 ? "s" : "");
            }
        }
    }
}