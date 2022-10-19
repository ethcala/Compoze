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

        if(model.Projects.Count > 0)
        {
            string latestId = model.Projects.OrderByDescending(p => p.EditDate).Take(1).First().ProjectId;
            Project latestProj = dal.GetProjectById(latestId);
            ViewBag.LastProject = latestProj;
        }
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
        newProject.EditDate = DateTime.Now;

        string projId = dal.CreateProject(newProject);

        return RedirectToAction("Project", new {projectId = projId});
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
        docModel.Documents = dal.GetDocumentsByProjectId(projectId);
        ViewBag.DocumentListModel = docModel;

        TemplateListViewModel templateModel = new TemplateListViewModel();
        templateModel.Templates.AddRange(dal.GetDefaultTemplates());
        templateModel.Templates.AddRange(dal.GetUserTemplates(userId));
        ViewBag.TemplateListModel = templateModel;

        return View();
    }
    [HttpPost]
    public IActionResult EditProject(Project newProject)
    {
        newProject.EditDate = DateTime.Now;
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
            updated.Categories += "[=]" + category;
        }
        updated.EditDate = DateTime.Now;
        dal.UpdateProject(projectId, updated);

        return RedirectToAction("Project", new {projectId = updated.ProjectId});
    }

    [HttpPost]
    public IActionResult CreateDocument(string projectId, string category, string docName, string template)
    {
        if(docName == "") docName = "Untitled";
        string templateContent = "";
        if(template != "None" && template != null && template != "")
        {
            templateContent = dal.GetTemplateById(template).TemplateContent;
        }
        Document doc = new Document(){ProjectId = projectId, DocumentName = docName, DocumentCategory = category, DocumentContent = templateContent};
        dal.CreateDocument(doc);
        
        Project editProject = dal.GetProjectById(projectId);
        editProject.EditDate = DateTime.Now;
        dal.UpdateProject(projectId, editProject);

        return RedirectToAction("Project", new {projectId = projectId});
    }

    [HttpGet]
    [Route("UserInfo/Document/{documentId}")]
    public IActionResult Document(string documentId, string lastEdit = "")
    {
        Document doc = dal.GetDocumentById(documentId);
        ViewBag.ThisDocument = doc;
        
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        ProjectListViewModel model = new ProjectListViewModel();
        model.Projects = dal.GetUserProjects(userId);
        ViewBag.ProjectListModel = model;

        ViewBag.EditedMessage = lastEdit;

        return View();
    }

    [HttpPost]
    public IActionResult EditDocument(Document newDoc)
    {
        string id = dal.UpdateDocument(newDoc.DocumentId, newDoc);
        string edit = "Last saved at " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + ".";

        Project editProject = dal.GetProjectById(newDoc.ProjectId);
        editProject.EditDate = DateTime.Now;
        dal.UpdateProject(newDoc.ProjectId, editProject);

        return RedirectToAction("Document", new {documentId = newDoc.DocumentId, lastEdit = edit});
    }

    [HttpPost]
    public IActionResult DeleteDocument(string docId)
    {
        string projId = dal.GetDocumentById(docId).ProjectId;
        dal.DeleteDocument(docId);
        return RedirectToAction("Project", new {projectId = projId});
    }
}