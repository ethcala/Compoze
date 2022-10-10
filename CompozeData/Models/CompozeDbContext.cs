using Microsoft.EntityFrameworkCore;

namespace CompozeData.Models
{
    public partial class CompozeDbContext : DbContext
    {
        public CompozeDbContext() { }
        public CompozeDbContext(DbContextOptions<CompozeDbContext> options) : base(options)
        { }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
    }
}