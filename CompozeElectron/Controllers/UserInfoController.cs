using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CompozeElectron.Controllers;
public class UserInfoController : Controller
{
    [Authorize]
    public IActionResult Dashboard() 
    {
        return View();
    }
    [Authorize]
    public IActionResult UserSettings() 
    {
        return View(new UserProfileViewModel()
        {
            Name = User.Identity.Name,
            EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
            ProfileImage = User.FindFirst(c => c.Type == "picture")?.Value
        });
    }
}