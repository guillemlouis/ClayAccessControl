namespace ClayAccessControl.Core.Entities{
    public class Office
    {
        public int OfficeId { get; set; }
        public string OfficeName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Door> Doors { get; set; } = new List<Door>();
    }
}

