using System;
using Mono.Cecil;

namespace SMAPIGameLoader;


internal class CustomResolver : BaseAssemblyResolver
{
    private DefaultAssemblyResolver _defaultResolver;

    public CustomResolver()
    {
        _defaultResolver = new DefaultAssemblyResolver();
        foreach (var dirPath in SMAPIActivity.GetDependenciesDirectorySearch)
            _defaultResolver.AddSearchDirectory(dirPath);
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
            Console.WriteLine(ex);
        }
        return assembly;
    }
}
