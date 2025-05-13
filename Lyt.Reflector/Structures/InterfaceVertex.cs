namespace Lyt.Reflector.Structures;

public sealed class InterfaceVertex : IKeyProvider<string>
{
    private readonly AssemblyVertex assemblyVertex;
    private readonly Type type;

    public InterfaceVertex(AssemblyVertex assemblyVertex, Type type)
    {
        this.assemblyVertex = assemblyVertex;
        this.type = type;
    }

    public Type InterfaceType => this.type;

    public string Key => this.type.FullName!;

}