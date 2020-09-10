using System.Collections.Generic;
using System.Linq;
using IotBackend.Api.Infrastructure.Models;

namespace IotBackend.Api.Infrastructure.Builders
{
    public interface IDeviceDataBuilder
    {
        List<DeviceData> BuildDeviceData(List<ISensorData> humidities, List<ISensorData> rainfalls, List<ISensorData> temperatures);
    }
    public class DeviceDataBuilder : IDeviceDataBuilder
    {
        public List<DeviceData> BuildDeviceData(List<ISensorData> humidities, List<ISensorData> rainfalls, List<ISensorData> temperatures)
        {
            return 
                (from humidity in humidities 
                join rainfall in rainfalls on humidity.TimeStamp equals rainfall.TimeStamp 
                join temperature in temperatures on humidity.TimeStamp equals temperature.TimeStamp 
                select new DeviceData
                {
                    TimeStamp = humidity.TimeStamp,
                        Humidity = humidity.Value,
                        Rainfall = rainfall.Value,
                        Temperature = temperature.Value
                }).ToList();
        }
    }
}