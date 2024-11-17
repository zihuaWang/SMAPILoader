using System.Reflection;
namespace SMAPILoader;

public static class Program
{
    const bool IsEnableLog = true;
    public static void Log(object msg)
    {
        if (!IsEnableLog)
            return;

        Android.Util.Log.Debug("SMAPI-Tag", msg?.ToString());
    }
    public static void Start()
    {
        Log("");
        Log("Starting SMAPI Loader DLL...");


        try
        {
            StartInternal();
        }
        catch (Exception e)
        {
            Log("Error try Start(); " + e);
        }


        Log("Successfully Start SMAPI Loader");
        Log("");

    }
    static void StartInternal()
    {
        var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        Log("current dir: " + currentDir);
        var smapiAsm = Assembly.LoadFrom(currentDir + "/StardewModdingAPI.dll");
        var program = smapiAsm.GetType("StardewModdingAPI.Program");
        var program_Main = program.GetMethod("StartFromSMAPILoader", BindingFlags.Static | BindingFlags.Public);
        program_Main.Invoke(null, []);
    }
}