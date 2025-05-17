namespace Lyt.Reflector.IL;

/// <summary> An attribute to provide the author of an assembly. </summary>
/// <remarks> Create an instance for the specified author. </remarks>
/// <param name="author">The author of the assembly.</param>
[AttributeUsage(AttributeTargets.Class)]
public class AssemblyAuthorAttribute(string author) : Attribute
{
    /// <summary> Gets the author of the assembly. </summary>
    public string Author { get; } = author ?? throw new ArgumentNullException(nameof(author));
}
