namespace CompozeData.Models;

public class CompozeDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string ProjectCollectionName { get; set; } = null!;
    public string TemplateCollectionName { get; set; } = null!;
    public string DocumentCollectionName { get; set; } = null!;
}