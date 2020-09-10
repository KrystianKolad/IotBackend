using System;
using IotBackend.Api.Infrastructure.Extensions;
using NUnit.Framework;

namespace IotBackend.Api.Tests.Infrastructure.Extensions
{
    [TestFixture]
    public class DateTimeExtensionsTests
    {
        [Test]
        public void ToFileName_ReturnsValidFileName_FromDateTime([ValueSource("_dateTimeExtensionsTestData")]DateTimeExtensionsTestData testData)
        {
            //arrange
            //act
            var result = testData.Value.ToFileName();

            //assert
            Assert.That(result, Is.EquivalentTo(testData.ExpectedResult));
        }

        private static DateTimeExtensionsTestData[] _dateTimeExtensionsTestData = new []
        {
            new DateTimeExtensionsTestData { Value = new DateTime(2018,01,10,21,24,24), ExpectedResult = "2018-01-10.csv"},
            new DateTimeExtensionsTestData { Value = new DateTime(2019,2,11,21,23,24), ExpectedResult = "2019-02-11.csv"},
            new DateTimeExtensionsTestData { Value = new DateTime(2020,10,10,21,23,24), ExpectedResult = "2020-10-10.csv"}
        };
    }

    public class DateTimeExtensionsTestData
    {
        public DateTime Value { get; set; }
        public string ExpectedResult { get; set; }
    }
}