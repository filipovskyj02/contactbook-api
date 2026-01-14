using ContactBookApi.Dtos.Contact;
using ContactBookApi.Dtos.Pagination;
using ContactBookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactBookApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ContactsController(IContactsService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<GetContactDto>>> List(
        [FromQuery] PaginationParams pagination,
        CancellationToken ct = default)
    {
        return Ok(await service.ListAsync(pagination, ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetContactDto>> Get(
        Guid id,
        CancellationToken ct)
    {
        var c = await service.GetAsync(id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<ActionResult<GetContactDto>> Create(
        [FromBody] CreateContactDto req,
        CancellationToken ct)
    {
        var created = await service.CreateAsync(req, ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GetContactDto>> Update(
        Guid id,
        [FromBody] UpdateContactDto req,
        CancellationToken ct)
    {
        var updated = await service.UpdateAsync(id, req, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken ct)
    {
        var ok = await service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<GetContactDto>>> Search(
        [FromQuery] string query,
        [FromQuery] PaginationParams pagination,
        CancellationToken ct = default)
    {
        return Ok(await service.SearchAsync(query, pagination, ct));
    }
}