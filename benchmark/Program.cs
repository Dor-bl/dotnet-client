using System;
using System.Diagnostics;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        var dict = new Dictionary<string, object>();
        for (int i = 0; i < 50; i++)
        {
            dict.Add($"key_{i}", $"value_{i}");
        }

        // warmup
        OldWay(dict);
        NewWay(dict);

        var sw = new Stopwatch();

        sw.Start();
        for(int i = 0; i < 100000; i++) OldWay(dict);
        sw.Stop();
        Console.WriteLine($"Old way: {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        for(int i = 0; i < 100000; i++) NewWay(dict);
        sw.Stop();
        Console.WriteLine($"New way: {sw.ElapsedMilliseconds} ms");
    }

    public static string OldWay(IDictionary<string, object> dict)
    {
        string result = string.Empty;
        foreach (var item in dict)
        {
            object value = item.Value;
            if (value == null) continue;

            value = $"\\\"{value}\\\"";
            string key = $"\\\"{item.Key}\\\"";
            if (string.IsNullOrEmpty(result))
            {
                result = $"{key}: {value}";
            }
            else
            {
                result = result + ", " + key + ": " + value;
            }
        }
        return "\"{" + result + "}\"";
    }

    public static string NewWay(IDictionary<string, object> dict)
    {
        var elements = new List<string>(dict.Count);
        foreach (var item in dict)
        {
            object value = item.Value;
            if (value == null) continue;

            value = $"\\\"{value}\\\"";
            string key = $"\\\"{item.Key}\\\"";
            elements.Add($"{key}: {value}");
        }
        return "\"{" + string.Join(", ", elements) + "}\"";
    }
}
