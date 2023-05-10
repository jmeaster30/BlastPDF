namespace BlastSharp.Dates;

public static class BlastDateTime
{
    public static DateTime Day(int day) => new DateTime(0, 0, day);
    public static DateTime Month(int month) => new DateTime(0, month, 0);
    public static DateTime Year(int year) => new DateTime(year, 0, 0);
    
    public static DateTime Day(this DateTime datetime, int day) => new DateTime(datetime.Year, datetime.Month, day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime Month(this DateTime datetime, int month) => new DateTime(datetime.Year, month, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime Year(this DateTime datetime, int year) => new DateTime(year, datetime.Month, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    
    public static DateTime January() => new DateTime(0, 1, 0);
    public static DateTime February() => new DateTime(0, 2, 0);
    public static DateTime March() => new DateTime(0, 3, 0);
    public static DateTime April() => new DateTime(0, 4, 0);
    public static DateTime May() => new DateTime(0, 5, 0);
    public static DateTime June() => new DateTime(0, 6, 0);
    public static DateTime July() => new DateTime(0, 7, 0);
    public static DateTime August() => new DateTime(0, 8, 0);
    public static DateTime September() => new DateTime(0, 9, 0);
    public static DateTime October() => new DateTime(0, 10, 0);
    public static DateTime November() => new DateTime(0, 11, 0);
    public static DateTime December() => new DateTime(0, 12, 0);
    
    public static DateTime January(this DateTime datetime) => new DateTime(datetime.Year, 1, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime February(this DateTime datetime) => new DateTime(datetime.Year, 2, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime March(this DateTime datetime) => new DateTime(datetime.Year, 3, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime April(this DateTime datetime) => new DateTime(datetime.Year, 4, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime May(this DateTime datetime) => new DateTime(datetime.Year, 5, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime June(this DateTime datetime) => new DateTime(datetime.Year, 6, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime July(this DateTime datetime) => new DateTime(datetime.Year, 7, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime August(this DateTime datetime) => new DateTime(datetime.Year, 8, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime September(this DateTime datetime) => new DateTime(datetime.Year, 9, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime October(this DateTime datetime) => new DateTime(datetime.Year, 10, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime November(this DateTime datetime) => new DateTime(datetime.Year, 11, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);
    public static DateTime December(this DateTime datetime) => new DateTime(datetime.Year, 12, datetime.Day,
        datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond, datetime.Kind);

    public static DateTime AddWeeks(this DateTime dateTime, int n) => dateTime.AddDays(n * 7);
    public static DateTime NthWeekDay(this DateTime datetime, int n, DayOfWeek dayOfWeek) =>
        datetime.AddDays((dayOfWeek - datetime.DayOfWeek + 7) % 7).AddWeeks(n);
    
    
}