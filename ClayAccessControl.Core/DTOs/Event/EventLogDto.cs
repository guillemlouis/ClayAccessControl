namespace ClayAccessControl.Core.DTOs{
    public class EventLogDto
    {
        public int EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string DoorName { get; set; } = string.Empty;
        public string OfficeName { get; set; } = string.Empty;
    }
}