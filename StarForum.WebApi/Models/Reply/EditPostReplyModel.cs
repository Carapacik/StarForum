namespace StarForum.WebApi.Models.Reply;

public class EditPostReplyModel
{
    public int PostId { get; set; }
    public string PostTitle { get; set; }
    public int ReplyId { get; set; }
    public string AuthorName { get; set; }
    public string Content { get; set; }
}