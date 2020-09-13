using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using IotBackend.Api.Infrastructure.Builders;
using IotBackend.Api.Infrastructure.Consts;
using IotBackend.Api.Infrastructure.Exceptions;
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
        private readonly IParserProvider _parserProvider;
        private readonly IFilePathBuilder _filePathBuilder;
        private readonly IBlobClientProvider _blobClientProvider;
        private readonly Func<Stream, ZipArchive> _zipArchiveProvider;
        private readonly IDeviceDataBuilder _deviceDataBuilder;
        public DevicesHandler(
            IParserProvider parserProvider,
            IFilePathBuilder filePathBuilder,
            IBlobClientProvider blobClientProvider,
            Func<Stream, ZipArchive> zipArchiveProvider,
            IDeviceDataBuilder deviceDataBuilder)
        {
            _parserProvider = parserProvider;
            _filePathBuilder = filePathBuilder;
            _blobClientProvider = blobClientProvider;
            _zipArchiveProvider = zipArchiveProvider;
            _deviceDataBuilder = deviceDataBuilder;
        }

        public async Task<List<ISensorData>> HandleGetDeviceSensorDailyData(string deviceName, string sensorType, DateTime date)
        {
            var result =  await Read(deviceName, date, _parserProvider.GetParser(sensorType));

            if (!result.Any())
            {
                throw new DataNotFoundException();
            }

            return result;
        }

        public async Task<List<DeviceData>> HandleGetDeviceDailyData(string deviceName, DateTime date)
        {
            var humidityTask = Read(deviceName, date, _parserProvider.GetParser(SensorTypesConsts.Humidity));
            var rainfallTask = Read(deviceName, date, _parserProvider.GetParser(SensorTypesConsts.Rainfall));
            var temperatureTask = Read(deviceName, date, _parserProvider.GetParser(SensorTypesConsts.Temperature));
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

            var result = _deviceDataBuilder.BuildDeviceData(humidities, rainfalls, temperatures);

            if (!result.Any())
            {
                throw new DataNotFoundException();
            }
            return result;
        }

        private async Task<List<ISensorData>> Read(string deviceName, DateTime date, IParser parser)
        {
            var filePath = _filePathBuilder.BuildFilePath(deviceName, parser.Type, date);
            var blob = _blobClientProvider.GetBlobClient(filePath);

            if (await blob.ExistsAsync())
            {
                using(var stream = await blob.OpenReadAsync())
                {
                    return parser.ParseStream(stream);
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
                        return GetArchiveFile(stream, date, parser);
                    }
                }
            }
            return new List<ISensorData>();
        }

        private List<ISensorData> GetArchiveFile(Stream stream, DateTime date, IParser parser)
        {
            using(var archive = _zipArchiveProvider(stream))
            {
                var entry = archive.Entries.FirstOrDefault(x => x.FullName.Equals(date.ToFileName()));
                if (entry != null)
                {
                    using(var entryStream = entry.Open())
                    {
                        return parser.ParseStream(entryStream);
                    }
                }
            }
            return new List<ISensorData>();
        }
    }
}