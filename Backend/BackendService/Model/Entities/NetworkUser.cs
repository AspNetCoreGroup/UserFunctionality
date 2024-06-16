using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("NetworkUsers")]
    public class NetworkUser
    {
        [Key]
        public int NetworkUserID { get; set; }

        public int NetworkID { get; set; }

        public int UserID { get; set; }


        public Network? Network { get; set; }

        public User? User { get; set; }
    }
}