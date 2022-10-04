using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;

public class AccountController : Controller
{
    public async Task Login(string returnUrl = "/") 
    {
        var authProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();
        
        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authProperties);
    }

    [Authorize]
    public async Task Logout(String returnUrl = "/") 
    {
        var authProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();
        
        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

}