using KuechenQuestAPI.Classes;
using KuechenQuestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Agreement.JPake;
using System.Text.Json;

namespace KuechenQuestAPI.Controllers
{
    [Route("KuechenQuest")]
    public class KuechenQuestController : ControllerBase
    {
        private Database database = new Database("SERVER=localhost;DATABASE=KuechenQuest;UID=root;PASSWORD=;");

        [HttpGet("User/Login/{username}/{password}")]
        public string Login(string username, string password)
        {
            return JsonSerializer.Serialize(this.database.Login(username, password));
        }

        [HttpGet("Recipe/Get/{id}")]
        public string GetRecipe(int id)
        {
            return JsonSerializer.Serialize(this.database.GetRecipe(id));
        }

        [HttpGet("Recipe/Get")]
        public string GetAllRecipes()
        {
            return JsonSerializer.Serialize(this.database.GetAllRecipes());
        }

        [HttpDelete("Recipe/Delete/{id}")]
        public string DeleteRecipe(int id) 
        {
            return JsonSerializer.Serialize(this.database.DeleteRecipe(id));
        }

        [HttpPost("Recipe/Create/{json}")]
        public string CreateRecipe(string json)
        {
            DataPackage package = JsonSerializer.Deserialize<DataPackage>(json) ?? new DataPackage();
            Recipe recipe = Recipe.CreateFromJson(package.Payload.ToString());

            return JsonSerializer.Serialize(this.database.CreateRecipe(recipe));
        }

        [HttpGet("User/Test/{ID}")]
        public string GetUser(int ID)
        {
            return JsonSerializer.Serialize(this.database.GetUser(ID));
        }
    }
}
