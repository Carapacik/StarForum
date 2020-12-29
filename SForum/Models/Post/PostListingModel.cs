using SForum.Models.Forum;

namespace SForum.Models.Post
{
    public class PostListingModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int AuthorRating { get; set; }
        public string AuthorId { get; set; }
        public string DatePosted { get; set; }
        public ForumListingModel Forum { get; set; }
        public int RepliesCount { get; set; }
    }
}