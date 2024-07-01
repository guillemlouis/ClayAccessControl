using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ClayAccessControl.Core.Entities{
    public enum DoorStatusEnum
    {
        [EnumMember(Value = "Locked")]
        Locked,
        [EnumMember(Value = "Unlocked")]
        Unlocked
    }
}

