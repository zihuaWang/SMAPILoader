using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Mobile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace SMAPIGameLoader;

[Activity(
    Label = "@string/app_name",
    MainLauncher = true,
    Icon = "@drawable/icon",
    Theme = "@style/Theme.Splash",
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ScreenOrientation = ScreenOrientation.SensorLandscape,
    ConfigurationChanges = (ConfigChanges.Keyboard
        | ConfigChanges.KeyboardHidden | ConfigChanges.Orientation
        | ConfigChanges.ScreenLayout | ConfigChanges.ScreenSize
        | ConfigChanges.UiMode))]
public class SMAPIActivity : AndroidGameActivity
{
    public SMAPIActivity()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in assemblies)
        {
            Console.WriteLine("already loaded in ctor: " + asm);
        }

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

    }
    public static SMAPIActivity Instance { get; private set; }
    public static string ExternalFilesDir => Instance.ApplicationContext.GetExternalFilesDir(null).AbsolutePath;

    Bundle currentBundle;
    public string getStardewDllFilePath => ExternalFilesDir + "/StardewValley.dll";
    protected override void OnCreate(Bundle bundle)
    {
        Instance = this;
        currentBundle = bundle;
        if (!ApkTool.IsInstalled)
        {
            Finish();
            return;
        }
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var asm in assemblies)
        {
            Console.WriteLine("already loaded in OnCreate(): " + asm);
        }

        LaunchGame();
    }
    void PrepareDll()
    {
        //fix assembly first
        var stardewDllFilePath = ExternalFilesDir + "/StardewValley.dll";
        MainActivityRewriter.Rewrite(stardewDllFilePath, out var isRewrite);
        if (isRewrite)
        {
            //exit app & new load dll again
            Finish();
        }
        Console.WriteLine("done setup dll references");
        //fix bug can't load or assembly 
        Assembly.LoadFrom(getStardewDllFilePath);
    }
    //Assembly stardewAssembly;
    void LaunchGame()
    {
        //prepare references
        PrepareDll();

        //copy game content assets

        //setup Activity
        IntegrateStardewMainActivity();

        //ready
        SV_OnCreate();
    }
    void IntegrateStardewMainActivity()
    {
        var instance_Field = typeof(MainActivity).GetField("instance", BindingFlags.Static | BindingFlags.Public);
        instance_Field.SetValue(null, this);
        Console.WriteLine("done setup MainActivity.instance with: " + instance_Field.GetValue(null));

    }
    static string[] _dependenciesDirectorySearch;
    public static string[] GetDependenciesDirectorySearch
    {
        get
        {
            if (_dependenciesDirectorySearch == null)
            {
                _dependenciesDirectorySearch = [
                       ExternalFilesDir,
                ];
            }
            return _dependenciesDirectorySearch;
        }
    }
    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        Console.WriteLine("try resolve assembly name: " + args.Name);
        //manual load at external files dir
        string assemblyName = new AssemblyName(args.Name).Name;
        var dllFileName = assemblyName + ".dll";
        foreach (var dir in GetDependenciesDirectorySearch)
        {
            var asm = Assembly.LoadFrom(Path.Combine(dir, dllFileName));
            if (asm != null)
                return asm;

        }
        Console.WriteLine("error can't resolve asm: " + args.Name);
        return null;
    }
    static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        Console.WriteLine("on asm loaded: " + args.LoadedAssembly.FullName);
    }


    void SV_OnCreate()
    {
        Log.It("MainActivity.OnCreate");
        RequestWindowFeature(WindowFeatures.NoTitle);
        if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
        {
            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
        }
        Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
        base.OnCreate(currentBundle);
        CheckAppPermissions();
    }
    public void LogPermissions()
    {
        Log.It("MainActivity.LogPermissions method PackageManager: , AccessNetworkState:" + PackageManager.CheckPermission("android.permission.ACCESS_NETWORK_STATE", PackageName).ToString() + ", AccessWifiState:" + PackageManager.CheckPermission("android.permission.ACCESS_WIFI_STATE", PackageName).ToString() + ", Internet:" + PackageManager.CheckPermission("android.permission.INTERNET", PackageName).ToString() + ", Vibrate:" + PackageManager.CheckPermission("android.permission.VIBRATE", PackageName));
        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            Log.It("MainActivity.LogPermissions: , AccessNetworkState:" + CheckSelfPermission("android.permission.ACCESS_NETWORK_STATE").ToString() + ", AccessWifiState:" + CheckSelfPermission("android.permission.ACCESS_WIFI_STATE").ToString() + ", Internet:" + CheckSelfPermission("android.permission.INTERNET").ToString() + ", Vibrate:" + CheckSelfPermission("android.permission.VIBRATE"));
        }
    }
    public bool HasPermissions
    {
        get
        {
            if (PackageManager.CheckPermission("android.permission.ACCESS_NETWORK_STATE", PackageName) == Permission.Granted
                && PackageManager.CheckPermission("android.permission.ACCESS_WIFI_STATE", PackageName) == Permission.Granted
                && PackageManager.CheckPermission("android.permission.INTERNET", PackageName) == Permission.Granted
                && PackageManager.CheckPermission("android.permission.VIBRATE", PackageName) == Permission.Granted)
            {
                return true;
            }
            return false;
        }
    }

    public void CheckAppPermissions()
    {
        LogPermissions();
        if (HasPermissions)
        {
            Log.It("MainActivity.CheckAppPermissions permissions already granted.");
            OnCreatePartTwo();
        }
        else
        {
            Log.It("MainActivity.CheckAppPermissions PromptForPermissions C");
            PromptForPermissionsWithReasonFirst();
        }
    }
    public static string PermissionMessageA(string languageCode)
    {
        //var method = MainActivityType.GetMethod("PermissionMessageA",

        //    BindingFlags.Instance | BindingFlags.NonPublic);
        //return method.Invoke(SV_instance, new object[] { languageCode }) as string;
        return "";
    }
    public static string PermissionMessageB(string languageCode)
    {
        //var method = typeof(MainActivity).GetMethod("PermissionMessageB",
        //    BindingFlags.Instance | BindingFlags.NonPublic);
        //return method.Invoke(SV_instance, new object[] { languageCode }) as string;
        return "";

    }
    public static string GetOKString(string languageCode)
    {
        //var method = typeof(MainActivity).GetMethod("GetOKString",
        //    BindingFlags.Instance | BindingFlags.NonPublic);
        //return method.Invoke(SV_instance, new object[] { languageCode }) as string;
        return "";
    }

    private void PromptForPermissionsWithReasonFirst()
    {
        Log.It("MainActivity.PromptForPermissionsWithReasonFirst...");
        if (!HasPermissions)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            string languageCode = Locale.Default.Language.Substring(0, 2);
            builder.SetMessage(PermissionMessageA(languageCode));
            builder.SetCancelable(false);
            builder.SetPositiveButton(GetOKString(languageCode), delegate
            {
                Log.It("MainActivity.PromptForPermissionsWithReasonFirst PromptForPermissions A");
                PromptForPermissions();
            });
            Dialog dialog = builder.Create();
            if (!IsFinishing)
            {
                dialog.Show();
            }
        }
        else
        {
            Log.It("MainActivity.PromptForPermissionsWithReasonFirst PromptForPermissions B");
            PromptForPermissions();
        }
    }
    private string[] requiredPermissions => [
        "android.permission.ACCESS_NETWORK_STATE", "android.permission.ACCESS_WIFI_STATE",
        "android.permission.INTERNET", "android.permission.VIBRATE"
    ];
    private string[] deniedPermissionsArray
    {
        get
        {
            List<string> list = new List<string>();
            string[] array = requiredPermissions;
            for (int i = 0; i < array.Length; i++)
            {
                if (PackageManager.CheckPermission(array[i], PackageName) != 0)
                {
                    list.Add(array[i]);
                }
            }
            return list.ToArray();
        }
    }
    public void PromptForPermissions()
    {
        Log.It("MainActivity.PromptForPermissions requesting permissions...deniedPermissionsArray:" + deniedPermissionsArray.Length);
        string[] array = deniedPermissionsArray;
        if (array.Length != 0)
        {
            Log.It("PromptForPermissions permissionsArray:" + array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                Log.It("PromptForPermissions permissionsArray[" + i + "]: " + array[i]);
            }
            RequestPermissions(array, 0);
        }
    }

    private void OnCreatePartTwo()
    {
        Log.It("MainActivity.OnCreatePartTwo");
        SetupDisplaySettings();
        SetPaddingForMenus();
        //setup SMAPI & SGameRunner
        bool isRunSMAPI = false;
        if (isRunSMAPI)
        {
            //Program.Main([]);
            //var gameRunner = GameRunner.instance;
        }
        else
        {
            var gameRunner = new GameRunner();
            GameRunner.instance = gameRunner;
        }

        SetContentView((View)GameRunner.instance.Services.GetService(typeof(View)));
        GameRunner.instance.Run();
    }
    public int GetBuild()
    {
        Context context = Application.Context;
        PackageManager packageManager = context.PackageManager;
        PackageInfo packageInfo = packageManager.GetPackageInfo(context.PackageName, (PackageInfoFlags)0);
        return packageInfo.VersionCode;
    }
    public void SetPaddingForMenus()
    {
        Log.It("MainActivity.SetPaddingForMenus build:" + GetBuild());
        if (Build.VERSION.SdkInt >= BuildVersionCodes.P && Window != null && Window.DecorView != null && Window.DecorView.RootWindowInsets != null && Window.DecorView.RootWindowInsets.DisplayCutout != null)
        {
            DisplayCutout displayCutout = Window.DecorView.RootWindowInsets.DisplayCutout;
            Log.It("MainActivity.SetPaddingForMenus DisplayCutout:" + displayCutout);
            if (displayCutout.SafeInsetLeft > 0 || displayCutout.SafeInsetRight > 0)
            {
                int num = Math.Max(displayCutout.SafeInsetLeft, displayCutout.SafeInsetRight);
                Game1.xEdge = Math.Min(90, num);
                Game1.toolbarPaddingX = num;
                Log.It("MainActivity.SetPaddingForMenus CUT OUT toolbarPaddingX:" + Game1.toolbarPaddingX + ", xEdge:" + Game1.xEdge);
                return;
            }
        }
        string manufacturer = Build.Manufacturer;
        string model = Build.Model;
        DisplayMetrics displayMetrics = new DisplayMetrics();
        WindowManager.DefaultDisplay.GetRealMetrics(displayMetrics);
        if (displayMetrics.HeightPixels >= 1920 || displayMetrics.WidthPixels >= 1920)
        {
            Game1.xEdge = 20;
            Game1.toolbarPaddingX = 20;
        }
    }

    //MobileDisplay
    public static void SetupDisplaySettings()
    {
        var MobileDisplayType = typeof(MainActivity).Assembly.GetType("StardewValley.Mobile.MobileDisplay");
        MobileDisplayType.GetMethod("SetupDisplaySettings", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
        return;
        SMAPIActivity instance = SMAPIActivity.Instance;
        Display defaultDisplay = instance.WindowManager.DefaultDisplay;
        Android.Graphics.Point point = new();
        defaultDisplay.GetRealSize(point);
        int x = point.X;
        int y = point.Y;
        DisplayMetrics displayMetrics = instance.Resources.DisplayMetrics;
        int ppi = Math.Max((int)displayMetrics.DensityDpi, Math.Max((int)displayMetrics.Xdpi, (int)displayMetrics.Ydpi));
        MobileDisplayType.GetMethod("Android_SetDisplaySettings",
            BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { x, y, ppi, 0 });
        PrintInfo(null, x, y, ppi);
    }
    private static void PrintInfo(MobileDevice? device, int pixelWidth, int pixelHeight, int ppi)
    {
    }
}
