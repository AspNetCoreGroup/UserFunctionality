using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("Devices")]
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DeviceID { get; set; }

        public required string DeviceCode { get; set; }

        public string? DeviceCustomTitle { get; set; }
    }
}