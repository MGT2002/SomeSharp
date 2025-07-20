namespace T4FileGenerator.Generators.Models;

public class TableInfo
{
    public string TableName { get; set; } = default!;
    public string Schema { get; set; } = default!;
    public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();

    public override string? ToString()
    {
        return $$"""
            {{Schema}}.{{TableName}}
            {
              {{Columns.Select(c => c.ToString())
              .Aggregate((a, b) => $"{a}\n  {b}")}}
            }
            """;
    }
}

public class ColumnInfo
{
    public string ColumnName { get; set; } = default!;
    public string PropertyName { get; set; } = default!;
    public Type PropertyType { get; set; } = default!;
    public string SqlType { get; set; } = default!;
    public bool IsNullable { get; set; } = default!;
    public bool IsPrimaryKey { get; set; } = default!;
    public bool IsForeignKey { get; set; } = default!;
    public string? Comment { get; set; } = default!;
    public string? CustomTypeName { get; set; } = default!; // From TypeName attribute

    public override string ToString()
    {
        return
            $"ColumnName: {ColumnName ?? "Null"}, {{" +
            $"\n\tPropertyName: {PropertyName ?? "Null"}, " +
            $"\n\tPropertyType: {PropertyType?.Name ?? "Null"}, " +
            $"\n\tSqlType: {SqlType ?? "Null"}, " +
            $"\n\tIsNullable: {IsNullable}, " +
            $"\n\tIsPrimaryKey: {IsPrimaryKey}, " +
            $"\n\tIsForeignKey: {IsForeignKey}, " +
            $"\n\tComment: {Comment ?? "Null"}, " +
            $"\n\tCustomTypeName: {CustomTypeName ?? "Null"}" +
            $"\n\t}}\n";
    }

}