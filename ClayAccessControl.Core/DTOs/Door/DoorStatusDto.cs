using ClayAccessControl.Core.Entities;

namespace ClayAccessControl.Core.DTOs{
    public class DoorStatusDto
    {
        public int DoorId { get; set; }
        public string DoorName { get; set; } = string.Empty;
        public DoorStatusEnum Status { get; set; }
    }
}
