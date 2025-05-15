namespace Lyt.Reflector.Structures;

public sealed record class MethodDescriptor(
    bool IsStatic, Type ReturnType, List<Type> ParameterTypes, string Name = "");