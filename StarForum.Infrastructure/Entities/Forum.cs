namespace StarForum.Infrastructure.Entities;

public class Forum
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime Created { get; set; }
    public List<Post> Posts { get; set; }
}