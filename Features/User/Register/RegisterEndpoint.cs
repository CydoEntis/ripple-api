using Ripple.Entities;

namespace Ripple.Features.User.Register;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly RegisterData _registerData;

    public RegisterEndpoint(RegisterData registerData)
    {
        _registerData = registerData;
    }

    public override void Configure()
    {
        Post("/users/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (await _registerData.EmailExists(req.Email))
        {
            AddError("Email is already in use.");
        }

        if (await _registerData.UserNameExists(req.UserName))
        {
            AddError("Username is already taken.");
        }

        ThrowIfAnyErrors();

        var user = new AppUser
        {
            Email = req.Email,
            UserName = req.UserName,
        };

        var result = await _registerData.CreateUser(user, req.Password);

        if (!result.Succeeded)
        {
            AddError(string.Join(" | ", result.Errors.Select(e => e.Description)));
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendOkAsync(ct);
    }
}