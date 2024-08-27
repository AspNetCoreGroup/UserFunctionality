namespace BackendService.Model
{
    public class DeviceMessage
    {
        public int DeviceId { get; set; }
        public required string SerialNumber { get; set; }
        public required string DeviceType { get; set; }
        public required string NetAddress { get; set; }
    }
}