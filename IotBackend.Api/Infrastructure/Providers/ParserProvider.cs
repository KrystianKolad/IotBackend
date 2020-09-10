using System.Collections.Generic;
using System.Linq;
using IotBackend.Api.Infrastructure.Exceptions;
using IotBackend.Api.Infrastructure.Parsers;

namespace IotBackend.Api.Infrastructure.Providers
{
    public interface IParserProvider
    {
        IParser GetParser(string sensorType);
    }
    public class ParserProvider : IParserProvider
    {
        private Dictionary<string, IParser> _parsersDictionary;
        public ParserProvider(IEnumerable<IParser> parsers)
        {
            _parsersDictionary = parsers.ToDictionary(x => x.Type, y => y);
        }
        public IParser GetParser(string sensorType)
        {
            if (_parsersDictionary.TryGetValue(sensorType, out var parser))
            {
                return parser;
            }
            throw new SensorNotSupportedException(sensorType);
        }
    }
}