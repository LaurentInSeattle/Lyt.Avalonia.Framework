namespace Lyt.Reflector.Structures;

public sealed class InterfaceVertex(AssemblyVertex assemblyVertex, Type type) : IKeyProvider<string>
{
    private readonly AssemblyVertex assemblyVertex = assemblyVertex;
    private readonly Type type = type;

    public Type InterfaceType => this.type;

    public string Key => this.type.FullName!;

}