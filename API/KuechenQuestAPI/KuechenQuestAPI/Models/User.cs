namespace KuechenQuestAPI.Models
{
    public class User
    {
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public int LEVEL { get; set; }
        public int XP { get; set; }
        public string EMAIL { get; set; } = string.Empty;
        public string IMAGE { get; set; } = string.Empty;
    }
}
