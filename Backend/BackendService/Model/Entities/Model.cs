using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("NetworkDevicesGroups")]
    public class NetworkDevicesGroup
    {
        [Key]
        public int NetworkDevicesGroupID { get; set; }
    }

    [Table("NetworkDevicesAccesses")]
    public class NetworkDevicesAccess
    {
        [Key]
        public int NetworkDevicesAccessID { get; set; }
    }

    [Table("NetworkUsersGroups")]
    public class NetworkUsersGroup
    {
        [Key]
        public int NetworkUsersGroupID { get; set; }
    }
}