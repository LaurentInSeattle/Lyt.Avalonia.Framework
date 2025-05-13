using System;

namespace Lyt.Reflector;

public sealed class ReflectionGraph(Assembly rootAssembly)
{
    private readonly Assembly rootAssembly = rootAssembly;
    public readonly Graph<string, AssemblyVertex> assemblyDependenciesGraph = new(64);
    public readonly Graph<string, ClassVertex> classDependenciesGraph = new(256);
    public readonly Graph<string, InterfaceVertex> interfaceDependenciesGraph = new(256);

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

        this.assemblyDependenciesGraph.AddVertex(assemblyVertex);
        string assemblyShortName = assemblyName!.Name!;
        Debug.WriteLine(assemblyShortName);

        Debug.Indent();
        this.LoadAssemblyRecursive(assemblyVertex);
        Debug.Unindent();

        var list = this.assemblyDependenciesGraph.Vertices;
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

        if (this.assemblyDependenciesGraph.HasCycle())
        {
            Debug.WriteLine("");
            Debug.WriteLine("*** Assemblies:  Cycle Detected !!!");
            Debug.WriteLine("");
        }

        foreach (var vertexAssemblyVertex in loadedAssemblies)
        {
            this.LoadClassesAndInterfaces(vertexAssemblyVertex.Value);
        }

        Debug.WriteLine("Found classes: " + this.classDependenciesGraph.Vertices.Count);
        Debug.Indent();
        foreach (var v in this.classDependenciesGraph.Vertices)
        {
            Debug.WriteLine(v.Value.Key);
        }
        Debug.Unindent();

        if (this.classDependenciesGraph.HasCycle())
        {
            Debug.WriteLine("");
            Debug.WriteLine("*** Classes:  Cycle Detected !!!");
            Debug.WriteLine("");
        }

        Debug.WriteLine("Found interfaces: " + this.interfaceDependenciesGraph.Vertices.Count);
        Debug.Indent();
        foreach (var v in this.interfaceDependenciesGraph.Vertices)
        {
            Debug.WriteLine(v.Value.Key);
        }
        Debug.Unindent();

        if (this.interfaceDependenciesGraph.HasCycle())
        {
            Debug.WriteLine("");
            Debug.WriteLine("*** Interfaces:  Cycle Detected !!!");
            Debug.WriteLine("");
        }

        this.ResolveClassInheritance();
        this.ResolveInterfaceInheritance(); 
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
            if (!this.assemblyDependenciesGraph.ContainsVertex(referencedAssemblyVertex))
            {
                this.assemblyDependenciesGraph.AddVertex(referencedAssemblyVertex);
            }

            // Add Edge if we dont have it already 
            if (!this.assemblyDependenciesGraph.HasEdge(assemblyVertex, referencedAssemblyVertex))
            {
                this.assemblyDependenciesGraph.AddEdge(assemblyVertex, referencedAssemblyVertex);
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

    private void LoadClassesAndInterfaces(AssemblyVertex assemblyVertex)
    {
        if (assemblyVertex.Assembly is null)
        {
            throw new Exception("Assembly not loaded");
        }

        Type[] types = assemblyVertex.Assembly.GetTypes();
        foreach (var type in types)
        {
            if ( type.ToString() == "Lyt.Avalonia.Controls.PanZoom.PanZoomControl")
            {
                // Debugger.Break();
            }

            if (type.ShouldBeIgnored())
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
                if (!this.classDependenciesGraph.ContainsVertex(classVertex))
                {
                    this.classDependenciesGraph.AddVertex(classVertex);
                }
            }
            else if (type.IsInterface)
            {
                // Add vertex if we do not have it yet 
                InterfaceVertex interfaceVertex = new(assemblyVertex, type);
                if (!this.interfaceDependenciesGraph.ContainsVertex(interfaceVertex))
                {
                    this.interfaceDependenciesGraph.AddVertex(interfaceVertex);
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
        var classVertices = this.classDependenciesGraph.Vertices;
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
                    if (this.classDependenciesGraph.ContainsVertex(key))
                    {
                        // We have a base type: Create an edge in the graph 
                        var baseClassVertex = this.classDependenciesGraph.GetVertex(key);
                        this.classDependenciesGraph.AddEdge(classVertex.Value, baseClassVertex.Value);
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
        // TODO 
        //
        //var classVertices = this.classDependenciesGraph.Vertices;
        //foreach (var classVertex in classVertices)
        //{
        //    Type? maybeBaseType = classVertex.Value.ClassType.BaseType;
        //    if ((maybeBaseType is Type baseType) && (baseType != typeof(object)))
        //    {
        //        if (baseType.IsClass)
        //        {
        //            if (baseType.HasNoSafeFullName())
        //            {
        //                continue;
        //            }

        //            // We may not have it if this something from a non-loaded assembly 
        //            string key = baseType.SafeFullName();
        //            if (this.classDependenciesGraph.ContainsVertex(key))
        //            {
        //                // We have a base type: Create an edge in the graph 
        //                var baseClassVertex = this.classDependenciesGraph.GetVertex(key);
        //                this.classDependenciesGraph.AddEdge(classVertex.Value, baseClassVertex.Value);
        //                Debug.WriteLine(
        //                    classVertex.Value.Key.ToString() + " -> " +
        //                    baseClassVertex.Value.Key.ToString());
        //            }
        //        }
        //    }
        //}
    }
}
