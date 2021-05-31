using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql;
using TourPlanner.Domain;

namespace TourPlanner.DAL
{
    
    public class DatabaseConnection : IDatabaseConnection
    {
        private static readonly log4net.ILog Log = 
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);

        private NpgsqlConnection Connection { get; set; }
        
        private string ConnectionString { get; set; }
        
        private NpgsqlDataReader _reader;

        public DatabaseConnection(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Map enum type to name of enum type of database
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="TEnum"></typeparam>
        public static void MapEnum<TEnum>(string name) where TEnum : struct, Enum
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<TEnum>(name);
        }
        
        public object ExecuteScalar(string sql)
        {
            var cmd = new NpgsqlCommand(sql);
            return ExecuteScalar(cmd);
        }
        
        public object ExecuteScalar(string sql, object dataObject)
        {
            var cmd = BuildCommand(sql, dataObject);
            return ExecuteScalar(cmd);
        }
        
        public object ExecuteScalar(NpgsqlCommand cmd)
        {
            this.Connection = new NpgsqlConnection(this.ConnectionString);
            this.Connection.Open();

            Log.Debug("Executing SQL-Command: " + cmd.CommandText);
            cmd.Connection = this.Connection;
            var result = cmd.ExecuteScalar();

            this.Connection.Close();
            return result;
        }
        
        public int Execute(string sql)
        {
            var cmd = new NpgsqlCommand(sql);
            return Execute(cmd);
        }
        
        public int Execute(string sql, object dataObject)
        {
            var cmd = BuildCommand(sql, dataObject);
            return Execute(cmd);
        }
        
        public int Execute(NpgsqlCommand cmd)
        {
            this.Connection = new NpgsqlConnection(this.ConnectionString);
            this.Connection.Open();

            Log.Debug("Executing SQL-Command: " + cmd.CommandText);
            cmd.Connection = this.Connection;
            int result = cmd.ExecuteNonQuery();

            this.Connection.Close();
            return result;
        }
        
        public List<T> Query<T>(string sql, int limit = 100) where T : new()
        {
            var cmd = new NpgsqlCommand(sql);
            return Query<T>(cmd, limit);
        }
        
        public List<T> Query<T>(string sql, object dataObject, int limit = 100) where T : new()
        {
            var cmd = BuildCommand(sql, dataObject);
            return Query<T>(cmd, limit);
        }
        
        public List<T> Query<T>(NpgsqlCommand cmd, int limit = 100) where T : new()
        {
            this.Connection = new NpgsqlConnection(this.ConnectionString);
            this.Connection.Open();
            
            Log.Debug("Executing SQL-Query: " + cmd.CommandText);
            cmd.Connection = this.Connection;
            this._reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
            var records = GetRecords<T>(limit);

            this._reader.Close();
            this.Connection.Close();
            return records;
        }
        
        public T QueryFirstOrDefault<T>(string sql, int limit = 100) where T : new()
        {
            var cmd = new NpgsqlCommand(sql);
            var result = QueryFirstOrDefault<T>(cmd, limit);
            return result;

        }
        
        public T QueryFirstOrDefault<T>(string sql, object dataObject, int limit = 100) where T : new()
        {
            var cmd = BuildCommand(sql, dataObject);
            return QueryFirstOrDefault<T>(cmd, limit);
        }
        
