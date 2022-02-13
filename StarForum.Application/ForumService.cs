using Microsoft.EntityFrameworkCore;
using StarForum.Infrastructure;
using StarForum.Infrastructure.Entities;
using StarForum.Infrastructure.Services;

namespace StarForum.Application;

public class ForumService : IForum
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ForumService(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task Create(Forum forum)
    {
        _context.Add(forum);
        await _unitOfWork.Commit();
    }

    public async Task Delete(int id)
    {
        var forum = await GetById(id);
        if (forum == null) return;
        _context.Remove(forum);
        await _unitOfWork.Commit();
    }

    public async Task Edit(Forum forum)
    {
        var forumOld = await GetById(forum.Id);
        if (forumOld == null) return;
        if (forum.Description != null) forumOld.Description = forum.Description;
        if (forum.Title != null) forumOld.Title = forum.Title;
        if (!string.IsNullOrEmpty(forum.ImageUrl)) forumOld.ImageUrl = forum.ImageUrl;
        _context.Forums.Update(forumOld);
        await _unitOfWork.Commit();
    }

    public async Task<IEnumerable<ApplicationUser>> GetActiveUsers(int id)
    {
        var forum = await GetById(id);
        var posts = forum?.Posts;
        if (posts == null || !posts.Any()) return new List<ApplicationUser>();
        var postUsers = posts.Select(p => p.User);
        var replyUsers = posts.SelectMany(r => r.Replies).Select(p => p.User);
        return postUsers.Union(replyUsers).Distinct();
    }

    public IEnumerable<Forum> GetAll()
    {
        return _context.Forums.Include(forum => forum.Posts);
    }

    public async Task<Forum?> GetById(int id)
    {
        return await _context.Forums.Where(f => f.Id == id)
            .Include(f => f.Posts)
            .ThenInclude(f => f.User)
            .Include(f => f.Posts)
            .ThenInclude(p => p.Replies)
            .ThenInclude(r => r.User)
            .FirstOrDefaultAsync();
    }

    public IEnumerable<Forum> GetPopularForums(int numberForums)
    {
        return GetAll().OrderByDescending(forum => forum.Posts.Count).Take(numberForums);
    }

    public async Task<bool> HasRecentPost(int id)
    {
        var forum = await GetById(id);
        if (forum == null) return false;
        const int hoursAgo = 12;
        var window = DateTime.Now.AddHours(-hoursAgo);
        return forum.Posts.Any(post => post.Created > window);
    }
}