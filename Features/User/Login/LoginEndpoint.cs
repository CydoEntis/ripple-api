using Microsoft.AspNetCore.Identity;
using Ripple.Entities;
using FastEndpoints.Security;
using Ripple.Features.User.RefreshToken;

namespace Ripple.Features.User.Login;

public class LoginEndpoint : Endpoint<LoginRequest, TokenResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly string? _secret;

    public LoginEndpoint(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        var jwtSettings = configuration.GetSection("JwtSettings");
        _secret = jwtSettings.GetValue<string>("SecretKey");
    }

    public override void Configure()
    {
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null)
        {
            ThrowError(r => r.Email, "Invalid credentials");
        }

        var signInResult = await _signInManager.PasswordSignInAsync(user, req.Password, false, false);

        if (!signInResult.Succeeded)
        {
            ThrowError(r => r.Email, "Invalid credentials");
        }


        var response = await CreateTokenWith<TokenService>(user.Id, u => { u.Claims.Add(new("UserId", user.Id)); });

        // var jwtToken = JwtBearer.CreateToken(
        //     o =>
        //     {
        //         o.SigningKey = _secret;
        //         o.ExpireAt = DateTime.UtcNow.AddMinutes(30);
        //         o.User.Claims.Add(("UserName", user.UserName));
        //         o.User["UserId"] = user.Id;
        //     });

        // var response = new LoginResponse
        // {
        //     Token = jwtToken
        // };

        await SendAsync(response);
    }
}