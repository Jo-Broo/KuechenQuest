namespace KuechenQuestAPI.Models
{
    public class Ingredient
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public string CATEGORY { get; set; } = string.Empty;
        public float QUANTITY { get; set; }
    }
}
