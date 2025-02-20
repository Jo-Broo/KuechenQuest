using System.Text.Json;

namespace KuechenQuestAPI.Models
{
    public class Ingredient
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public int CATEGORY { get; set; }
        public float QUANTITY { get; set; }
        public string IMAGE { get; set; } = string.Empty;

        public static Ingredient CreateFromJson(string? json)
        {
            if (json == null) { return new Ingredient(); }
            return JsonSerializer.Deserialize<Ingredient>(json) ?? new Ingredient();
        }
    }
}
