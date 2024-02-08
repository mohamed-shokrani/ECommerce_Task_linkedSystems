
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
namespace Infrastructure.GenericRepository;
public class UnitOfWork :IUnitOfWork
{
    private readonly AppDbContext _context;
    public IGenericRepository<Product> Products { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new GenericRepository<Product>(_context);
      
    }
    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
