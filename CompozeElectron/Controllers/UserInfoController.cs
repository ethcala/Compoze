using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CompozeData.Services;
using CompozeData.Models;

namespace CompozeElectron.Controllers;
public class UserInfoController : Controller
{
    private readonly CompozeService dal;
    public UserInfoController(CompozeService dal)
    {
        this.dal = dal;
    }
    [Authorize]
    public IActionResult Dashboard() 
    {
        ProjectListViewModel model = new ProjectListViewModel();
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        model.Projects = dal.GetUserProjects(userId);
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