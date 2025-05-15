namespace Lyt.Reflector.Structures; 

public sealed record class FieldDescriptor( 
    bool IsStatic , Type Type, List<Type> DependantTypes, string Name = "" );
