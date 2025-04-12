using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Ripple.Data;

namespace Ripple.Features.User.RefreshToken;

public class TokenService : RefreshTokenService<TokenRequest, TokenResponse>
{
    private readonly AppDbContext _context;
    private readonly string _tokenSigningKey;

    public TokenService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _tokenSigningKey = config["JwtSettings:SecretKey"];
        Setup(o =>
        {
            o.TokenSigningKey = _tokenSigningKey;
            o.AccessTokenValidity = TimeSpan.FromMinutes(30);
            o.RefreshTokenValidity = TimeSpan.FromHours(6);

            o.Endpoint("/user/refresh-token",
                ep =>
                {
                    ep.Summary(s => s.Summary = "Refresh token endpoint");
                });
        });
    }

    public override async Task PersistTokenAsync(TokenResponse response)
    {
        var refreshToken = new Entities.RefreshToken
        {
            Token = response.RefreshToken,
            UserId = response.UserId,
            Expiry = response.RefreshExpiry
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public override async Task RefreshRequestValidationAsync(TokenRequest req)
    {
        var refreshToken = await _context.RefreshTokens
            .Where(t => t.UserId == req.UserId && t.Token == req.RefreshToken)
            .FirstOrDefaultAsync();

        if (refreshToken == null)
        {
            AddError(r => r.RefreshToken, "Refresh token is invalid or not found!");
            return;
        }

        if (refreshToken.Expiry < DateTime.UtcNow)
        {
            AddError(r => r.RefreshToken, "Refresh token has expired!");
        }
    }

    public override Task SetRenewalPrivilegesAsync(TokenRequest request, UserPrivileges privileges)
    {
        return Task.CompletedTask;
    }
}