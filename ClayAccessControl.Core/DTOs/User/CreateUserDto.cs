namespace ClayAccessControl.Core.DTOs{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? OfficeId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
