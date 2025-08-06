using System.Data;
using System.Reflection;

namespace VSLibrary.Database;

/// <summary>
/// An abstract base class for database providers that implements <see cref="DBInterface"/>.
/// It provides common functionality and default implementations for standard SQL builders.
/// </summary>
public abstract class DBBase : DBInterface
{
    protected string _connectionString;
    protected bool isConnected = false;

    public DBBase(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    // Abstract members to be implemented by concrete providers
    public abstract DatabaseProvider GetDatabaseProvider();
    public virtual bool IsConnected() => isConnected;
    public abstract void ConnDb();
    public abstract void CloseDbConn();
    public abstract void BeginTransaction();
    public abstract void CommitTransaction();
    public abstract void RollbackTransaction();
    public abstract Task<DataTable> GetDataTableAsync(string query, params IDbDataParameter[] parameters);
    public abstract Task<IEnumerable<T>> QueryAsync<T>(string query, params IDbDataParameter[] parameters) where T : new();
    public abstract Task<object> ExecuteScalarAsync(string query, params IDbDataParameter[] parameters);
    public abstract Task<int> ExecuteNonQueryAsync(string query, params IDbDataParameter[] parameters);
    public abstract IDbDataParameter CreateParameter(string parameterName, object value);

    // Virtual SQL builders with default implementations for standard SQL
    public virtual string BuildInsertSql<T>(IEnumerable<PropertyInfo> properties) where T : new()
    {
        var type = typeof(T);
        var colNames = string.Join(", ", properties.Select(p => p.Name));
        var valPlaceholders = string.Join(", ", properties.Select(p => "@" + p.Name));
        return $"INSERT INTO {type.Name} ({colNames}) VALUES ({valPlaceholders})";
    }

    public virtual string BuildUpdateSql<T>(PropertyInfo pkProperty, IEnumerable<PropertyInfo> propsToUpdate) where T : new()
    {
        var type = typeof(T);
        var setClauses = string.Join(", ", propsToUpdate.Select(p => $"{p.Name} = @{p.Name}"));
        return $"UPDATE {type.Name} SET {setClauses} WHERE {pkProperty.Name} = @{pkProperty.Name}";
    }

    public virtual string BuildDeleteSql<T>(PropertyInfo pkProperty) where T : new()
    {
        return $"DELETE FROM {typeof(T).Name} WHERE {pkProperty.Name} = @{pkProperty.Name}";
    }

    public virtual string BuildSelectByPkSql<T>(PropertyInfo pkProperty) where T : new()
    {
        return $"SELECT * FROM {typeof(T).Name} WHERE {pkProperty.Name} = @{pkProperty.Name}";
    }

    public virtual string BuildSelectAllSql<T>() where T : new()
    {
        return $"SELECT * FROM {typeof(T).Name}";
    }

    // IDisposable Implementation
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            CloseDbConn();
        }
    }
}

