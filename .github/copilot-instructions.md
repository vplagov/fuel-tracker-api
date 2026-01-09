# FuelTracker Project - Copilot Instructions

## Project Overview

**FuelTracker** is an ASP.NET Core REST API for tracking fuel consumption and expenses for vehicles. 
Users can register, create multiple cars, and log fuel entries with detailed information 
(liters, cost, odometer reading, date).

### Technology Stack
- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 10.0
- **API Documentation**: Swagger/OpenAPI
- **Architecture Pattern**: Repository Pattern with Dependency Injection

---

## Project Structure

```
FuelTracker.API/
├── Controllers/          # HTTP request handlers
├── Services/             # Business logic and use cases
├── Repositories/         # Data access layer
├── Entities/             # Domain models (EF Core entities)
├── Models/               # DTOs (Data Transfer Objects) for API requests/responses
├── Database/             # DbContext and EF Core configuration
├── Migrations/           # EF Core database migrations
├── Extensions/           # Mapping and utility extensions
├── Shared/               # Shared types (Result, ErrorType)
├── Properties/           # App configuration files
└── Program.cs            # Application startup configuration
```

### Layer Responsibilities

1. **Controllers** (`Controllers/`): Handle HTTP requests/responses, call services
2. **Services** (`Services/`): Contain business logic, validation, coordinate repositories
3. **Repositories** (`Repositories/`): Direct database access, query operations
4. **Entities** (`Entities/`): EF Core entity models representing database tables
5. **Models** (`Models/`): Request/Response DTOs for API contracts
6. **Database** (`Database/`): DbContext and data model configuration
7. **Extensions** (`Extensions/`): Mapping between entities and DTOs

---

## Architecture & Design Patterns

### Repository Pattern
- Each entity has a corresponding repository (`CarRepository`, `FuelEntryRepository`)
- Repositories handle all database queries and persistence operations
- Services depend on repositories and orchestrate business logic

### Dependency Injection Setup
Services and repositories are registered as **Scoped** in `Program.cs`:

```csharp
builder.Services.AddScoped<CarRepository>();
builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<FuelEntryRepository>();
builder.Services.AddScoped<FuelEntryService>();
```

**Why Scoped?** 
- One instance per HTTP request
- Suitable for DbContext and repository/service patterns
- Ensures clean state per request
- Better than Singleton for data access

### Entity Relationships

**User → Cars → FuelEntries** (hierarchical ownership)

```
UserEntity (1) ──── (many) CarEntity ──── (many) FuelEntry
  ├─ Id                  ├─ Id              ├─ Id
  ├─ Username            ├─ Name            ├─ CarId (FK)
  ├─ Email (optional)    ├─ UserId (FK)     ├─ Date
  ├─ PasswordHash         └─ UserEntity (nav) ├─ Liters
  ├─ CreatedAt (optional) └─ FuelEntries (nav) ├─ PricePerLiter
  └─ CarEntities (nav)                      ├─ TotalCost
                                             └─ CarEntity (nav)
```

**Foreign Key Cascade Behavior**: `OnDelete(DeleteBehavior.Cascade)`
- Deleting a user deletes all their cars and fuel entries
- Deleting a car deletes all its fuel entries

---

## Coding Conventions

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes | PascalCase | `CarEntity`, `FuelEntryService` |
| Properties | PascalCase | `UserId`, `CreatedAt` |
| Variables | camelCase | `carId`, `totalCost` |
| Parameters | camelCase | `userId`, `fuelEntry` |
| Methods | PascalCase | `GetCarById`, `CreateFuelEntry` |
| Constants | UPPER_SNAKE_CASE | `MAX_FUEL_PRICE` |
| Entities | Suffix with `Entity` | `CarEntity`, `UserEntity`, `FuelEntry` |
| Repositories | Suffix with `Repository` | `CarRepository` |
| Services | Suffix with `Service` | `CarService` |

### Entity Conventions