        public T QueryFirstOrDefault<T>(NpgsqlCommand cmd, int limit = 100) where T : new()
        {
            this.Connection = new NpgsqlConnection(this.ConnectionString);
            this.Connection.Open();
            
            Log.Debug("Executing SQL-Query: " + cmd.CommandText);
            cmd.Connection = this.Connection;
            this._reader = cmd.ExecuteReader(CommandBehavior.KeyInfo);
            var record = GetNextRecord<T>(limit);

            this._reader.Close();
            this.Connection.Close();
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
                var value = currentRow[columnName] is DBNull ? default : currentRow[columnName];
                property.SetValue(record, value);
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
            // find nested type of list
            var type = property?.PropertyType.GenericTypeArguments.FirstOrDefault();

            if (type is null)
            {
                return;
            }

            // if lazy find nested type of lazy
            bool lazy = property.PropertyType.AssemblyQualifiedName?.StartsWith("System.Lazy") ?? false;

            if (lazy)
            {
                type = type.GenericTypeArguments.FirstOrDefault() ?? type;
            }
            

            // get the primary key
            object primaryKey = GetPrimaryKey();

            if (primaryKey is null)
            {
                return;
            }

            // build query to find foreign key entries
            string sql = $"SELECT * FROM {oneToMany.Table} WHERE {oneToMany.ForeignKey} = @key";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("key", primaryKey);
            
            // if using lazy list => create lazy list that holds query-method
            // if using non-lazy list => perform query immediately and update list with rows
            object result = lazy 
                ? GetLazyQuery(type, nameof(BuildLazyList), cmd, limit) 
                : ExecuteQuery(type, nameof(Query), cmd, limit);

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
            // if lazy find nested type of lazy
            var type = property.PropertyType;
            bool lazy = property.PropertyType.AssemblyQualifiedName?.StartsWith("System.Lazy") ?? false;

            if (lazy)
            {
                type = type.GenericTypeArguments.FirstOrDefault() ?? type;
            }
            
            // get the foreign key
            object foreignKey = GetForeignKey(manyToOne);
            
            if (foreignKey is null)
            {
                return;
            }

            // build query to find matching entry in other table
            string sql = $"SELECT * FROM {manyToOne.Table} WHERE {manyToOne.PrimaryKey} = @key";
            var cmd = new NpgsqlCommand(sql);
            cmd.Parameters.AddWithValue("key", foreignKey);
            
            // if using lazy list => create lazy list that holds query-method
            // if using non-lazy list => perform query immediately and update list with rows
            object result = lazy 
                ? GetLazyQuery(type, nameof(BuildLazyEntity), cmd, limit) 
                : ExecuteQuery(type, nameof(QueryFirstOrDefault), cmd, limit);
            
            property.SetValue(record, result);
        }
        
        /// <summary>
        /// Returns value of primary key for current row
        /// </summary>
        /// <returns></returns>
        private object GetPrimaryKey()
        {
            var schema = this._reader.GetColumnSchema();
            var keyColumn = schema.FirstOrDefault(x => x?.IsKey ?? false);

            if (keyColumn?.ColumnOrdinal is null)
            {
                return null;
            }

            int keyColumnOrdinal = (int) keyColumn.ColumnOrdinal;
            object primaryKey = this._reader.GetValue(keyColumnOrdinal);
            return primaryKey;
        }

        /// <summary>
        /// Returns value of foreign key of  for current row
        /// </summary>
        /// <param name="manyToOne"></param>
        /// <returns></returns>
        private object GetForeignKey(ManyToOneAttribute manyToOne)
        {
            var columnSchema = this._reader.GetColumnSchema();
            var foreignKeyColumn = columnSchema.FirstOrDefault(x => 
                string.Equals(x.ColumnName, manyToOne.ForeignKey, StringComparison.CurrentCultureIgnoreCase));

            if (foreignKeyColumn?.ColumnOrdinal is null)
            {
                return null;
            }

            int keyColumnOrdinal = (int) foreignKeyColumn.ColumnOrdinal;
            object foreignKey = this._reader.GetValue(keyColumnOrdinal);
            return foreignKey;
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
                object value = this._reader.GetValue(ordinal);
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
        /// Get a lazy query for type to be executed later on
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private object GetLazyQuery(Type type, string methodName, NpgsqlCommand cmd, int limit)
        {
            const BindingFlags methodFlags = BindingFlags.NonPublic | BindingFlags.Instance;
            var lazyListMethod = GetType().GetMethod(methodName, methodFlags);
            var lazyListMethodInfo = lazyListMethod?.MakeGenericMethod(type);
            return lazyListMethodInfo?.Invoke(this, new object[] { cmd, limit });
        }

        /// <summary>
        /// Execute query for type and return results
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="cmd"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private object ExecuteQuery(Type type, string methodName, NpgsqlCommand cmd, int limit)
        {
            var queryParameterTypes = new Type[] { typeof(NpgsqlCommand), typeof(int) };
            var queryMethod = GetType().GetMethod(methodName, queryParameterTypes);
            var queryMethodInfo = queryMethod?.MakeGenericMethod(type);
            return queryMethodInfo?.Invoke(Copy(), new object[] { cmd, limit });
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
            return new Lazy<T>(() => Copy().QueryFirstOrDefault<T>(cmd, limit));
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
            return new Lazy<List<T>>(() => Copy().Query<T>(cmd, limit));
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