namespace Lyt.Reflector.Structures; 

public sealed class ClassVertex : IKeyProvider<string>
{
    private readonly AssemblyVertex assemblyVertex;
    private readonly Type type;

    public ClassVertex(AssemblyVertex assemblyVertex, Type type)
    {
        this.assemblyVertex = assemblyVertex;
        this.type = type;

        if (type.Name.StartsWith('<'))
        {
            // All computer generated classes should have been filtered out 
            Debugger.Break();
        }

        this.Key = type.SafeFullName();

        if (type.IsGenericType)
        {
            // Debug.WriteLine("Generic: " + type.Name);
            // FullName is null for generics so we figure our way to provide a key for generics
            this.Key = string.Concat( assemblyVertex.Key + "." + type.Name) ; 
        }
        else
        {
            if (type.FullName is null)
            {
                Debugger.Break();
            }

        }
    }

    public Type ClassType => this.type;


    public string Key { get; private set; } 

    // BaseClass (See: BaseType) 
    // Interfaces (See: GetInterfaces) 
    // Events
    // Fields 
    // Methods
    // Properties 
    // NestedTypes 

}
