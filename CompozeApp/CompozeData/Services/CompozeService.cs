using CompozeData.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CompozeData.Services;
public class CompozeService
{
    private readonly IMongoCollection<Document> _documentsCollection;
    private readonly IMongoCollection<Project> _projectsCollection;
    private readonly IMongoCollection<Template> _templatesCollection;
    private readonly IMongoCollection<User> _usersCollection;

    public CompozeService(IOptions<CompozeDatabaseSettings> compozeDatabaseSettings)
    {
        var mongoClient = new MongoClient(compozeDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(compozeDatabaseSettings.Value.DatabaseName);
        _documentsCollection = mongoDatabase.GetCollection<Document>(compozeDatabaseSettings.Value.DocumentCollectionName);
        _templatesCollection = mongoDatabase.GetCollection<Template>(compozeDatabaseSettings.Value.TemplateCollectionName);
        _projectsCollection = mongoDatabase.GetCollection<Project>(compozeDatabaseSettings.Value.ProjectCollectionName);
        _usersCollection = mongoDatabase.GetCollection<User>(compozeDatabaseSettings.Value.UserCollectionName);
    }

    // Unique Methods
    // Project Methods
    public List<Project> GetUserProjects(string userId) 
    {
        return _projectsCollection.Find(p => p.UserId == userId).ToList();
    }       
    public Project GetProjectById(string projectId)
    {
        Project found = _projectsCollection.Find(p => p.ProjectId == projectId).First();
        return found;
    }

    public void UpdateCategoryName(string projectId, string changedString)
    {
        Project updateProj = GetProjectById(projectId);
        updateProj.Categories = changedString;
        UpdateProject(projectId, updateProj);
    }

    // Template Methods
    public List<Template> GetDefaultTemplates()
    {
        return _templatesCollection.Find(t => t.UserId == "default").ToList();
    }
    public List<Template> GetUserTemplates(string userId)
    {
        return _templatesCollection.Find(t => t.UserId == userId).ToList();
    }
    public Template GetTemplateById(string templateId)
    {
        return _templatesCollection.Find(t => t.TemplateId == templateId).First();
    }

    // Document Methods
    public List<Document> GetDocumentsByProjectId(string projectId)
    {
        return _documentsCollection.Find(d => d.ProjectId == projectId).ToList();
    }
    public Document GetDocumentById(string docId)
    {
        return _documentsCollection.Find(d => d.DocumentId == docId).First();
    }

    // User Methods
    public User CreateUser(User newUser)
    {
        _usersCollection.InsertOne(newUser);
        return GetUser(newUser.UserId);
    }
    public void UpdateUser(string userId, User updated)
    {
        _usersCollection.ReplaceOne(x => x.UserId == userId, updated);
    }
    public User GetUser(string userId)
    {
        return _usersCollection.Find(u => u.UserId == userId).FirstOrDefault();
    }
    public bool DoesUserUseDarkMode(string userId)
    {
        bool darkMode;
        try
        {
            darkMode = _usersCollection.Find(u => u.UserId == userId).First().DarkMode;
        } catch (Exception exception)
        {
            darkMode = false;
        }
        return darkMode;
    }

    // misc methods
    public List<object>[] SearchDocuments(string search, string userId)
    {
        // get all documents to search
        List<Project> allProjects = GetUserProjects(userId);
        List<Document> allDocuments = new List<Document>();
        allProjects.ForEach(p => allDocuments.AddRange(GetDocumentsByProjectId(p.ProjectId)));

        List<Project> searchProjects = new List<Project>();
        List<Document> searchDocuments = new List<Document>();
        allDocuments.ForEach(d => {
            if(d.DocumentContent.Contains(search))
            {
                searchDocuments.Add(d);
                allProjects.ForEach(p => {
                    if(p.ProjectId == d.ProjectId && !searchProjects.Contains(p))
                    {
                        searchProjects.Add(p);
                    }
                });
            }
        });

        List<object>[] lists = new List<object>[] { searchProjects.Cast<object>().ToList(), searchDocuments.Cast<object>().ToList() };
        return lists;
    }

    // Other

    // Get Methods
    public async Task<List<Template>> GetTemplatesAsync() => 
        await _templatesCollection.Find(_ => true).ToListAsync();
    public async Task<List<Project>> GetProjectsAsync() => 
        await _projectsCollection.Find(_ => true).ToListAsync();
    public async Task<List<Document>> GetDocumentsAsync() => 
        await _documentsCollection.Find(_ => true).ToListAsync();

    // Create Methods
    public string CreateTemplate(Template newTemplate) {
        _templatesCollection.InsertOne(newTemplate);
        return newTemplate.TemplateId;        
    }
    public string CreateProject(Project newProject) {
        _projectsCollection.InsertOne(newProject);
        return newProject.ProjectId;
    }
    public string CreateDocument(Document newDocument) {
        _documentsCollection.InsertOne(newDocument);
        return newDocument.DocumentId;
    }

    // Update Methods
    public async Task UpdateTemplateAsync(string id, Template updated) =>
        await _templatesCollection.ReplaceOneAsync(x => x.TemplateId == id, updated);
    public string UpdateProject(string id, Project updated) {
        _projectsCollection.ReplaceOne(x => x.ProjectId == id, updated);
        return id;
    }
    public string UpdateDocument(string id, Document updated) {
        _documentsCollection.ReplaceOne(x => x.DocumentId == id, updated);
        return id;
    }
    // Delete Methods
    public async Task DeleteTemplate(string id) =>
        await _templatesCollection.DeleteOneAsync(x => x.TemplateId == id);
    public void DeleteDocument(string id) =>
        _documentsCollection.DeleteOne(x => x.DocumentId == id);
    public void DeleteProject(string id) {
        List<Document> docs = GetDocumentsByProjectId(id);
        docs.ForEach(doc => DeleteDocument(doc.DocumentId));
        
        _projectsCollection.DeleteOne(x => x.ProjectId == id);
    }
}