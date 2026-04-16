using System.Text;
using Fenio1.API.Middleware;
using Fenio1.Core.Interfaces;
using Fenio1.Core.Services;
using Fenio1.Infrastructure.Data;
using Fenio1.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ========================
//  DATABASE - EF Core + SQLite
// ========================

// Logika za connection string:
// 1. Provjeri environment varijablu SQLITE_DB_PATH (najjednostavnije na Railway)
// 2. Provjeri konfiguraciju ConnectionStrings:DefaultConnection
// 3. Fallback: u produkciji /app/data/fenio1.db, lokalno fenio1.db
var dbPath = Environment.GetEnvironmentVariable("SQLITE_DB_PATH");

string connectionString;
if (!string.IsNullOrEmpty(dbPath))
{
    connectionString = $"Data Source={dbPath}";
}
else
{
    var configConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(configConnStr))
    {
        connectionString = configConnStr;
    }
    else
    {
        // Fallback
        var isProduction = builder.Environment.IsProduction();
        connectionString = isProduction
            ? "Data Source=/app/data/fenio1_1.db"
            : "Data Source=fenio1_1.db";
    }
}

// Osiguraj da direktorij postoji
var dbFile = connectionString.Replace("Data Source=", "").Trim();
var dbDir = Path.GetDirectoryName(dbFile);
if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
{
    Directory.CreateDirectory(dbDir);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// IAppDbContext interface za ScoringService
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// ========================
//  SERVICES
// ========================
builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();

// ========================
//  JWT AUTH
// ========================
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT:Key nije postavljen u konfiguraciji.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ========================
//  CONTROLLERS
// ========================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ========================
//  SWAGGER
// ========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Fenio1 API",
        Version = "v1",
        Description = "API za Formula 1 prognoze i rezultate utrka.\n\n" +
                      "**Uloge:**\n- `User` - unos prognoza, čitanje rezultata\n- `Admin` - unos rezultata, upravljanje svim podacima\n\n" +
                      "**Zadani korisnici:**\n- admin / Admin@123\n- user1 / User@123"
    });

    // JWT u Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT token. Unesite: **Bearer {vaš_token}**",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // Uključi XML komentare ako postoje
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ========================
//  CORS (za frontend dev)
// ========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// ========================
//  MIDDLEWARE
// ========================
app.UseMiddleware<ErrorHandlingMiddleware>();

// Swagger dostupan u svim environmentima
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fenio1 API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ========================
//  DATABASE INICIJALIZACIJA
// ========================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Provjeri može li se spojiti na bazu
        var canConnect = await db.Database.CanConnectAsync();
        logger.LogInformation("Spajanje na bazu: {Status}", canConnect ? "OK" : "GREŠKA");

        if (canConnect)
        {
            // Baza postoji - provjeri ima li Users tablicu
            var usersExist = await db.Users.AnyAsync();
            logger.LogInformation("Baza pronađena, korisnika: {Count}",
                await db.Users.CountAsync());
        }
        else
        {
            // Baza ne postoji - kreiraj je s EnsureCreated (uključuje seed)
            logger.LogInformation("Kreiram novu bazu...");
            await db.Database.EnsureCreatedAsync();
            logger.LogInformation("Baza kreirana s seed podacima.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Greška pri inicijalizaciji baze: {Message}", ex.Message);
        // Ne prekidaj startup - možda baza postoji ali ima manji problem
    }
}

app.Run();
