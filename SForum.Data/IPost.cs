using System.Collections.Generic;
using System.Threading.Tasks;
using SForum.Data.Models;

namespace SForum.Data
{
    public interface IPost
    {
        Task Add(Post post);
        Task Archive(int id);
        Task Delete(int id);
        Task EditPostContent(int id, string content);

        Post GetById(int id);
        IEnumerable<Post> GetAll();
        IEnumerable<Post> GetFilteredPosts(string searchQuery);
        IEnumerable<Post> GetPostsByForum(int id);
        IEnumerable<Post> GetLatestPosts(int numberPosts);


        //Task AddReply(PostReply reply);
    }
}