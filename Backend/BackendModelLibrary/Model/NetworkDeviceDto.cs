namespace BackendModelLibrary.Model
{
    public class NetworkDeviceDto
    {
        public int NetworkDeviceID { get; set; }

        public int NetworkID { get; set; }

        public int DeviceID { get; set; }


        public NetworkDto? Network { get; set; }

        public DeviceDto? Device { get; set; }
    }


}