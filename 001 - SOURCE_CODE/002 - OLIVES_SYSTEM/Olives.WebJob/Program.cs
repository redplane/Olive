using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Shared.Models;

namespace Olives.WebJob
{
    class Program
    {
        private static OlivesHealthEntities _context = new OlivesHealthEntities();

        static void Main(string[] args)
        {
            var container = new ContainerBuilder();
            

            Console.WriteLine("Web job has been started");

            
        }
        
    }
}
