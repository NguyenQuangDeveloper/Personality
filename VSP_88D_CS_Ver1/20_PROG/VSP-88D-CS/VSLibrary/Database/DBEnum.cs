using System;

namespace VSLibrary.Database
{

    /// <summary>
    /// Defines the types of supported database providers.
    /// </summary>
    public enum DatabaseProvider
    {
        /// <summary>
        /// A file-based lightweight database that operates without a server.
        /// </summary>
        SQLite,

        /// <summary>
        /// A widely used open-source relational database.
        /// </summary>
        MySQL,

        /// <summary>
        /// An enterprise-grade database primarily used in corporate environments.
        /// </summary>
        Oracle,
    }
}