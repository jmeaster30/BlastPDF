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
}