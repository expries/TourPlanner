using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql;
using TourPlanner.Domain;

namespace TourPlanner.DAL
{
    /// <summary>
    /// Performs sql statements on postgres database and performs mapping of query results to entities.
    /// </summary>
    public class DatabaseConnection
    {
        private NpgsqlConnection Connection { get; }
        
        private string ConnectionString { get; set; }
        
        private NpgsqlDataReader _reader;

        public DatabaseConnection(string connectionString)
        {
            this.Connection = new NpgsqlConnection(connectionString);
            this.Connection.Open();
            this.ConnectionString = connectionString;
        }
        
        /// <summary>
        /// Perform non query statement. Returns the number of affected rows
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(string sql)
        {
            var cmd = new NpgsqlCommand(sql);
            return Execute(cmd);
        }

        /// <summary>
        /// Perform non query statement. Returns the number of affected rows
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public int Execute(string sql, object dataObject)
        {
            var cmd = BuildCommand(sql, dataObject);
            return Execute(cmd);
        }

        /// <summary>
        /// Perform non query statement. Returns the number of affected rows
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int Execute(NpgsqlCommand cmd)
        {
            cmd.Connection = Connection;
            return cmd.ExecuteNonQuery();
        }
        
        /// <summary>
        /// Map result rows for a given query to a list of entities.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Query<T>(string sql, int limit = 100) where T : new()
        {
            var cmd = new NpgsqlCommand(sql);
            return Query<T>(cmd, limit);
        }

        /// <summary>
        /// Map result rows for a given query to a list of entities.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Query<T>(string sql, object dataObject, int limit = 100) where T : new()
        {
            var cmd = BuildCommand(sql, dataObject);
            return Query<T>(cmd, limit);
        }

        /// <summary>
        /// Map result rows for a given query to a list of entities
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Query<T>(NpgsqlCommand cmd, int limit = 100) where T : new()
        {
            cmd.Connection = this.Connection;
            Console.WriteLine(cmd.CommandText);
            this._reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
            var records = GetRecords<T>(limit);
            this._reader.Close();
            return records;
        }
        
        /// <summary>
        /// Map first result rows for a given query to an entity
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql, int limit = 100) where T : new()
        {
            var cmd = new NpgsqlCommand(sql);
            return QueryFirstOrDefault<T>(cmd, limit);
        }
        
