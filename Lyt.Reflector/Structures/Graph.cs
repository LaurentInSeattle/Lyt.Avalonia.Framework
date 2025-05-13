namespace Lyt.Reflector.Structures;

/// <summary> 
/// Represents a Directed Graph as a dictionary of vertices, each containing two hashsets for edges
/// holding a reference to the 'destination' vertex. 
/// </summary>
/// <typeparam name="TKey">Any equatable type. </typeparam>
/// <typeparam name="T">Any class with an equatable class property named 'Key', usually string. </typeparam>
public sealed class Graph<TKey, T>(int capacity = 16) 
    where T : class , IKeyProvider<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly Dictionary<TKey, Vertex<T>> vertices = new(capacity);

    public Vertex<T> GetVertex(TKey key) => this.vertices[key];

    public Vertex<T> GetVertex(T value) => this.vertices[value.Key];

    public List<Vertex<T>> Vertices => [.. this.vertices.Values];

    /// <summary> Returns true if we already have a vertex with the provided key. </summary>
    public bool ContainsVertex(TKey key) => this.vertices.ContainsKey(key);

    /// <summary> Returns true if we already have the provided vertex. </summary>
    public bool ContainsVertex(T value) => this.vertices.ContainsKey(value.Key);

    /// <summary> Returns true if we have an edge between the given source and destination. </summary>
    public bool HasEdge(T source, T dest)
    {
        if (!this.vertices.TryGetValue(source.Key, out Vertex<T>? sourceVertex) ||
            !this.vertices.TryGetValue(dest.Key, out Vertex<T>? destVertex) ||
            sourceVertex is null ||
            destVertex is null)
        {
            throw new Exception("Source or Destination Vertex is not in this graph.");
        }

        return sourceVertex.OutEdges.Contains(destVertex) &&
               destVertex.InEdges.Contains(sourceVertex);
    }

    /// <summary> Add a new vertex to this graph. </summary>
    public void AddVertex(T value) => this.vertices.Add(value.Key, new Vertex<T>(value));

    /// <summary> 
    /// Remove an existing vertex from the graph.
    /// Time complexity: O(V) where V is the total number of vertices in this graph.
    /// </summary>
    public void RemoveVertex(T value)
    {
        if (!this.vertices.TryGetValue(value.Key, out Vertex<T>? _))
        {
            throw new Exception("Vertex not in this graph.");
        }

        var targetVertex = this.vertices[value.Key];
        foreach (var vertex in targetVertex.InEdges)
        {
            vertex.OutEdges.Remove(targetVertex);
        }

        foreach (var vertex in targetVertex.OutEdges)
        {
            vertex.InEdges.Remove(targetVertex);
        }

        this.vertices.Remove(value.Key);
    }

    /// <summary> Add an edge from source to destination vertices. </summary>
    public void AddEdge(T source, T dest)
    {
        if (!this.vertices.TryGetValue(source.Key, out Vertex<T>? sourceVertex) ||
            !this.vertices.TryGetValue(dest.Key, out Vertex<T>? destVertex) ||
            sourceVertex is null ||
            destVertex is null)
        {
            throw new Exception("Source or Destination Vertex is not in this graph.");
        }

        if (sourceVertex.OutEdges.Contains(destVertex) ||
            destVertex.InEdges.Contains(sourceVertex))
        {
            throw new Exception("Edge already exists.");
        }

        sourceVertex.OutEdges.Add(destVertex);
        destVertex.InEdges.Add(sourceVertex);
    }

    /// <summary> Remove an existing edge between source and destination vertices. </summary>
    public void RemoveEdge(T source, T dest)
    {
        if (!this.vertices.TryGetValue(source.Key, out Vertex<T>? sourceVertex) ||
            !this.vertices.TryGetValue(dest.Key, out Vertex<T>? destVertex) ||
            sourceVertex is null ||
            destVertex is null)
        {
            throw new Exception("Source or Destination Vertex is not in this graph.");
        }


        if (!sourceVertex.OutEdges.Contains(destVertex) ||
            !destVertex.InEdges.Contains(sourceVertex))
        {
            throw new Exception("Edge does not exist.");
        }

        sourceVertex.OutEdges.Remove(destVertex);
        destVertex.InEdges.Remove(sourceVertex);
    }

    /// <summary> Clone this graph. </summary>
    public Graph<TKey, T> Clone()
    {
        var newGraph = new Graph<TKey, T>();

        foreach (var vertex in this.vertices.Values)
        {
            newGraph.AddVertex(vertex.Value);
        }

        foreach (var vertex in this.vertices)
        {
            foreach (var edge in vertex.Value.OutEdges)
            {
                newGraph.AddEdge(vertex.Value.Value, edge.Value);
            }
        }

        return newGraph;
    }

    /// <summary> Returns true if a cycle exists </summary>
    /// <remarks>
    /// Time complexity: O(V + E), where V is the number of vertices and E is the number of edges in the graph.
    /// </remarks>
    public bool HasCycle()
    {
        static bool DepthFirstTraversal(Vertex<T> current, HashSet<T> visited, HashSet<T> visiting)
        {
            visiting.Add(current.Value);
            foreach (var edge in current.OutEdges)
            {
                //if we encountered a visiting vertex again then there is a cycle
                if (visiting.Contains(edge.Value))
                {
                    return true;
                }

                if (visited.Contains(edge.Value))
                {
                    continue;
                }

                if (DepthFirstTraversal(edge, visited, visiting))
                {
                    return true;
                }
            }

            visiting.Remove(current.Value);
            visited.Add(current.Value);
            return false;
        }

        var visiting = new HashSet<T>();
        var visited = new HashSet<T>();

        foreach (var vertex in this.Vertices)
        {
            if (!visited.Contains(vertex.Value))
            {
                if (DepthFirstTraversal(vertex, visited, visiting))
                {
                    return true;
                }
            }
        }

        return false;
    }

}
