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

        public required int CreatorUserID { get; set; }

        public bool IsDeleted { get; set; }

        public List<NetworkUser>? NetworkUsers { get; set; }

        public List<NetworkDevice>? NetworkDevices { get; set; }
    }
}