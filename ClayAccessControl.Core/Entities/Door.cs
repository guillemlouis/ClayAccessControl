namespace ClayAccessControl.Core.Entities{
    public class Door
    {
        public int DoorId { get; set; }
        public string DoorName { get; set; } = string.Empty;
        public int RequiredAccessLevel { get; set; }

        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!;
        public DoorStatusEnum Status { get; set; } = DoorStatusEnum.Locked;


        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<UserDoorAccess> UserDoorAccesses { get; set; } = new List<UserDoorAccess>();
    }
}

