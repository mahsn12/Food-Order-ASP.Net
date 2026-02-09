using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FoodDelivery.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FoodDelivery.API.Services;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}

public class JwtTokenService(
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager) : IJwtTokenService
{
    public async Task<string> CreateTokenAsync(ApplicationUser user)
    {
        var key = configuration["Jwt:Key"] ?? "super-secret-dev-key-with-at-least-32-chars";
        var issuer = configuration["Jwt:Issuer"] ?? "FoodDeliveryAPI";
        var audience = configuration["Jwt:Audience"] ?? "FoodDeliveryClients";
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("fullName", user.FullName)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(12),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
