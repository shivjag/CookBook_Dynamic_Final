using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CookBook_Dynamic_Final.Data;
using System.Text.Json;

namespace CookBook_Dynamic_Final.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly CookBookDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiController(CookBookDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // Search recipes using Forkify API
        [HttpGet("search")]
        public async Task<IActionResult> SearchRecipes([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query parameter is required" });

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(
                    $"https://forkify-api.herokuapp.com/api/v2/recipes?search={Uri.EscapeDataString(query)}");

                if (!response.IsSuccessStatusCode)
                    return StatusCode(500, new { error = "Failed to fetch recipes" });

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ForkifySearchResponse>(content);
                var results = apiResponse?.data?.recipes?.Select(r => new
                {
                    id = r.id,
                    title = r.title,
                    image = r.image_url
                }).ToList() ?? new List<object>();

                return Ok(new { results });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"API error: {ex.Message}" });
            }
        }

        // Get single recipe details
        [HttpGet("recipe/{id}")]
        public async Task<IActionResult> GetRecipeDetails(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://forkify-api.herokuapp.com/api/v2/recipes/{id}");

                if (!response.IsSuccessStatusCode)
                    return StatusCode(500, new { error = "Failed to fetch recipe details" });

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ForkifyRecipeResponse>(content);
                var recipeData = apiResponse?.data?.recipe;

                if (recipeData == null)
                    return NotFound(new { error = "Recipe not found" });

                var ingredients = recipeData.ingredients?.Select(ing => new
                {
                    name = ing.description,
                    amount = ing.quantity ?? 0,
                    unit = ing.unit ?? ""
                }).ToList() ?? new List<object>();

                var recipe = new
                {
                    id = recipeData.id,
                    title = recipeData.title,
                    image = recipeData.image_url,
                    publisher = recipeData.publisher,
                    sourceUrl = recipeData.source_url,
                    servings = recipeData.servings ?? 4,
                    readyInMinutes = recipeData.cooking_time ?? 30,
                    extendedIngredients = ingredients
                };

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"API error: {ex.Message}" });
            }
        }

        // Local DB stats endpoint
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var totalRecipes = await _context.Recipes.CountAsync();
                var avgRating = await _context.Recipes
                    .Where(r => r.Rating.HasValue)
                    .AverageAsync(r => (double?)r.Rating) ?? 0;
                var topCategory = await _context.Recipes
                    .Where(r => !string.IsNullOrEmpty(r.Category))
                    .GroupBy(r => r.Category)
                    .Select(g => new { g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    totalRecipes,
                    avgRating = Math.Round(avgRating, 2),
                    topCategory = topCategory?.Key ?? "None"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Failed to fetch stats: {ex.Message}" });
            }
        }

        // Helper DTOs
        public class ForkifySearchResponse
        {
            public string? status { get; set; }
            public DataContainer? data { get; set; }
            public class DataContainer { public List<RecipeInfo>? recipes { get; set; } }
            public class RecipeInfo { public string? id { get; set; } public string? title { get; set; } public string? image_url { get; set; } }
        }

        public class ForkifyRecipeResponse
        {
            public string? status { get; set; }
            public RecipeDataContainer? data { get; set; }
            public class RecipeDataContainer { public RecipeDetail? recipe { get; set; } }
            public class RecipeDetail
            {
                public string? id { get; set; }
                public string? title { get; set; }
                public string? image_url { get; set; }
                public string? publisher { get; set; }
                public string? source_url { get; set; }
                public int? servings { get; set; }
                public int? cooking_time { get; set; }
                public List<IngredientInfo>? ingredients { get; set; }
            }
            public class IngredientInfo
            {
                public string? description { get; set; }
                public double? quantity { get; set; }
                public string? unit { get; set; }
            }
        }
    }
}
