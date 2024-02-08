using Core.Models;
namespace Core.Interfaces;
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Product> Products { get; }
    Task<int> Complete();

}
