using HarmonyLib;

[Harmony]
internal class Program
{
    private static void Main(string[] args)
    {
        Program program = new Program();
        Console.WriteLine("Hello, World!");
        var harmony = new Harmony("eee");
        harmony.PatchAll();

        while (true)
        {
            program.Update();
        }
    }

    protected void Update()
    {
        Console.WriteLine("program update");
        UpdateGamePlay();
    }
    protected void UpdateGamePlay()
    {
        Console.WriteLine("program update gameplay");
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Program), nameof(Update))]
    static void PrefixUpdate()
    {
        Console.WriteLine("prefix update");
    }

}