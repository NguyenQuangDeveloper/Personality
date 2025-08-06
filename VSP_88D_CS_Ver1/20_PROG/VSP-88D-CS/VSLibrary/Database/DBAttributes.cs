namespace VSLibrary.Database;

/// <summary>
/// Specifies that a property is the primary key of the table model.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PrimaryKeyAttribute : Attribute { }

/// <summary>
/// Specifies that a property is an auto-incrementing column.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class AutoIncrementAttribute : Attribute { }

/// <summary>
/// Specifies that a property should be ignored during INSERT and UPDATE operations.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class IgnoreColumnAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class UniqueGroupAttribute : Attribute
{
    public string GroupName { get; }
    public UniqueGroupAttribute(string groupName) => GroupName = groupName;
}

[AttributeUsage(AttributeTargets.Property)]
public class NotNullAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class UniqueAttribute : Attribute
{
}
