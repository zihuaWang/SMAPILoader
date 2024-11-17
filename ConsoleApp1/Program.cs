using MonoMod.RuntimeDetour;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var srcMethod = typeof(Game).GetMethod("Start", BindingFlags.Static | BindingFlags.Public);
        var detourMethod = typeof(Game).GetMethod(nameof(Game.PrefixStart), BindingFlags.Static | BindingFlags.Public);
        var d = new Hook(srcMethod, detourMethod);

        Game game = new Game();
        Game.Start();

        int ww = 0;

    }
}
class Game
{
    public static void Start()
    {

    }
    public static void PrefixStart()
    {
        Console.WriteLine("on prefix start");
    }
}