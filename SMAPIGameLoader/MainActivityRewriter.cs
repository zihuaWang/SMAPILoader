using System;
using System.Linq;
using Mono.Cecil;
using System.IO;

namespace SMAPIGameLoader;

internal static class MainActivityRewriter
{
    public static TypeReference FindType(ModuleDefinition moduleDefinition, Type type)
    {
        return moduleDefinition.Types.First(t => t.FullName == type.FullName);
    }
    public static ReaderParameters MonoCecilReaderConfig = new()
    {
        AssemblyResolver = new CustomResolver(),
    };
    public static ModuleDefinition ReadModule(string filePath)
    {
        return ModuleDefinition.ReadModule(filePath, MonoCecilReaderConfig);
    }
    public static ModuleDefinition ReadModule(Stream stream)
    {
        return ModuleDefinition.ReadModule(stream, MonoCecilReaderConfig);
    }
    public static string ExternalFilesDir => SMAPIActivity.ExternalFilesDir;
    public static void Rewrite(string stardewDllFilePath)
    {
        var stardewDllStream = File.Open(stardewDllFilePath, FileMode.Open, FileAccess.ReadWrite);
        var stardewModule = ReadModule(stardewDllStream);

        try
        {
            var mainActivityTypeDef = stardewModule.Types.First(t => t.Name == "MainActivity");
            var instance_FieldDef = mainActivityTypeDef.Fields.First(f => f.Name == "instance");
            //change DeclaringType MainActivity instance to SMAPIActivity instance;
            if (instance_FieldDef.DeclaringType.Name != typeof(SMAPIActivity).Name)
            {
                var importedSmapiActivityType = stardewModule.ImportReference(typeof(SMAPIActivity)).Resolve();
                importedSmapiActivityType.IsImport = true;
                instance_FieldDef.DeclaringType = importedSmapiActivityType;

                Console.WriteLine("done replace declaring MainActivity.instance to SMAPIActivity.instance");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        //var newModuleSaveFilePath = SMAPIActivity.ExternalFilesDir + "/StardewValley-New.dll";
        stardewModule.Write();
    }
}
