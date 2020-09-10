using System;

namespace IotBackend.Api.Infrastructure.Models
{
    public interface ISensorData
    {
        public DateTime TimeStamp { get; set; }
        public float Value { get; set; }
    }
}