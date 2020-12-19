using System.Threading.Tasks;
using SForum.Data.Models;

namespace SForum.Data
{
    public interface IPostReply
    {
        PostReply GetById(int id);
        Task Edit(int id, string message);
        Task Delete(int id);
    }
}