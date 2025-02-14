using KuechenQuestAPI.Classes;
using KuechenQuestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using System.Text.Json;

namespace KuechenQuestAPI.Controllers
{
    [Route("KuechenQuest/Ingredient")]
    public class IngredientController : ControllerBase
    {
        private Database database = new Database("SERVER=localhost;DATABASE=KuechenQuest;UID=root;PASSWORD=;");

        [HttpGet("{id}")]
        public IActionResult GetIngredient(int id)
        {
            Ingredient? result = this.database.GetIngredientByID(id);
            if(result == null) { return BadRequest(); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpGet]
        public IActionResult GetIngredients()
        {
            List<Ingredient> result = this.database.GetAllIngredients();
            if(result.Count == 0) { return new StatusCodeResult(500); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpPost]
        public IActionResult CreateIngredient([FromBody] Ingredient ingredient)
        {
            Ingredient? result = this.database.CreateIngredient(ingredient);
            if(result == null) { return new StatusCodeResult(500); }
            return CreatedAtAction(nameof(GetIngredient), new { id = result.ID }, result);
        }

        [HttpPut]
        public IActionResult UpdateIngredient([FromBody] Ingredient ingredient)
        {
            Ingredient? result = this.database.UpdateIngredient(ingredient);
            if (result == null) { return new StatusCodeResult(500); }
            return Ok();
        }
    }
}
