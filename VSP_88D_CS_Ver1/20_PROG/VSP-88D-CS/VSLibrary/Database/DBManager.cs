using Microsoft.Data.Sqlite;
using System.Data;
using System.IO;
using System.Reflection;

namespace VSLibrary.Database;

/// <summary>
/// Acts as a Facade for the database library, providing a single, simplified entry point for all database operations.
/// It orchestrates the underlying providers to perform tasks.
/// </summary>
public class DBManager : IDisposable
{
    private readonly DBInterface _provider = null!;

    #region Private Helper Methods
    /// <summary>
    /// Finds and returns the PropertyInfo for the property marked with [PrimaryKey].
    /// Throws an exception if no PK is found or more than one is found.
    /// </summary>
    private PropertyInfo GetPrimaryKeyProperty<T>()
    {
        var type = typeof(T);
        var pkProperties = type.GetProperties()
                               .Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                               .ToList();
        if (pkProperties.Count == 0)
            throw new InvalidOperationException($"Model '{type.Name}' does not have a [PrimaryKey] attribute.");
        if (pkProperties.Count > 1)
            throw new InvalidOperationException($"Model '{type.Name}' has more than one [PrimaryKey] attribute.");
        return pkProperties.Single();
    }
    #endregion

    #region Constructor & Provider Creation
    /// <summary>
    /// Initializes a new instance of the <see cref="DBManager"/> class for a specific database provider.
    /// </summary>
    /// <param name="dbType">The type of database provider to use.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    public DBManager(DatabaseProvider dbType, string connectionString)
    {
        if (dbType == DatabaseProvider.SQLite)
        {
            var builder = new SqliteConnectionStringBuilder(connectionString); 
            var dbPath = builder.DataSource;

            if (string.IsNullOrWhiteSpace(dbPath))
                return;

            string? folderPath = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        _provider = CreateProvider(dbType, connectionString);
    }

    private DBInterface CreateProvider(DatabaseProvider dbType, string connectionString)
    {
        return dbType switch
        {
            DatabaseProvider.MySQL => new MySQL(connectionString),
            DatabaseProvider.Oracle => new Oracle(connectionString),
            DatabaseProvider.SQLite => new SQLite(connectionString),
            _ => throw new ArgumentException("Unsupported DB Provider type.")
        };
    }
    #endregion

    #region Low-Level API Wrappers
    /// <inheritdoc cref="DBInterface.IsConnected"/>
    public bool IsConnected() => _provider.IsConnected();
    /// <inheritdoc cref="DBInterface.ConnDb"/>
    public void ConnDb() => _provider.ConnDb();
    /// <inheritdoc cref="DBInterface.CloseDbConn"/>
    public void CloseDbConn() => _provider.CloseDbConn();
    /// <inheritdoc cref="DBInterface.BeginTransaction"/>
    public void BeginTransaction() => _provider.BeginTransaction();
    /// <inheritdoc cref="DBInterface.CommitTransaction"/>
    public void CommitTransaction() => _provider.CommitTransaction();
    /// <inheritdoc cref="DBInterface.RollbackTransaction"/>
    public void RollbackTransaction() => _provider.RollbackTransaction();
    /// <inheritdoc cref="DBInterface.GetDataTableAsync"/>
    public Task<DataTable> GetDataTableAsync(string query, params IDbDataParameter[] parameters) => _provider.GetDataTableAsync(query, parameters);
    /// <inheritdoc cref="DBInterface.QueryAsync{T}"/>
    public Task<IEnumerable<T>> QueryAsync<T>(string query, params IDbDataParameter[] parameters) where T : new() => _provider.QueryAsync<T>(query, parameters);
    /// <inheritdoc cref="DBInterface.ExecuteScalarAsync"/>
    public Task<object> ExecuteScalarAsync(string query, params IDbDataParameter[] parameters) => _provider.ExecuteScalarAsync(query, parameters);
    /// <inheritdoc cref="DBInterface.ExecuteNonQueryAsync"/>
    public Task<int> ExecuteNonQueryAsync(string query, params IDbDataParameter[] parameters) => _provider.ExecuteNonQueryAsync(query, parameters);
    /// <inheritdoc cref="DBInterface.CreateParameter"/>
    public IDbDataParameter CreateParameter(string parameterName, object value) => _provider.CreateParameter(parameterName, value);
    #endregion

    #region High-Level API (Automated CRUD)
    /// <summary>
    /// Asynchronously inserts a model object into the database.
    /// Ignores properties with [IgnoreColumn] or [AutoIncrement] attributes.
    /// </summary>
    /// <typeparam name="T">The type of the model object.</typeparam>
    /// <param name="item">The object to insert.</param>
    /// <returns>A task representing the async operation, containing the number of rows affected.</returns>
    public async Task<int> InsertAsync<T>(T  item) where T : new()
    {
        var type = typeof(T);
        var propertiesToInsert = type.GetProperties().Where(p =>
            p.GetCustomAttribute<IgnoreColumnAttribute>() == null &&
            p.GetCustomAttribute<AutoIncrementAttribute>() == null
        ).ToList();
        string query = _provider.BuildInsertSql<T>(propertiesToInsert);
        var parameters = propertiesToInsert.Select(p =>
            _provider.CreateParameter("@" + p.Name, p.GetValue(item) ?? DBNull.Value)
        ).ToArray();

        int rowsAffected = await _provider.ExecuteNonQueryAsync(query, parameters);
        if (rowsAffected > 0)
        {
            object? scalar = await _provider.ExecuteScalarAsync("SELECT last_insert_rowid();");
            if (scalar != null && int.TryParse(scalar.ToString(), out int id))
                return id;
        }    

        return 0;
    }

    /// <summary>
    /// Asynchronously updates an existing record in the database based on the object's Primary Key.
    /// </summary>
    /// <typeparam name="T">The type of the model object.</typeparam>
    /// <param name="item">The object with updated values. Its Primary Key property must be set.</param>
    /// <returns>A task representing the async operation, containing the number of rows affected.</returns>
    public async Task<int> UpdateAsync<T>(T item) where T : new()
    {
        var type = typeof(T);
        var pkProperty = GetPrimaryKeyProperty<T>();
        var propertiesToUpdate = type.GetProperties().Where(p =>
            p.Name != pkProperty.Name &&
            p.GetCustomAttribute<IgnoreColumnAttribute>() == null
        );
        string query = _provider.BuildUpdateSql<T>(pkProperty, propertiesToUpdate);
        var parameters = propertiesToUpdate.Select(p => _provider.CreateParameter($"@{p.Name}", p.GetValue(item) ?? DBNull.Value)).ToList();
        parameters.Add(_provider.CreateParameter($"@{pkProperty.Name}", pkProperty.GetValue(item) ?? DBNull.Value));
        return await _provider.ExecuteNonQueryAsync(query, parameters.ToArray());
    }

    /// <summary>
    /// Asynchronously deletes a record from the database using its Primary Key.
    /// </summary>
    /// <typeparam name="T">The type of the model object.</typeparam>
    /// <param name="primaryKeyValue">The primary key value of the record to delete.</param>
    /// <returns>A task representing the async operation, containing the number of rows affected.</returns>
    public async Task<int> DeleteAsync<T>(object primaryKeyValue) where T : new()
    {
        var pkProperty = GetPrimaryKeyProperty<T>();
        string query = _provider.BuildDeleteSql<T>(pkProperty);
        var parameter = _provider.CreateParameter($"@{pkProperty.Name}", primaryKeyValue);
        return await _provider.ExecuteNonQueryAsync(query, parameter);
    }

    /// <summary>
    /// Asynchronously selects a single record from the database using its Primary Key.
    /// </summary>
    /// <typeparam name="T">The type of the model object.</typeparam>
    /// <param name="primaryKeyValue">The primary key value of the record to select.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the mapped object, or null if not found.</returns>
    public async Task<T?> SelectByPkAsync<T>(object primaryKeyValue) where T : class, new()
    {
        var pkProperty = GetPrimaryKeyProperty<T>();
        string query = _provider.BuildSelectByPkSql<T>(pkProperty);
        var parameter = _provider.CreateParameter($"@{pkProperty.Name}", primaryKeyValue);
        var result = await _provider.QueryAsync<T>(query, parameter);
        return result.FirstOrDefault();
    }

    /// <summary>
    /// Asynchronously selects all records from a table corresponding to the model type.
    /// </summary>
    /// <typeparam name="T">The type of the model object.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all mapped objects from the table.</returns>
    public async Task<IEnumerable<T>> SelectAllAsync<T>() where T : new()
    {
        string query = _provider.BuildSelectAllSql<T>();
        return await _provider.QueryAsync<T>(query);
    }
    #endregion

    #region IDisposable Implementation
    /// <summary>
    /// Releases all resources used by the <see cref="DBManager"/> by disposing the underlying provider.
    /// </summary>
    public void Dispose() => _provider?.Dispose();
    #endregion
}
