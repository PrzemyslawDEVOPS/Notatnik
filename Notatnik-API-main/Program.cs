using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotatnikAPI.Data;
using NotatnikAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for browser-based tests
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=NotatnikDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "NotatnikAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "NotatnikAPI";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        // Ensure token is read from Authorization header
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log authentication failures for debugging
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable static files from wwwroot (but not as default)
app.UseStaticFiles();

// CORS must be before UseAuthentication
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Main info page
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Notatnik API</title>
    <style>
        body { font-family: Arial, sans-serif; max-width: 800px; margin: 50px auto; padding: 20px; }
        h1 { color: #333; }
        .endpoint { background: #f5f5f5; padding: 10px; margin: 10px 0; border-radius: 5px; }
        .method { font-weight: bold; color: #007bff; }
        a { color: #007bff; text-decoration: none; }
        a:hover { text-decoration: underline; }
        .swagger-link { display: inline-block; margin-top: 20px; padding: 10px 20px; background: #007bff; color: white; border-radius: 5px; }
    </style>
</head>
<body>
    <h1>Notatnik API</h1>
    <p>REST API do zarządzania notatkami z autoryzacją JWT</p>
    
    <h2>Dostępne endpointy:</h2>
    <div class=""endpoint"">
        <span class=""method"">POST</span> /register - Rejestracja użytkownika (email, password)
    </div>
    <div class=""endpoint"">
        <span class=""method"">POST</span> /login - Logowanie (email, password) - zwraca JWT token
    </div>
    <div class=""endpoint"">
        <span class=""method"">GET</span> /notes - Lista notatek (wymaga autoryzacji)
    </div>
    <div class=""endpoint"">
        <span class=""method"">POST</span> /notes - Tworzenie notatki (wymaga autoryzacji)
    </div>
    <div class=""endpoint"">
        <span class=""method"">PUT</span> /notes/{id} - Aktualizacja notatki (wymaga autoryzacji)
    </div>
    <div class=""endpoint"">
        <span class=""method"">DELETE</span> /notes/{id} - Usunięcie notatki (wymaga autoryzacji)
    </div>
    
    <a href=""/swagger"" class=""swagger-link"">Otwórz Swagger UI →</a>
</body>
</html>", "text/html; charset=utf-8"));

// Test page - serves index.html from wwwroot
app.MapGet("/test", () => Results.File("index.html", "text/html"));

app.MapControllers();

app.Run();

