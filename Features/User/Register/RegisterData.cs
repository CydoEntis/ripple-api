using Microsoft.AspNetCore.Identity;
using Ripple.Entities;

namespace Ripple.Features.User.Register;

public class RegisterData
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterData(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> CreateUser(AppUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<bool> EmailExists(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    public async Task<bool> UserNameExists(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user is not null;
    }
}