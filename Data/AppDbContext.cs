using Microsoft.EntityFrameworkCore;
using PostQueryService.Models;

namespace PostQueryService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
            
    }

    
    public DbSet<ViewPost> ViewPosts { get; set; }
}