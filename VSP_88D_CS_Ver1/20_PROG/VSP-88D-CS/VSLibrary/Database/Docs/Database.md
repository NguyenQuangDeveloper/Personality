# 📘 Database Library (English Version)

This document describes the **generic database access library** provided under the `VSLibrary.Database` namespace.
It supports **SQLite / MySQL / Oracle**, and provides:

* Auto SQL generation
* ORM-style object mapping

---

## 📦 Namespace

```
VSLibrary.Database
```

---

## 🧱 Main Classes

| Class Name                  | Description                                                    |
| --------------------------- | -------------------------------------------------------------- |
| `DBInterface`               | Standard interface that all DB providers must implement        |
| `DBBase`                    | Abstract base class with common database logic                 |
| `MySQL`, `Oracle`, `SQLite` | Concrete DB providers extending `DBBase`                       |
| `DBData`                    | Container for model classes like `UserTbl`, `LogTbl`, etc.     |
| `DBEnum`                    | Enums such as `DatabaseProvider`                               |
| `DBManager`                 | Unified access point for developers; provides all DB functions |

---

## 🪩 Class Diagram

```
Database
├─ DBManager
│  ├─ Auto CRUD: InsertAsync<T>, UpdateAsync<T>, DeleteAsync<T> ...
│  └─ Direct query methods: QueryAsync<T>, ExecuteNonQueryAsync ...
│
├─ DBInterface (Contract)
│  └─ DBBase (Abstract Base)
│     ├─ SQLite
│     ├─ MySQL
│     └─ Oracle
│
└─ DBData (Models)
   └─ UserTbl, LogTbl, CustTbl ...
```

---

## ⚙️ Quick Usage Guide

### 1. Create DBManager

```csharp
string connStr = "Data Source=test.db";  // Example for SQLite
var db = new DBManager(DatabaseProvider.SQLite, connStr);
```

### 2. CRUD with Model Class

```csharp
// INSERT: Add a new user
var newUser = new UserTbl { UserID = "admin", UserName = "Admin", Grade = 1, Password="1234", Department="sw" };
await db.InsertAsync(newUser);

// SELECT by Primary Key
var user = await db.SelectByPkAsync<UserTbl>("admin");
if (user == null) return;

// UPDATE: Modify a field and save
user.UserName = "Updated Admin";
await db.UpdateAsync(user);

// SELECT ALL
var allUsers = await db.SelectAllAsync<UserTbl>();
Console.WriteLine($"Total users: {allUsers.Count()}");

// DELETE by Primary Key
await db.DeleteAsync<UserTbl>("admin");
```

---

### 3. Execute Custom Queries

```csharp
// Execute DML
int affectedRows = await db.ExecuteNonQueryAsync("DELETE FROM UserTbl WHERE Grade < @Grade",
    db.CreateParameter("@Grade", 1));

// Scalar query (COUNT, SUM, etc.)
object count = await db.ExecuteScalarAsync("SELECT COUNT(*) FROM UserTbl");
Console.WriteLine($"User count: {Convert.ToInt32(count)}");

// Retrieve as object list
var users = await db.QueryAsync<UserTbl>("SELECT * FROM UserTbl WHERE Grade > @Grade AND Department = @Dept",
    db.CreateParameter("@Grade", 2),
    db.CreateParameter("@Dept", "R&D")
);

// Retrieve as DataTable
var userTable = await db.GetDataTableAsync("SELECT UserID, UserName FROM UserTbl");
// Can be bound directly to DataGridView
```

### 🔹 Why use `CreateParameter()`?

* Prevents SQL injection
* Automatically maps .NET types to DB types
* Ensures compatibility across DB types (e.g., Oracle's `:` prefix)

#### ✅ You can still use string interpolation:

```csharp
await db.ExecuteNonQueryAsync($"DELETE FROM UserTbl WHERE UserID = 'admin'");
```

> ⚠️ Only safe for hardcoded values. Avoid using with user input.

---

## 📝 Sample Model Definition

```csharp
public class UserTbl
{
    public string UserID { get; set; }
    public string UserName { get; set; }
    public int Grade { get; set; }
    public string Password { get; set; }
    public string Department { get; set; }
}
```

---

## 🔧 DB Provider Highlights

| Provider | Description                                                 |
| -------- | ----------------------------------------------------------- |
| `SQLite` | File-based lightweight DB for local use                     |
| `MySQL`  | Open-source standard RDBMS                                  |
| `Oracle` | Special handling required (e.g., colon-prefixed parameters) |

---

## 📌 Key Function Summary

| Function                     | Description               |
| ---------------------------- | ------------------------- |
| `ConnDb()` / `CloseDbConn()` | Open/close DB connection  |
| `BeginTransaction()`         | Begin transaction         |
| `InsertAsync<T>()`           | Insert using object model |
| `UpdateAsync<T>()`           | Update using object model |
| `DeleteAsync<T>()`           | Delete by primary key     |
| `SelectByPkAsync<T>()`       | Select by primary key     |
| `SelectAllAsync<T>()`        | Select all records        |
| `CreateParameter()`          | Create database parameter |
| `BuildInsertSql<T>()`        | Auto-generate INSERT SQL  |

---

## 🧩 Extending to New DB Providers

* Inherit from `DBBase`
* Implement `DBInterface`
* Override SQL builders if syntax differs (e.g., `BuildInsertSql()`)

---

📅 Document Date: 2025-06-24  
🖋️ Author: Han Eidong
