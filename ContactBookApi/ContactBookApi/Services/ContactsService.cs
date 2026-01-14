using AutoMapper;
using ContactBookApi.Data;
using ContactBookApi.Domain;
using ContactBookApi.Dtos.Contact;
using ContactBookApi.Dtos.Pagination;
using ContactBookApi.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ContactBookApi.Services;

public class ContactsService : IContactsService
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ContactsService(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<PagedResult<GetContactDto>> ListAsync(PaginationParams pageParams, CancellationToken ct)
    {
        var entityQuery = _dbContext.Contacts
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt);

        var page = await entityQuery.PaginateAsync(pageParams, ct);

        return new PagedResult<GetContactDto>
        {
            Items = page.Items.Select(c => _mapper.Map<GetContactDto>(c)).ToList(),
            Total = page.Total,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }

    public async Task<GetContactDto?> GetAsync(Guid id, CancellationToken ct)
    {
        var contact = await _dbContext.Contacts
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return contact is null ? null : _mapper.Map<GetContactDto>(contact);
    }

    public async Task<GetContactDto> CreateAsync(CreateContactDto req, CancellationToken ct)
    {
        var toSave = new Contact
        {
            Id = Guid.NewGuid(),
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            Phone = req.Phone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _dbContext.Contacts.Add(toSave);
        await _dbContext.SaveChangesAsync(ct);
        
        return _mapper.Map<GetContactDto>(toSave);
    }

    public async Task<GetContactDto?> UpdateAsync(Guid id, UpdateContactDto req, CancellationToken ct)
    {
        var contact = await _dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (contact is null) return null;

        contact.FirstName = req.FirstName;
        contact.LastName = req.LastName;
        contact.Phone = req.Phone;
        contact.Email = req.Email;
        contact.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);

        return _mapper.Map<GetContactDto>(contact);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var contact = await _dbContext.Contacts
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (contact is null) return false;

        _dbContext.Contacts.Remove(contact);
        await _dbContext.SaveChangesAsync(ct);

        return true;
    }
    
    public async Task<PagedResult<GetContactDto>> SearchAsync(string query, PaginationParams pagination, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new PagedResult<GetContactDto>
            {
                Items = Array.Empty<GetContactDto>(),
                Total = 0,
                Page = pagination.SafePage,
                PageSize = pagination.SafePageSize
            };
        }

        var q = query.Trim().ToLower();

        var entityQuery = _dbContext.Contacts
            .AsNoTracking()
            .Where(c =>
                c.FirstName.ToLower().Contains(q) ||
                c.LastName.ToLower().Contains(q) ||
                c.Phone.ToLower().Contains(q) ||
                (c.Email != null && c.Email.ToLower().Contains(q)))
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName);

        var page = await entityQuery.PaginateAsync(pagination, ct);

        return new PagedResult<GetContactDto>
        {
            Items = page.Items.Select(c => _mapper.Map<GetContactDto>(c)).ToList(),
            Total = page.Total,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }
}