async function searchRecipes() {
    var query = document.getElementById('searchInput').value;
    var resultsDiv = document.getElementById('results');

    if (!query.trim()) {
      resultsDiv.innerHTML = '<p style="color: #e74c3c; text-align: center;">Please enter a search term</p>';
      return;
    }

    resultsDiv.innerHTML = '<p style="text-align: center; padding: 2rem;">Loading...</p>';

    try {
      var response = await fetch(`/api/search?query=${encodeURIComponent(query)}`);
      var data = await response.json();

      if (data.results && data.results.length > 0) {
        var html = '<div style="grid-column: 1/-1; text-align: center; margin-bottom: 2rem;">';
        html += '<p>Found <strong>' + data.results.length + '</strong> recipes</p></div>';

        for (var i = 0; i < data.results.length; i++) {
          var recipe = data.results[i];
          html += '<div class="recipe-card" onclick="showRecipeDetails(\'' + recipe.id + '\')">';
          html += '<img src="' + recipe.image + '" alt="' + recipe.title + '" style="width: 100%; height: 180px; object-fit: cover;">';
          html += '<h3 style="margin: 1rem;">' + recipe.title + '</h3>';
          html += '<button style="margin: 0 1rem 1rem 1rem; width: calc(100% - 2rem);">View Recipe</button>';
          html += '</div>';
        }

        resultsDiv.innerHTML = html;
      } else {
        resultsDiv.innerHTML = '<div style="text-align: center; padding: 3rem;"><p>No recipes found</p></div>';
      }
    } catch (err) {
      console.error(err);
      resultsDiv.innerHTML = '<p style="color: red; text-align: center;">Error loading recipes</p>';
    }
  }

  async function showRecipeDetails(recipeId) {
    var modal = document.getElementById('recipeModal');
    var detailsDiv = document.getElementById('recipeDetails');

    modal.style.display = 'block';
    detailsDiv.innerHTML = '<div>Loading...</div>';

    try {
      var response = await fetch(`/api/recipe/${recipeId}`);
      var recipe = await response.json();

      var ingredientsList = '';
      for (var i = 0; i < recipe.extendedIngredients.length; i++) {
        var ing = recipe.extendedIngredients[i];
        ingredientsList += '<li>' + ing.amount + ' ' + ing.unit + ' ' + ing.name + '</li>';
      }

      var instructions = recipe.instructions.replace(/\n/g, '<br>');

      var html = '<img src="' + recipe.image + '" style="width: 100%; max-height: 300px; object-fit: cover;">';
      html += '<h2>' + recipe.title + '</h2>';
      html += '<p>Time: ' + recipe.readyInMinutes + ' minutes | Servings: ' + recipe.servings + '</p>';
      html += '<h3>Ingredients</h3>';
      html += '<ul>' + ingredientsList + '</ul>';
      html += '<h3>Instructions</h3>';
      html += '<p>' + instructions + '</p>';

      if (recipe.sourceUrl) {
        html += '<a href="' + recipe.sourceUrl + '" target="_blank">View Full Recipe</a>';
      }

      detailsDiv.innerHTML = html;
    } catch (err) {
      console.error(err);
      detailsDiv.innerHTML = '<p style="color: red;">Error loading details</p>';
    }
  }

  function closeModal() {
    document.getElementById('recipeModal').style.display = 'none';
  }

  window.onclick = function(event) {
    var modal = document.getElementById('recipeModal');
    if (event.target == modal) {
      modal.style.display = 'none';
    }
  }

  document.getElementById('searchInput').addEventListener('keypress', function(e) {
    if (e.key === 'Enter') {
      searchRecipes();
    }
  });
