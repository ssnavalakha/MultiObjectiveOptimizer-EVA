using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;
using MySql.Data.MySqlClient;

namespace eva.example.mill.mill
{
   /// <summary>
    /// The Connection Manager
    /// </summary>
    public class DBConnection :IDisposable
    {
        /// <summary>
        /// A way to retrieve connection making framework independant of where the connection is stored)
        /// </summary>
        private IConnectionRetriever ConnectionRetriever { get; set; }

        /// <summary>
        /// The Connection Manager
        /// </summary>
        /// <param name="retreiveCon">A way to retrieve connection making framework independant of where the connection is stored)</param>
        public DBConnection(IConnectionRetriever retreiveCon)
        {
            ConnectionRetriever = retreiveCon;
        }
        /// <summary>
        /// the MYSQL connection Object 
        /// </summary>
        private MySqlConnection connection = null;
        /// <summary>
        /// the MYSQL connection Object 
        /// </summary>
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        /// <summary>
        /// The DbConnection Instance
        /// </summary>
        private DBConnection _instance = null;
        /// <summary>
        /// Returns an instace of DB Connection
        /// </summary>
        /// <returns>returns an instance of DB Connection</returns>
        public DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection(ConnectionRetriever);
            return _instance;
        }
        /// <summary>
        /// Checks if connection is already established else creates a new connection to the database and returns true
        /// </summary>
        /// <returns>true</returns>
        public bool IsConnect()
        {
            if (Connection == null)
            {
                // The connection string
                string connstring = ConnectionRetriever.RetrieveConnection();
                connection = new MySqlConnection(connstring);
                connection.Open();
            }

            return true;
        }

        /// <summary>
        /// closes the underlying connection object
        /// </summary>
        public void Close()
        {
            connection.Close();
        }

        /// <summary>
        /// Disposes the connection object 
        /// </summary>
        public void Dispose()
        {
            connection.Close();
        }
    }
}
