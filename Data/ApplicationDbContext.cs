using Microsoft.EntityFrameworkCore;
using RandomUserProject.Models;

namespace RandomUserProject.Data
{
    /// <summary>
    /// Autor: Samuel Sabino - 30/09/205
    /// Descriçao: Class responsavel por mapear as configuraçoes do banco
    /// para o Entity.
    /// </summary>
    
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => new { e.FirstName, e.LastName });
            });
        }
    }
}