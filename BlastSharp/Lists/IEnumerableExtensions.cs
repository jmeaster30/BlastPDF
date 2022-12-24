namespace BlastSharp.Lists;

public static class EnumerableExtensions {
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
    
    public static IEnumerable<T> PadLeft<T>(this IEnumerable<T> a, int amount, T value)
    {
        int toAdd = amount - a.Count();
        if (toAdd <= 0) return a;
        for (int i = 0; i < toAdd; i++)
        {
            a = a.Prepend(value);
        }
        
        return a;
    }

    // TODO I feel like this API could be better :/
    public static IEnumerable<(T, int)> RunGroupBy<T, U>(this IEnumerable<T> input, Func<T, U> selector, int maxChunkSize)
    {
        var result = new List<(T, int)>();
        (T, int) current = (default!, 0);
        foreach (var i in input)
        {
            if (current.Item2 != 0 && EqualityComparer<U>.Default.Equals(selector(current.Item1), selector(i)) && current.Item2 < maxChunkSize - 1)
            {
                current = (current.Item1, current.Item2 + 1);
            }
            else
            {
                if (current.Item2 > 0) result.Add(current);
                current = (i, 1);
            }
        }
        if (current.Item2 > 0) result.Add(current);
        return result;
    }

    public static IEnumerable<T> Repeat<T>(this T value, int amount)
    {
        var results = new List<T>();
        while (results.Count < amount)
        {
            results.Add(value);
        }

        return results;
    }

    public static (int, T?, T?) FirstDifference<T>(this IEnumerable<T> left, IEnumerable<T> right) where T : struct
    {
        var index = 0;
        T? lResult = null;
        T? rResult = null;
        for (; index < left.Count(); index++)
        {
            lResult = left.ElementAt(index);
            rResult = index < right.Count() ? right.ElementAt(index) : null;
            if (rResult == null) break;
            if (!EqualityComparer<T>.Default.Equals(lResult.Value, rResult.Value)) break;
        }

        if (index == left.Count() && index == right.Count())
        {
            return (-1, null, null);
        }
        
        return (index, lResult, rResult);
    }

    public static string Hash(this IEnumerable<byte> input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        return Convert.ToHexString(md5.ComputeHash(input.ToArray()));
    }
}