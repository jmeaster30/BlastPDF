namespace BlastSharp.Lists;

public static class IEnumerableExtensions {
    public static IEnumerable<T> PadRight<T>(this IEnumerable<T> a, int amount, T value)
    {
        int toAdd = amount - a.Count();
        if (toAdd <= 0) return a;
        for (int i = 0; i < toAdd; i++)
        {
            a = a.Append(value);
        }
        
        return a;
    }
}