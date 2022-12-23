namespace BlastSharp.Numbers;

public static class Extensions
{
    public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
    
    public static float Remap(this int value, int fromMin, int fromMax, int toMin, int toMax)
    {
        return (float)(value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    public static int Floor(this float value)
    {
        return (int)Math.Floor(value);
    }

    public static int Ceiling(this double value)
    {
        return (int) Math.Ceiling(value);
    }
}