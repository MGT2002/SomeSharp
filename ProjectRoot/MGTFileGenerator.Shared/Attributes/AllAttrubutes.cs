namespace MGTFileGenerator.Shared.Attributes;

using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ForeignKeyExtendedAttribute : Attribute
{
    public string ReferenceTable { get; }
    public string ReferenceColumn { get; }
    public string ReferenceSchema { get; }

    public ForeignKeyExtendedAttribute(
        string referenceSchema,
        string referenceTable,
        string referenceColumn)
    {
        ReferenceTable = referenceTable;
        ReferenceColumn = referenceColumn;
        ReferenceSchema = referenceSchema;
    }
}

