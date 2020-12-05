using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SForum.Data;
using SForum.Data.Models;

namespace SForum.Service
{
    public class PostService : IPost
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task Add(IPost post)
        {
            throw new NotImplementedException();
        }

        public Task Add(Post post)
        {
            throw new NotImplementedException();
        }

        public Task Archive(int id)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditPostContent(int id, string newContent)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPost> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        public Post GetById(int id)
        {
            return _context.Posts.Where(post => post.Id == id)
                .Include(post => post.User)
                .Include(post => post.Replies).ThenInclude(reply => reply.User)
                .Include(post => post.Forum).First();
        }

        public IEnumerable<IPost> GetFilteredPosts(string searchQuary)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetPostsByForum(int id)
        {
            return _context.Forums.Where(forum => forum.Id == id).First().Posts;
        }
    }
}