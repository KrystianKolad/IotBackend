using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using IotBackend.Api.Infrastructure.Builders;
using IotBackend.Api.Infrastructure.Exceptions;
using IotBackend.Api.Infrastructure.Handlers;
using IotBackend.Api.Infrastructure.Models;
using IotBackend.Api.Infrastructure.Parsers;
using IotBackend.Api.Infrastructure.Providers;
using NSubstitute;
using NUnit.Framework;

namespace IotBackend.Api.Tests.Infrastructure.Handlers
{
    [TestFixture]
    public class DevicesHandlerTests
    {
        [Test]
        public void HandleGetDeviceSensorDailyData_DataNotFoundException_WhenFileIsNotFound()
        {
            //arrange
            _humidityBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            _humidityHistoricalBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            //act
            //assert
            Assert.ThrowsAsync<DataNotFoundException>(async() => await _sut.HandleGetDeviceSensorDailyData(_deviceName, _humiditySensorType, _date));
        }

        [Test]
        public void HandleGetDeviceDailyData_ThrowsDataNotFoundException_WhenFileIsNotFound()
        {
            //arrange
            _deviceDataBuilder.BuildDeviceData(Arg.Any<List<ISensorData>>(),Arg.Any<List<ISensorData>>(),Arg.Any<List<ISensorData>>()).Returns(new List<DeviceData>());
            //act
            //assert
            Assert.ThrowsAsync<DataNotFoundException>(async() => await _sut.HandleGetDeviceDailyData(_deviceName, _date));
        }

        [Test]
        public async Task HandleGetDeviceHumiditySensorDailyData_ReturnsDataFromFile()
        {
            //arrange
            //act
            var result = await _sut.HandleGetDeviceSensorDailyData(_deviceName, _humiditySensorType, _date);

            //assert
            AssertCollectionsAreEqual(result, _humidityParsedData);
        }

        [Test]
        public async Task HandleGetDeviceHumiditySensorDailyData_ReturnsDataFromHistoricalFile()
        {
            //arrange
            _humidityBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            //act
            var result = await _sut.HandleGetDeviceSensorDailyData(_deviceName, _humiditySensorType, _date);

            //assert
            AssertCollectionsAreEqual(result, _humidityHistoricalParsedData);
        }

        [Test]
        public async Task HandleGetDeviceRainfallSensorDailyData_ReturnsDataFromFile()
        {
            //arrange
            //act
            var result = await _sut.HandleGetDeviceSensorDailyData(_deviceName, _rainfallSensorType, _date);

            //assert
            AssertCollectionsAreEqual(result, _rainfallParsedData);
        }

        [Test]
        public async Task HandleGetDeviceRainfallSensorDailyData_ReturnsDataFromHistoricalFile()
        {
            //arrange
            _rainfallBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            //act
            var result = await _sut.HandleGetDeviceSensorDailyData(_deviceName, _rainfallSensorType, _date);

            //assert
            AssertCollectionsAreEqual(result, _rainfallHistoricalParsedData);
        }

        [Test]
        public async Task HandleGetDeviceTemperatureSensorDailyData_ReturnsDataFromFile()
        {
            //arrange
            //act
            var result = await _sut.HandleGetDeviceSensorDailyData(_deviceName, _temperatureSensorType, _date);

            //assert
            AssertCollectionsAreEqual(result, _temperatureParsedData);
        }

        [Test]
        public async Task HandleGetDeviceTemperatureSensorDailyData_ReturnsDataFromHistoricalFile()
        {
            //arrange
            _temperatureBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            //act
            var result = await _sut.HandleGetDeviceSensorDailyData(_deviceName, _temperatureSensorType, _date);

            //assert
            AssertCollectionsAreEqual(result, _temperatureHistoricalParsedData);
        }

        [Test]
        public async Task HandleGetDeviceDailyData_ReturnsCombinedDataFromFiles()
        {
            //arrange
            //act
            var result = await _sut.HandleGetDeviceDailyData(_deviceName, _date);

            //assert
            Assert.That(result, Is.EqualTo(_deviceData));
        }

