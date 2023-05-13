using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TimeZoneInfo = System.TimeZoneInfo;

namespace BlastSharp.Dates;

public struct Timestamp
{
    public TimeZoneInfo TimeZone { get; }
    public int Year { get; }
    public int Month { get; }
    public int Day { get; }
    public int Hour { get; }
    public int Minute { get; }
    public int Second { get; }
    public int Millisecond { get; }
    public long Ticks => new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond).Ticks;

    public Timestamp(DateTime datetime, TimeZoneInfo timezone)
    {
        TimeZone = timezone;

        datetime = TimeZoneInfo.ConvertTime(datetime, datetime.Kind == DateTimeKind.Local ? TimeZoneInfo.Local : TimeZoneInfo.Utc, timezone);

        Year = datetime.Year;
        Month = datetime.Month;
        Day = datetime.Day;
        Hour = datetime.Hour;
        Minute = datetime.Minute;
        Second = datetime.Second;
        Millisecond = datetime.Millisecond;
    }
    
    public Timestamp(long ticks, TimeZoneInfo timezone)
    {
        TimeZone = timezone;

        var datetime = new DateTime(ticks);
        
        Year = datetime.Year;
        Month = datetime.Month;
        Day = datetime.Day;
        Hour = datetime.Hour;
        Minute = datetime.Minute;
        Second = datetime.Second;
        Millisecond = datetime.Millisecond;
    }
    
    public Timestamp(int year, int month, int day, int hour, int minute, int second, int millisecond, TimeZoneInfo timezone)
    {
        TimeZone = timezone;

        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
        Millisecond = millisecond;
    }

    public static Timestamp Now()
    {
        return new Timestamp(DateTime.UtcNow, TimeZoneInfo.Utc);
    }

    public static Timestamp Now(TimeZoneInfo timeZone)
    {
        return new Timestamp(DateTime.UtcNow, timeZone);
    }

    public static TimeSpan operator -(Timestamp a, Timestamp b)
    {
        return new TimeSpan(a.Ticks - b.Ticks);
    }
    
    public static Timestamp operator +(Timestamp a, TimeSpan b)
    {
        return new Timestamp(a.Ticks + b.Ticks, a.TimeZone);
    }

    public static bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
    }

    public Timestamp Date()
    {
        return new Timestamp(Year, Month, Day, 0, 0, 0, 0, TimeZone);
    }

    public Timestamp ToTimeZone(TimeZoneInfo timezone) => 
        new(TimeZoneInfo.ConvertTime(new DateTime(Ticks), TimeZone, timezone), 
            timezone);
}


