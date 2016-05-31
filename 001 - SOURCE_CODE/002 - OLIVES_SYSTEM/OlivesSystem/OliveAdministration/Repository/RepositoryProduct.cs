using System;
using DotnetSignalR.Interfaces;

namespace DotnetSignalR.Repository
{
    public class RepositoryProduct : IRepositoryProduct
    {
        private readonly string[] _names;

        public RepositoryProduct()
        {
            
        }

        public RepositoryProduct(string[] names)
        {
            _names = names;
        }
        
        public string[] GetProducts()
        {
            return new[] {"1", "2", "3", "4"};
        }

        public string[] GetNames()
        {
            return _names;
        }
    }
}