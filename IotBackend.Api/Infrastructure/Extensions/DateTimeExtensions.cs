using System;

namespace IotBackend.Api.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFileName(this DateTime date)
        {
            return $"{date:yyyy-MM-dd}.csv";
        }
    }
}