using System;

namespace IotBackend.Api.Infrastructure.Models
{
    public class DeviceData
    {
        public DateTime TimeStamp { get; set; }
        public float Humidity { get; set; }
        public float Rainfall { get; set; }
        public float Temperature { get; set; }
    }
}