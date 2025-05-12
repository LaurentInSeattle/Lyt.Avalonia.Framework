namespace Lyt.Reflector;

public sealed class ReflectionGraph
{
    private readonly Assembly rootAssembly;
    private readonly Dictionary<string, Assembly> assemblies;

    // Broken: Not the Droid Structure we are looking for, need a graph 
    private readonly Dictionary<string, string> assemblyDependencies;

    public ReflectionGraph(Assembly assembly)
    {
        this.rootAssembly = assembly;
        this.assemblies = [];
        this.assemblyDependencies = [];
    }

    public void BuildGraph()
    {
        string assemblyName = this.rootAssembly.GetName()!.Name!;
        Debug.WriteLine(assemblyName);
        Debug.Indent();
        this.LoadAssemblyRecursive(this.rootAssembly);
        Debug.Unindent();
        Debug.WriteLine("Assembly loaded: " + this.assemblies.Count);
        Debug.WriteLine("Dependencies: " + this.assemblyDependencies.Count);
    }

    private void LoadAssemblyRecursive(Assembly assembly)
    {
        string assemblyName = assembly.GetName()!.Name!;
        AssemblyName[] referencedAssemblyNames = assembly.GetReferencedAssemblies();
        foreach (var referencedAssemblyName in referencedAssemblyNames)
        {
            string referencedShortName = referencedAssemblyName.Name!;
            Debug.WriteLine(referencedShortName);

            // No good here ! 
            _ = this.assemblyDependencies.TryAdd(assemblyName, referencedShortName);

            // If we have a system or 'do not load' assembly, do not load and do not recurse 
            // TODO better than this hack !!!
            if (referencedShortName.StartsWith("System.") ||
                referencedShortName.StartsWith("Microsoft.") ||
                referencedShortName.StartsWith("Avalonia."))
            {
                continue;
            }

            var referencedAssembly = Assembly.Load(referencedAssemblyName);
            bool added = this.assemblies.TryAdd(referencedShortName, referencedAssembly);
            if (added)
            {
                Debug.Indent();
                this.LoadAssemblyRecursive(referencedAssembly);
                Debug.Unindent();
            }
        }
    }
}
