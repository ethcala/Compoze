using CompozeData.Models;

namespace CompozeElectron.Models;

public class TemplateListViewModel
{
    public List<Template> Templates = new List<Template>();
    public TemplateListViewModel()
    {
        this.Templates = new List<Template>();
    }
}