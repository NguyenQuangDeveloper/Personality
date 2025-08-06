using Microsoft.Data.Sqlite;
using System.Data;
using System.Reflection;

namespace VSLibrary.Database;

/// <summary>
/// Concrete implementation of <see cref="DBInterface"/> for the SQLite database.
/// </summary>
public class SQLite : DBBase
{
    private SqliteConnection _connection = null!;
    private SqliteTransaction _transaction = null!;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the SQLite provider.
    /// </summary>
    /// <param name="connectionString">The connection string for the SQLite database file.</param>
    public SQLite(string connectionString) : base(connectionString) { }

    /// <inheritdoc/>
    public override DatabaseProvider GetDatabaseProvider() => DatabaseProvider.SQLite;

    /// <inheritdoc/>
    public override void ConnDb()
    {
        if (isConnected) return;
        try
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
            isConnected = true;
        }
        catch (Exception ex)
        {
            isConnected = false;
            throw new Exception($"SQLite connection failed: {ex.Message}", ex);
        }
    }

    /// <inheritdoc/>
    public override void CloseDbConn() => _connection?.Close();

    /// <inheritdoc/>
    public override void BeginTransaction()
    {
        if (_transaction != null) throw new InvalidOperationException("A transaction is already in progress.");
        if (!isConnected || _connection == null) ConnDb();
        _transaction = _connection!.BeginTransaction();
    }

    /// <inheritdoc/>
    public override void CommitTransaction()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null!;
    }

    /// <inheritdoc/>
    public override void RollbackTransaction()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null!;
    }

    /// <inheritdoc/>
    public override IDbDataParameter CreateParameter(string parameterName, object value)
    {
        return new SqliteParameter(parameterName, value ?? DBNull.Value);
    }

    /// <inheritdoc/>
    public override async Task<int> ExecuteNonQueryAsync(string query, params IDbDataParameter[] parameters)
    {
        await using var cmd = new SqliteCommand(query, _connection, _transaction);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        return await cmd.ExecuteNonQueryAsync();
    }

    /// <inheritdoc/>
    public override async Task<object> ExecuteScalarAsync(string query, params IDbDataParameter[] parameters)
    {
        await using var cmd = new SqliteCommand(query, _connection, _transaction);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        var result = await cmd.ExecuteScalarAsync();
        return result ?? DBNull.Value; // Ensure a non-null value is returned
    }

    /// <inheritdoc/>
    public override async Task<DataTable> GetDataTableAsync(string query, params IDbDataParameter[] parameters)
    {
        await using var cmd = new SqliteCommand(query, _connection, _transaction);
        if (parameters != null) cmd.Parameters.AddRange(parameters);
        await using var reader = await cmd.ExecuteReaderAsync();
        var dataTable = new DataTable();
        dataTable.Load(reader);
        return dataTable;
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<T>> QueryAsync<T>(string query, params IDbDataParameter[] parameters)
    {
        var list = new List<T>();
        await using (var cmd = new SqliteCommand(query, _connection, _transaction))
        {
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var obj = new T();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var prop = typeof(T).GetProperty(reader.GetName(i), BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (prop != null && !reader.IsDBNull(i))
                        {
                            var dbValue = reader.GetValue(i);
                            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            object convertedValue = null;

                            if (targetType.IsEnum)
                                convertedValue = Enum.ToObject(targetType, dbValue);
                            else
                                convertedValue = Convert.ChangeType(dbValue, targetType);

                            prop.SetValue(obj, convertedValue, null);
                        }
                    }
                    list.Add(obj);
                }
            }
        }
        return list;
    }

    /// <summary>
    /// Releases the managed resources used by the SQLite provider.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }
}