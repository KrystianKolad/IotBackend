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
    public class HumidityParserTests
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

            _sut = new HumidityParser(_streamReaderProvider);
        }

        private IParser _sut;
        private Func<Stream, StreamReader> _streamReaderProvider;
        private Stream _stream;
        private StreamReader _streamReader;
        private string _lines;

        private void AssertThatResultIsValid(List<ISensorData> result)
        {
            Assert.That(result.Count, Is.EqualTo(7));
            AssertThatResultItemIsValid(result[0], new DateTime(2019,1,10,0,1,5),9.41f);
            AssertThatResultItemIsValid(result[1], new DateTime(2019,1,10,0,1,10),9.40f);
            AssertThatResultItemIsValid(result[2], new DateTime(2019,1,10,0,1,15),9.40f);
            AssertThatResultItemIsValid(result[3], new DateTime(2019,1,10,0,1,20),9.39f);
            AssertThatResultItemIsValid(result[4], new DateTime(2019,1,10,0,1,25),9.39f);
            AssertThatResultItemIsValid(result[5], new DateTime(2019,1,10,0,1,30),9.39f);
            AssertThatResultItemIsValid(result[6], new DateTime(2019,1,10,0,1,35),9.38f);
        }

        private void AssertThatResultItemIsValid(ISensorData item, DateTime expectedTimeStamp, float expectedValue)
        {
            Assert.That(item.TimeStamp, Is.EqualTo(expectedTimeStamp));
            Assert.That(item.Value, Is.EqualTo(expectedValue));
        }

        private void SetUpStreamReader()
        {
            _lines =
                "2019-01-10T00:01:05;9,41" + Environment.NewLine +
                "2019-01-10T00:01:10;9,40" + Environment.NewLine +
                "2019-01-10T00:01:15;9,40" + Environment.NewLine +
                "2019-01-10T00:01:20;9,39" + Environment.NewLine +
                "2019-01-10T00:01:25;9,39" + Environment.NewLine +
                "2019-01-10T00:01:30;9,39" + Environment.NewLine +
                "2019-01-10T00:01:35;9,38";

            _streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(_lines)));
        }
    }
}