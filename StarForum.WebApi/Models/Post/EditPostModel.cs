using System.ComponentModel.DataAnnotations;

namespace StarForum.WebApi.Models.Post;

public class EditPostModel
{
    public int Id { get; set; }


    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public string Title { get; set; }

    [StringLength(350, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 3)]
    public string Content { get; set; }

    public string AuthorName { get; set; }
}