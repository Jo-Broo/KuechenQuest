using Microsoft.AspNetCore.Mvc;

namespace KuechenQuestAPI.Controllers
{
    [Route("test")]
    public class TestController : ControllerBase
    {
        // Endpunkt 1
        [HttpGet("hello")]
        public string GetHello()
        {
            return "Hello, World!";
        }

        // Endpunkt 2
        [HttpGet("goodbye")]
        public string GetGoodbye()
        {
            return "Goodbye, World!";
        }

        // Endpunkt 3 mit einem Parameter
        [HttpGet("greet/{name}")]
        public string GreetPerson(string name)
        {
            return $"Hello, {name}!";
        }
    }

}