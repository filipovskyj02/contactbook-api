using System.Net;
using System.Net.Http.Json;
using ContactBookApi.Dtos.Contact;
using ContactBookApi.Dtos.Pagination;
using FluentAssertions;

namespace ContactBookApi.Tests;

public sealed class ContactsApiTests : IAsyncLifetime
{
    private TestAppFactory _factory = null!;
    private HttpClient _client = null!;
    
    private const string BasePath = "/api/v1/contacts";
    
    public Task InitializeAsync()
    {
        _factory = new TestAppFactory();
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        return Task.CompletedTask;
    }
    
    private static CreateContactDto ValidContact() => new()
    {
        FirstName = "Adam",
        LastName = "Novak",
        Phone = "+420777123456",
        Email = "adam@example.com"
    };

    [Fact]
    public async Task Create_then_get_returns_contact()
    {
        var create = ValidContact();

        var post = await _client.PostAsJsonAsync(BasePath, create);
        post.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await post.Content.ReadFromJsonAsync<GetContactDto>();
        created.Should().NotBeNull();

        var get = await _client.GetAsync($"{BasePath}/{created!.Id}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_unknown_returns_404()
    {
        var get = await _client.GetAsync($"{BasePath}/{Guid.NewGuid()}");
        get.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_invalid_first_name_returns_400()
    {
        var dto = ValidContact() with
        {
            FirstName = "   "
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_too_long_first_name_returns_400()
    {
        var dto = ValidContact() with
        {
            FirstName = new string('a', 34)
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_invalid_last_name_returns_400()
    {
        var dto = ValidContact() with
        {
            LastName = ""
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_too_long_last_name_returns_400()
    {
        var dto = ValidContact() with
        {
            LastName = new string('b', 70)
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_invalid_phone_returns_400()
    {
        var dto = ValidContact() with
        {
            Phone = "NOT_A_PHONE"
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_too_long_phone_returns_400()
    {
        var dto = ValidContact() with
        {
            Phone = new string('1', 35)
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_invalid_email_returns_400()
    {
        var dto = ValidContact() with
        {
            Email = "not-an-email"
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_too_long_email_returns_400()
    {
        var dto = ValidContact() with
        {
            Email = new string('1', 220)
        };

        var post = await _client.PostAsJsonAsync(BasePath, dto);

        post.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task List_is_paginated()
    {
        for (var i = 0; i < 3; i++)
        {
            await _client.PostAsJsonAsync(BasePath, new CreateContactDto
            {
                FirstName = $"A{i}",
                LastName = "Zed",
                Phone = $"+42070000000{i}",
                Email = $"a{i}@example.com"
            });
        }

        var page1 = await _client.GetFromJsonAsync<PagedResult<GetContactDto>>(
        $"{BasePath}?page=1&pageSize=2");

        page1.Should().NotBeNull();
        page1!.Items.Count.Should().Be(2);

        var page2 = await _client.GetFromJsonAsync<PagedResult<GetContactDto>>(
            $"{BasePath}?page=2&pageSize=2");

        page2.Should().NotBeNull();
        // Only one item on page 2 since we have 3 in total
        page2!.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task Search_returns_matches()
    {
        await _client.PostAsJsonAsync(BasePath, new CreateContactDto
        {
            FirstName = "Petr",
            LastName = "Novak",
            Phone = "+420111111111",
            Email = "petr@abc.com"
        });

        await _client.PostAsJsonAsync(BasePath, new CreateContactDto
        {
            FirstName = "Jana",
            LastName = "Svobodova",
            Phone = "+420222222222",
            Email = "jana@abc.com"
        });

        var res = await _client.GetFromJsonAsync<PagedResult<GetContactDto>>(
            $"{BasePath}/search?query=novak&page=1&pageSize=10");

        res.Should().NotBeNull();
        res!.Items.Should().ContainSingle(x => x.LastName == "Novak");
    }
}