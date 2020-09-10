using IotBackend.Api.Infrastructure.Exceptions;
using IotBackend.Api.Infrastructure.Parsers;
using IotBackend.Api.Infrastructure.Providers;
using NSubstitute;
using NUnit.Framework;

namespace IotBackend.Api.Tests.Infrastructure.Providers
{
    [TestFixture]
    public class ParserProviderTests
    {
        [Test]
        public void GetParser_ReturnsParser_ForGicenSensorType()
        {
            //arrange
            //act
            var result = _sut.GetParser("firstParserType");

            //assert
            Assert.That(result, Is.EqualTo(_firstParser));
        }

        [Test]
        public void GetParser_ThrowsSensorNotSupportedException_WhenParserIsNotFound()
        {
            //arrange
            //act
            //assert
            Assert.Throws<SensorNotSupportedException>(() => _sut.GetParser("notExistingParser"));
        }

        [SetUp]
        public void SetUp()
        {
            _firstParser = Substitute.For<IParser>();
            _firstParser.Type.Returns("firstParserType");

            _secondParser = Substitute.For<IParser>();
            _secondParser.Type.Returns("secondtParserType");

            var parsers = new []
            {
                _firstParser,
                _secondParser
            };
            _sut = new ParserProvider(parsers);
        }
        private IParserProvider _sut;
        private IParser _firstParser;
        private IParser _secondParser;
    }
}