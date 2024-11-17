using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewValley;
using System;
using System.IO;
using System.IO.Compression;

namespace SMAPIGameLoader;

[HarmonyPatch]
internal static class GameAssetTool
{
    static Harmony harmony;
    public static void SetupLoadAssetPathHook()
    {
        harmony = new(nameof(GameAssetTool));
        harmony.PatchAll();
        Console.WriteLine("done harmony patch all");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TitleContainer), nameof(TitleContainer.OpenStream))]
    static void PrefixOpenStream(string name)
    {
        Console.WriteLine("Prefix try open stream: " + name);
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ContentManager), "OpenStream")]
    static void Prefix_CMOpenStream(string name)
    {
        Console.WriteLine("Prefix try CM open stream: " + name);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Game1), "CreateContentManager")]
    static void PrefixCreateContentManager(IServiceProvider serviceProvider, string rootDirectory)
    {
        Console.WriteLine("On PrefixCreateContentManager");
    }

    public static void VerifyAssets()
    {
        //check & update game content
        var baseContentApk = ApkTool.ContentApkPath;
        using (FileStream apkFileStream = new FileStream(baseContentApk, FileMode.Open, FileAccess.Read))
        using (ZipArchive apkArchive = new ZipArchive(apkFileStream, ZipArchiveMode.Read))
        {
            //Console.WriteLine("Contents of APK:");
            var externalAssetsDir = Path.Combine(FileTool.ExternalFilesDir, FileTool.StardewAssetFolderName);
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

