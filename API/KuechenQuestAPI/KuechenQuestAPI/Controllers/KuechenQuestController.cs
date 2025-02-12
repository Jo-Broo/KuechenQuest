using KuechenQuestAPI.Classes;
using KuechenQuestAPI.Models;
using Microsoft.AspNetCore.Mvc;
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
            DataPackage package = new DataPackage();
            package.Payload = this.database.Login(username, password);
            if (package.Payload == null) { package.Error = true; }

            return JsonSerializer.Serialize(package);
        }

        [HttpGet("User/Test/{ID}")]
        public string GetUser(int ID)
        {
            return JsonSerializer.Serialize(this.database.GetUser(ID));
        }
    }
}
