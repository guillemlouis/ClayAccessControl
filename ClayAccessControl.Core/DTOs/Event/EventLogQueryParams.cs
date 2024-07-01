namespace ClayAccessControl.Core.DTOs{
    public class EventLogQueryParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? EventType { get; set; }
        public int? UserId { get; set; }
        public int? DoorId { get; set; }
        public int? OfficeId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}