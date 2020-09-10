using System.Net;

namespace IotBackend.Api.Infrastructure.Exceptions
{
    public class SensorNotSupportedException : BaseException
    {
        public SensorNotSupportedException(string sensorType) : base($"Sensor {sensorType} is not supported", (int)HttpStatusCode.BadRequest)
        {
        }
    }
}