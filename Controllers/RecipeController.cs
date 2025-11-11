using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CookBook_Dynamic_Final.Data;
using CookBook_Dynamic_Final.Models;

namespace CookBook_Dynamic_Final.Controllers
{
    public class RecipeController : Controller
    {
        private readonly CookBookDbContext _context;

        public RecipeController(CookBookDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string filter = "all")
        {
            var recipesQuery = _context.Recipes.AsQueryable();

            switch (filter.ToLower())
            {
                case "untried":
                    recipesQuery = recipesQuery.Where(r => !r.IsTried);
                    break;
                case "favorites":
                    recipesQuery = recipesQuery.Where(r => r.IsFavorite);
                    break;
                case "tried":
                    recipesQuery = recipesQuery.Where(r => r.IsTried);
                    break;
            }

            var recipes = await recipesQuery
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.CurrentFilter = filter;
            return View(recipes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Recipe recipe, string ingredientsString)
        {
            if (ModelState.IsValid)
            {
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrWhiteSpace(ingredientsString))
                {
                    var ingredientsList = ingredientsString.Split(',')
                        .Select(i => i.Trim())
                        .Where(i => !string.IsNullOrEmpty(i));

                    foreach (var ingredientName in ingredientsList)
                    {
                        var ingredient = new Ingredient
                        {
                            RecipeId = recipe.Id,
                            Name = ingredientName
                        };
                        _context.Ingredients.Add(ingredient);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Recipe created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            ViewBag.IngredientsString = string.Join(", ", recipe.Ingredients.Select(i => i.Name));
            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Recipe recipe, string ingredientsString)
        {
            if (id != recipe.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Recipes.Update(recipe);

                var existingIngredients = await _context.Ingredients
                    .Where(i => i.RecipeId == id)
                    .ToListAsync();
                _context.Ingredients.RemoveRange(existingIngredients);

                if (!string.IsNullOrWhiteSpace(ingredientsString))
                {
                    var ingredientsList = ingredientsString.Split(',')
                        .Select(i => i.Trim())
                        .Where(i => !string.IsNullOrEmpty(i));

                    foreach (var ingredientName in ingredientsList)
                    {
                        var ingredient = new Ingredient
                        {
                            RecipeId = recipe.Id,
                            Name = ingredientName
                        };
                        _context.Ingredients.Add(ingredient);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Recipe updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Recipe deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Mark recipe as tried/untried
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsTried(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                recipe.IsTried = !recipe.IsTried;
                recipe.LastCookedDate = recipe.IsTried ? DateTime.Now : null;
                await _context.SaveChangesAsync();
                TempData["Success"] = recipe.IsTried ? "Recipe marked as tried!" : "Recipe marked as untried!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Toggle favorite status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                recipe.IsFavorite = !recipe.IsFavorite;
                await _context.SaveChangesAsync();
                TempData["Success"] = recipe.IsFavorite ? "Added to favorites!" : "Removed from favorites!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
