using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookBook_Dynamic_Final.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Column(TypeName = "TEXT")]
        public string? Instructions { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [Range(0, 5)]
        public double? Rating { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsTried { get; set; } = false;
        public bool IsFavorite { get; set; } = false;

        [Column("last_cooked_date")]
        public DateTime? LastCookedDate { get; set; }

        [Column(TypeName = "TEXT")]
        public string? CookingNotes { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}
