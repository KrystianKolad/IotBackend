using System;
using IotBackend.Api.Infrastructure.Builders;
using NUnit.Framework;

namespace IotBackend.Api.Tests.Infrastructure.Builders
{
    [TestFixture]
    public class FilePathBuilderTests
    {
        [Test]
        public void BuildFilePath_ReturnsValidFilePath_ForGivenArguments([ValueSource("_buildFilePathTestData")] BuildFilePathTestData testData)
        {
            //arrange
            //act
            var result = _sut.BuildFilePath(testData.DeviceName, testData.SensorType, testData.Date);

            //assert
            Assert.That(result, Is.EquivalentTo(testData.ExpectedResult));
        }

        [TestCase("device1","humidity", "device1/humidity/historical.zip")]
        [TestCase("device2","rainfall", "device2/rainfall/historical.zip")]
        [TestCase("device3","temperature", "device3/temperature/historical.zip")]
        public void BuildHistoricalFilePath_ReturnsValidFilePath_ForGivenArguments(string deviceName, string sensorType, string expectedResult)
        {
            //arrange
            //act
            var result = _sut.BuildHistoricalFilePath(deviceName, sensorType);

            //assert
            Assert.That(result, Is.EquivalentTo(expectedResult));
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new FilePathBuilder();
        }

        private IFilePathBuilder _sut;

        private static BuildFilePathTestData[] _buildFilePathTestData = new []
        {
            new BuildFilePathTestData { DeviceName = "device1", SensorType = "humidity", Date = new DateTime(2019, 10, 10), ExpectedResult = "device1/humidity/2019-10-10.csv" },
            new BuildFilePathTestData { DeviceName = "device2", SensorType = "rainfall", Date = new DateTime(2019, 10, 10), ExpectedResult = "device2/rainfall/2019-10-10.csv" },
            new BuildFilePathTestData { DeviceName = "device3", SensorType = "temperature", Date = new DateTime(2019, 10, 10), ExpectedResult = "device3/temperature/2019-10-10.csv" }
        };
    }

    public class BuildFilePathTestData
    {
        public string DeviceName { get; set; }
        public string SensorType { get; set; }
        public DateTime Date { get; set; }
        public string ExpectedResult { get; set; }
    }
}