using System.Text;
using FuelTracker.API.Configuration;
using FuelTracker.API.Database;
using FuelTracker.API.Repositories;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["DATABASE_HOST"] != null
    ? new NpgsqlConnectionStringBuilder
    {
        Host = builder.Configuration["DATABASE_HOST"],
        Username = builder.Configuration["DATABASE_USER"],
        Password = builder.Configuration["DATABASE_PASSWORD"],
        Database = builder.Configuration["DATABASE_NAME"],
        Port = 5432,
        SslMode = SslMode.Prefer
    }.ToString()
    : builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FuelTrackerContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration("JwtSettings")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
    {
        var settings = jwtOptions.Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = settings.Issuer,
            ValidAudience = settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };
    });

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularUI",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<CarRepository>();
builder.Services.AddScoped<FuelEntryRepository>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<CarService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FuelEntryService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<UserContextService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularUI");

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();