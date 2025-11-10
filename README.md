README.md

# CookBook - Recipe Management Web Application

A full-stack recipe management application built with ASP.NET Core MVC. This project lets users search for recipes from an external API, manage their own recipe collection with full CRUD functionality, and track cooking statistics.

## Tech Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C#
- **Database**: SQLite
- **ORM**: Entity Framework Core
- **Views**: Razor (.cshtml)
- **API**: Forkify API (free recipe search)

## Project Structure

```
CookBook_CSharp/
├── Controllers/          # C# controllers for handling requests
│   ├── HomeController.cs
│   ├── RecipeController.cs
│   ├── ContactController.cs
│   └── ApiController.cs
├── Models/              # Entity models
│   ├── Recipe.cs
│   ├── Ingredient.cs
│   └── Contact.cs
├── Data/                # Database context
│   └── CookBookDbContext.cs
├── Views/               # Razor views
│   ├── Home/
│   ├── Recipe/
│   ├── Contact/
│   └── Shared/
├── wwwroot/            # Static files (CSS, JS, images)
├── Program.cs          # Application entry point
└── appsettings.json   # Configuration
```

## Features

1. **Recipe Search**: Search thousands of recipes using the Forkify API
2. **My Recipes**: Full CRUD operations - create, view, edit, and delete your own recipes
3. **Ingredients Management**: Add multiple ingredients to each recipe (comma-separated input)
4. **Recipe Status**: Mark recipes as "Tried" or "Favorite" with quick-toggle buttons
5. **Filtering**: View all recipes, or filter by Tried/Untried/Favorites
6. **Statistics Dashboard**: View total recipes, average ratings, and top categories
7. **Contact Form**: Users can submit messages through the contact page

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or any C# IDE

### Installation & Running

1. Clone the repository:
```bash
git clone <repository-url>
cd CookBook_CSharp
```

2. Restore packages:
```bash
dotnet restore
```

3. Run the app:
```bash
dotnet run
```

4. Open your browser:
   - `http://localhost:5000` or `https://localhost:5001`

The SQLite database is created automatically on first run at the project root.

## Database Schema

### Recipes Table
- `id` - Primary key (auto-increment)
- `title` - Recipe name (required, max 200 chars)
- `instructions` - Cooking steps
- `category` - Breakfast, Lunch, Dinner, Dessert, Snack
- `rating` - 0-5 stars
- `is_tried` - Boolean flag
- `is_favorite` - Boolean flag
- `last_cooked_date` - Timestamp
- `cooking_notes` - Additional notes
- `created_at` - Creation timestamp

### Ingredients Table
- `id` - Primary key
- `recipe_id` - Foreign key → recipes(id)
- `name` - Ingredient name
- `quantity` - Amount needed

**Relationship**: One Recipe → Many Ingredients (one-to-many)
Foreign key constraint with `ON DELETE CASCADE` - deleting a recipe removes all its ingredients.

### Contacts Table
- `id` - Primary key
- `name` - Sender name
- `email` - Contact email
- `message` - Message content
- `created_at` - Submission time

## Routes & Endpoints

### Home Pages
- `GET /` - Homepage with recipe search
- `GET /Home/About` - About page with team info
- `GET /Home/Privacy` - Privacy policy

### Recipe CRUD Operations
- `GET /Recipe` or `GET /Recipe/Index?filter={all|tried|untried|favorites}` - List recipes with filtering
- `GET /Recipe/Create` - Show create form
- `POST /Recipe/Create` - Save new recipe
- `GET /Recipe/Edit/{id}` - Show edit form
- `POST /Recipe/Edit/{id}` - Update existing recipe
- `GET /Recipe/Details/{id}` - View single recipe
- `POST /Recipe/Delete/{id}` - Remove recipe
- `POST /Recipe/MarkAsTried/{id}` - Toggle tried status
- `POST /Recipe/ToggleFavorite/{id}` - Toggle favorite status

