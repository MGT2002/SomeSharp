using MGTFileGenerator.Generators.Helpers;
using MGTFileGenerator.Generators.Models;
using System.Text;
using System.Xml.Linq;

namespace MGTFileGenerator.Generators;

public class FileGenerator : IFileGenerator
{
    private readonly XDocument? xml;
    private readonly string? outputDirectory;

    public FileGenerator(
        string? xmlDocFilePath = null,
        string? outputDirectory = null)
    {
        if (xmlDocFilePath is not null)
        {
            xml = XDocument.Load(xmlDocFilePath);
        }

        this.outputDirectory = outputDirectory;
    }

    public void CreateTableFromClass(Type type, string? generatedFilePath = null)
    {
        var tableInfo = TableInfoExtractor.GetTableInfo(type, xml);

        //Console.WriteLine(tableInfo);

        var sql = GenerateCreateTableSql(tableInfo);

        if (outputDirectory is null)
        {
            Console.WriteLine(sql);
            return;
        }

        File.WriteAllText(GetCreateTablePath(fileName: tableInfo.Schema + tableInfo.TableName), sql);
    }

    public static string GenerateCreateTableSql(TableInfo table)
    {
        var tableName = table.TableName;
        var schema = table.Schema;
        var columns = table.Columns;

        var lines = new List<string>();
        var primaryKeys = new List<string>();
        var foreignKeys = new List<(string name, ForeignKeyInfo info)>();

        foreach (var column in columns)
        {
            string line = $"[{column.ColumnName}] {column.SqlType}";

            line += column.IsNullable ? " NULL" : " NOT NULL";


            if (column.IsPrimaryKey)
                primaryKeys.Add(column.ColumnName);

            if (column.IsForeignKey)
            {
                foreignKeys.Add((name: column.ColumnName, info: column.ForeignKeyInfo!));
            }

            line += ", ";
            if (!string.IsNullOrWhiteSpace(column.Comment))
                line += $" -- {column.Comment}";
            lines.Add(line);
        }

        string fullTableName = $"[{schema}].[{tableName}]";
        string body = string.Join("\n\t", lines);
        string createTableSql = $"CREATE TABLE {fullTableName} (\n    {body}\n);";

        string primaryKeysSql = GeneratePrimaryKeySql(primaryKeys, schema, tableName);
        string foreignKeysSql = GenerateForeignKeySql(foreignKeys, schema, tableName);

        return $"""
            {createTableSql}

            {primaryKeysSql}

            {foreignKeysSql}
            """;
    }

    public static string GenerateForeignKeySql(
     List<(string name, ForeignKeyInfo info)> foreignKeys,
     string schema,
     string tableName)
    {
        var sb = new StringBuilder();

        foreach (var fk in foreignKeys)
        {
            string constraintName = $"FK_{schema}_{tableName}_{fk.name}";
            sb.AppendLine($@"
ALTER TABLE [{schema}].[{tableName}]
ADD CONSTRAINT [{constraintName}]
FOREIGN KEY ([{fk.name}])
REFERENCES [{fk.info.ReferenceSchema}].[{fk.info.ReferenceTable}]([{fk.info.ReferenceColumn}]);");
        }

        return sb.ToString().Trim();
    }


    public static string GeneratePrimaryKeySql(List<string> primaryKeys, string schema, string tableName)
    {
        if (primaryKeys.Count == 0)
            return string.Empty;

        string constraintName = $"PK_{schema}_{tableName}";
        string columnList = string.Join(", ", primaryKeys.Select(col => $"[{col}]"));

        return $"ALTER TABLE [{schema}].[{tableName}] ADD CONSTRAINT [{constraintName}] " +
            $"PRIMARY KEY ({columnList});";
    }


    private string GetCreateTablePath(string fileName, string start = "CreateTable_", string fileExtension = ".sql") =>
        Path.Combine(outputDirectory ?? throw new NullReferenceException("output directory is not provided!"),
            start + fileName + fileExtension);
}