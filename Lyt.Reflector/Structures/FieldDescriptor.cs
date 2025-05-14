namespace Lyt.Reflector.Structures; 

public sealed record class FieldDescriptor( bool IsStatic , Type Type, string Name = "" );
