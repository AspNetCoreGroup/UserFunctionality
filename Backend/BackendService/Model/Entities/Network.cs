using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("Networks")]
    public class Network
    {
        [Key]
        public int NetworkID { get; set; }

        public required string NetworkTitle { get; set; }


        public List<NetworkUser>? NetworkUsers { get; set; }

        public List<NetworkUsersGroup>? NetworkUsersGroup { get; set; }

        public List<NetworkDevice>? NetworkDevices { get; set; }

        public List<NetworkDevicesGroup>? NetworkDevicesGroup { get; set; }
    }
}