/// <summary>
/// A generic repository for dynamic database access using DBManager.
/// Provides basic CRUD operations and dynamic table creation based on type T.
/// </summary>
/// <typeparam name="T">Model type to be handled by this repository</typeparam>
public class DynamicRepository<T> : IDynamicRepository<T> where T : class, new()
{
    private readonly DBManager _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicRepository{T}"/> class.
    /// </summary>
    /// <param name="db">The database manager instance to operate with</param>
    public DynamicRepository(DBManager db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves a single item by its primary key asynchronously.
    /// </summary>
    /// <param name="id">The primary key of the item</param>
    /// <returns>The item if found, otherwise null</returns>
    public async Task<T?> GetByIdAsync(object id)
        => await _db.SelectByPkAsync<T>(id);

    /// <summary>
    /// Retrieves all items from the table asynchronously.
    /// </summary>
    /// <returns>All items as an enumerable</returns>
    public async Task<IEnumerable<T>> GetAllAsync()
        => await _db.SelectAllAsync<T>();

    /// <summary>
    /// Inserts an item into the database.
    /// </summary>
    /// <param name="item">The item to insert</param>
    /// <returns>True if the insert was successful</returns>
    public async Task<bool> InsertAsync(T item)
        => await _db.InsertAsync(item) > 0;
    
    /// <summary>
    /// Updates an item in the database.
    /// </summary>
    /// <param name="item">The item to update</param>
    /// <returns>True if the update was successful</returns>
    public async Task<bool> UpdateAsync(T item)
        => await _db.UpdateAsync(item) > 0;

    /// <summary>
    /// Deletes an item from the database by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the item to delete</param>
    /// <returns>True if the deletion was successful</returns>
    public async Task<bool> DeleteAsync(object id)
        => await _db.DeleteAsync<T>(id) > 0;

    /// <summary>
    /// Executes a custom WHERE clause and returns matching items.
    /// </summary>
    /// <param name="whereClause">The SQL WHERE clause (without "WHERE")</param>
    /// <param name="parameters">Optional anonymous object with parameters</param>
    /// <returns>List of matching items</returns>
    public async Task<IEnumerable<T>> WhereAsync(string whereClause, object? parameters = null)
    {
        string query = $"SELECT * FROM {typeof(T).Name} WHERE {whereClause}";
        var paramList = DBHelper.ObjectToParameters(_db, parameters);
        return await _db.QueryAsync<T>(query, paramList.ToArray());
    }

    /// <summary>
    /// Ensures the database connection is open.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if DBManager is null or not connected</exception>
    private void EnsureConnectionAsync()
    {
        if (_db == null)
            throw new InvalidOperationException("DBManager instance is null.");

        if (!_db.IsConnected())
            _db.ConnDb();
    }

    /// <summary>
    /// Creates the database table for the model type if it does not already exist.
    /// The table name is the class name of T.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown if model has no properties</exception>
    public async Task EnsureTableAsync()
    {
        EnsureConnectionAsync();

        var type = typeof(T);
        string tableName = type.Name;
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var pk = props.FirstOrDefault(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null);
        if (pk == null)
            throw new InvalidOperationException($"EnsureTableAsync: Model '{type.Name}' must have one property marked with [PrimaryKey].");

        bool isAutoIncrement = (pk.PropertyType == typeof(int) || pk.PropertyType == typeof(long)) && !DBHelper.IsNullableType(pk.PropertyType);
        string primaryKey = pk.Name;

        string colDefs = string.Join(",\n", props.Select(p =>
        {
            string name = p.Name;
            string dbType = DBHelper.MapToDbType(p.PropertyType);

            if (p.Name == primaryKey && isAutoIncrement)
            {
                return $"    {name} {dbType} PRIMARY KEY AUTOINCREMENT";
            }

            string nullable = DBHelper.IsNullableType(p.PropertyType) ? "" : "NOT NULL";
            return $"    {name} {dbType} {nullable}";
        }));

        string query = isAutoIncrement ? $@"CREATE TABLE IF NOT EXISTS {tableName} ({colDefs});" : $@"CREATE TABLE IF NOT EXISTS {tableName} ({colDefs},PRIMARY KEY ({primaryKey}));";

        try
        {
            await _db.ExecuteNonQueryAsync(query);
        }
        catch (Exception ex)
        {
            throw new Exception($"[{tableName}] Error occurred while creating table: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Helper class for working with DBManager parameter conversion and type mapping.
/// </summary>
public static class DBHelper
{
    /// <summary>
    /// Converts an anonymous object into a list of SQL parameters.
    /// </summary>
    /// <param name="db">The DBManager instance</param>
    /// <param name="paramObj">The anonymous object to convert</param>
    /// <returns>List of IDbDataParameter</returns>
    public static List<IDbDataParameter> ObjectToParameters(DBManager db, object? paramObj)
    {
        var result = new List<IDbDataParameter>();

        if (paramObj == null)
            return result;

        var props = paramObj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var prop in props)
        {
            var name = prop.Name;
            var value = prop.GetValue(paramObj) ?? DBNull.Value;
            result.Add(db.CreateParameter("@" + name, value));
        }

        return result;
    }

    /// <summary>
    /// Maps a .NET type to a SQL column type.
    /// </summary>
    /// <param name="type">The type to map</param>
    /// <returns>The corresponding SQL type string</returns>
    public static string MapToDbType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type switch
        {
            var t when t == typeof(int) => "INTEGER",
            var t when t == typeof(long) => "BIGINT",
            var t when t == typeof(double) => "REAL",
            var t when t == typeof(decimal) => "NUMERIC",
            var t when t == typeof(bool) => "BOOLEAN",
            var t when t == typeof(DateTime) => "DATETIME",
            var t when t == typeof(string) => "TEXT",
            var t when t.FullName!.Contains(nameof(Enum)) => "INTEGER",
            _ => "TEXT"
        };
    }

    /// <summary>
    /// Checks if the type is nullable.
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if nullable, otherwise false</returns>
    public static bool IsNullableType(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }
}
