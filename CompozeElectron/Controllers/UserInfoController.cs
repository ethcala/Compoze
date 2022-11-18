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
    // public IActionResult Dashboard(bool searchMode = false, ProjectListViewModel model = null, DocumentListViewModel docs = null) 
    public IActionResult Dashboard(bool searchMode, string search) 
    {
        ViewBag.Dal = dal;
        ViewBag.SearchMode = searchMode;

        ProjectListViewModel model = new ProjectListViewModel();
        DocumentListViewModel documents = new DocumentListViewModel();
            
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

        if(!searchMode)
        {
            model = new ProjectListViewModel();
            model.Projects = dal.GetUserProjects(userId);
        } else
        {
            List<object>[] lists = dal.SearchDocuments(search, userId);

            model.Projects = lists[0].Cast<Project>().ToList();
            documents.Documents = lists[1].Cast<Document>().ToList();
        }

        if(model.Projects.Count > 0)
        {
            string latestId = model.Projects.OrderByDescending(p => p.EditDate).Take(1).First().ProjectId;
            Project latestProj = dal.GetProjectById(latestId);
            ViewBag.LastProject = latestProj;
        }
            
        ViewBag.ProjectListModel = model;
        ViewBag.DocumentListViewModel = documents;

        return View(model);
    }

    // create user settings document for users and present it to user
    [Authorize]
    public IActionResult UserSettings() 
    {
        ViewBag.Dal = dal;
        
        // if no user then create user. if user then get user
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = dal.GetUser(userId);
        if(user.OneNoteOnly == null)
        {
            user.OneNoteOnly = false;
            dal.UpdateUser(userId, user);
        }

        if(user == null)
        {
            user = dal.CreateUser(new User(){UserId = userId, DarkMode = false, AuthorName = "", ProjectLayout = "Chapters", CustomColor = "#1c0766", OneNoteOnly = false});
        }

        return View(new UserProfileViewModel()
        {
            UserId = userId,
            Name = User.Identity.Name,
            EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
            ProfileImage = User.FindFirst(c => c.Type == "picture")?.Value,
            DarkMode = user.DarkMode,
            AuthorName = user.AuthorName,
            ProjectLayout = user.ProjectLayout,
            CustomColor = user.CustomColor,
            OneNoteOnly = user.OneNoteOnly
        });
    }

    [HttpPost]
    public IActionResult EditSettings(UserProfileViewModel model)
    {
        User user = dal.GetUser(model.UserId);
        user.DarkMode = model.DarkMode;
        user.AuthorName = model.AuthorName;
        user.ProjectLayout = model.ProjectLayout;
        user.CustomColor = model.CustomColor;
        user.OneNoteOnly = model.OneNoteOnly;

        dal.UpdateUser(model.UserId, user);

        return Redirect("UserSettings");
    }

    // Create new project with default author name and project layout if user has set one
    public IActionResult NewProject()
    {
        ViewBag.Dal = dal;

        string authorName = "";
        string projectLayout = "";
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = dal.GetUser(userId);
        if(user != null)
        {
            authorName = user.AuthorName;
            projectLayout = user.ProjectLayout;
        }
        ViewBag.AuthorName = authorName;
        ViewBag.ProjectLayout = projectLayout;
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
        ViewBag.Dal = dal;

        Project thisProject = dal.GetProjectById(projectId);
        if(thisProject.Categories == null || thisProject.Categories == "")
        {
            switch(thisProject.DocumentLayout)
            {
                case "Chapters":
                    thisProject.Categories = "Chapters";
                    break;
                case "Scenes":
                    thisProject.Categories = "Scenes";
                    break;
                case "Connected":
                    thisProject.Categories = "Parts";
                    break;
                default:
                    thisProject.Categories = "Chapters";
                    break;
            }
            thisProject.Categories += "[=]Notes";
        }
        ViewBag.ThisProject = thisProject;

        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        
        ProjectListViewModel model = new ProjectListViewModel();
        model.Projects = dal.GetUserProjects(userId);
        ViewBag.ProjectListModel = model;

        DocumentListViewModel docModel = new DocumentListViewModel();
        docModel.Documents = dal.GetDocumentsByProjectId(projectId);
        ViewBag.DocumentListViewModel = docModel;

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
        return RedirectToAction("Dashboard", new {searchMode = false, search = ""});
    }
    [HttpPost]
    public IActionResult CreateCategory(string projectId, string category)
    {
        Project updated = dal.GetProjectById(projectId);

        Console.WriteLine(updated.Categories);

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

        return RedirectToAction("Document", new {documentId = doc.DocumentId});
    }

    [HttpGet]
    [Route("UserInfo/Document/{documentId}")]
    public IActionResult Document(string documentId, string lastEdit = "")
    {
        ViewBag.Dal = dal;
        
        Document doc = dal.GetDocumentById(documentId);
        if(doc.DocumentNotes == null)
        {
            doc.DocumentNotes = new List<string>();
        }
        ViewBag.ThisDocument = doc;
        
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        User user = dal.GetUser(userId);

        ProjectListViewModel model = new ProjectListViewModel();
        model.Projects = dal.GetUserProjects(userId);
        ViewBag.ProjectListModel = model;

        ViewBag.OneNoteOnly = user.OneNoteOnly;
        ViewBag.EditedMessage = lastEdit;
        ViewBag.DarkMode = dal.DoesUserUseDarkMode(userId);

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
    public IActionResult UpdateDocumentCategory(Document newDoc)
    {
        string id = dal.UpdateDocument(newDoc.DocumentId, newDoc);

        Project editProject = dal.GetProjectById(newDoc.ProjectId);
        editProject.EditDate = DateTime.Now;
        dal.UpdateProject(newDoc.ProjectId, editProject);

        return RedirectToAction("Project", new {projectId = newDoc.ProjectId});
    }

    [HttpPost]
    public IActionResult DeleteDocument(string docId)
    {
        string projId = dal.GetDocumentById(docId).ProjectId;
        dal.DeleteDocument(docId);
        return RedirectToAction("Project", new {projectId = projId});
    }

    [HttpPost]
    public IActionResult CreateNote(string documentId, string newNote, string documentContent)
    {
        Document doc = dal.GetDocumentById(documentId);
        if(doc.DocumentNotes == null)
        {
            doc.DocumentNotes = new List<string>();
        }
        doc.DocumentNotes.Add(newNote);
        doc.DocumentContent = documentContent;
        dal.UpdateDocument(documentId, doc);
        return RedirectToAction("Document", new {documentId = documentId});
    }

    [HttpPost]
    public IActionResult UpdateNote(string documentId, string originalNote, string updatedNote, string documentContent)
    {
        Document doc = dal.GetDocumentById(documentId);
        for (int i = 0; i < doc.DocumentNotes.Count; i++)
        {
            if(doc.DocumentNotes[i] == originalNote)
            {
                doc.DocumentNotes[i] = updatedNote;
            }
        }
        doc.DocumentContent = documentContent;

        dal.UpdateDocument(documentId, doc);
        return RedirectToAction("Document", new {documentId = documentId});
    }

    [HttpPost]
    public IActionResult DeleteCategory(string projectId, string deletedName, string projectCategories)
    {
        if(projectCategories.StartsWith(deletedName))
        {
            projectCategories = projectCategories.Remove(0, deletedName.Length+3);
        } else {
            int index = projectCategories.IndexOf(projectCategories);
            int startInd = index - 3;
            int endInd = index + deletedName.Length - 1;
            projectCategories = projectCategories.Remove(startInd, endInd);
        }

        Project proj = dal.GetProjectById(projectId);
        proj.Categories = projectCategories;

        dal.UpdateProject(projectId, proj);

        return RedirectToAction("Project", new {projectId = projectId});
    }

    [HttpPost]
    public IActionResult UpdateCategoryName(string projectId, string updatedName, string oldName, string projectCategories)
    {
        List<Document> docs = dal.GetDocumentsByProjectId(projectId);
        foreach (Document doc in docs) {
            if(doc.DocumentCategory == oldName) {
                doc.DocumentCategory = updatedName;
                dal.UpdateDocument(doc.DocumentId, doc);
            }
        }

        string newCategories = "";
        int i = 0;

        foreach(string cat in projectCategories.Split("[=]")) {
            if(i != 0) {
                newCategories += "[=]";
            }

            if(cat == oldName) {
                newCategories += updatedName;
            } else {
                newCategories += cat;
            }
            i++;
        }

        dal.UpdateCategoryName(projectId, newCategories);

        return RedirectToAction("Project", new {projectId = projectId});
    }

    [HttpPost]
    public IActionResult SearchDocuments(string search)
    {
        return RedirectToAction("Dashboard", new {searchMode = true, search = search});
    }

    [HttpPost]
    public IActionResult CreateTemplate(string documentId, string templateName, string templateCategory, string templateContent)
    {
        string userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        Template template = new Template() { TemplateName = templateName, TemplateCategory = templateCategory, TemplateContent = templateContent, UserId = userId };
        dal.CreateTemplate(template);

        return RedirectToAction("Document", new {documentId = documentId});
    }
}