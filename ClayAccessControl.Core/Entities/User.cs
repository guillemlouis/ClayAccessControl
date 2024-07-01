namespace ClayAccessControl.Core.Entities{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int? OfficeId { get; set; }
        public Office? Office { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<UserDoorAccess> UserDoorAccesses { get; set; } = new List<UserDoorAccess>();
    }
}

