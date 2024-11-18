
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using System;
using System.IO;
using System.IO.Compression;
using HarmonyLib;

namespace SMAPIGameLoader;

[HarmonyPatch]
static class GameAssetTool
{
    public static void SetupLoadAssetPathHook()
    {
        Console.WriteLine("done harmony patch all");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TitleContainer), nameof(TitleContainer.OpenStream))]
    static bool PrefixOpenStream(ref Stream __result, string name)
    {
        __result = FixOpenStream(name);
        return false;
    }
    public const string StardewAssetFolderName = "Stardew Assets";
    static string _gameAssetDir = null;
    static string GetGameAssetsDir
    {
        get
        {
            if (_gameAssetDir == null)
                _gameAssetDir = Path.Combine(FileTool.ExternalFilesDir, StardewAssetFolderName);
            return _gameAssetDir;
        }
    }
    static Stream FixOpenStream(string assetName)
    {
        try
        {
            assetName = assetName.Replace("//", "/"); //safePath
            assetName = assetName.Replace("\\", "/"); //safePath
            var rootDirectory = GetGameAssetsDir;
            string assetAbsolutePath = Path.Combine(rootDirectory, assetName);
            return File.OpenRead(assetAbsolutePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(GameAssetTool), nameof(VerifyAssets))]
    //static void PrefixVerifyAssets()
    //{
    //    Console.WriteLine("on prefix verify asset");
    //}
    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(ContentManager), "OpenStream")]
    //static void Prefix_CMOpenStream(string name)
    //{
    //    Console.WriteLine("Prefix try CM open stream: " + name);
    //}

    //[HarmonyPrefix]
    //[HarmonyPatch(typeof(Game1), "CreateContentManager")]
    //static void PrefixCreateContentManager(IServiceProvider serviceProvider, string rootDirectory)
    //{
    //    Console.WriteLine("On PrefixCreateContentManager");
    //}

    public static void VerifyAssets()
    {
        //check & update game content
        var baseContentApk = ApkTool.ContentApkPath;
        using (FileStream apkFileStream = new FileStream(baseContentApk, FileMode.Open, FileAccess.Read))
        using (ZipArchive apkArchive = new ZipArchive(apkFileStream, ZipArchiveMode.Read))
        {
            //Console.WriteLine("Contents of APK:");
            var externalAssetsDir = Path.Combine(FileTool.ExternalFilesDir, StardewAssetFolderName);
            foreach (ZipArchiveEntry entry in apkArchive.Entries)
            {
                if (entry.FullName.StartsWith("assets/Content") == false)
                    continue;

                //Console.WriteLine($"- {entry.FullName} ({entry.Length} bytes), date: {entry.LastWriteTime}");

                var destFilePath = Path.Combine(externalAssetsDir, entry.FullName.Replace("assets/", ""));
                var destFolderFullPath = Path.GetDirectoryName(destFilePath);
                if (Directory.Exists(destFolderFullPath) == false)
                {
                    Directory.CreateDirectory(destFolderFullPath);
                }
                using var entryStream = entry.Open();
                //if (File.Exists(destFilePath))
                //{
                //check if same file don't clone just skip
                //using var destFileStreamCheck = File.OpenRead(destFilePath);
                //if (FileTool.IsSameFile(entryStream, destFileStreamCheck))
                //{
                //    Console.WriteLine("skip file: " + destFilePath);
                //    continue;
                //}
                //}

                using var destFileStream = new FileStream(destFilePath, FileMode.Create, FileAccess.ReadWrite);
                entryStream.CopyTo(destFileStream);
                //Console.WriteLine("done clone file to: " + destFilePath);
            }
        }

        Console.WriteLine("successfully verify & clone assets");
    }
}

