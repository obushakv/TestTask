using System;

namespace LegacyApp.Extensions;

public static class DateTimeExtensions
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long ConvertToTimestamp(this DateTime value)
    {
        var elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalSeconds;
    }
}