using Android.App;
using Android.Content.PM;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FakeStardewGame;

[Activity]
public sealed class SMAPIActivity : AndroidGameActivity
{
    static SMAPIActivity()
    {
        AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }
    public static SMAPIActivity instance { get; private set; }
    protected override void OnCreate(Android.OS.Bundle bundle)
    {
        instance = this;

        try
        {
            var smapiLoaderAsm = Assembly.LoadFrom(ApplicationContext.GetExternalFilesDir(null) + "/SMAPILoader.dll");
            smapiLoaderAsm.GetType("SMAPILoader.Program").GetMethod("Start", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
        }
        catch (Exception e)
        {
            Android.Util.Log.Error("SMAPI-Tag", e.ToString());
        }

        base.OnCreate(bundle);
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        Console.WriteLine("try solve asm: " + args.Name);
        //var newAssemblyDirectory = instance.GetExternalFilesDir(null).AbsolutePath;
        //var assemblyName = new AssemblyName(args.Name).Name;
        //var assemblyPath = System.IO.Path.Combine(newAssemblyDirectory, $"{assemblyName}.dll");
        //var resultAssembly = Assembly.LoadFrom(assemblyPath);
        //Console.WriteLine("result Assembly.LoadFrom: " + resultAssembly);

        //return resultAssembly;
        return null;
    }
    static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
    {
        Console.WriteLine("on asm loaded: " + args.LoadedAssembly.FullName);
    }
}
