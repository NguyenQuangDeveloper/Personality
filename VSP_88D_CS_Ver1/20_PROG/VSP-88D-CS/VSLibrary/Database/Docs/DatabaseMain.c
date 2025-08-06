/*!
 * \mainpage VSLibrary.Database
 *
 * \section intro Introduction
 * This is a general-purpose database access library provided by the VSLibrary.Database namespace.
 * It supports SQLite, MySQL, and Oracle,
 * and provides automatic SQL builders and ORM (Object-Relational Mapping)-style mapping features.
 *
 * \section modules Main Components
 *
 * - `DBManager`: The main entry point class that encapsulates each DBProvider
 * - `DBInterface`: Interface that defines the specifications for DB Provider functionality
 * - `DBBase`: Base class that abstracts common provider functionality
 * - `SQLite`, `MySQL`, `Oracle`: Classes implementing actual DB-specific functionality
 * - `DBData`: Container for model classes mapped to database tables
 *
 * \section features Key Features
 * - **Supported DBs**: SQLite, MySQL, Oracle
 * - **Automatic CRUD**: Model-based data operations such as InsertAsync, UpdateAsync, DeleteAsync
 * - **Direct Queries**: Execute raw SQL with QueryAsync, ExecuteNonQueryAsync
 * - **Transaction Management**: Supports BeginTransaction, Commit, and Rollback
 * - **SQL Injection Prevention**: Safe parameter handling via CreateParameter
 * - **Automatic SQL Generation**: Support for generating SQL syntax such as BuildInsertSql
 *
 * \section usage Usage Example
 * \code{.cs}
 * // 1. Create a DBManager instance (Example for SQLite)
 * string connStr = "Data Source=test.db";
 * var db = new DBManager(DatabaseProvider.SQLite, connStr);
 *
 * // 2. Model-based INSERT
 * var user = new UserTbl { UserID = "admin", UserName = "Administrator", Grade = 1 };
 * await db.InsertAsync(user);
 *
 * // 3. Retrieve by Primary Key
 * var selectedUser = await db.SelectByPkAsync<UserTbl>("admin");
 * \endcode
 *
 * \section version Version History
 *
 * | Date       | Version | Author         | Description                                                 |
 * |------------|---------|----------------|-------------------------------------------------------------|
 * | 2025-06-24 | 1.0.0   | Euidong Han    | Initial documentation for the Database library    |
 * | 2025-07-23 | 1.0.0   | Jang Minsu   | Added DynamicRepository<T> for generic CRUD access and EnsureTableAsync auto-schema creation |
 * | 2025-07-24 | 1.0.0   | ChatGPT (GPT-4)| All core and public documentation fully translated to English<br> full Doxygen/markdown integration.           |
 * 
 * \section license License
 *
 * This project is intended for internal enterprise use only and is not allowed to be distributed externally.
 *
 * \section contact Contact
 *
 * Email: edhan@visionsemicon.co.kr
 */