        /// <summary>
        /// Map first result rows for a given query to an entity
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql, object dataObject, int limit = 100) where T : new()
        {
            var cmd = BuildCommand(sql, dataObject);
            return QueryFirstOrDefault<T>(cmd, limit);
        }

        /// <summary>
        /// Map first result rows for a given query to an entity
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(NpgsqlCommand cmd, int limit = 100) where T : new()
        {
            cmd.Connection = this.Connection;
            Console.WriteLine(cmd.CommandText);
            this._reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
            var record = GetNextRecord<T>(limit);
            this._reader.Close();
            return record;
        }

        /// <summary>
        /// Get all the rows for the current query up to a specified limit
        /// </summary>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private List<T> GetRecords<T>(int limit) where T : new()
        {
            var records = new List<T>();
            var record = GetNextRecord<T>(limit);

            while (records.Count < limit && record is not null)
            {
                records.Add(record);
                record = GetNextRecord<T>(limit);
            }

            return records;
        }

        /// <summary>
        /// Map next result row of this query to an entity
        /// </summary>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetNextRecord<T>(int limit) where T : new()
        {
            this._reader.Read();
            var record = new T();
            
            // return default object if the result-set is empty
            if (!this._reader.IsOnRow)
            {
                return default;
            }

            // read current result row
            var currentRow = GetRow();
            var properties = typeof(T).GetProperties().Where(x => x.CanWrite);

            // map every property based on the attributes it holds
            foreach (var property in properties)
            {
                var column = property.GetCustomAttribute(typeof(ColumnAttribute), true);

                if (column is ColumnAttribute columnAttribute)
                {
                    MapColumn(record, property, columnAttribute, currentRow);
                }
                
                var oneToMany = property.GetCustomAttribute(typeof(OneToManyAttribute), true);

                if (oneToMany is OneToManyAttribute oneToManyAttribute)
                {
                    MapOneToMany(record, property, oneToManyAttribute, limit);
                }
                
                var manyToOne = property.GetCustomAttribute(typeof(ManyToOneAttribute), true);

                if (manyToOne is ManyToOneAttribute manyToOneAttribute)
                {
                    MapManyToOne(record, property, manyToOneAttribute, limit);
                }
            }

            return record;
        }

        /// <summary>
        /// Map column of database row to property with column attribute
        /// </summary>
        /// <param name="record"></param>
        /// <param name="property"></param>
        /// <param name="column"></param>
        /// <param name="currentRow"></param>
        /// <typeparam name="T"></typeparam>
        private void MapColumn<T>(T record, 
                                  PropertyInfo property, 
                                  ColumnAttribute column, 
                                  Dictionary<string, object> currentRow)
        {
            string columnName = column.Name.ToLower();

            if (currentRow.ContainsKey(columnName))
            {
                property.SetValue(record, currentRow[columnName]);
            }

        }
        
        /// <summary>
        /// Find all database rows for a foreign key property that has a OneToMany-Attribute placed on it.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="property"></param>
        /// <param name="oneToMany"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        private void MapOneToMany<T>(T record, PropertyInfo property, OneToManyAttribute oneToMany, int limit)
        {
            // find entity type to map to (should be nested in a list) 
            var nestedType = property?.PropertyType.GenericTypeArguments.FirstOrDefault();

            if (nestedType is null)
            {
                return;
            }

            bool lazy = property?.PropertyType.AssemblyQualifiedName?.StartsWith("System.Lazy") ?? false;

            if (lazy)
            {
                nestedType = nestedType.GenericTypeArguments.FirstOrDefault() ?? nestedType;
            }

            // find the primary key column
            var schema = this._reader.GetColumnSchema();
            var keyColumn = schema.FirstOrDefault(x => x?.IsKey ?? false);

            if (keyColumn?.ColumnOrdinal is null)
            {
                return;
            }

            int keyColumnOrdinal = (int) keyColumn.ColumnOrdinal;
            var rowKey = this._reader.GetValue(keyColumnOrdinal);
            
            // build query to find foreign key entries
            string sql = $"SELECT * FROM {oneToMany.Table} WHERE {oneToMany.ForeignKey} = @key";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("key", rowKey);
            
            object result;

            // if using lazy list => create lazy list that holds query-method
            if (lazy)
            {
                const BindingFlags methodFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var lazyListMethod = GetType().GetMethod(nameof(BuildLazyList), methodFlags);
                var lazyListMethodInfo = lazyListMethod?.MakeGenericMethod(nestedType);
                result = lazyListMethodInfo?.Invoke(this, new object[] { cmd, limit });
            }
            // if using "normal" list => perform query immediately and update list with rows
            else
            {
                var queryParameterTypes = new Type[] { typeof(NpgsqlCommand), typeof(int) };
                var queryMethod = GetType().GetMethod(nameof(Query), queryParameterTypes);
                var queryMethodInfo = queryMethod?.MakeGenericMethod(nestedType);
                result = queryMethodInfo?.Invoke(Copy(), new object[] { cmd, limit });
            }

            property.SetValue(record, result);
        }
        
        /// <summary>
        /// Find matching database row for a foreign key property that has a ManyToOne-Attribute placed on it.
        /// </summary>
        /// <param name="record"></param>
        /// <param name="property"></param>
        /// <param name="manyToOne"></param>
        /// <typeparam name="T"></typeparam>
        private void MapManyToOne<T>(T record, PropertyInfo property, ManyToOneAttribute manyToOne, int limit)
        {
            // find entity type to map to (should be nested in a list) 
            var type = property.PropertyType;
            bool lazy = property?.PropertyType.AssemblyQualifiedName?.StartsWith("System.Lazy") ?? false;

            if (lazy)
            {
                type = type.GenericTypeArguments.FirstOrDefault() ?? type;
            }
            
            // find foreign key column
            var columnSchema = this._reader.GetColumnSchema();
            var foreignKeyColumn = columnSchema.FirstOrDefault(x =>
            {
                return string.Equals(x.ColumnName, manyToOne.ForeignKey, StringComparison.CurrentCultureIgnoreCase);
            });

            if (foreignKeyColumn?.ColumnOrdinal is null)
            {
                return;
            }

            int keyColumnOrdinal = (int) foreignKeyColumn.ColumnOrdinal;
            var foreignKey = this._reader.GetValue(keyColumnOrdinal);
            
            // build query to find matching entry in other table
            string sql = $"SELECT * FROM {manyToOne.Table} WHERE {manyToOne.PrimaryKey} = @key";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("key", foreignKey);

            object result;
            
            // if using lazy list => create lazy list that holds query-method
            if (lazy)
            {
                const BindingFlags methodFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var lazyListMethod = GetType().GetMethod(nameof(BuildLazyEntity), methodFlags);
                var lazyListMethodInfo = lazyListMethod?.MakeGenericMethod(type);
                result = lazyListMethodInfo?.Invoke(this, new object[] { cmd, limit });
            }
            // if using "normal" list => perform query immediately and update list with rows
            else
            {
                var queryParameterTypes = new Type[] { typeof(NpgsqlCommand), typeof(int) };
                var queryMethod = GetType().GetMethod(nameof(QueryFirstOrDefault), queryParameterTypes);
                var queryMethodInfo = queryMethod?.MakeGenericMethod(type);
                result = queryMethodInfo?.Invoke(Copy(), new object[] { cmd, limit });
            }
            
            property.SetValue(record, result);
        }

        /// <summary>
        /// Returns dictionary that holds the table's column names as keys and the current row values as values
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetRow()
        {
            var columnSchema = this._reader.GetColumnSchema();
            var row = new Dictionary<string, object>();

            foreach (var column in columnSchema)
            {
                if (column.ColumnOrdinal is null)
                {
                    continue;
                }

                int ordinal = column.ColumnOrdinal.Value;
                var value = this._reader.GetValue(ordinal);
                string columnName = column.ColumnName.ToLower();
                row[columnName] = value;
            }

            return row;
        }

        /// <summary>
        /// Creates a NpgsqlCommand with a given sql statement and provides all the values in the
        /// anonymous data object as parameters for the sql statement.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        private static NpgsqlCommand BuildCommand(string sql, object dataObject)
        {
            var cmd = new NpgsqlCommand(sql);
            var readableProperties = dataObject.GetType()
                .GetProperties()
                .Where(property => property.CanRead);
            
            foreach (var property in readableProperties)
            {
                var value = property.GetValue(dataObject);
                cmd.Parameters.AddWithValue(property.Name, value ?? DBNull.Value);
            }

            return cmd;
        }
        
        /// <summary>
        /// Provide function to perform query to a lazy entity. The lazy entity will only perform the query
        /// once its values are accessed.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Lazy<T> BuildLazyEntity<T>(NpgsqlCommand cmd, int limit) where T : new()
        {
            return new Lazy<T>(() =>
            {
                var dbManager = Copy();
                var entity = dbManager.QueryFirstOrDefault<T>(cmd, limit);
                return entity;
            });
        }
        
        /// <summary>
        /// Provide function to perform a query to a lazy list. The lazy list will only perform the query
        /// once its values are accessed.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Lazy<List<T>> BuildLazyList<T>(NpgsqlCommand cmd, int limit) where T : new()
        {
            return new Lazy<List<T>>(() =>
            {
                var dbManager = Copy();
                var list = dbManager.Query<T>(cmd, limit);
                return list;
            });
        }

        /// <summary>
        /// Create new database manager with the same connection info as the current one
        /// </summary>
        /// <returns></returns>
        private DatabaseConnection Copy()
        {
            return new DatabaseConnection(this.ConnectionString);
        }
    }
}