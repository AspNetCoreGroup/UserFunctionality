namespace BackendModelLibrary.Model
{
    public class DeviceDto
    {
        public int DeviceID { get; set; }

        public required string DeviceCode { get; set; }

        public string? DeviceCustomTitle;
    }
}