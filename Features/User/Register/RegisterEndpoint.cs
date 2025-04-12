using Microsoft.AspNetCore.Identity;
using Ripple.Entities;

namespace Ripple.Features.User.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterEndpoint(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var existingUserByEmail = await _userManager.FindByEmailAsync(req.Email);
        if (existingUserByEmail is not null)
        {
            AddError("Email is already in use.");
        }

        var existingUserByName = await _userManager.FindByNameAsync(req.UserName);
        if (existingUserByName is not null)
        {
            AddError("Username is already taken.");
        }

        ThrowIfAnyErrors();

        var user = new AppUser
        {
            Email = req.Email,
            UserName = req.UserName,
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            AddError(string.Join(" | ", result.Errors.Select(e => e.Description)));
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendOkAsync(ct);
    }
}