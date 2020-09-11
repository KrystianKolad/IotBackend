using System.Net;

namespace IotBackend.Api.Infrastructure.Exceptions
{
    public class DataNotFoundException : BaseException
    {
        public DataNotFoundException() : base("", (int)HttpStatusCode.NoContent)
        {
        }
    }
}