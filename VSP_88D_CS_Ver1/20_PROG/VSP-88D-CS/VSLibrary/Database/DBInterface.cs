using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace VSLibrary.Database;

/// <summary>
/// Defines the contract for all database providers, ensuring a consistent API for database operations.
/// </summary>
public interface DBInterface : IDisposable
{
    /// <summary>
    /// Gets the type of the current database provider.
    /// </summary>
    DatabaseProvider GetDatabaseProvider();

    /// <summary>
    /// Checks if the database connection is currently open.
    /// </summary>
    bool IsConnected();

    /// <summary>
    /// Opens a connection to the database.
    /// </summary>
    void ConnDb();

    /// <summary>
    /// Closes the database connection.
    /// </summary>
    void CloseDbConn();

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    void BeginTransaction();

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    void CommitTransaction();

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    void RollbackTransaction();

    /// <summary>
    /// Executes a query and returns the results as a DataTable.
    /// </summary>
    Task<DataTable> GetDataTableAsync(string query, params IDbDataParameter[] parameters);

    /// <summary>
    /// Executes a query and maps the result to a collection of strongly-typed objects.
    /// </summary>
    Task<IEnumerable<T>> QueryAsync<T>(string query, params IDbDataParameter[] parameters) where T : new();

    /// <summary>
    /// Executes a query and returns the first column of the first row in the result set.
    /// </summary>
    Task<object> ExecuteScalarAsync(string query, params IDbDataParameter[] parameters);

    /// <summary>
    /// Executes a non-query SQL statement (e.g., INSERT, UPDATE, DELETE).
    /// </summary>
    Task<int> ExecuteNonQueryAsync(string query, params IDbDataParameter[] parameters);

    /// <summary>
    /// Creates a database parameter object specific to the underlying provider.
    /// </summary>
    IDbDataParameter CreateParameter(string parameterName, object value);

    /// <summary>
    /// Builds an INSERT SQL statement for a given model type.
    /// </summary>
    string BuildInsertSql<T>(IEnumerable<PropertyInfo> properties) where T : new();

    /// <summary>
    /// Builds an UPDATE SQL statement for a given model type.
    /// </summary>
    string BuildUpdateSql<T>(PropertyInfo pkProperty, IEnumerable<PropertyInfo> propertiesToUpdate) where T : new();

    /// <summary>
    /// Builds a DELETE SQL statement for a given model type based on its Primary Key.
    /// </summary>
    string BuildDeleteSql<T>(PropertyInfo pkProperty) where T : new();

    /// <summary>
    /// Builds a SELECT SQL statement to query a single record by its Primary Key.
    /// </summary>
    string BuildSelectByPkSql<T>(PropertyInfo pkProperty) where T : new();

    /// <summary>
    /// Builds a SELECT SQL statement to query all records from a table.
    /// </summary>
    string BuildSelectAllSql<T>() where T : new();
}

/// <summary>
/// Defines a dynamic repository interface for basic CRUD operations and table initialization.
/// Supports generic type T, where T must be a reference type with a parameterless constructor.
/// </summary>
/// <typeparam name="T">The model type to operate on</typeparam>
public interface IDynamicRepository<T> where T : class, new()
{
    /// <summary>
    /// Gets a single record by its primary key asynchronously.
    /// </summary>
    /// <param name="id">The primary key value</param>
    /// <returns>The matching entity or null if not found</returns>
    Task<T?> GetByIdAsync(object id);

    /// <summary>
    /// Retrieves all records from the table asynchronously.
    /// </summary>
    /// <returns>A collection of all records</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Inserts a new record into the table asynchronously.
    /// </summary>
    /// <param name="item">The entity to insert</param>
    /// <returns>True if insertion was successful; otherwise false</returns>
    Task<bool> InsertAsync(T item);

    /// <summary>
    /// Updates an existing record in the table asynchronously.
    /// </summary>
    /// <param name="item">The entity to update</param>
    /// <returns>True if update was successful; otherwise false</returns>
    Task<bool> UpdateAsync(T item);

    /// <summary>
    /// Deletes a record by its primary key asynchronously.
    /// </summary>
    /// <param name="id">The primary key value of the entity to delete</param>
    /// <returns>True if deletion was successful; otherwise false</returns>
    Task<bool> DeleteAsync(object id);

    /// <summary>
    /// Retrieves records matching the given WHERE clause and parameters asynchronously.
    /// </summary>
    /// <param name="whereClause">The SQL WHERE clause (without the 'WHERE' keyword)</param>
    /// <param name="parameters">Optional anonymous object containing parameter values</param>
    /// <returns>A collection of matching records</returns>
    Task<IEnumerable<T>> WhereAsync(string whereClause, object? parameters = null);

    /// <summary>
    /// Ensures the database table for the model type T exists. 
    /// If not, it will be created dynamically based on property definitions.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation</returns>
    Task EnsureTableAsync();
}
