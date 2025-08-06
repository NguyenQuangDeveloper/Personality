using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace VSLibrary.Database
{
    /// <summary>
    /// Concrete implementation of <see cref="DBInterface"/> for MySQL.
    /// <para>NOTE: This is a stub implementation. Logic needs to be filled in.</para>
    /// </summary>
    public class MySQL : DBBase
    {
        private readonly MySqlConnection _connection = null!; // Made readonly to address IDE0044
        private readonly MySqlTransaction _transaction = null!;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the MySQL provider.
        /// </summary>
        public MySQL(string connectionString) : base(connectionString)
        {
            _connection = new MySqlConnection(connectionString); // Assigning to fix CS0649
        }

        /// <inheritdoc/>
        public override DatabaseProvider GetDatabaseProvider() => DatabaseProvider.MySQL;

        /// <inheritdoc/>
        public override void ConnDb() => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override void CloseDbConn() => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override void BeginTransaction()
        {
            if (_transaction != null) throw new InvalidOperationException("A transaction is already in progress.");
            throw new NotImplementedException("MySQL provider is not implemented.");
        }

        /// <inheritdoc/>
        public override void CommitTransaction() => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override void RollbackTransaction() => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override IDbDataParameter CreateParameter(string parameterName, object value) => new MySqlParameter(parameterName, value ?? DBNull.Value);

        /// <inheritdoc/>
        public override Task<DataTable> GetDataTableAsync(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override Task<IEnumerable<T>> QueryAsync<T>(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override Task<object> ExecuteScalarAsync(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <inheritdoc/>
        public override Task<int> ExecuteNonQueryAsync(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("MySQL provider is not implemented.");

        /// <summary>
        /// Releases the managed resources used by the MySQL provider.
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
}