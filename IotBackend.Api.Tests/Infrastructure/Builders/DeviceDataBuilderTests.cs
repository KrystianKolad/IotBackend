using System;
using System.Collections.Generic;
using IotBackend.Api.Infrastructure.Builders;
using IotBackend.Api.Infrastructure.Models;
using NUnit.Framework;

namespace IotBackend.Api.Tests.Infrastructure.Builders
{
    [TestFixture]
    public class DeviceDataBuilderTests
    {
        [Test]
        public void BuildDeviceData_CombinesDataProperly()
        {
            //arrange
            //act
            var result = _sut.BuildDeviceData(_humidities, _rainfalls, _temperatures);

            //assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].TimeStamp, Is.EqualTo(new DateTime(2019,1,10,1,1,1)));
            Assert.That(result[0].Humidity, Is.EqualTo(1));
            Assert.That(result[0].Rainfall, Is.EqualTo(11));
            Assert.That(result[0].Temperature, Is.EqualTo(111));
            Assert.That(result[1].TimeStamp, Is.EqualTo(new DateTime(2019,1,10,1,1,2)));
            Assert.That(result[1].Humidity, Is.EqualTo(2));
            Assert.That(result[1].Rainfall, Is.EqualTo(22));
            Assert.That(result[1].Temperature, Is.EqualTo(222));
        }

        [SetUp]
        public void SetUp()
        {
            _humidities = new List<ISensorData>
            {
                new Humidity {TimeStamp = new DateTime(2019,1,10,1,1,1), Value = 1},
                new Humidity {TimeStamp = new DateTime(2019,1,10,1,1,2), Value = 2}
            };
            _rainfalls = new List<ISensorData>
            {
                new Rainfall {TimeStamp = new DateTime(2019,1,10,1,1,1), Value = 11},
                new Rainfall {TimeStamp = new DateTime(2019,1,10,1,1,2), Value = 22}
            };
            _temperatures = new List<ISensorData>
            {
                new Temperature {TimeStamp = new DateTime(2019,1,10,1,1,1), Value = 111},
                new Temperature {TimeStamp = new DateTime(2019,1,10,1,1,2), Value = 222}
            };

            _sut = new DeviceDataBuilder();
        }
        private IDeviceDataBuilder _sut;
        private List<ISensorData> _humidities;
        private List<ISensorData> _rainfalls;
        private List<ISensorData> _temperatures;
    }
}