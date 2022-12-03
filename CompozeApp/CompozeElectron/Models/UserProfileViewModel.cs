namespace CompozeElectron.Models;

public class UserProfileViewModel
{
    public string UserId { get; set; }
    public string EmailAddress { get; set; }
    public string Name { get; set; }
    public string ProfileImage { get; set; }
    public bool DarkMode { get; set; }
    public string? AuthorName { get; set; }
    public string? ProjectLayout { get; set; }
    public string? CustomColor { get; set; }
    public bool? OneNoteOnly { get; set; }

}