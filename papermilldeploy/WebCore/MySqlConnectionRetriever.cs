using eva.core.framework.framework;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace papermilldeploy.WebCore
{
    /// <summary>
    /// Class responsible to fetch connection string from the web config
    /// </summary>
    public class MySqlConnectionRetriever : IConnectionRetriever
    {
        private IConfiguration _iconfiguration;
        public Func<string> RetrieveConnection { get; set; }

        public MySqlConnectionRetriever(IConfiguration configuration)
        {
            _iconfiguration = configuration;
            RetrieveConnection = () => readConnectionFromWebConfig();
        }

        /// <summary>
        /// reads and returns the connection string
        /// </summary>
        /// <returns>the connection string</returns>
        private String readConnectionFromWebConfig()
        {
            return _iconfiguration.GetSection("Data").GetSection("ConnectionString").Value;
        }
    }
}
