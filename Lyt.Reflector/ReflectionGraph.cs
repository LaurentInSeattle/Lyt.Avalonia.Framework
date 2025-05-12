namespace Lyt.Reflector;

public sealed class ReflectionGraph(Assembly rootAssembly)
{
    private readonly Assembly rootAssembly = rootAssembly;
    public readonly Graph<string, AssemblyVertex> assemblies = new (64);

    public void BuildGraph()
    {
        // Root vertex 
        var assemblyName = this.rootAssembly.GetName();
        var assemblyVertex = new AssemblyVertex(assemblyName);
        if (!assemblyVertex.Load())
        {
            // Failed to load the root assembly !
            throw new Exception("Failed to load the specified root assembly.");
        }
        
        this.assemblies.AddVertex(assemblyVertex);
        string assemblyShortName = assemblyName!.Name!;
        Debug.WriteLine(assemblyShortName);

        Debug.Indent();
        this.LoadAssemblyRecursive(assemblyVertex);
        Debug.Unindent();

        var list = this.assemblies.Vertices;
        var sorted = (from v in list orderby v.Value.Key ascending select v).ToList();
        var loaded = (from v in sorted
                      where v.Value.IsLoaded
                      orderby v.Value.Key ascending select v)
                      .ToList();

        Debug.WriteLine("");
        Debug.WriteLine("Assembly referenced: " + sorted.Count);
        foreach (var v in sorted)
        {
            Debug.WriteLine(v.Value.Key);
        }

        Debug.WriteLine("");
        Debug.WriteLine("Assembly loaded: " + loaded.Count);
        foreach (var v in loaded)
        {
            Debug.WriteLine(v.Value.Key);
        }

        if ( this.assemblies.HasCycle() )
        {
            Debug.WriteLine("Cycle Detected !!!");
        }
    }

    private void LoadAssemblyRecursive(AssemblyVertex assemblyVertex)
    {
        if (assemblyVertex.Assembly is null)
        {
            return; 
        } 

        Assembly assembly = assemblyVertex.Assembly; 
        AssemblyName[] referencedAssemblyNames = assembly.GetReferencedAssemblies();
        foreach (var referencedAssemblyName in referencedAssemblyNames)
        {
            string referencedShortName = referencedAssemblyName.Name!;
            Debug.WriteLine(referencedShortName);

            var referencedAssemblyVertex = new AssemblyVertex(referencedAssemblyName);

            // Add vertex if we do not have it yet 
            if ( !this.assemblies.ContainsVertex(referencedAssemblyVertex))
            {
                this.assemblies.AddVertex(referencedAssemblyVertex); 
            }

            // Add Edge if we dont have it already 
            if (!this.assemblies.HasEdge(assemblyVertex, referencedAssemblyVertex))
            {
                this.assemblies.AddEdge(assemblyVertex, referencedAssemblyVertex);
            } 

            // If we have a system or 'do not load' assembly, do not load and do not recurse 
            if (referencedShortName.StartsWith("System.") ||
                referencedShortName.StartsWith("Microsoft.") ||
                referencedShortName.StartsWith("Avalonia."))
            {
                // TODO: Parametrize that better !
                // System assembly or spec'd to skip (Ex: Avalonia.) 
                // Do not recurse 
                continue;
            }
            else
            {
                if (!referencedAssemblyVertex.Load())
                {
                    // Failed to load : Do not recurse because we can't
                    continue; 
                }
                else
                {
                    // Recurse 
                    Debug.Indent();
                    this.LoadAssemblyRecursive(referencedAssemblyVertex);
                    Debug.Unindent();
                }
            }
        }
    }
}
