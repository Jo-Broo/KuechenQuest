using KuechenQuestAPI.Classes;
using KuechenQuestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KuechenQuestAPI.Controllers
{
    [Route("KuechenQuest/Utensil")]
    public class UtensilController : ControllerBase
    {
        private Database database = new Database("SERVER=localhost;DATABASE=KuechenQuest;UID=root;PASSWORD=;");

        [HttpGet("{id}")]
        public IActionResult GetUtensil(int id)
        {
            Utensil? result = this.database.GetUtensil(id);
            if (result == null) { return new StatusCodeResult(500); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpGet]
        public IActionResult GetUtensil()
        {
            List<Utensil> result = this.database.GetAllUtensils();
            if (result.Count == 0) { return new StatusCodeResult(500); }
            return Ok(JsonSerializer.Serialize(result));
        }

        [HttpPost]
        public IActionResult CreateUtensil([FromBody] Utensil utensil)
        {
            Utensil? result = this.database.CreateUtensil(utensil);
            if (result == null) { return new StatusCodeResult(500); }
            return CreatedAtAction(nameof(GetUtensil), new { id = result.ID }, result);
        }

        [HttpPut]
        public IActionResult UpdateUtensil([FromBody] Utensil utensil)
        {
            Utensil? result = this.database.UpdateUtensil(utensil);
            if (result == null) { return new StatusCodeResult(500); }
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUtensil(int id)
        {
            return Ok();
        }
    }
}
