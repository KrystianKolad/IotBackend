using System;

namespace IotBackend.Api.Infrastructure.Models
{
    public class Temperature : ISensorData
    {
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }        
    }
}