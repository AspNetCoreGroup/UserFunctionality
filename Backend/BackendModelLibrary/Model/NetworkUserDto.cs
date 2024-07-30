namespace BackendModelLibrary.Model
{
    public class NetworkUserDto
    {
        public int NetworkUserID { get; set; }

        public int NetworkID { get; set; }

        public int UserID { get; set; }


        public NetworkDto? Network { get; set; }

        public UserDto? User { get; set; }
    }


}