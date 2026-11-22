using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ExoApi.Controllers;

public sealed record TokenRequest(
    [System.ComponentModel.DataAnnotations.Required] string Usuario,
    [System.ComponentModel.DataAnnotations.Required] string Senha);
public sealed record TokenResponse(string AccessToken, string Tipo, DateTime ExpiraEmUtc);

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IConfiguration configuration) : ControllerBase
{
    // Credenciais de demonstração vêm de variáveis de ambiente; nunca grave senhas ou chaves no repositório.
    [AllowAnonymous]
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<TokenResponse> EmitirToken(TokenRequest request)
    {
        var expectedUser = configuration["DemoAuth:User"];
        var expectedPassword = configuration["DemoAuth:Password"];
        if (string.IsNullOrWhiteSpace(expectedUser) || string.IsNullOrWhiteSpace(expectedPassword) ||
            !CryptographicEquals(request.Usuario, expectedUser) || !CryptographicEquals(request.Senha, expectedPassword))
            return Unauthorized(new ProblemDetails { Status = 401, Title = "Credenciais inválidas" });

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(30);
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, expectedUser),
            new Claim(ClaimTypes.Role, configuration["DemoAuth:Role"] ?? "User")
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"] ?? "ExoApi",
            audience: configuration["Jwt:Audience"] ?? "ExoApiClient",
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expires));
    }

    private static bool CryptographicEquals(string supplied, string expected)
    {
        var left = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(supplied));
        var right = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        return System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(left, right);
    }
}
