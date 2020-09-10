using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IotBackend.Api.Infrastructure.Models;
using IotBackend.Api.Infrastructure.Parsers;
using NSubstitute;
using NUnit.Framework;

namespace IotBackend.Api.Tests.Infrastructure.Parsers
{
    [TestFixture]
    public class TemperatureParserTests
    {
        [Test]
        public void ParseStream_ReturnsListOfParsedLines()
        {
            //arrange
            //act
            var result = _sut.ParseStream(_stream);

            //assert
            AssertThatResultIsValid(result);
        }
        [SetUp]
        public void SetUp()
        {
            _stream = Substitute.For<Stream>();
            SetUpStreamReader();

            _streamReaderProvider = Substitute.For<Func<Stream, StreamReader>>();
            _streamReaderProvider(_stream).Returns(_streamReader);

            _sut = new TemperatureParser(_streamReaderProvider);
        }
        private IParser _sut;
        private Func<Stream, StreamReader> _streamReaderProvider;
        private Stream _stream;
        private StreamReader _streamReader;
        private string _lines;

        private void AssertThatResultIsValid(List<ISensorData> result)
        {
            Assert.That(result.Count, Is.EqualTo(7));
            AssertThatResultItemIsValid(result[0], new DateTime(2019,1,12,0,1,0),-0.95f);
            AssertThatResultItemIsValid(result[1], new DateTime(2019,1,12,0,1,5),-0.95f);
            AssertThatResultItemIsValid(result[2], new DateTime(2019,1,12,0,1,10),-0.95f);
            AssertThatResultItemIsValid(result[3], new DateTime(2019,1,12,0,1,15),-0.95f);
            AssertThatResultItemIsValid(result[4], new DateTime(2019,1,12,0,1,20),-0.95f);
            AssertThatResultItemIsValid(result[5], new DateTime(2019,1,12,0,1,25),-0.96f);
            AssertThatResultItemIsValid(result[6], new DateTime(2019,1,12,0,1,30),-0.96f);
        }

        private void AssertThatResultItemIsValid(ISensorData item, DateTime expectedTimeStamp, float expectedValue)
        {
            Assert.That(item.TimeStamp, Is.EqualTo(expectedTimeStamp));
            Assert.That(item.Value, Is.EqualTo(expectedValue));
        }
        
        private void SetUpStreamReader()
        {
            _lines =
                "2019-01-12T00:01:00;-,95" + Environment.NewLine +
                "2019-01-12T00:01:05;-,95" + Environment.NewLine +
                "2019-01-12T00:01:10;-,95" + Environment.NewLine +
                "2019-01-12T00:01:15;-,95" + Environment.NewLine +
                "2019-01-12T00:01:20;-,95" + Environment.NewLine +
                "2019-01-12T00:01:25;-,96" + Environment.NewLine +
                "2019-01-12T00:01:30;-,96";

            _streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(_lines)));
        }
    }
}