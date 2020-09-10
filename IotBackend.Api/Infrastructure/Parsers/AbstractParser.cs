using System;
using System.Collections.Generic;
using System.IO;
using IotBackend.Api.Infrastructure.Models;

namespace IotBackend.Api.Infrastructure.Parsers
{
    public abstract class AbstractParser : IParser
    {
        public string Type { get; }
        private Func<Stream, StreamReader> _streamReaderProvider;

        public AbstractParser(string type, Func<Stream, StreamReader> streamReaderProvider)
        {
            Type = type;
            _streamReaderProvider = streamReaderProvider;
        }

        public List<ISensorData> ParseStream(Stream stream)
        {
            var result = new List<ISensorData>();
            using(var reader = _streamReaderProvider(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(ParseLine(line));
                }
            }
            return result;
        }

        protected abstract ISensorData ParseLine(string line);
    }
}