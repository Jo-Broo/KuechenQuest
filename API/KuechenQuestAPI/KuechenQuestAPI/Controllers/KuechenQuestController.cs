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

        #region Endpoints

        #region User
        [HttpGet("User/Login/{username}/{password}")]
        public string Login(string username, string password)
        {
            return JsonSerializer.Serialize(this.database.Login(username, password));
        }
        [HttpPost("User/Register/{username}/{email}/{password}")]
        public string Register(string username, string email, string password)
        {
            return JsonSerializer.Serialize(this.database.Register(username, email, password));
        }
        #endregion

        #region Rezept
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
            Recipe recipe = Recipe.CreateFromJson(package.Payload?.ToString());

            return JsonSerializer.Serialize(this.database.CreateRecipe(recipe));
        }

        [HttpPut("Recipe/Update/{json}")]
        public string UpdateRecipe(string json)
        {
            DataPackage package = JsonSerializer.Deserialize<DataPackage>(json) ?? new DataPackage();
            Recipe recipe = Recipe.CreateFromJson(package.Payload?.ToString());

            return JsonSerializer.Serialize(this.database.UpdateRecipe(recipe));
        }
        #endregion

        #region Utensilien
        [HttpGet("Utensil/Get/{id}")]
        public string GetUtensil(int id) 
        { 
            return JsonSerializer.Serialize(this.database.GetUtensil(id));
        }
        [HttpGet("Utensil/Get")]
        public string GetAllUtensil() 
        { 
            return JsonSerializer.Serialize(this.database.GetAllUtensils()); 
        }
        //[HttpDelete("Utensil/Delete/{id}")]
        //public string DeleteUtensil(int id) 
        //{ 
        //    return JsonSerializer.Serialize(this.database.DeleteUntensil(id));
        //}
        [HttpPost("Utensil/Create/{json}")]
        public string CreateUtensil(string json) 
        { 
            DataPackage package = JsonSerializer.Deserialize<DataPackage>(json) ?? new DataPackage();
            Utensil utensil = Utensil.CreateFromJson(package.Payload?.ToString());

            return JsonSerializer.Serialize(this.database.CreateUtensil(utensil)); 
        }
        [HttpPut("Utensil/Update/{json}")]
        public string UpdateUtensil(string json) 
        {
            DataPackage package = JsonSerializer.Deserialize<DataPackage>(json) ?? new DataPackage();
            Utensil utensil = Utensil.CreateFromJson(package.Payload?.ToString());

            return JsonSerializer.Serialize(this.database.UpdateUtensil(utensil));
        }
        #endregion

        #region Zutaten
        [HttpGet("Ingredient/Get/{id}")]
        public string GetIngredient(int id)
        {
            return JsonSerializer.Serialize(this.database.GetIngredient(id));
        }
        [HttpGet("Ingredient/Get")]
        public string GetAllIngredient()
        {
            return JsonSerializer.Serialize(this.database.GetAllIngredients());
        }
        //[HttpDelete("Ingredient/Delete/{id}")]
        //public string DeleteIngredient(int id)
        //{
        //    return JsonSerializer.Serialize(this.database.DeleteIngredient(id));
        //}
        [HttpPost("Ingredient/Create/{json}")]
        public string CreateIngredient(string json)
        {
            DataPackage package = JsonSerializer.Deserialize<DataPackage>(json) ?? new DataPackage();
            Ingredient ingredient = Ingredient.CreateFromJson(package.Payload?.ToString());

            return JsonSerializer.Serialize(this.database.CreateIngredient(ingredient));
        }
        [HttpPut("Ingredient/Update/{json}")]
        public string UpdateIngredient(string json)
        {
            DataPackage package = JsonSerializer.Deserialize<DataPackage>(json) ?? new DataPackage();
            Ingredient ingredient = Ingredient.CreateFromJson(package.Payload?.ToString());

            return JsonSerializer.Serialize(this.database.UpdateIngredient(ingredient));
        }
        #endregion

        #endregion
    }
}
