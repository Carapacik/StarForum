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

        public async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task AddReply(PostReply reply)
        {
            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();
        }   

        public Task Archive(int id)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(int id)
        {
            var post = GetById(id);
            _context.Remove(post);
            await _context.SaveChangesAsync();
        }

        public Task EditPostContent(int id, string newContent)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAll()
        {
            var posts = _context.Posts
                .Include(post => post.User)
                .Include(post => post.Replies)
                .ThenInclude(reply => reply.User)
                .Include(post => post.Forum);
            return posts;
        }

        public Post GetById(int id)
        {
            return _context.Posts.Where(post => post.Id == id)
                .Include(post => post.User)
                .Include(post => post.Replies).ThenInclude(reply => reply.User)
                .Include(post => post.Forum).First();
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
        {
            return string.IsNullOrEmpty(searchQuery)
                ? forum.Posts
                : forum.Posts
                    .Where(post => post.Title.Contains(searchQuery) || post.Content.Contains(searchQuery));
        }

        public IEnumerable<Post> GetFilteredPosts(string searchQuery)
        {
            var query = searchQuery.ToLower();

            return GetAll().Where(post =>
                post.Title.ToLower().Contains(query)
                || post.Content.ToLower().Contains(query));
        }

        public IEnumerable<Post> GetLatestPosts(int numberPosts)
        {
            return GetAll().OrderByDescending(post => post.Created).Take(numberPosts);
        }

        public IEnumerable<Post> GetPostsByForum(int id)
        {
            return _context.Forums.Where(forum => forum.Id == id).First().Posts;
        }
    }
}