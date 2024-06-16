namespace BackendModelLibrary.Model
{
    public class UserDto
    {
        public int UserID { get; set; }

        public required string UserLogin { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string? Patronymic { get; set; }

        public string? Email { get; set; }

        public string? NotificationEmail { get; set; }

        public string? NotificationTelegramID { get; set; }
    }
}