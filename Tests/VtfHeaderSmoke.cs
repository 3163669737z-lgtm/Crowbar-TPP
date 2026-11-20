using System;
using System.Reflection;
using Crowbar;

internal static class VtfHeaderSmoke
{
    private static int Main(string[] args)
    {
        var headerType = typeof(App).Assembly.GetType("Crowbar.SourceVtfHeader", true);
        const BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic;
        try
        {
            object result = args.Length == 1
                ? headerType.GetMethod("Inspect", flags, null, new[] { typeof(string) }, null).Invoke(null, args)
                : headerType.GetMethod("SaveRepairedCopy", flags).Invoke(null, args);
            Console.WriteLine(result.GetType().GetProperty("NeedsRepair", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(result, null));
            return 0;
        }
        catch (TargetInvocationException error)
        {
            Console.WriteLine(error.InnerException.GetType().Name);
            return 1;
        }
    }
}

