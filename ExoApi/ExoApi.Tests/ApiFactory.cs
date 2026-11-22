using System.Security.Claims;
using System.Text.Encodings.Web;
using ExoApi.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExoApi.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"ExoApiTests-{Guid.NewGuid():N}";

    static ApiFactory()
    {
        // Configuração isolada do processo de teste; estes valores nunca são credenciais de produção.
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable("Jwt__Key", "chave-de-teste-nao-usar-em-producao-com-32-bytes");
        Environment.SetEnvironmentVariable("DemoAuth__User", "tester");
        Environment.SetEnvironmentVariable("DemoAuth__Password", "test-password");
        Environment.SetEnvironmentVariable("DemoAuth__Role", "Admin");
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ExoApiContext>();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-de-teste-nao-usar-em-producao-com-32-bytes",
                ["DemoAuth:User"] = "tester",
                ["DemoAuth:Password"] = "test-password",
                ["DemoAuth:Role"] = "Admin"
            }));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ExoApiContext>();
            services.RemoveAll<DbContextOptions<ExoApiContext>>();
            services.AddDbContext<ExoApiContext>(options => options.UseInMemoryDatabase(databaseName));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        });
    }
}

public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Role", out var role) || string.IsNullOrWhiteSpace(role))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-user"),
            new Claim(ClaimTypes.Name, "Aluno de teste"),
            new Claim(ClaimTypes.Role, role.ToString())
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}
