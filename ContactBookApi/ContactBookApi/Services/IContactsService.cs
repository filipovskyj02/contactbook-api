using ContactBookApi.Dtos.Contact;
using ContactBookApi.Dtos.Pagination;

namespace ContactBookApi.Services;

public interface IContactsService
{
    Task<PagedResult<GetContactDto>> ListAsync(PaginationParams pageParams, CancellationToken ct);
    Task<GetContactDto?> GetAsync(Guid id, CancellationToken ct);
    Task<GetContactDto> CreateAsync(CreateContactDto req, CancellationToken ct);
    Task<GetContactDto?> UpdateAsync(Guid id, UpdateContactDto req, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    Task<PagedResult<GetContactDto>> SearchAsync(string query, PaginationParams pagination, CancellationToken ct);
}