using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using IotBackend.Api.Infrastructure.Builders;
using IotBackend.Api.Infrastructure.Consts;
using IotBackend.Api.Infrastructure.Extensions;
using IotBackend.Api.Infrastructure.Models;
using IotBackend.Api.Infrastructure.Parsers;
using IotBackend.Api.Infrastructure.Providers;

namespace IotBackend.Api.Infrastructure.Handlers
{
    public interface IDevicesHandler
    {
        Task<List<ISensorData>> HandleGetDeviceSensorDailyData(string deviceName, string sensorType, DateTime date);
        Task<List<DeviceData>> HandleGetDeviceDailyData(string deviceName, DateTime date);
    }
    public class DevicesHandler : IDevicesHandler
    {
        private Dictionary<string, IParser> _parsers;
        private IFilePathBuilder _filePathBuilder;
        private readonly IBlobClientProvider _blobClientProvider;
        private readonly Func<Stream, ZipArchive> _zipArchiveProvider;
        public DevicesHandler(IEnumerable<IParser> parsers, IFilePathBuilder filePathBuilder, IBlobClientProvider blobClientProvider,Func<Stream, ZipArchive> zipArchiveProvider)
        {
            _parsers = parsers.ToDictionary(x => x.Type, y => y);
            _filePathBuilder = filePathBuilder;
            _blobClientProvider = blobClientProvider;
            _zipArchiveProvider = zipArchiveProvider;
        }

        public async Task<List<ISensorData>> HandleGetDeviceSensorDailyData(string deviceName, string sensorType, DateTime date)
        {
            if (_parsers.TryGetValue(sensorType, out var parser))
            {
                return await Read(deviceName, date, parser);
            }
            throw new Exception($"Sensor {sensorType} is not supported");
        }

        public async Task<List<DeviceData>> HandleGetDeviceDailyData(string deviceName, DateTime date)
        {
            var humidityTask = Read(deviceName, date, _parsers[SensorTypesConsts.Humidity]);
            var rainfallTask = Read(deviceName, date, _parsers[SensorTypesConsts.Rainfall]);
            var temperatureTask = Read(deviceName, date, _parsers[SensorTypesConsts.Temperature]);
            var tasks = new Task[]
            {
                humidityTask,
                rainfallTask,
                temperatureTask
            };

            await Task.WhenAll(tasks);

            var humidities = humidityTask.Result;
            var rainfalls = rainfallTask.Result;
            var temperatures = temperatureTask.Result;

            var result = (
                from humidity in humidities 
                join rainfall in rainfalls on humidity.TimeStamp equals rainfall.TimeStamp 
                join temperature in temperatures on humidity.TimeStamp equals temperature.TimeStamp 
                select new DeviceData
                {
                    TimeStamp = humidity.TimeStamp,
                    Humidity = humidity.Value,
                    Rainfall = rainfall.Value,
                    Temperature = temperature.Value
                }).ToList();

            return result;
        }

        private async Task<List<ISensorData>> Read(string deviceName, DateTime date, IParser parser)
        {
            var filePath = _filePathBuilder.BuildFilePath(deviceName, parser.Type, date);
            var blob = _blobClientProvider.GetBlobClient(filePath);
            var result = new List<ISensorData>();

            if (await blob.ExistsAsync())
            {
                using(var stream = await blob.OpenReadAsync())
                {
                    result.AddRange(parser.ParseStream(stream));
                }
            }
            else
            {
                var historicalFilePath = _filePathBuilder.BuildHistoricalFilePath(deviceName, parser.Type);
                blob = _blobClientProvider.GetBlobClient(historicalFilePath);
                if (await blob.ExistsAsync())
                {
                    using(var stream = await blob.OpenReadAsync())
                    {
                        result.AddRange(GetArchiveFile(stream, date, parser));
                    }
                }
            }
            return result;
        }

        private List<ISensorData> GetArchiveFile(Stream stream, DateTime date, IParser parser)
        {
            using(var archive = _zipArchiveProvider(stream))
            {
                var entry = archive.Entries.FirstOrDefault(x => x.FullName.Equals(date.ToFileName()));
                if (entry != null)
                {
                    using (var entryStream = entry.Open())
                    {
                        return parser.ParseStream(entryStream);
                    }
                }
            }
            return new List<ISensorData>();
        }
    }
}