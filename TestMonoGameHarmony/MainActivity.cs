using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using HarmonyLib;

using Microsoft.Xna.Framework;
using MonoMod.Core;
using MonoMod.Utils;
using System;

namespace TestMonoGameHarmony
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.FullUser,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class MainActivity : AndroidGameActivity
    {
        private Game1 _game;
        private View _view;

        Harmony harmony;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            harmony = new("Game");
            try
            {
                int ww = 0;
                //var current.CreateDetour();
                harmony.PatchAll();
                Hello();
            }
            catch (Exception ex)
            {
                var msg = ex.ToString();
            }


            _game = new Game1();
            _view = _game.Services.GetService(typeof(View)) as View;
            SetContentView(_view);
            _game.Run();

        }
        public void Hello()
        {
            int ww = 1;
        }
    }
    [HarmonyPatch]
    static class MainPacther
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MainActivity), nameof(MainActivity.Hello))]
        static void PrefixHello()
        {
            int ww = 1;
        }
    }
}
