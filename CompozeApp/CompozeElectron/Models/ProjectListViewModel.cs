using CompozeData.Models;

namespace CompozeElectron.Models;

public class ProjectListViewModel
{
    public List<Project> Projects = new List<Project>();
    public ProjectListViewModel()
    {
        this.Projects = new List<Project>();
    }
}