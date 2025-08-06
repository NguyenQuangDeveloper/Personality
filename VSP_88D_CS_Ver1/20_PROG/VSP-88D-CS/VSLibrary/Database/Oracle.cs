using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace VSLibrary.Database
{
    /// <summary>
    /// Concrete implementation of <see cref="DBInterface"/> for Oracle.
    /// <para>NOTE: This is a stub implementation. Logic needs to be filled in.</para>
    /// </summary>
    public class Oracle : DBBase
    {
        private readonly OracleConnection _connection = null!;
        private readonly OracleTransaction _transaction = null!;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the Oracle provider.
        /// </summary>
        public Oracle(string connectionString) : base(connectionString) { }

        /// <inheritdoc/>
        public override DatabaseProvider GetDatabaseProvider() => DatabaseProvider.Oracle;

        #region SQL Builder Overrides for Oracle
        /// <inheritdoc/>
        /// <remarks>This override replaces the standard '@' parameter marker with Oracle's ':' marker.</remarks>
        public override string BuildInsertSql<T>(IEnumerable<PropertyInfo> properties) => base.BuildInsertSql<T>(properties).Replace('@', ':');
        /// <inheritdoc/>
        /// <remarks>This override replaces the standard '@' parameter marker with Oracle's ':' marker.</remarks>
        public override string BuildUpdateSql<T>(PropertyInfo pkProperty, IEnumerable<PropertyInfo> propsToUpdate) => base.BuildUpdateSql<T>(pkProperty, propsToUpdate).Replace('@', ':');
        /// <inheritdoc/>
        /// <remarks>This override replaces the standard '@' parameter marker with Oracle's ':' marker.</remarks>
        public override string BuildDeleteSql<T>(PropertyInfo pkProperty) => base.BuildDeleteSql<T>(pkProperty).Replace('@', ':');
        /// <inheritdoc/>
        /// <remarks>This override replaces the standard '@' parameter marker with Oracle's ':' marker.</remarks>
        public override string BuildSelectByPkSql<T>(PropertyInfo pkProperty) => base.BuildSelectByPkSql<T>(pkProperty).Replace('@', ':');
        #endregion

        /// <inheritdoc/>
        public override void ConnDb() => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        public override void CloseDbConn() => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        public override void BeginTransaction()
        {
            if (_transaction != null) throw new InvalidOperationException("A transaction is already in progress.");
            throw new NotImplementedException("Oracle provider is not implemented.");
        }

        /// <inheritdoc/>
        public override void CommitTransaction() => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        public override void RollbackTransaction() => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        /// <remarks>This implementation converts standard '@' parameter names to Oracle's ':' format.</remarks>
        public override IDbDataParameter CreateParameter(string parameterName, object value)
        {
            string oracleParamName = parameterName.StartsWith("@") ? ":" + parameterName.Substring(1) : parameterName;
            return new OracleParameter(oracleParamName, value ?? DBNull.Value);
        }

        /// <inheritdoc/>
        public override Task<DataTable> GetDataTableAsync(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        public override Task<IEnumerable<T>> QueryAsync<T>(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        public override Task<object> ExecuteScalarAsync(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <inheritdoc/>
        public override Task<int> ExecuteNonQueryAsync(string query, params IDbDataParameter[] parameters) => throw new NotImplementedException("Oracle provider is not implemented.");

        /// <summary>
        /// Releases the managed resources used by the Oracle provider.
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