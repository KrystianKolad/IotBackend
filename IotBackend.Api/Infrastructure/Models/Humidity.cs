using System;

namespace IotBackend.Api.Infrastructure.Models
{
    public class Humidity : ISensorData
    {
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }
    }
}