using ClayAccessControl.Core.Entities;

namespace ClayAccessControl.Core.DTOs{
    public class GrantAccessDto
    {
        public int UserId { get; set; }
        public int DoorId { get; set; }
    }
}
