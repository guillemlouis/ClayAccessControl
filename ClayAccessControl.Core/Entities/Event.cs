namespace ClayAccessControl.Core.Entities{
    public class Event
    {
        public int EventId { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int DoorId { get; set; }
        public Door Door { get; set; } = null!;
        public string EventType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}

