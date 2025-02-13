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
        public string IMAGE { get; set; } = string.Empty;

        public static Recipe CreateFromJson(string? json)
        {
            if(json == null) { return new Recipe(); }
            return JsonSerializer.Deserialize<Recipe>(json) ?? new Recipe();
        }
    }
}
