using System;
using IotBackend.Api.Infrastructure.Extensions;

namespace IotBackend.Api.Infrastructure.Builders
{
    public interface IFilePathBuilder
    {
        string BuildFilePath(string deviceName, string sensorType, DateTime date);

        string BuildHistoricalFilePath(string deviceName, string sensorType);
    }
    public class FilePathBuilder : IFilePathBuilder
    {
        public string BuildFilePath(string deviceName, string sensorType, DateTime date)
        {
            return $"{deviceName}/{sensorType}/{date.ToFileName()}";
        }

        public string BuildHistoricalFilePath(string deviceName, string sensorType)
        {
            return $"{deviceName}/{sensorType}/historical.zip";
        }
    }
}