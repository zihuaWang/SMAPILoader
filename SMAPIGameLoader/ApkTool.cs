using Android.Content.PM;
using Android.OS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPIGameLoader;

internal static class ApkTool
{
    public const string PackageName = "com.chucklefish.stardewvalley";
    public static PackageInfo PackageInfo => SMAPIActivity.Instance.PackageManager.GetPackageInfo(PackageName, 0);
    public static ApplicationInfo ApplicationInfo
    {
        get
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                return SMAPIActivity.Instance.PackageManager.GetApplicationInfo(PackageName,
                    PackageManager.ApplicationInfoFlags.Of(0));

            return SMAPIActivity.Instance.PackageManager.GetApplicationInfo(PackageName, 0);
        }
    }
    public static bool IsInstalled
    {
        get
        {
            try
            {
                var version = PackageInfo.VersionName;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public static void StartGame()
    {
        var intent = SMAPIActivity.Instance.PackageManager.GetLaunchIntentForPackage(PackageName);
        SMAPIActivity.Instance.StartActivity(intent);
    }
    public static string BaseApkPath => ApkTool.PackageInfo.ApplicationInfo.PublicSourceDir;
    public static IList<string> SplitApks => ApkTool.PackageInfo.ApplicationInfo.SplitSourceDirs;
    public static string ContentApkPath = SplitApks.First(path => path.Contains("split_content"));
    public static string ConfigApkPath => SplitApks.First(path => path.Contains("split_config"));
}