- All entities have `Guid Id` as primary key, auto-generated
- Foreign key properties end with `Id` (e.g., `UserId`, `CarId`)
- Navigation properties are included (e.g., `UserEntity`, `CarEntities`)
- Collections use `ICollection<T>` with list initialization: `= new List<T>()`

### Optional Fields

Optional fields are configured via **Fluent API in DbContext**:

```csharp
modelBuilder.Entity<UserEntity>(userEntity =>
{
    userEntity.Property(u => u.Email).IsRequired(false);
    userEntity.Property(u => u.CreatedAt).IsRequired(false);
});
```

Properties are still non-nullable in C# (use `string` not `string?`, `DateTime` not `DateTime?`).
The `.IsRequired(false)` allows NULL in the database, not in the C# model.

### Unique Constraints

Unique fields use index configuration:

```csharp
userEntity.HasIndex(u => u.Username).IsUnique();
```

---

## API Response Patterns

### Generic Result Type

The `Result<T>` class in `Shared/Result.cs` is used for all API responses:

```csharp
public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public ErrorType? ErrorType { get; set; }
}
```

### Response Example

```csharp
// Success
return Ok(new Result<CarResponse>
{
    Success = true,
    Data = carResponse,
    Message = "Car created successfully"
});

// Error
return BadRequest(new Result<CarResponse>
{
    Success = false,
    Message = "Invalid car data",
    ErrorType = ErrorType.ValidationError
});
```

### HTTP Status Code Conventions

| Status | Use Case | Example |
|--------|----------|---------|
| 200 OK | Successful GET, PUT | Retrieve/update resource |
| 201 Created | Successful POST | Resource created |
| 400 Bad Request | Validation errors | Invalid input data |
| 404 Not Found | Resource doesn't exist | Car ID not found |
| 500 Internal Server Error | Unhandled exceptions | Database connection error |

---

## Entity Framework Core & Database

### DbContext Configuration

File: `Database/FuelTrackerContext.cs`

- Database provider: PostgreSQL via Npgsql
- All relationships configured in `OnModelCreating`
- Foreign keys use cascade delete by default
- Unique constraints defined via `HasIndex().IsUnique()`

### Migrations

**Create a migration after entity changes:**

```powershell
dotnet ef migrations add DescriptiveNameOfChange
```

**Examples:**
- `AddUserIdToCarEntity`
- `MakeEmailOptional`
- `ConfigureUserCarRelationship`

**Apply migrations to database:**

```powershell
dotnet ef database update
```

**Revert to previous migration:**

```powershell
dotnet ef database update PreviousMigrationName
```

**Remove last migration (before applying):**

```powershell
dotnet ef migrations remove
```

### Important Migration Practices

- ✅ **DO** commit migration files to git
- ✅ **DO** create descriptive migration names
- ❌ **DON'T** manually edit migration files unless absolutely necessary
- ✅ **DO** run migrations before deploying
- ✅ **DO** test migrations locally first

---

## Data Transfer Objects (DTOs)

Location: `Models/`

### Naming Convention
- Request DTOs: `{Entity}Request` (e.g., `CarRequest`, `FuelEntryRequest`)
- Response DTOs: `{Entity}Response` (e.g., `CarResponse`, `FuelEntryResponse`)

### Usage Rules

1. **Controllers receive Request DTOs, return Response DTOs**
2. **Services work with entities internally**
3. **Repositories return/accept entities**
4. **Mapping between entities and DTOs** happens in extensions or services

### Mapping Example

File: `Extensions/FuelEntryMappingExtensions.cs`

```csharp
public static FuelEntryResponse ToResponse(this FuelEntry fuelEntry)
{
    return new FuelEntryResponse
    {
        Id = fuelEntry.Id,
        CarId = fuelEntry.CarId,
        Date = fuelEntry.Date,
        // ...
    };
}
```

---

## Common Development Tasks

### Adding a New Entity

1. Create entity class in `Entities/` with:
   - `Guid Id` primary key
   - Required and optional properties
   - Navigation properties if needed

2. Add DbSet to `FuelTrackerContext`:
   ```csharp
   public DbSet<YourEntity> YourEntities => Set<YourEntity>();
   ```

