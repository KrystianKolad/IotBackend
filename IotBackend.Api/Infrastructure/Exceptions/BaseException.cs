using System;
using System.Net;

namespace IotBackend.Api.Infrastructure.Exceptions
{
    public abstract class BaseException : Exception
    {
        public int StatusCode { get; set; }
        public BaseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}