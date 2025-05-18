namespace Lyt.Reflector.IL;

/// <summary> A decorator pattern class to help obtain commonly displayed information about an assembly. </summary>
/// <remarks> Create an instance for the specified assembly. </remarks>
/// <param name="assembly">The assembly.</param>
public class AssemblyInformation
{
	private readonly Assembly assembly;
    private readonly string author;
	private readonly string company;
	private readonly string location;
	private readonly string name;
	private readonly string version;
	private readonly string entryPoint;

    public AssemblyInformation(Assembly assembly)
    {
		this.assembly = assembly;
		this.author =
			this.GetAttribute(out AssemblyAuthorAttribute? attributeAuthor) ?
				attributeAuthor!.Author :
				string.Empty;
        this.company =
            this.GetAttribute(out AssemblyCompanyAttribute? attributeCompany) ?
                attributeCompany!.Company:
                string.Empty;
		this.location = this.assembly.Location;
		string? maybeName = this.assembly.GetName().Name; 
        this.name = maybeName is not null ? maybeName : string.Empty;
		Version? maybeVersion = this.assembly.GetName().Version;
        this.version = maybeVersion is not null ? maybeVersion.ToString() : string.Empty;

        MethodInfo? method = this.assembly.EntryPoint;
		if (method is null)
		{
			this.entryPoint = string.Empty;
		}
		else
		{
			Type? type = method.DeclaringType; 
            this.entryPoint = 
				 type is null ? 
					method.Name : 
					$"{type.Namespace}.{type.Name}.{method.Name}";
		} 
    }

    /// <summary> Gets the underlying assembly for this instance. </summary>
    public Assembly Assembly => this.assembly ;

    /// <summary> Gets the full name of the assembly. </summary>
    public string? FullName => this.Assembly.FullName;

    /// <summary> Gets the name of the assembly. </summary>
    public string Name => this.name;

    /// <summary> Gets the name of the assembly. </summary>
    public string Version => this.version;

    /// <summary> Gets the author of the assembly. </summary>
	public string Author => this.author;

	/// <summary> Gets the company that created the assembly. </summary>
	public string Company => this.company;

	/// <summary> Gets the entry point (if any) for the assembly.</summary>
	public string EntryPoint => this.entryPoint;

	/// <summary> Gets the location of the assembly. </summary>
	public string Location => this.location;

	private bool GetAttribute<TAttribute>(out TAttribute? attribute)
			where TAttribute : Attribute =>
		(attribute = this.assembly
			.GetCustomAttributes<TAttribute>()
			.FirstOrDefault()) != null;
}
