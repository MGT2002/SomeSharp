using MGTFileGenerator.Generators.Models;
using MGTFileGenerator.Shared.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Xml.Linq;

namespace MGTFileGenerator.Generators.Helpers;

public static class TableInfoExtractor
{
    public static TableInfo GetTableInfo(Type type, XDocument? xml)
    {
        var tableInfo = new TableInfo();

        // Extract table name and schema from Table attribute
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();
        if (tableAttribute != null)
        {
            tableInfo.TableName = !string.IsNullOrEmpty(tableAttribute.Name)
                ? tableAttribute.Name
                : type.Name;
            tableInfo.Schema = tableAttribute.Schema ?? throw new InvalidOperationException("Table schema is undefined! Add schema name!");
        }
        else
        {
            tableInfo.TableName = type.Name;
        }

        // Get all public properties
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Check if property has Column attribute
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            if (columnAttribute == null) continue;

            var columnInfo = new ColumnInfo
            {
                PropertyName = property.Name,
                ColumnName = !string.IsNullOrEmpty(columnAttribute.Name)
                    ? columnAttribute.Name
                    : property.Name,
                PropertyType = property.PropertyType,
                CustomTypeName = columnAttribute.TypeName
            };

            // Determine if nullable
            columnInfo.IsNullable = CheckIsNullableType(property.PropertyType);

            // Map C# types to SQL types
            columnInfo.SqlType = string.IsNullOrWhiteSpace(columnInfo.CustomTypeName) ?
                MapToSqlType(property.PropertyType, columnAttribute.TypeName) : columnInfo.CustomTypeName;

            // Check for primary key
            columnInfo.IsPrimaryKey = CheckIsPrimaryKey(property);

            // Check for foreign key
            columnInfo.IsForeignKey = CheckIsForeignKey(property, out var foreignKeyExtendedAttribute);
            if (columnInfo.IsForeignKey)
            {
                columnInfo.ForeignKeyInfo = GetForeignKeyInfo(property, foreignKeyExtendedAttribute);
            }

            // Extract XML documentation comments if available
            columnInfo.Comment = GetPropertyComment(type, propertyName: property.Name, xml: xml);

            tableInfo.Columns.Add(columnInfo);
        }

        return tableInfo;
    }

    private static bool CheckIsNullableType(Type type)
    {
        return Nullable.GetUnderlyingType(type) != null ||
               !type.IsValueType ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    private static string MapToSqlType(Type type, string? customTypeName)
    {
        // If custom type name is specified, use it
        if (!string.IsNullOrEmpty(customTypeName))
        {
            return customTypeName;
        }

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.Name switch
        {
            nameof(Int32) => "INT",
            nameof(Int64) => "BIGINT",
            nameof(Int16) => "SMALLINT",
            nameof(Byte) => "TINYINT",
            nameof(Boolean) => "BIT",
            nameof(DateTime) => "DATETIME2",
            nameof(DateOnly) => "DATE",
            nameof(TimeOnly) => "TIME",
            nameof(Decimal) => "DECIMAL(18,2)",
            nameof(Double) => "FLOAT",
            nameof(Single) => "REAL",
            nameof(Guid) => "UNIQUEIDENTIFIER",
            nameof(String) => "NVARCHAR(MAX)", // You might want to make this configurable
            _ => "NVARCHAR(MAX)" // Default fallback
        };
    }

    public static bool CheckIsPrimaryKey(PropertyInfo property)
    {
        return property.GetCustomAttribute<KeyAttribute>() is not null;
    }

    public static bool CheckIsForeignKey(PropertyInfo property, out ForeignKeyExtendedAttribute? attr)
    {
        attr = property.GetCustomAttribute<ForeignKeyExtendedAttribute>();

        return attr is not null;
    }

    private static string? GetPropertyComment(Type type, string propertyName, XDocument? xml)
    {
        if (xml is null)
            return null;

        string memberName = $"P:{type.FullName?.Replace("+", ".")}.{propertyName}";

        var member = xml.Descendants("member")
                        .FirstOrDefault(m => m.Attribute("name")?.Value == memberName);

        return member?.Element("summary")?.Value.Trim();
    }

    public static ForeignKeyInfo GetForeignKeyInfo(PropertyInfo prop, ForeignKeyExtendedAttribute? attr)
    {
        if (attr == null)
            throw new InvalidOperationException(nameof(ForeignKeyExtendedAttribute) + " Must be used on foreign key property.");

        return new ForeignKeyInfo(
            ReferenceTable: attr.ReferenceTable,
            ReferenceColumn: attr.ReferenceColumn,
            ReferenceSchema: attr.ReferenceSchema
        );
    }
}
