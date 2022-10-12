using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompozeElectron.Models;
using Microsoft.AspNetCore.Authentication;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CompozeData.Services;
using CompozeData.Models;
using System.Net.Http.Headers;

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
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        model.Projects = dal.GetUserProjects(userId);
        ViewBag.Projects = model.Projects;
        return View(model);
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
    public IActionResult NewProject()
    {
        if(!User.Identity.IsAuthenticated) {
            return RedirectToAction("Index","Home");
        }
        return View();
    }
    [HttpPost]
    public IActionResult CreateProject(Project newProject)
    {
        if(newProject.ProjectName == null || newProject.ProjectName == "") {
            newProject.ProjectName = "Untitled";
        }

        if(newProject.AuthorName == null || newProject.AuthorName == "") {
            newProject.AuthorName = "Author";
        }

        newProject.UserId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

        string projectId = dal.CreateProject(newProject);
        
        var client = new HttpClient();

        var request = new HttpRequestMessage(new HttpMethod("PATCH"), "https://login.auth0.com/api/v2/users/" + newProject.UserId);
        request.Content = new StringContent("{\"lastEdited\": \"" + projectId + "\"}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json"); 
        
        var response = client.SendAsync(request);

        Console.WriteLine(response);

        return RedirectToAction("Dashboard","UserInfo");
    }
}