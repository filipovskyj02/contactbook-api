namespace ContactBookApi.Dtos.Contact;

public sealed class GetContactDto
{
    public Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Phone { get; init; }
    public string? Email { get; init; }
}