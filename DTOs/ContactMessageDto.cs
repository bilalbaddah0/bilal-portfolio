using System.ComponentModel.DataAnnotations;

namespace BilalPortfolio.DTOs;

public sealed class ContactMessageDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(150, MinimumLength = 3)]
    public string Subject { get; set; } = string.Empty;

    [Required, StringLength(4000, MinimumLength = 10)]
    public string Message { get; set; } = string.Empty;

    public string? Website { get; set; }
}
