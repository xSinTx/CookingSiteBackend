using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;


namespace Temalabor.Models
{
    public class RecipeContext : DbContext
    {

        public RecipeContext(DbContextOptions<RecipeContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

                modelBuilder.Entity<Recipe>(entity =>
            {
                entity.HasOne(r => r.User).WithMany(u => u.Recipes);
                modelBuilder.Entity<Recipe>()
                    .HasMany(r => r.Ingredients)
                    .WithMany(i => i.Recipes)
                    .UsingEntity<Dictionary<string, object>>(
                        "RecipeIngredient",
                        j => j
                        .HasOne<Ingredient>()
                        .WithMany()
                        .HasForeignKey("IngredientId")
                        .HasConstraintName("FK_RecipeIngredient_Ingredients_IngredientId"),
                        j => j  
                        .HasOne<Recipe>()
                        .WithMany()
                        .HasForeignKey("RecipeId")
                        .HasConstraintName("FK_RecipeIngredient_Recipes_RecipeId"));
            });
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
    }
}

