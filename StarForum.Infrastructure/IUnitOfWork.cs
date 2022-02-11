namespace StarForum.Infrastructure;

public interface IUnitOfWork
{
    Task Commit();
}