        [Test]
        public async Task HandleGetDeviceDailyData_ReturnsCombinedDataFromHistoricalFiles()
        {
            //arrange
            _humidityBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            _rainfallBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            _temperatureBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(false, default))));
            //act
            var result = await _sut.HandleGetDeviceDailyData(_deviceName, _date);

            //assert
            Assert.That(result, Is.EqualTo(_historicalDeviceData));
        }

        [SetUp]
        public void SetUp()
        {
            SetUpParsedData();
            SetUpVariables();
            SetUpFilePathBuilder();
            SetUpDeviceDataBuilder();
            SetUpBlobClientProvider();
            SetUpZipArchiveProvider();
            SetUpParsers();

            _sut = new DevicesHandler(_parserProvider, _filePathBuilder, _blobClientProvider, _zipArchiveProvider, _deviceDataBuilder);
        }

        private IDevicesHandler _sut;
        private IParserProvider _parserProvider;
        private IFilePathBuilder _filePathBuilder;
        private IBlobClientProvider _blobClientProvider;
        private Func<Stream, ZipArchive> _zipArchiveProvider;
        private IDeviceDataBuilder _deviceDataBuilder;
        private string _deviceName;
        private DateTime _date;
        private string _humidityFilePath;
        private string _rainfallFilePath;
        private string _temperatureFilePath;
        private string _humidityHistoricalFilePath;
        private string _rainfallHistoricalFilePath;
        private string _temperatureHistoricalFilePath;
        private string _humiditySensorType;
        private string _rainfallSensorType;
        private string _temperatureSensorType;
        private List<ISensorData> _humidityParsedData;
        private List<ISensorData> _rainfallParsedData;
        private List<ISensorData> _temperatureParsedData;
        private List<DeviceData> _deviceData;
        private List<ISensorData> _humidityHistoricalParsedData;
        private List<ISensorData> _rainfallHistoricalParsedData;
        private List<ISensorData> _temperatureHistoricalParsedData;
        private List<DeviceData> _historicalDeviceData;
        private BlobClient _humidityBlob;
        private BlobClient _rainfallBlob;
        private BlobClient _temperatureBlob;
        private BlobClient _humidityHistoricalBlob;
        private BlobClient _rainfallHistoricalBlob;
        private BlobClient _temperatureHistoricalBlob;
        private Stream _humidityStream;
        private Stream _rainfallStream;
        private Stream _temperatureStream;
        private Stream _humidityHistoricalStream;
        private Stream _rainfallHistoricalStream;
        private Stream _temperatureHistoricalStream;
        private Stream _humidityHistoricalEntryStream;
        private Stream _rainfallHistoricalEntryStream;
        private Stream _temperatureHistoricalEntryStream;

        void AssertCollectionsAreEqual(List<ISensorData> result, List<ISensorData> expected)
        {
            Assert.That(result, Is.EqualTo(expected));
        }

        private void SetUpParsers()
        {
            var humidityParser = Substitute.For<IParser>();
            humidityParser.Type.Returns("humidity");
            humidityParser.ParseStream(_humidityStream).Returns(_humidityParsedData);
            humidityParser.ParseStream(Arg.Is<Stream>(x=>x.Length.Equals(_humidityHistoricalEntryStream.Length))).Returns(_humidityHistoricalParsedData);

            var rainfallParser = Substitute.For<IParser>();
            rainfallParser.Type.Returns("rainfall");
            rainfallParser.ParseStream(_rainfallStream).Returns(_rainfallParsedData);
            rainfallParser.ParseStream(Arg.Is<Stream>(x=>x.Length.Equals(_rainfallHistoricalEntryStream.Length))).Returns(_rainfallHistoricalParsedData);

            var temperatureParser = Substitute.For<IParser>();
            temperatureParser.Type.Returns("temperature");
            temperatureParser.ParseStream(_temperatureStream).Returns(_temperatureParsedData);
            temperatureParser.ParseStream(Arg.Is<Stream>(x=>x.Length.Equals(_temperatureHistoricalEntryStream.Length))).Returns(_temperatureHistoricalParsedData);

            _parserProvider = Substitute.For<IParserProvider>();
            _parserProvider.GetParser(_humiditySensorType).Returns(humidityParser);
            _parserProvider.GetParser(_rainfallSensorType).Returns(rainfallParser);
            _parserProvider.GetParser(_temperatureSensorType).Returns(temperatureParser);
        }

        void SetUpParsedData()
        {
            _humidityParsedData = new List<ISensorData>
            {
                Substitute.For<ISensorData>()
            };
            _rainfallParsedData = new List<ISensorData>
            {
                Substitute.For<ISensorData>()
            };
            _temperatureParsedData = new List<ISensorData>
            {
                Substitute.For<ISensorData>()
            };
            _humidityHistoricalParsedData = new List<ISensorData>
            {
                Substitute.For<ISensorData>()
            };
            _rainfallHistoricalParsedData = new List<ISensorData>
            {
                Substitute.For<ISensorData>()
            };
            _temperatureHistoricalParsedData = new List<ISensorData>
            {
                Substitute.For<ISensorData>()
            };
            _deviceData = new List<DeviceData>
            {
                new DeviceData()
            };
            _historicalDeviceData = new List<DeviceData>
            {
                new DeviceData()
            };
        }

        private void SetUpVariables()
        {
            _humidityFilePath = "humidityFilePath";
            _rainfallFilePath = "rainfallFilePath";
            _temperatureFilePath = "temperatureFilePath";
            _humidityHistoricalFilePath = "humidityHistoricalFilePath";
            _rainfallHistoricalFilePath = "rainfallHistoricalFilePath";
            _temperatureHistoricalFilePath = "temperatureHistoricalFilePath";
            _humiditySensorType = "humidity";
            _rainfallSensorType = "rainfall";
            _temperatureSensorType = "temperature";
            _deviceName = "testDevice";
            _date = new DateTime(2019, 1, 10);

            _humidityStream = Substitute.For<Stream>();
            _rainfallStream = Substitute.For<Stream>();
            _temperatureStream = Substitute.For<Stream>();
            _humidityHistoricalStream = Substitute.For<Stream>();
            _rainfallHistoricalStream = Substitute.For<Stream>();
            _temperatureHistoricalStream = Substitute.For<Stream>();
            _humidityHistoricalEntryStream = new MemoryStream(Encoding.UTF8.GetBytes("a"));
            _rainfallHistoricalEntryStream = new MemoryStream(Encoding.UTF8.GetBytes("aa"));
            _temperatureHistoricalEntryStream = new MemoryStream(Encoding.UTF8.GetBytes("aaa"));

            _humidityBlob = Substitute.For<BlobClient>();
            _humidityBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(true, default))));
            _humidityBlob.OpenReadAsync().Returns(_humidityStream);

            _rainfallBlob = Substitute.For<BlobClient>();
            _rainfallBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(true, default))));
            _rainfallBlob.OpenReadAsync().Returns(_rainfallStream);

            _temperatureBlob = Substitute.For<BlobClient>();
            _temperatureBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(true, default))));
            _temperatureBlob.OpenReadAsync().Returns(_temperatureStream);

            _humidityHistoricalBlob = Substitute.For<BlobClient>();
            _humidityHistoricalBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(true, default))));
            _humidityHistoricalBlob.OpenReadAsync().Returns(_humidityHistoricalStream);

            _rainfallHistoricalBlob = Substitute.For<BlobClient>();
            _rainfallHistoricalBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(true, default))));
            _rainfallHistoricalBlob.OpenReadAsync().Returns(_rainfallHistoricalStream);

            _temperatureHistoricalBlob = Substitute.For<BlobClient>();
            _temperatureHistoricalBlob.ExistsAsync().Returns(Task.FromResult((Response.FromValue(true, default))));
            _temperatureHistoricalBlob.OpenReadAsync().Returns(_temperatureHistoricalStream);

        }

        private void SetUpFilePathBuilder()
        {
            _filePathBuilder = Substitute.For<IFilePathBuilder>();
            _filePathBuilder.BuildFilePath(_deviceName, _humiditySensorType, _date).Returns(_humidityFilePath);
            _filePathBuilder.BuildFilePath(_deviceName, _rainfallSensorType, _date).Returns(_rainfallFilePath);
            _filePathBuilder.BuildFilePath(_deviceName, _temperatureSensorType, _date).Returns(_temperatureFilePath);
            _filePathBuilder.BuildHistoricalFilePath(_deviceName, _humiditySensorType).Returns(_humidityHistoricalFilePath);
            _filePathBuilder.BuildHistoricalFilePath(_deviceName, _rainfallSensorType).Returns(_rainfallHistoricalFilePath);
            _filePathBuilder.BuildHistoricalFilePath(_deviceName, _temperatureSensorType).Returns(_temperatureHistoricalFilePath);
        }

        private void SetUpDeviceDataBuilder()
        {
            _deviceDataBuilder = Substitute.For<IDeviceDataBuilder>();
            _deviceDataBuilder.BuildDeviceData(_humidityParsedData, _rainfallParsedData, _temperatureParsedData).Returns(_deviceData);
            _deviceDataBuilder.BuildDeviceData(_humidityHistoricalParsedData, _rainfallHistoricalParsedData, _temperatureHistoricalParsedData).Returns(_historicalDeviceData);
        }

        private void SetUpBlobClientProvider()
        {
            _blobClientProvider = Substitute.For<IBlobClientProvider>();
            _blobClientProvider.GetBlobClient(_humidityFilePath).Returns(_humidityBlob);
            _blobClientProvider.GetBlobClient(_rainfallFilePath).Returns(_rainfallBlob);
            _blobClientProvider.GetBlobClient(_temperatureFilePath).Returns(_temperatureBlob);
            _blobClientProvider.GetBlobClient(_humidityHistoricalFilePath).Returns(_humidityHistoricalBlob);
            _blobClientProvider.GetBlobClient(_rainfallHistoricalFilePath).Returns(_rainfallHistoricalBlob);
            _blobClientProvider.GetBlobClient(_temperatureHistoricalFilePath).Returns(_temperatureHistoricalBlob);
        }

        private void SetUpZipArchiveProvider()
        {
            _zipArchiveProvider = Substitute.For<Func<Stream, ZipArchive>>();
            _zipArchiveProvider(_humidityHistoricalStream).Returns(CreateFakeZipArchive(_humidityHistoricalEntryStream, "2019-01-10.csv"));
            _zipArchiveProvider(_rainfallHistoricalStream).Returns(CreateFakeZipArchive(_rainfallHistoricalEntryStream, "2019-01-10.csv"));
            _zipArchiveProvider(_temperatureHistoricalStream).Returns(CreateFakeZipArchive(_temperatureHistoricalEntryStream, "2019-01-10.csv"));
        }

        private ZipArchive CreateFakeZipArchive(Stream entryStream, string entryName)
        {
            var memoryStream = new MemoryStream();
            using(var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create,true))
            {
                var entry = archive.CreateEntry(entryName);
                using(var stream = entry.Open())
                {
                    entryStream.CopyTo(stream);
                }
            }

            memoryStream.Position = 0;
            return new ZipArchive(memoryStream, ZipArchiveMode.Update);
        }
    }
}