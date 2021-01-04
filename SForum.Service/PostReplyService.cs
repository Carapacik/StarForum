using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SForum.Data;
using SForum.Data.Models;

namespace SForum.Service
{
    public class PostReplyService : IPostReply
    {
        private readonly ApplicationDbContext _context;

        public PostReplyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int id)
        {
            var reply = GetById(id);
            _context.Remove(reply);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, string content)
        {
            var reply = GetById(id);
            if (string.IsNullOrEmpty(content)) reply.Content = content;
            _context.Update(reply);
            await _context.SaveChangesAsync();
        }

        public PostReply GetById(int id)
        {
            return _context.PostReplies
                .Include(r => r.Post)
                .ThenInclude(post => post.Forum)
                .Include(r => r.Post)
                .ThenInclude(post => post.User).First(r => r.Id == id);
        }
    }
}