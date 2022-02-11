namespace StarForum.WebApi.Models.Forum;

public class EditForumModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile ImageUpload { get; set; }
}