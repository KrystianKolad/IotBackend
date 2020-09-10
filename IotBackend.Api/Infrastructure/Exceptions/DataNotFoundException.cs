using System.Net;

namespace IotBackend.Api.Infrastructure.Exceptions
{
    public class DataNotFoundException : BaseException
    {
        public DataNotFoundException(string fileName) : base($"File {fileName} was not found", (int)HttpStatusCode.NoContent)
        {
        }
    }
}