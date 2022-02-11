namespace StarForum.Infrastructure.Entities;

public class Post
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public bool IsArchived { get; set; }
    public DateTime Created { get; set; }
    public ApplicationUser User { get; set; }
    public List<PostReply> Replies { get; set; }
    public Forum Forum { get; set; }
}