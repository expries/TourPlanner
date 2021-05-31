using System.Collections.Generic;
using Npgsql;

namespace TourPlanner.DAL
{
    /// <summary>
    /// Performs sql statements on postgres database and performs mapping of query results to entities.
    /// </summary>
    public interface IDatabaseConnection
    {
        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql);

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object dataObject);

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public object ExecuteScalar(NpgsqlCommand cmd);

        /// <summary>
        /// Perform non query statement. Returns the number of affected rows
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(string sql);

        /// <summary>
        /// Perform non query statement. Returns the number of affected rows
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public int Execute(string sql, object dataObject);

        /// <summary>
        /// Perform non query statement. Returns the number of affected rows
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int Execute(NpgsqlCommand cmd);

        /// <summary>
        /// Perform query and Map result rows to a list of entities.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Query<T>(string sql, int limit = 100) where T : new();

        /// <summary>
        /// Perform query and Map result rows to a list of entities.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Query<T>(string sql, object dataObject, int limit = 100) where T : new();

        /// <summary>
        /// Perform query and Map result rows to a list of entities.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Query<T>(NpgsqlCommand cmd, int limit = 100) where T : new();

        /// <summary>
        /// Perform query and map first row in the result set to an entity.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql, int limit = 100) where T : new();

        /// <summary>
        /// Perform query and map first row in the result set to an entity.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql, object dataObject, int limit = 100) where T : new();

        /// <summary>
        /// Perform query and map first row in the result set to an entity.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(NpgsqlCommand cmd, int limit = 100) where T : new();
    }
}