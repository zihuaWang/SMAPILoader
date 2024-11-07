internal class Program
{
    private static void Main(string[] args)
    {
        var originalAssemblyDir = @"C:\\Users\\narat\\Desktop\\Stardew Valley Android\\assemblies-original\\out";
        var modAssemblyDir = @"C:\Users\narat\Desktop\Stardew Valley Android\assemblies-mod\out";
        var fakeStardewGameAssemblyDir = @"C:\Users\narat\Desktop\Stardew Valley Android\FakeStardewGame.FakeStardewGame\unknown\assemblies\out";

        var originalFlies = Directory.GetFiles(originalAssemblyDir);
        var modAssemblyFiles = Directory.GetFiles(modAssemblyDir);

        Console.WriteLine("original files: " + originalFlies.Length);

        foreach (var originalFile in originalFlies)
        {
            var originalFileInfo = new FileInfo(originalFile);
            var pureAssemblyFilePath = Path.Combine(fakeStardewGameAssemblyDir, originalFileInfo.Name);
            var modAssemblyFilePath = Path.Combine(modAssemblyDir, originalFileInfo.Name);
            File.Copy(pureAssemblyFilePath, modAssemblyFilePath, true);
            Console.WriteLine("done copy to file: " + modAssemblyFilePath);
        }

        Console.ReadLine();
    }
}