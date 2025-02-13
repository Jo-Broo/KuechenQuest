using System.Text.Json;

namespace KuechenQuestAPI.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public int TIME { get; set; }
        public int DIFFICULTY { get; set; }
        public string INSTRUCTIONS { get; set; } = string.Empty;
        public int RATING { get; set; }
        public int RATINGCOUNT { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Utensil> Utensils { get; set; } = new List<Utensil> { };
        public object? IMAGE { get; set; }

        public static Recipe CreateFromJson(string json)
        {
            return JsonSerializer.Deserialize<Recipe>(json);
        }
    }
}
