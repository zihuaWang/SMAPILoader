using Xamarin.Android.AssemblyStore;

internal class Program
{
    private static void Main(string[] args)
    {
        string assembliesFolderPath = @"C:\Users\narat\Desktop\Stardew Valley Android\Apks Latest File\assemblies";
        string outputDir = Path.Combine(assembliesFolderPath, "output-console-app");
        Directory.CreateDirectory(outputDir);
        var blobPath = Path.Combine(assembliesFolderPath, "assemblies.blob");
        var asmStore = new AssemblyStoreExplorer(blobPath, keepStoreInMemory: true);

        foreach (var assembly in asmStore.Assemblies)
        {
            assembly.ExtractImage(outputDir);
            Console.WriteLine("save dll: " + assembly.DllName);
        }
        Console.Read();
    }
}