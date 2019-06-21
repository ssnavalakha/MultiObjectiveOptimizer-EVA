using System;
using System.Collections.Generic;
using System.Text;

namespace eva.core.framework.framework
{
    /// <summary>
    /// Interface responsible to get a connection to the undelying database
    /// </summary>
    public interface IConnectionRetriever
    {
        /// <summary>
        /// Anonymous function used to get connection string
        /// </summary>
        Func<String> RetrieveConnection { get; set; }
    }
}
