﻿namespace Ripple.Features.User.Register;

public class RegisterRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}