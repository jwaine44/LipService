#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace LipService.Models;

public class LipServiceContext : DbContext 
{ 
    public LipServiceContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users { get; set; } 
    public DbSet<Song> Songs { get; set; }
}