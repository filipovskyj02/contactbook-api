using System.ComponentModel.DataAnnotations;

namespace ContactBookApi.Dtos.Contact;

public record CreateContactDto
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(32)]
    public required string FirstName { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(64)]
    public required string LastName { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(32)]
    [RegularExpression(
        @"^\+?[0-9\s\-()]{6,32}$",
        ErrorMessage = "Invalid phone number format."
    )]
    public required string Phone { get; init; }

    [EmailAddress]
    [StringLength(254)]
    public string? Email { get; init; }
}