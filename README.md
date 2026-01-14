# ContactBook API

Simple REST API for managing a shared contact book.

---

## Features

- CRUD operations for contacts
- Paginated listing
- Search across name, phone, and email
- Input validation
- API versioning via `/api/v1`
- Swagger / OpenAPI
- SQLite for local development
- Integration tests with in-memory SQLite

---

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- AutoMapper
- Swagger (Swashbuckle)
- xUnit + WebApplicationFactory

---
Base path:

/api/v1


| Method | Endpoint | Description |
|------|---------|-------------|
| GET | `/contacts` | List contacts (paginated) |
| GET | `/contacts/{id}` | Get contact by ID |
| POST | `/contacts` | Create contact |
| PUT | `/contacts/{id}` | Update contact |
| DELETE | `/contacts/{id}` | Delete contact |
| GET | `/contacts/search` | Search contacts (paginated) |

---

## Pagination

Query parameters:

```?page=1&pageSize=20```


Response:
```json
{
  "items": [],
  "total": 0,
  "page": 1,
  "pageSize": 20
}
```
# Running
## To run the API 
```
dotnet run
```
## Swagger
```
http://localhost:5130/swagger
```

## Running tests
```
dotnet test
```