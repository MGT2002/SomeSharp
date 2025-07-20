using MGTFileGenerator.Shared.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainProject.Models;

[Table(name: nameof(BaseModel), Schema = "DTOg")]
internal class BaseModel
{
    /// <summary>
    /// Id of table
    /// </summary>
    [Column, Key]
    public long Id { get; set; } = default!;

    /// <summary>
    /// Name of table
    /// </summary>
    [Column]
    public string Name { get; set; } = default!;

    [Column]
    public string Description { get; set; } = default!;

    [Column]
    public string Type { get; set; } = default!;

    /// <summary>
    /// Crated date which should alwasy be DateOnly -> Date
    /// </summary>
    [Column(TypeName = "Date")]
    public DateOnly CreatedDate { get; set; } = default!;

    /// <summary>
    /// Foregin key to user
    /// </summary>
    [Column(TypeName = "BIGINT"), ForeignKeyExtended("ASD", "User", "Id")]
    public long CreatedUserId { get; set; } = default!;
}
