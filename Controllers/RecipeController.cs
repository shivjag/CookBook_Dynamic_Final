[16:14, 11/9/2025] Shivani Usf:  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using CookBook_CSharp.Data;
  using CookBook_CSharp.Models;

  namespace CookBook_CSharp.Controllers
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

              // Apply filters based on recipe status
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
                  // "all" shows everything (default)
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
          public async Task<IActionResult> Create(Recipe recipe, string 
  ingredientsString)
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

              ViewBag.IngredientsString = string.Join(", ",
  recipe.Ingredients.Select(i => i.Name));
              return View(recipe);
          }

          [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Edit(int id, Recipe recipe, 
  string ingredientsString)
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
                  _context.Ingredi…
[16:14, 11/9/2025] Shivani Usf: Controllers/HomeController.cs


using System.Diagnostics;
  using Microsoft.AspNetCore.Mvc;
  using CookBook_CSharp.Models;

  namespace CookBook_CSharp.Controllers;

  public class HomeController : Controller
  {
      private readonly ILogger<HomeController> _logger;

      public HomeController(ILogger<HomeController> logger)
      {
          _logger = logger;
      }

      public IActionResult Index()
      {
          return View();
      }

      public IActionResult About()
      {
          return View();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, 
  NoStore = true)]
      public IActionResult Error()
      {
          return View(new ErrorViewModel { RequestId = Activity.Current?.Id
  ?? HttpContext.TraceIdentifier });
      }
  }