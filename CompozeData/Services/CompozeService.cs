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

    // Get Methods
    public async Task<List<Template>> GetTemplatesAsync() => 
        await _templatesCollection.Find(_ => true).ToListAsync();
    public async Task<List<Project>> GetProjectsAsync() => 
        await _projectsCollection.Find(_ => true).ToListAsync();
    public async Task<List<Document>> GetDocumentsAsync() => 
        await _documentsCollection.Find(_ => true).ToListAsync();

    // Create Methods
    public async Task CreateTemplateAsync(Template newTemplate) =>
        await _templatesCollection.InsertOneAsync(newTemplate);
    public async Task CreateProjectAsync(Project newProject) =>
        await _projectsCollection.InsertOneAsync(newProject);
    public async Task CreateDocumentAsync(Document newDocument) =>
        await _documentsCollection.InsertOneAsync(newDocument);

    // Update Methods
    public async Task UpdateTemplateAsync(string id, Template updated) =>
        await _templatesCollection.ReplaceOneAsync(x => x.TemplateId == id, updated);
    public async Task UpdateProjectAsync(string id, Project updated) =>
        await _projectsCollection.ReplaceOneAsync(x => x.ProjectId == id, updated);
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