using Microsoft.EntityFrameworkCore;
using StarForum.Infrastructure;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;

namespace StarForum.Application;

public class PostService : IPost
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public PostService(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Add(Post post)
    {
        _context.Add(post);
        await _unitOfWork.Commit();
    }

    public async Task AddReply(PostReply reply)
    {
        await _context.PostReplies.AddAsync(reply);
        await _unitOfWork.Commit();
    }

    public async Task Archive(int id)
    {
        var post = await GetById(id);
        if (post == null) return;
        post.IsArchived = true;
        _context.Update(post);
        await _unitOfWork.Commit();
    }

    public async Task Delete(int id)
    {
        var post = await GetById(id);
        if (post == null) return;
        _context.Remove(post);
        await _unitOfWork.Commit();
    }

    public async Task Edit(Post post)
    {
        var oldPost = await GetById(post.Id);
        if (oldPost == null) return;
        if (post.Title != null) oldPost.Title = post.Title;
        if (post.Content != null) oldPost.Content = post.Content;
        _context.Posts.Update(oldPost);
        await _unitOfWork.Commit();
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

    public async Task<Post?> GetById(int id)
    {
        return await _context.Posts.Where(post => post.Id == id)
            .Include(post => post.User)
            .Include(post => post.Replies)
            .ThenInclude(reply => reply.User)
            .Include(post => post.Forum).FirstOrDefaultAsync();
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

    public async Task<int> GetReplyCount(int id)
    {
        var post = await GetById(id);
        return post == null ? 0 : post.Replies.Count;
    }

    public async Task UnArchive(int id)
    {
        var post = await GetById(id);
        if (post == null) return;
        post.IsArchived = false;
        _context.Update(post);
        await _unitOfWork.Commit();
    }
}