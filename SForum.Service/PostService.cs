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
            await _context.PostReplies.AddAsync(reply);
            await _context.SaveChangesAsync();
        }

        public async Task Archive(int id)
        {
            var post = GetById(id);
            post.IsArchived = true;
            _context.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var post = GetById(id);
            _context.Remove(post);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(Post post)
        {
            var oldPost = GetById(post.Id);
            oldPost.Title = post.Title;
            oldPost.Content = post.Content;
            _context.Posts.Update(oldPost);
            await _context.SaveChangesAsync();
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
                .Include(post => post.Replies)
                .ThenInclude(reply => reply.User)
                .Include(post => post.Forum).First();
        }

        public IEnumerable<Post> GetFilteredPosts(Forum forum, string searchQuery)
        {
            return string.IsNullOrEmpty(searchQuery)
                ? forum.Posts
                : forum.Posts
                    .Where(post =>
                        post.Title.ToLower().Contains(searchQuery) || post.Content.ToLower().Contains(searchQuery));
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

        public IEnumerable<Post> GetPostsByForumId(int id)
        {
            return _context.Forums.First(forum => forum.Id == id).Posts;
        }

        public IEnumerable<Post> GetPostsByUserId(int id)
        {
            return _context.Posts.Where(post => post.User.Id == id.ToString());
        }

        public int GetReplyCount(int id)
        {
            return GetById(id).Replies.Count();
        }

        public async Task UnArchive(int id)
        {
            var post = GetById(id);
            post.IsArchived = false;
            _context.Update(post);
            await _context.SaveChangesAsync();
        }
    }
}