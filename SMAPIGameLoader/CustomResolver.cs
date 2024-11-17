using System;
using Mono.Cecil;

namespace SMAPIGameLoader;

internal class CustomResolver : BaseAssemblyResolver
{
    private DefaultAssemblyResolver _defaultResolver;

    public CustomResolver()
    {
        _defaultResolver = new DefaultAssemblyResolver();
    }

    public override AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        AssemblyDefinition assembly = null;
        try
        {
            assembly = _defaultResolver.Resolve(name);
        }
        catch (AssemblyResolutionException ex)
        {
            var loadPath = SMAPIActivity.ExternalFilesDir + "/" + name.Name + ".dll";
            Console.WriteLine("try resolve manual path: " + loadPath);
            assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(loadPath);
            Console.WriteLine("loading assembly: " + assembly);
        }
        return assembly;
    }
}
