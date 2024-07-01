namespace ClayAccessControl.Core.DTOs{
    public class UpdateDoorDto
    {
        public int DoorId { get; set; }
        public string DoorName { get; set; } = string.Empty;
        public int RequiredAccessLevel { get; set; }
    }
}