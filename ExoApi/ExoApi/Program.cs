using System.Text;
using ExoApi.Data;
using ExoApi.Middleware;
using ExoApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ExoApi")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=ExoApi;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<ExoApiContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ExoApi — Gerenciamento de projetos",
        Version = "v1",
        Description = "API acadêmica para operações CRUD de projetos."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT emitido por /api/auth/token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configure Jwt:Key por variável de ambiente ou segredo local.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key deve ter pelo menos 32 bytes.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ExoApi",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ExoApiClient",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromSeconds(30)
    });

builder.Services.AddAuthorization(options => options.AddPolicy("ProjectDelete", policy =>
    policy.RequireAuthenticatedUser().RequireRole("Admin")));

var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
