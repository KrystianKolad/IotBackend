using System;
using System.Globalization;
using System.IO;
using IotBackend.Api.Infrastructure.Consts;
using IotBackend.Api.Infrastructure.Models;

namespace IotBackend.Api.Infrastructure.Parsers
{
    public class TemperatureParser : AbstractParser
    {
        public TemperatureParser(Func<Stream, StreamReader> streamReaderProvider) : base(SensorTypesConsts.Temperature, streamReaderProvider)
        {
        }

        protected override ISensorData ParseLine(string line)
        {
            var parts = line.Split(";");
            return new Temperature { TimeStamp = DateTime.Parse(parts[0]), Value = float.Parse(parts[1], new CultureInfo("pl-PL").NumberFormat) };
        }
    }
}