using CompozeData.Models;

namespace CompozeElectron.Models;
public class DocumentListViewModel
{
    public List<Document> Documents = new List<Document>();
    public DocumentListViewModel()
    {
        this.Documents = new List<Document>();
    }
}