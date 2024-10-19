using Habr.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace Habr.Common.DTOs.Requests;

public class RegisterRequestModel
{
    [Required]
    [EmailAddress]
    [MaxLength(EntityConfigConsts.UserEmailMaxLength)]
    public string? Email { get; set; }

    [Required]
    [MinLength(4)]
    public string? Password { get; set; }
}
