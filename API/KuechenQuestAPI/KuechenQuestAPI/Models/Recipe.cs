namespace KuechenQuestAPI.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public List<int> INGREDIENTS { get; set; }
        public List<float> I_QUANTITIES { get; set; }
        public int TIME { get; set; }
        public int DIFFICULTY { get; set; }
        public string INSTRUCTIONS { get; set; } = string.Empty;
        public int RATING { get; set; }
        public int RATINGCOUNT { get; set; }
        public List<int> UTENSILS { get; set; }
        public List<int> U_QUANTITIES { get; set; }
        public object? IMAGE { get; set; }
    }
}
