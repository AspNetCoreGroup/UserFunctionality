using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public required string UserLogin { get; set; }

        public required string UserPassword { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? Patronymic { get; set; }

        public string? Email { get; set; }

        public string? NotificationEmail { get; set; }

        public string? NotificationTelegramID { get; set; }
    }
}