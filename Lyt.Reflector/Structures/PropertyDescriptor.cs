namespace Lyt.Reflector.Structures;

public sealed record class PropertyDescriptor(
    bool IsStatic, Type Type, List<Type> DependantTypes, string Name = "" );
