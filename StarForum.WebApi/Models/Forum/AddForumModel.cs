namespace StarForum.WebApi.Models.Forum;

public class AddForumModel
{
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile ImageUpload { get; set; }
}