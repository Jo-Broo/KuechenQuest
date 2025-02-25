using KuechenQuestAPI.Classes;
using KuechenQuestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KuechenQuestAPI.Controllers
{
    [Route("KuechenQuest/Recipe")]
    public class RecipeController : ControllerBase
    {
        private Database database = new Database("SERVER=localhost;DATABASE=KuechenQuest;UID=root;PASSWORD=;");

        [HttpGet]
        public IActionResult GetRecipes()
        {
            List<Recipe> result = this.database.GetAllRecipes();
            if (result.Count == 0) { return new StatusCodeResult(500); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpGet("{id}")]
        public IActionResult GetRecipe(int id)
        {
            Recipe? result = this.database.GetRecipeByID(id);
            if (result == null) { return new StatusCodeResult(500); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpPost]
        public IActionResult CreateRecipe([FromBody] Recipe recipe)
        {
            Recipe? result = this.database.CreateRecipe(recipe);
            if (result == null) { return new StatusCodeResult(500); }
            return CreatedAtAction(nameof(GetRecipe), new { id = result.ID }, result);
        }

        [HttpPut]
        public IActionResult UpdateRecipe([FromBody] Recipe recipe)
        {
            Recipe? result = this.database.UpdateRecipe(recipe);
            if (result == null) { return new StatusCodeResult(500); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRecipe(int id)
        {
            bool result = this.database.DeleteRecipeByID(id);
            if (result == false) { return new StatusCodeResult(500); }
            return Ok();
        }

        [HttpGet("Difficulty")]
        public IActionResult GetAllDifficultys()
        {
            List<Difficulty> difficulties = this.database.GetDifficulty();
            if(difficulties.Count == 0) { return StatusCode(500); }
            return Ok(JsonSerializer.Serialize(difficulties));
        }
    }
}
