//---------------------------------------------------------------------------------------------------------
//	Copyright ©  - 2017 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
//	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
//---------------------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;

public static class DateTimeHelperClass
{
    private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
    public static long CurrentUnixTimeMillis()
    {
        return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
    }

    /// <summary>
    /// https://stackoverflow.com/a/44136515/13254773
    /// </summary>
    /// <returns></returns>
    public static long nanoTime()
    {
        /*long nano = 10000L * Stopwatch.GetTimestamp();
        nano /= TimeSpan.TicksPerMillisecond;
        nano *= 100L;
        return nano;*/
        return (long)(System.DateTime.UtcNow.Ticks - Jan1st1970.Ticks) * 100L;
    }
}