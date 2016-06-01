using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Shared.Interfaces;
using Shared.ViewModels;

namespace DotnetSignalR.Repository
{
    public class RepositoryAccount : IRepositoryAccount
    {
        /// <summary>
        /// Instance which connects to neo4j database.
        /// </summary>
        private readonly GraphClient _graphClient;

        /// <summary>
        /// Initialize an instance of RepositoryAccount class.
        /// </summary>
        /// <param name="graphClient"></param>
        public RepositoryAccount(GraphClient graphClient)
        {
            _graphClient = graphClient;
        }
        
        /// <summary>
        /// Log user in 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<IPerson> Login(LoginViewModel info)
        {
            try
            {
                // Query construction.
                var query = await _graphClient.Cypher.Match("(admin:Person)")
                    .Where<IPerson>(admin => admin.Email == info.Email)
                    .AndWhere<IPerson>(admin => admin.Password == info.Password)
                    .Return(admin => admin.As<IPerson>())
                    .Limit(1)
                    .ResultsAsync;

                // Invalid query result.
                var result = query.FirstOrDefault();
                if (result == null)
                    return null;

                result.Password = "";
                result.Id = "";

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}