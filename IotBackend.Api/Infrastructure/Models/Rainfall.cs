using System;

namespace IotBackend.Api.Infrastructure.Models
{
    public class Rainfall : ISensorData
    {
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }
    }
}