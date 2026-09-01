using Microsoft.EntityFrameworkCore;
using auth11.Models;
namespace auth11.Data;


public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User>User{get;set;}
}