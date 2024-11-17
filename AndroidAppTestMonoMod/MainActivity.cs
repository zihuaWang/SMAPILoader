using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.RuntimeDetour;

namespace AndroidAppTestMonoMod;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        //setup hook

        var gameMethod = typeof(Game).GetMethod(nameof(Game.Start), BindingFlags.Static | BindingFlags.Public);
        var detourMethod = typeof(GamePatcher).GetMethod(nameof(GamePatcher.PrefixStart), BindingFlags.Static | BindingFlags.Public);
        var d = new Hook(gameMethod, detourMethod);
        Console.WriteLine("done create detour: " + d);

        //setup game
        try
        {
            Game.Start(); //app crash here
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        int ww = 0;
    }
}

static class GamePatcher
{
    public static void PrefixStart()
    {
        Console.WriteLine("On Prefix Start");
    }
}

static class Game
{
    public static void Start()
    {
        Console.WriteLine("On Game.Start()");
    }
}