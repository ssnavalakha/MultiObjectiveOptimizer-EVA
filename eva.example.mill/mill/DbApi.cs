using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eva.core.framework.framework;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace eva.example.mill.mill
{
    /// <summary>
    /// The Database API
    /// </summary>
    public class DbApi : IDbApi
    {
        /// <summary>
        /// A way to retrieve connection making framework independant of where the connection is stored)
        /// </summary>
        public IConnectionRetriever ConnectionRetriever { get; set; }

        /// <summary>
        /// The Connection Manager
        /// </summary>
        /// <param name="connectionRetriever">A way to retrieve connection making framework independant of where the connection is stored)</param>
        public DbApi(IConnectionRetriever connectionRetriever)
        {
            ConnectionRetriever = connectionRetriever;
        }
        /// <summary>
        /// Creates a connection to the database
        /// executes a query which fetches data based on a solution id and batchid
        /// </summary>
        /// <param name="solutionid">the solution id of the solution</param>
        /// <param name="batchid">the batch id of the solution</param>
        /// <returns>a string which consists the serialized solution</returns>
        public String FetchSerializedDataFromDb(int solutionid, int batchid)
        {
            string fetchdValue = null;
            // get the connection object (The using statement handles closing of the connection no need to handle it separately)
            using (var dbcon=new DBConnection(ConnectionRetriever).Instance())
            {
                if (dbcon.IsConnect())
                {
                    // create command
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand())
                    {
                        cmd.Connection = dbcon.Connection;
                        // the sql string
                        cmd.CommandText = String.Format(@"select serialized_data from all_solutions
                        where solution_id={0} and batch_id={1}",solutionid,batchid);

                        try
                        {
                            // create the reader and execute
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    fetchdValue = reader.GetValue(0).ToString();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
            }
            // return the fetched serialized value
            return fetchdValue;
        }
        

        /// <summary>
        /// Inserts solutions into the database table all_slutions using a prepared statement
        /// </summary>
        /// <param name="slns"> the solutions to be inserted</param>
        public void insertIntoAllSolutions(IEnumerable<ISolution> slns)
        {
            // get the connection object (The using statement handles closing of the connection no need to handle it separately)
            using (var dbcon=new DBConnection(ConnectionRetriever).Instance())
            {
                if (dbcon.IsConnect())
                {
                    // create the command
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand())
                    {
                        cmd.Connection = dbcon.Connection;
                        // the insert statement
                        cmd.CommandText = @"INSERT INTO all_solutions(solution_id,batch_id,serialized_data) 
                                            VALUES(@slnid,@batchid,@slr)";
                        cmd.Prepare();
                        
                        cmd.Parameters.Add(new MySqlParameter("@slnid",MySqlDbType.Int32));
                        cmd.Parameters.Add(new MySqlParameter("@batchid",MySqlDbType.Int32));
                        cmd.Parameters.Add(new MySqlParameter("@slr",MySqlDbType.JSON));
                        foreach (var sln in slns)
                        {
                            cmd.Parameters["@slnid"].Value = sln.solutionid;
                            cmd.Parameters["@batchid"].Value = sln.batchid;
                            cmd.Parameters["@slr"].Value = JsonConvert.SerializeObject(sln);

                            cmd.ExecuteNonQuery();
                            // returns each object one at a time
                            // the returned object can then be used by accesing the iterator defined in the calling method
                            //yield return sln;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fetches the next available batch id after running a transaction which fetches the avialable batch id
        /// and increments it by one
        /// </summary>
        /// <returns>the new batch id</returns>
        public int getBatchId()
        {
            int fetchedId = -1;
            // get the connection object (The using statement handles closing of the connection no need to handle it separately)
            using (var dbCon = (new DBConnection(ConnectionRetriever)).Instance())
            {
                if (dbCon.IsConnect())
                {
                    // the fetch sql
                    string query = String.Format(@"select value from highlow_uniquekey
                                                   where name='batch_id';");
                    // the update query
                    string query2 = @"update highlow_uniquekey
                                    set value=value+1
                                    where name='batch_id'";
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    var cmd2 = new MySqlCommand(query2, dbCon.Connection);
                    // initialize a transaction 
                    var tran = dbCon.Connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                fetchedId = reader.GetInt32(0);
                            }
                        }
                        cmd2.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            // returned the fetched id
            return fetchedId;
        }
        /// <summary>
        /// Inserts solutions into the database table deleted_solutions using a prepared statement
        /// </summary>
        /// <param name="solutions"> the solutions to be inserted</param>
        public void insertIntoDeletedSolutions(IEnumerable<ISolution> solutions)
        {
            // get the connection object (The using statement handles closing of the connection no need to handle it separately)
            using (var dbcon = new DBConnection(ConnectionRetriever).Instance())
            {
                if (dbcon.IsConnect())
                {
                    using (var cmd = new MySql.Data.MySqlClient.MySqlCommand())
                    {
                        cmd.Connection = dbcon.Connection;
                        cmd.CommandText = "INSERT INTO deleted_solutions(solution_id,batch_id) VALUES(@slnid,@batchid)";
                        cmd.Prepare();
                        cmd.Parameters.Add(new MySqlParameter("@slnid",MySqlDbType.Int32));
                        cmd.Parameters.Add(new MySqlParameter("@batchid",MySqlDbType.Int32));

                        foreach (var sln in solutions)
                        {
                            cmd.Parameters["@slnid"].Value = sln.solutionid;
                            cmd.Parameters["@batchid"].Value =  sln.batchid;

                            cmd.ExecuteNonQuery();
                            // returns each object one at a time
                            // the returned object can then be used by accesing the iterator defined in the calling method
                            //yield return sln;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Joins deleted_solutions and all_solutions to find the non dominated solutions
        /// for a particular batch id 
        /// </summary>
        /// <param name="batchId">the batch id who's non-dominated solutions are to be found</param>
        /// <param name="convertSerializedData"> an anonymous functions which typically converts the serialized data of the solution
        /// fetched into a Isolution Object</param>
        /// <returns>non dominated solutions for that batch id</returns>
        public IEnumerable<ISolution> getSolutionsForABatchId(int batchId,
            Func<String, ISolution> convertSerializedData)
        {
            var solutions = new List<ISolution>();
            // get the connection object (The using statement handles closing of the connection no need to handle it separately)
            using (var dbCon = (new DBConnection(ConnectionRetriever)).Instance())
            {
                if (dbCon.IsConnect())
                {
                    // the sql which joins the two tables
                    string query = String.Format(@"select alsln.solution_id,alsln.serialized_data
                                                   from all_solutions alsln
                                                   left join deleted_solutions dsln on alsln.solution_id=dsln.solution_id
												   and dsln.batch_id=alsln.batch_id
                                                   where alsln.batch_id={0} and dsln.solution_id is null;", batchId);
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // the serialized string is then converted to a deserilaized object
                                solutions.Add(convertSerializedData(reader.GetString("serialized_data")));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
            return solutions;
        }

        /// <summary>
        /// Fetches new available ids
        /// </summary>
        /// <param name="tableName">the table name to fetch ids from</param>
        /// <param name="estimatedRange">the number of ids to fetch</param>
        /// <returns>The first available id and books a slot for you based on the range</returns>
        public int fetchNewAvailableIds(String tableName, int estimatedRange)
        {
            // get connection
            using (var dbCon = (new DBConnection(ConnectionRetriever)).Instance())
            {
                int initialId = -1;
                if (dbCon.IsConnect())
                {
                    // read and update the high low uniqe key in one transaction
                    string query = String.Format(@"select value from highlow_uniquekey
                                                   where name='{0}';", tableName);
                    string query2 = String.Format(@"update highlow_uniquekey
                                                    set value=value+{1}
                                                    where name='{0}'", tableName,estimatedRange);

                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    var cmd2 = new MySqlCommand(query2, dbCon.Connection);
                    using (var tran = dbCon.Connection.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    initialId = reader.GetInt32(0);
                                }
                            }
                            cmd2.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            throw;
                        }
                        
                    }
                }
                return initialId;
            }
        }
    }
}
