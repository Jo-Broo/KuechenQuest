using System.Text.Json;

namespace KuechenQuestAPI.Models
{
    public class Utensil
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public float QUANTITY { get; set; }
        public string IMAGE { get; set; } = string.Empty;
    }
}
