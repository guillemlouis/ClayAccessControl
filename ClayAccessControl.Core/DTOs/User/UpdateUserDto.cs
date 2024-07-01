namespace ClayAccessControl.Core.DTOs{
    public class UpdateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? OfficeId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
