namespace KuechenQuestAPI.Models
{
    public class DataPackage
    {
        public object? Payload { get; set; }
        public object Error { get; set; } = false;
    }
}
