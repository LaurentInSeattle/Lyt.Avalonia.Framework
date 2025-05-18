namespace Lyt.Reflector;

public sealed class ReflectionGraph(Assembly rootAssembly, List<string> excludedNamespaces)
{
    private readonly Assembly rootAssembly = rootAssembly;
    private readonly List<string> excludedNamespaces = excludedNamespaces;
    public readonly Graph<string, AssemblyVertex> AssemblyDependenciesGraph = new(64);
    public readonly Graph<string, ClassVertex> ClassInheritanceGraph = new(512);
    public readonly Graph<string, InterfaceVertex> InterfaceInheritanceGraph = new(64);

    public void BuildGraph()
    {
        ReflectionUtilities.SetExcludedNamespaces(this.excludedNamespaces);

        // Root vertex 
        var assemblyName = this.rootAssembly.GetName();
        var assemblyVertex = new AssemblyVertex(assemblyName);
        if (!assemblyVertex.Load())
        {
            // Failed to load the root assembly !
            throw new Exception("Failed to load the specified root assembly.");
        }

        this.AssemblyDependenciesGraph.AddVertex(assemblyVertex);
        string assemblyShortName = assemblyName!.Name!;
        Debug.WriteLine(assemblyShortName);

        Debug.Indent();
        this.LoadAssemblyRecursive(assemblyVertex);
        Debug.Unindent();

        var list = this.AssemblyDependenciesGraph.Vertices;
        var sortedAssemblies = (from v in list orderby v.Value.Key ascending select v).ToList();
        var loadedAssemblies =
            (from v in sortedAssemblies
             where v.Value.IsLoaded
             orderby v.Value.Key ascending
             select v)
            .ToList();

        Debug.WriteLine("");
        Debug.WriteLine("Assembly referenced: " + sortedAssemblies.Count);
        foreach (var v in sortedAssemblies)
        {
            Debug.Indent();
            Debug.WriteLine(v.Value.Key);
            Debug.Unindent();
        }

        Debug.WriteLine("");
        Debug.WriteLine("Assembly loaded: " + loadedAssemblies.Count);
        Debug.Indent();
        foreach (var v in loadedAssemblies)
        {
            Debug.WriteLine(v.Value.Key);
        }
        Debug.Unindent();

        if (this.AssemblyDependenciesGraph.HasCycle())
        {
            Debug.WriteLine("");
            Debug.WriteLine("*** Assemblies:  Cycle Detected !!!");
            Debug.WriteLine("");
        }

        foreach (var vertexAssemblyVertex in loadedAssemblies)
        {
            this.LoadClassesAndInterfaces(vertexAssemblyVertex.Value);
        }

        Debug.WriteLine("Found classes: " + this.ClassInheritanceGraph.Vertices.Count);
        Debug.Indent();
        foreach (var v in this.ClassInheritanceGraph.Vertices)
        {
            Debug.WriteLine(v.Value.Key);
        }
        Debug.Unindent();

        if (this.ClassInheritanceGraph.HasCycle())
        {
            Debug.WriteLine("");
            Debug.WriteLine("*** Classes:  Cycle Detected !!!");
            Debug.WriteLine("");
        }

        Debug.WriteLine("Found interfaces: " + this.InterfaceInheritanceGraph.Vertices.Count);
        Debug.Indent();
        foreach (var v in this.InterfaceInheritanceGraph.Vertices)
        {
            Debug.WriteLine(v.Value.Key);
        }
        Debug.Unindent();

        if (this.InterfaceInheritanceGraph.HasCycle())
        {
            Debug.WriteLine("");
            Debug.WriteLine("*** Interfaces:  Cycle Detected !!!");
            Debug.WriteLine("");
        }

        this.ResolveClassInheritance();
        this.ResolveInterfaceInheritance();
        this.ReflectIntoClasses(); 
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
            if (!this.AssemblyDependenciesGraph.ContainsVertex(referencedAssemblyVertex))
            {
                this.AssemblyDependenciesGraph.AddVertex(referencedAssemblyVertex);
            }

            // Add Edge if we dont have it already 
            if (!this.AssemblyDependenciesGraph.HasEdge(assemblyVertex, referencedAssemblyVertex))
            {
                this.AssemblyDependenciesGraph.AddEdge(assemblyVertex, referencedAssemblyVertex);
            }

            // If we have a system or 'do not load' assembly, do not load and do not recurse 
            if (referencedShortName.IsExcludedNamespace())
            {
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

    private void LoadClassesAndInterfaces(AssemblyVertex assemblyVertex)
    {
        if (assemblyVertex.Assembly is null)
        {
            throw new Exception("Assembly not loaded");
        }

        Type[] types = assemblyVertex.Assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.HasNameWithSpecialCharacters())
            {
                // Special characters in type name: Computer generated class 
                continue;
            }

            if (type.Attributes.HasFlag(TypeAttributes.Public))
            {
                // View and controls are 'IsCompilerGenerated' for Avalonia 
                // Need to check for WPF 
            }
            else
            {
                if (type.IsCompilerGenerated() || type.HasNoSafeFullName())
                {
                    Debug.WriteLine("Excluded: " + type.ToString());
                    //var attributes = type.CustomAttributes;
                    //Debug.Indent();
                    //Debug.WriteLine(type.Attributes.ToString());
                    //foreach (var attribute in attributes)
                    //{
                    //    Debug.WriteLine(attribute.ToString());
                    //}
                    //Debug.Unindent();

                    continue;
                }
            }

            if (type.IsClass)
            {
                // Add vertex if we do not have it yet 
                ClassVertex classVertex = new(assemblyVertex, type);
                if (!this.ClassInheritanceGraph.ContainsVertex(classVertex))
                {
                    this.ClassInheritanceGraph.AddVertex(classVertex);
                }
            }
            else if (type.IsInterface)
            {
                // Add vertex if we do not have it yet 
                InterfaceVertex interfaceVertex = new(assemblyVertex, type);
                if (!this.InterfaceInheritanceGraph.ContainsVertex(interfaceVertex))
                {
                    this.InterfaceInheritanceGraph.AddVertex(interfaceVertex);
                }
            }
            else
            {
                // MORE here! 
            }
        }
    }

