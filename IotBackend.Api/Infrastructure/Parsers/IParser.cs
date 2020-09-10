using System.Collections.Generic;
using System.IO;
using IotBackend.Api.Infrastructure.Models;

namespace IotBackend.Api.Infrastructure.Parsers
{
    public interface IParser
    {
        string Type { get; }
        List<ISensorData> ParseStream(Stream stream);
    }
}