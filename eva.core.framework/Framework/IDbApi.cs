using System;
using System.Collections.Generic;

namespace eva.core.framework.framework
{
    /// <summary>
    /// The database Api
    /// </summary>
    public interface IDbApi
    {
        /// <summary>
        /// A way to retrieve connection making framework independant of where the connection is stored)
        /// </summary>
        IConnectionRetriever ConnectionRetriever { get; set; }
        /// <summary>
        /// Inserts solutions into the database table all_slutions using a prepared statement
        /// </summary>
        /// <param name="slns"> the solutions to be inserted</param>
        void insertIntoAllSolutions(IEnumerable<ISolution> slns);
        /// <summary>
        /// Fetches the next available batch id after running a transaction which fetches the avialable batch id
        /// and increments it by one
        /// </summary>
        /// <returns>the new batch id</returns>
        int getBatchId();
        /// <summary>
        /// Inserts solutions into the database table deleted_solutions using a prepared statement
        /// </summary>
        /// <param name="solutions"> the solutions to be inserted</param>
        void insertIntoDeletedSolutions(IEnumerable<ISolution> solutions);
        /// <summary>
        /// Joins deleted_solutions and all_solutions to find the non dominated solutions
        /// for a particular batch id 
        /// </summary>
        /// <param name="batchId">the batch id who's non-dominated solutions are to be found</param>
        /// <param name="convertSerializedData"> an anonymous functions which typically converts the serialized data of the solution
        /// fetched into a Isolution Object</param>
        /// <returns>the non dominated solutions for that batch id</returns>
        IEnumerable<ISolution> getSolutionsForABatchId(int batchId, Func<String, ISolution> convertSerializedData);
        /// <summary>
        /// Creates a connection to the database
        /// executes a query which fetches data based on a solution id and batchid
        /// </summary>
        /// <param name="solutionid">the solution id of the solution</param>
        /// <param name="batchid">the batch id of the solution</param>
        /// <returns>a string whic consists the serialized solution</returns>
        String FetchSerializedDataFromDb(int solutionid, int batchid);

        /// <summary>
        /// Fetches new available ids
        /// </summary>
        /// <param name="tableName">the table name to fetch ids from</param>
        /// <param name="estimatedRange">the number of ids to fetch</param>
        /// <returns>The first available id and books a slot for you based on the range</returns>
        int fetchNewAvailableIds(String tableName, int estimatedRange);
    }
}