### API Endpoints (AJAX calls)
- `GET /api/search?query={term}` - Search Forkify API
- `GET /api/recipe/{id}` - Get recipe details from Forkify
- `GET /api/stats` - Get your saved recipe statistics

### Contact
- `GET /Contact` - Contact form
- `POST /Contact/Submit` - Submit message

## How It Works

### MVC Architecture
This project follows the Model-View-Controller pattern:

- **Models** (`Models/`) - C# classes representing data (Recipe, Ingredient, Contact)
- **Views** (`Views/`) - Razor templates (.cshtml) for UI
- **Controllers** (`Controllers/`) - Handle user requests and coordinate between models and views

### Key Components

**RecipeController.cs** - Main controller handling all recipe operations:
- `Index()` - Lists recipes with optional filtering
- `Create()` - GET shows form, POST saves to database
- `Edit(id)` - GET shows populated form, POST updates database
- `Delete(id)` - Removes recipe and related ingredients
- `MarkAsTried(id)` & `ToggleFavorite(id)` - Quick status toggles

**CookBookDbContext.cs** - Entity Framework database context:
- Defines `DbSet<Recipe>`, `DbSet<Ingredient>`, `DbSet<Contact>`
- Configures table/column mappings
- Sets up foreign key relationships

**Recipe.cs** - Main model with:
- Data annotations for validation (`[Required]`, `[StringLength]`)
- Navigation property to ingredients
- Boolean flags for tried/favorite status

### Entity Framework Core
We use EF Core's "Code First" approach:
1. Define models as C# classes
2. Configure relationships in DbContext
3. EF Core automatically creates database tables
4. LINQ queries get translated to SQL

**Example**:
```csharp
// C# LINQ query
var recipes = await _context.Recipes
    .Where(r => r.IsTried)
    .OrderByDescending(r => r.CreatedAt)
    .ToListAsync();

// Gets translated to SQL:
// SELECT * FROM recipes WHERE is_tried = 1 ORDER BY created_at DESC
```

## Challenges We Faced

1. **Learning Entity Framework Core** - Coming from raw SQL, understanding the ORM approach and LINQ queries took time. We had to learn about navigation properties, eager loading with `.Include()`, and how EF tracks changes.

2. **Handling Related Data** - Managing the recipe-ingredient relationship required understanding foreign keys and cascade deletes. We chose the simple approach of deleting all old ingredients and re-adding them on edit rather than implementing a diff algorithm.

3. **Razor Syntax** - Switching from HTML to Razor views with `@model`, `@foreach`, and `@Html.AntiForgeryToken()` was a learning curve. We had to understand how model binding works between forms and controllers.

4. **Async/Await Pattern** - Every database operation in EF Core requires async/await. We had to learn when to use `Task<IActionResult>`, `await`, `.ToListAsync()`, and `.SaveChangesAsync()`.

5. **Security Best Practices** - Implementing anti-forgery tokens on all POST requests and understanding why they're needed to prevent CSRF attacks.

## What We Learned

- How to structure an ASP.NET Core MVC application following best practices
- Working with Entity Framework Core for database operations
- Understanding one-to-many relationships and foreign key constraints
- Model validation using Data Annotations
- Dependency injection with DbContext
- Async programming patterns in C#
- Razor view syntax and layout pages
- Integrating external APIs in ASP.NET Core
- Security considerations (CSRF protection, input validation)

## Team

**Shivani Jagannatham** - Frontend & Views
Designed the UI, worked on Razor layouts, and implemented the recipe cards with filtering buttons.

**Revanth Malisetty** - Backend & Controllers
Built the RecipeController, implemented CRUD operations, and set up routing.

**Rishika Katna** - Database & Models
Created the Entity Framework models, DbContext, relationships, and API integration.

**Praneeth Venkata Sai Eluri** - Configuration & Documentation
Set up the project structure, Program.cs configuration, and project documentation.

---

**Course**: ISM 6225 - Final Project
**Institution**: University of South Florida, Muma College of Business

Built as a course assignment demonstrating ASP.NET Core MVC fundamentals and CRUD operations.