3. Configure relationships in `OnModelCreating` if needed

4. Create migration:
   ```powershell
   dotnet ef migrations add Add{EntityName}
   ```

5. Apply migration:
   ```powershell
   dotnet ef database update
   ```

### Adding a New API Endpoint

1. Create Request/Response DTOs in `Models/`
2. Create Repository in `Repositories/` if needed
3. Create Service in `Services/` with business logic
4. Add Controller method in `Controllers/`
5. Return `Result<T>` wrapped response

**Controller pattern:**
```csharp
[HttpPost("create")]
public async Task<IActionResult> Create([FromBody] CarRequest request)
{
    var result = await _carService.CreateCarAsync(request);
    
    if (!result.Success)
        return BadRequest(result);
    
    return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
}
```

### Handling DateTime

Always use **UTC timezone**:

```csharp
// Get current UTC time
DateTime utcNow = DateTime.UtcNow;

// Configure in DbContext to store as UTC
b.Property(e => e.CreatedAt)
    .HasColumnType("timestamp with time zone");
```

---

## Development Setup

### Prerequisites
- .NET 9.0 or 10.0 SDK
- PostgreSQL (via Docker)
- Visual Studio, Rider, or VS Code

### Initialize Local Environment

1. **Start PostgreSQL Docker container:**
   ```powershell
   docker run --name fueltracker-postgres `
     -e POSTGRES_DB=fueltracker `
     -e POSTGRES_USER=postgres `
     -e POSTGRES_PASSWORD=postgres `
     -p 5432:5432 `
     -d postgres:latest
   ```

2. **Apply database migrations:**
   ```powershell
   dotnet ef database update
   ```

3. **Run the API:**
   ```powershell
   dotnet run
   ```

4. **Access Swagger UI:**
   - Navigate to: `https://localhost:5001/swagger/index.html`
   - Or: `https://localhost:7000/swagger/index.html` (depends on environment)

### Useful Commands

```powershell
# Build solution
dotnet build

# Run tests (if available)
dotnet test

# Clean build artifacts
dotnet clean

# Restore packages
dotnet restore

# View migration history
dotnet ef migrations list

# Add migration
dotnet ef migrations add {MigrationName}

# Update database
dotnet ef database update

# Revert database
dotnet ef database update {PreviousMigrationName}
```

---

## Debugging Tips

### Enable EF Core Logging

Add to `Program.cs`:
```csharp
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<Program>();
    
    // Log SQL queries
}
```

### Check Pending Model Changes

```powershell
dotnet ef migrations has-pending-changes
```

### View Current Database Schema

Use PostgreSQL client or DbVisualizer to inspect tables and relationships.

---

## Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| "PendingModelChangesWarning" | Run `dotnet ef migrations add MigrationName` then `dotnet ef database update` |
| 404 errors on endpoints | Check controller routing attributes and method names in Swagger |
| Foreign key constraint violations | Ensure parent entities exist before creating children |
| Duplicate username in database | Username has unique index - validate before creation |
| Connection string errors | Check `appsettings.json` connection string matches database |
| Migration conflicts | Resolve conflicts in migration Designer.cs files |

---

## Git Conventions

### Commit Messages

- `feat: Add {feature}` - New feature
- `fix: Fix {issue}` - Bug fix
- `refactor: {description}` - Code refactoring
- `chore: {description}` - Maintenance tasks
- `docs: {description}` - Documentation
- `db: {description}` - Database migrations

**Examples:**
- `feat: Add fuel entry creation endpoint`
- `db: Add UserId foreign key to CarEntity`
- `fix: Correct decimal precision for prices`

### What to Commit
- ✅ Source code (`.cs` files)
- ✅ Migration files (`Migrations/`)
- ✅ Configuration files (`appsettings.json`)
- ✅ Project files (`.csproj`)
- ❌ Build artifacts (`bin/`, `obj/`)
- ❌ Environment secrets (use user secrets)

---

## Helpful Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [Entity Framework Core Documentation](https://learn.microsoft.com/ef/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [REST API Best Practices](https://restfulapi.net/)

