using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookBook_Dynamic_Final.Models
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Recipe")]
        [Column("recipe_id")]
        public int RecipeId { get; set; }

        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(100)]
        public string? Quantity { get; set; }

        public Recipe Recipe { get; set; } = null!;
    }
}
