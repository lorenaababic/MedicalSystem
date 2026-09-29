# Medical System

A REST API for managing patients and their medical records: examinations, prescriptions, medical documentation and medical images. The data is stored in a PostgreSQL database hosted on Supabase.

## Features

- CRUD for patients, examinations, prescriptions and medical documentation
- Patient search and a detailed patient view with their full medical history
- Upload and download of medical images for each examination
- Export of patient data to CSV
- Validation of patients, examinations and prescriptions before saving

## Tech stack

| Area | Technologies |
|------|--------------|
| Backend | C#, ASP.NET Core Web API (.NET 7) |
| Database | PostgreSQL (Supabase), Npgsql |
| API docs | Swagger / OpenAPI |

## Architecture

- **Repository pattern** with a **Repository Factory** that creates repositories for each entity
- **Service layer** for business logic, validation and file storage
- Controllers only handle HTTP requests and responses

```
├── Controllers/    # API endpoints
├── Services/       # business logic, validation, file storage
├── Repositories/   # data access + factory
├── Models/         # entities and DTOs
└── medical-system.sql   # database schema
```

## Running locally

1. Create a PostgreSQL database and run `medical-system.sql` to create the tables.
2. Set your connection string in `appsettings.json` under `ConnectionStrings:DefaultConnection`.
3. Run:
   ```bash
   dotnet run
   ```
4. Open Swagger at `https://localhost:<port>/swagger` to try the endpoints.
