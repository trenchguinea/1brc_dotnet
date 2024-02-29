using System.Diagnostics;

namespace ConsoleApp;

public static class Time
{
    public static void Me(string name, Action fn)
    {
        TimeSpan span;
        var sw = Stopwatch.StartNew();
        try
        {
            fn();
        }
        finally
        {
            sw.Stop();
            span = sw.Elapsed;
        }

        Console.WriteLine($"Time of {name}: {span.Microseconds}");
    }
    
    public static T Me<T>(string name, Func<T> fn)
    {
        TimeSpan span;
        T val;
        var sw = Stopwatch.StartNew();
        try
        {
            val = fn();
        }
        finally
        {
            sw.Stop();
            span = sw.Elapsed;
        }

        Console.WriteLine($"Time of {name}: {span.Microseconds}");
        return val;
    }

    public static T Me<T>(string name, Func<object?, T> fn, object? arg)
    {
        TimeSpan span;
        T val;
        var sw = Stopwatch.StartNew();
        try
        {
            val = fn(arg);
        }
        finally
        {
            sw.Stop();
            span = sw.Elapsed;
        }

        Console.WriteLine($"Time of {name}: {span.Microseconds}");
        return val;
    }
}