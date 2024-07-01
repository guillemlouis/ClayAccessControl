namespace ClayAccessControl.Core.DTOs{
    public class CreateDoorDto
    {
        public string DoorName { get; set; } = string.Empty;
        public int RequiredAccessLevel { get; set; }
        public int OfficeId { get; set; }
    }
}