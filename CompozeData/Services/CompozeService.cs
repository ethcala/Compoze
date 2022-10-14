using CompozeData.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CompozeData.Services;
public class CompozeService
{
    private readonly IMongoCollection<Document> _documentsCollection;
    private readonly IMongoCollection<Project> _projectsCollection;
    private readonly IMongoCollection<Template> _templatesCollection;

    public CompozeService(IOptions<CompozeDatabaseSettings> compozeDatabaseSettings)
    {
        var mongoClient = new MongoClient(compozeDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(compozeDatabaseSettings.Value.DatabaseName);
        _documentsCollection = mongoDatabase.GetCollection<Document>(compozeDatabaseSettings.Value.DocumentCollectionName);
        _templatesCollection = mongoDatabase.GetCollection<Template>(compozeDatabaseSettings.Value.TemplateCollectionName);
        _projectsCollection = mongoDatabase.GetCollection<Project>(compozeDatabaseSettings.Value.ProjectCollectionName);
    }

    // Unique Methods
    public List<Project> GetUserProjects(string userId) 
    {
        return _projectsCollection.Find(p => p.UserId == userId).ToList();
    }       

    public Project GetProjectById(string projectId)
    {
        Project found = _projectsCollection.Find(p => p.ProjectId == projectId).First();
        return found;
    }
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
        ReplaceOneResult result = _projectsCollection.ReplaceOne(x => x.ProjectId == id, updated);
        return id;
    }
    public async Task UpdateDocumentAsync(string id, Document updated) =>
        await _documentsCollection.ReplaceOneAsync(x => x.DocumentId == id, updated);

    // Delete Methods
    public async Task DeleteTemplate(string id) =>
        await _templatesCollection.DeleteOneAsync(x => x.TemplateId == id);
    public async Task DeleteProject(string id) =>
        await _projectsCollection.DeleteOneAsync(x => x.ProjectId == id);
    public async Task DeleteDocument(string id) =>
        await _documentsCollection.DeleteOneAsync(x => x.DocumentId == id);
}