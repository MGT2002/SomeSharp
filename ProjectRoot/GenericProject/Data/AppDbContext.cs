using Microsoft.EntityFrameworkCore;
using GenericProject.WebApi.Models;

namespace GenericProject.WebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks { get; set; }
    }
}
