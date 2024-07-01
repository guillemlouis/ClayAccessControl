namespace ClayAccessControl.Core.DTOs{
    public class DoorDto
{
    public int DoorId { get; set; }
    public string DoorName { get; set; } = string.Empty;
    public int RequiredAccessLevel { get; set; }
    public int OfficeId { get; set; }
}
}