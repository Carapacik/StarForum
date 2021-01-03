using System.Collections.Generic;
using System.Threading.Tasks;
using SForum.Data.Models;

namespace SForum.Data
{
    public interface IPost
    {
        Task Add(Post post);
        Task AddReply(PostReply reply);
        Task Archive(int id);
        Task Delete(int id);
        Task Edit(Post post);
        Post GetById(int id);
        int GetReplyCount(int id);
        IEnumerable<Post> GetAll();
        IEnumerable<Post> GetPostsByUserId(int id);
        IEnumerable<Post> GetPostsByForumId(int id);
        IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery);
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        IEnumerable<Post> GetLatestPosts(int numberPosts);
        Task UnArchive(int id);
    }
}