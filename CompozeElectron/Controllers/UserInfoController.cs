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
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        ProjectListViewModel model = new ProjectListViewModel();
        model.Projects = dal.GetUserProjects(userId);
        ViewBag.ProjectListModel = model;
        //ViewBag.LastProject = GetLastProject(userId).Result;
        return View(model);
    }
    
    public Project GetLastProject(string userId)
    {

        var client = new HttpClient();

        var request = new HttpRequestMessage(new HttpMethod("GET"), "https://login.auth0.com/api/v2/users/" + userId);
        var response = client.Send(request);

        //string responseStr = await response.Content.ReadAsStringAsync();

        //Console.WriteLine(responseStr);
        return new Project(){ProjectName = "Test"};
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
        
        // var client = new HttpClient();

        // var request = new HttpRequestMessage(new HttpMethod("PATCH"), "https://login.auth0.com/api/v2/users/" + newProject.UserId);
        // request.Content = new StringContent("{\"lastEdited\": \"" + projectId + "\"}");
        // request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json"); 
        
        // var response = client.SendAsync(request);

        return RedirectToAction("Dashboard","UserInfo");
    }
    [HttpGet]
    [Route("UserInfo/Project/{projectId}")]
    public IActionResult Project(string projectId)
    {
        Project thisProject = dal.GetProjectById(projectId);
        ViewBag.ThisProject = thisProject;
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        
        ProjectListViewModel model = new ProjectListViewModel();
        model.Projects = dal.GetUserProjects(userId);
        ViewBag.ProjectListModel = model;

        DocumentListViewModel docModel = new DocumentListViewModel();
        dal.GetDocumentsByProjectId(projectId);
        ViewBag.DocumentListModel = docModel;

        return View();
    }
    [HttpPost]
    public IActionResult EditProject(Project newProject)
    {
        string id = dal.UpdateProject(newProject.ProjectId, newProject);
        return RedirectToAction("Project", new {projectId = newProject.ProjectId});
    }
    
    [HttpPost]
    public IActionResult DeleteProject(string projId)
    {
        dal.DeleteProject(projId);
        return Redirect("Dashboard");
    }
    [HttpPost]
    public IActionResult CreateCategory(string projectId, string category)
    {
        Project updated = dal.GetProjectById(projectId);
        if(updated.Categories == null || updated.Categories == "")
        {
            updated.Categories = category;
        } 
        else 
        {
            updated.Categories += " " + category;
        }
        dal.UpdateProject(projectId, updated);

        return RedirectToAction("Project", new {projectId = updated.ProjectId});
    }
}