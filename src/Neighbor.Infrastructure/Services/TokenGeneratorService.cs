using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Neighbor.Contract.Abstractions.Services;
using Neighbor.Contract.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Neighbor.Infrastructure.Services;

public sealed class TokenGeneratorService : ITokenGeneratorService
{
    private readonly AuthenticationSetting _authenticationConfiguration;

    public TokenGeneratorService(IOptions<AuthenticationSetting> authenticationConfiguration)
    {
        _authenticationConfiguration = authenticationConfiguration.Value;
    }

    public string GenerateToken
        (string secretKey, string issuer, string audience, double expirationMinutes, IEnumerable<Claim>? claims = null)
    {
        SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new
            (issuer, audience, claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(expirationMinutes), credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateAccessToken(Guid userId, int roleName)
    {
        List<Claim> claims = new() {
            new Claim("UserId", userId.ToString()),
            new Claim(ClaimTypes.Role, roleName.ToString())
        };
        if (_authenticationConfiguration.AccessSecretToken != null && _authenticationConfiguration.Issuer != null && _authenticationConfiguration.Audience != null)
            return GenerateToken
                (_authenticationConfiguration.AccessSecretToken,
                _authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                _authenticationConfiguration.AccessTokenExpMinute,
                claims);
        return null;
    }

    public string GenerateRefreshToken(Guid userId, int roleName)
    {
        List<Claim> claims = new() {
            new Claim("UserId", userId.ToString()),
            new Claim(ClaimTypes.Role, roleName.ToString())
        };
        if (_authenticationConfiguration.RefreshSecretToken != null && _authenticationConfiguration.Issuer != null && _authenticationConfiguration.Audience != null)
            return GenerateToken
                (_authenticationConfiguration.RefreshSecretToken,
                _authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                _authenticationConfiguration.RefreshTokenExpMinute,
                claims);
        return null;
    }

    public string ValidateAndGetUserIdFromRefreshToken(string refreshToken)
    {
        TokenValidationParameters validationParameters = new()
        {
            IssuerSigningKey = new SymmetricSecurityKey
            (Encoding.UTF8.GetBytes(_authenticationConfiguration.RefreshSecretToken)),
            ValidIssuer = _authenticationConfiguration.Issuer,
            ValidAudience = _authenticationConfiguration.Audience,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero,
        };

        JwtSecurityTokenHandler tokenHandler = new();
        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "UserId");
            return userIdClaim?.Value;
        }
        catch (Exception)
        {
            return null;
        }
    }

}
