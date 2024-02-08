using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Infrastructure.Data;

public class AppDbContext  : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
    {
    }
    public virtual DbSet<AppUser> AppUsers { get; set; }
    public virtual DbSet<Product> Products { get; set; }

}
