using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("NetworkDevices")]
    public class NetworkDevice
    {
        [Key]
        public int NetworkDeviceID { get; set; }

        public int NetworkID { get; set; }

        public int DeviceID { get; set; }


        public Network? Network { get; set; }

        public Device? Device { get; set; }
    }
}