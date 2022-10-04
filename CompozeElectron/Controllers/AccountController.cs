using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;

public class AccountController : Controller
{
    public async Task Login(string returnUrl = "/") {
        var authProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();
        
        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authProperties);
    }

}