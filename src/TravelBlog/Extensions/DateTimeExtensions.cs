using System;

namespace TravelBlog.Extensions;

public static class DateTimeExtensions
{
    public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    {
        return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
    }
}
