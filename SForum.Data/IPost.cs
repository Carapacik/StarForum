using System.Collections.Generic;
using System.Threading.Tasks;
using SForum.Data.Models;

namespace SForum.Data
{
    public interface IPost
    {
        Post GetById(int id);
        IEnumerable<IPost> GetAll(int id);
        IEnumerable<IPost> GetFilteredPosts(string searchQuary);
        IEnumerable<Post> GetPostsByForum(int id);

        Task Add(IPost post);
        Task Delete(int id);
        Task EditPostContent(int id, string newContent);

        //Task AddReply(PostReply reply);
    }
}