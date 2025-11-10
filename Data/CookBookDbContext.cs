using Microsoft.EntityFrameworkCore;
using CookBook_Dynamic_Final.Models;

namespace CookBook_Dynamic_Final.Data
{
    public class CookBookDbContext : DbContext
    {
        public CookBookDbContext(DbContextOptions<CookBookDbContext> options)
            : base(options)
        { }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Table configuration
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("recipes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("ingredients");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.HasOne(e => e.Recipe)
                      .WithMany(r => r.Ingredients)
                      .HasForeignKey(e => e.RecipeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("contacts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}
