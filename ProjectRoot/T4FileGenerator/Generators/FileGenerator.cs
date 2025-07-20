
using System.Xml.Linq;
using T4FileGenerator.Generators.Helpers;

namespace T4FileGenerator.Generators;

public class FileGenerator : IFileGenerator
{
    private XDocument? xml;

    public FileGenerator(string? xmlDocFilePath)
    {
        if (xmlDocFilePath is not null)
        {
            xml = XDocument.Load(xmlDocFilePath);
        }
    }

    public void CreateTableFromClass(Type type, string? generatedFilePath = null)
    {
        var table = TableInfoExtractor.GetTableInfo(type, xml);

        Console.WriteLine(table);
    }
}

/*V1
 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
   

    // Helper method to generate CREATE TABLE SQL
    public static string GenerateCreateTableSql(TableInfo tableInfo)
    {
        var sql = $"CREATE TABLE ";
        
        if (!string.IsNullOrEmpty(tableInfo.Schema))
        {
            sql += $"[{tableInfo.Schema}].";
        }
        
        sql += $"[{tableInfo.TableName}] (\n";

        var columnDefinitions = new List<string>();

        foreach (var column in tableInfo.Columns)
        {
            var columnDef = $"    [{column.ColumnName}] {column.SqlType}";
            
            if (!column.IsNullable)
            {
                columnDef += " NOT NULL";
            }

            if (column.IsPrimaryKey)
            {
                columnDef += " IDENTITY(1,1) PRIMARY KEY";
            }

            columnDefinitions.Add(columnDef);
        }

        sql += string.Join(",\n", columnDefinitions);
        sql += "\n);";

        return sql;
    }
}

// Usage example:
public class Example
{
    public static void Main()
    {
        // Extract table info from your BaseModel class
        var tableInfo = TableInfoExtractor.GetTableInfo<BaseModel>();
        
        Console.WriteLine($"Table: {tableInfo.Schema}.{tableInfo.TableName}");
        Console.WriteLine("Columns:");
        
        foreach (var column in tableInfo.Columns)
        {
            Console.WriteLine($"  {column.ColumnName} ({column.PropertyType.Name}) -> {column.SqlType}");
        }
        
        // Generate CREATE TABLE SQL
        var createTableSql = TableInfoExtractor.GenerateCreateTableSql(tableInfo);
        Console.WriteLine("\nGenerated SQL:");
        Console.WriteLine(createTableSql);
    }
}
 */