    private void ResolveClassInheritance()
    {
        var classVertices = this.ClassInheritanceGraph.Vertices;
        foreach (var classVertex in classVertices)
        {
            Type? maybeBaseType = classVertex.Value.ClassType.BaseType;
            if ((maybeBaseType is Type baseType) && (baseType != typeof(object)))
            {
                if (baseType.IsClass)
                {
                    if (baseType.HasNoSafeFullName())
                    {
                        continue;
                    }

                    // We may not have it if this something from a non-loaded assembly 
                    string key = baseType.SafeFullName();
                    if (this.ClassInheritanceGraph.ContainsVertex(key))
                    {
                        // We have a base type: Create an edge in the graph 
                        var baseClassVertex = this.ClassInheritanceGraph.GetVertex(key);
                        this.ClassInheritanceGraph.AddEdge(classVertex.Value, baseClassVertex.Value);
                        Debug.WriteLine(
                            classVertex.Value.Key.ToString() + " -> " +
                            baseClassVertex.Value.Key.ToString());
                    }
                }
            }
        }
    }

    private void ResolveInterfaceInheritance()
    {
        var interfaceVertices = this.InterfaceInheritanceGraph.Vertices;
        foreach (var interfaceVertex in interfaceVertices)
        {
            Type[] interfaces = interfaceVertex.Value.InterfaceType.GetInterfaces();
            if (interfaces.Length> 0)
            {
                foreach ( Type interfaceType in interfaces)
                {
                    if (interfaceType.HasNoSafeFullName())
                    {
                        continue;
                    }

                    // We may not have it if this something from a non-loaded assembly 
                    string key = interfaceType.SafeFullName();
                    if (this.InterfaceInheritanceGraph.ContainsVertex(key))
                    {
                        // We have a base type: Create an edge in the graph 
                        var baseInterfaceVertex = this.InterfaceInheritanceGraph.GetVertex(key);
                        this.InterfaceInheritanceGraph.AddEdge(interfaceVertex.Value, baseInterfaceVertex.Value);
                        Debug.WriteLine(
                            interfaceVertex.Value.Key.ToString() + "  ->  " +
                            baseInterfaceVertex.Value.Key.ToString());
                    }
                }
            }
        }
    }

    private void ReflectIntoClasses ()
    {
        List<Vertex<ClassVertex>> classVertices = this.ClassInheritanceGraph.Vertices;
        foreach (var item in classVertices)
        {
            ClassVertex classVertex = item.Value;

            if ( classVertex.ClassType.SafeFullName().StartsWith ( "Lyt.Reflector.Graph"))
            {
                Debugger.Break(); 
            }

            classVertex.Load(); 
        }
    }
}
