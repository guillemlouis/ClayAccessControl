namespace ClayAccessControl.Core.Entities{
    public class UserDoorAccess
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int DoorId { get; set; }
        public Door Door { get; set; } = null!;

        public bool HasAccess { get; set; }
    }
}

