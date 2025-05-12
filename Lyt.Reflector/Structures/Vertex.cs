namespace Lyt.Reflector.Structures;

public sealed class Vertex<T>(T value) where T : class
{
    public readonly HashSet<Vertex<T>> OutEdges = [];

    public readonly HashSet<Vertex<T>> InEdges = [];

    public readonly T Value = value;
}