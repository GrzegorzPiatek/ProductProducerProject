using Microsoft.EntityFrameworkCore;

namespace PWProject.Models
{
    public class DataContext : DbContext
    {
        private IConfiguration _configuration;

        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("SqliteConnectionString"));
        }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Producer)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.ProducerId);
        }
    }
}
