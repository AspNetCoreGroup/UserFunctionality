using System;
namespace BackendModelLibrary.Requests
{
    public class GraphRequestWrapper
    {
        public DateTime MinDateTime { get; set; }

        public DateTime MaxDateTime { get; set; }

        public int? NetworkID { get; set; }

        public int? DeviceID { get; set; }
    }
}