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
    public class RainfallParserTests
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

            _sut = new RainfallParser(_streamReaderProvider);
        }
        private IParser _sut;
        private Func<Stream, StreamReader> _streamReaderProvider;
        private Stream _stream;
        private StreamReader _streamReader;
        private string _lines;

        private void AssertThatResultIsValid(List<ISensorData> result)
        {
            Assert.That(result.Count, Is.EqualTo(7));
            AssertThatResultItemIsValid(result[0], new DateTime(2019,1,11,0,0,0),0f);
            AssertThatResultItemIsValid(result[1], new DateTime(2019,1,11,0,0,05),0f);
            AssertThatResultItemIsValid(result[2], new DateTime(2019,1,11,0,0,10),0f);
            AssertThatResultItemIsValid(result[3], new DateTime(2019,1,11,0,0,15),0f);
            AssertThatResultItemIsValid(result[4], new DateTime(2019,1,11,0,0,20),0f);
            AssertThatResultItemIsValid(result[5], new DateTime(2019,1,11,0,0,25),0f);
            AssertThatResultItemIsValid(result[6], new DateTime(2019,1,11,0,0,30),0f);
        }

        private void AssertThatResultItemIsValid(ISensorData item, DateTime expectedTimeStamp, float expectedValue)
        {
            Assert.That(item.TimeStamp, Is.EqualTo(expectedTimeStamp));
            Assert.That(item.Value, Is.EqualTo(expectedValue));
        }

        private void SetUpStreamReader()
        {
            _lines =
                "2019-01-11T00:00:00;,00" + Environment.NewLine +
                "2019-01-11T00:00:05;,00" + Environment.NewLine +
                "2019-01-11T00:00:10;,00" + Environment.NewLine +
                "2019-01-11T00:00:15;,00" + Environment.NewLine +
                "2019-01-11T00:00:20;,00" + Environment.NewLine +
                "2019-01-11T00:00:25;,00" + Environment.NewLine +
                "2019-01-11T00:00:30;,00";

            _streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(_lines)));
        }
    }
}