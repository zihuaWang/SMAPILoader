using System.Reflection;
namespace SMAPILoader;

public static class Program
{
    public static void Log(object msg)
    {
        Android.Util.Log.Debug("SMAPI-Tag", msg?.ToString());
    }
    public static void Start()
    {
        Android.Util.Log.Debug("SMAPI-Tag", "Starting SMAPI Loader DLL...");

        try
        {
            StartInternal();
        }
        catch (Exception e)
        {
            Log("Error try Start(); " + e);
        }


        Android.Util.Log.Debug("SMAPI-Tag", "Successfully Start SMAPI Loader");
    }
    static void StartInternal()
    {
        AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
        AppDomain.CurrentDomain.AssemblyResolve += AppDomain_AssemblyResolve;
        var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Log("current dir: " + currentDir);
        var smapiAsm = Assembly.LoadFrom(currentDir + "/StardewModdingAPI.dll");
        var program = smapiAsm.GetType("StardewModdingAPI.Program");
        var program_Main = program.GetMethod("StartFromSMAPILoader", BindingFlags.Static | BindingFlags.Public);
        program_Main.Invoke(null, []);
    }

    private static Assembly? CurrentDomain_TypeResolve(object? sender, ResolveEventArgs args)
    {
        Log("on type resolve: " + args.Name);
        return null;
    }

    static Assembly? AppDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
    {
        Log("On AssemblyResolve");
        Log("asem: " + args.Name);

        return null;
    }
}