using Android.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMAPIGameLoader;

internal static class FileTool
{
    public const string StardewValleyDllFileName = "StardewValley.dll";
    public const string MonoGameFrameworkFileName = "MonoGame.Framework.dll";
    public const string StardewAssetFolderName = "Stardew Assets";

    static Activity activity;
    public static string ExternalFilesDir => activity.GetExternalFilesDir(null).AbsolutePath;
    internal static void Init(SMAPIActivity sMAPIActivity)
    {
        activity = sMAPIActivity;
    }

    //simple data
    internal static bool IsSameFile(Stream leftStream, FileStream rightStream)
    {
        if (leftStream.Length != rightStream.Length)
            return false;
        return false;
        //return ComputeHash(leftStream) == ComputeHash(rightStream);
    }
    private static byte[] ComputeHash(Stream data)
    {
        using HashAlgorithm algorithm = MD5.Create();
        byte[] bytes = algorithm.ComputeHash(data);
        data.Seek(0, SeekOrigin.Begin); //I'll use this trick so the caller won't end up with the stream in unexpected position
        return bytes;
    }
}
