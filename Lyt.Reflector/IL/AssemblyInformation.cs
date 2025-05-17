namespace Lyt.Reflector.IL;

/// <summary> A decorator pattern class to help obtain commonly displayed information about an assembly. </summary>
/// <remarks> Create an instance for the specified assembly. </remarks>
/// <param name="assembly">The assembly.</param>
/// <exception cref="System.ArgumentNullException"> <paramref name="assembly"/> is null. </exception>
public class AssemblyInformation(Assembly assembly)
{
    private string author;
	private string company;
	private string copyright;
	private string description;
	private string entryPoint;
	private string location;
	private string name;
	private string product;
	private string title;
	private string version;

    /// <summary> Gets the underlying assembly for this instance. </summary>
    [Browsable(false)]
    public Assembly Assembly { get; } = assembly ?? throw new ArgumentNullException(nameof(assembly));

    /// <summary> Gets the author of the assembly. </summary>
    [Browsable(true)]
	public string Author =>
		author = author ?? (GetAttribute(out AssemblyAuthorAttribute attribute) ?
				attribute.Author : string.Empty);

	/// <summary> Gets the company that created the assembly. </summary>
	[Browsable(true)]
	public string Company =>
		company = company ?? (GetAttribute(out AssemblyCompanyAttribute attribute) ?
				attribute.Company : string.Empty);

	/// <summary> Gets copyright information for the assembly. </summary>
	[Browsable(true)]
	public string Copyright =>
		copyright = copyright ?? (GetAttribute(out AssemblyCopyrightAttribute attribute) ?
			attribute.Copyright : string.Empty);

	/// <summary>
	/// Gets a description of the assembly.
	/// </summary>
	[Browsable(true)]
	public string Description =>
		description = description ?? (GetAttribute(out AssemblyDescriptionAttribute attribute) ?
			attribute.Description : string.Empty);

	/// <summary>
	/// Gets the entry point (if any) for the assembly.
	/// </summary>
	[Browsable(true)]
	public string EntryPoint
	{
		get
		{
			if (entryPoint != null)
				return entryPoint;

			MethodInfo method = Assembly.EntryPoint;
			if (method == null)
				return entryPoint = string.Empty;

			Type type = method.DeclaringType;
			if (type == null)
				return entryPoint = method.Name;

			return entryPoint = $"{type.Namespace}.{type.Name}.{method.Name}";
		}
	}

	/// <summary>
	/// Gets the full name of the assembly.
	/// </summary>
	[Browsable(true)]
	public string FullName =>
		Assembly.FullName;

	/// <summary>
	/// Gets the location of the assembly.
	/// </summary>
	public string Location =>
		location = location ?? new Uri(Assembly.CodeBase).LocalPath;

	/// <summary>
	/// Gets the name of the assembly.
	/// </summary>
	[Browsable(false)]
	public string Name =>
		name = name ?? Assembly.GetName().Name;

	/// <summary>
	/// Gets the name of the product containing the assembly.
	/// </summary>
	[Browsable(true)]
	public string Product =>
		product = product ?? (GetAttribute(out AssemblyProductAttribute attribute) ?
			attribute.Product : string.Empty);

	/// <summary>
	/// Gets the title of the assembly.
	/// </summary>
	[Browsable(true)]
	public string Title =>
		title = title ?? (GetAttribute(out AssemblyTitleAttribute attribute) &&
				!string.IsNullOrEmpty(attribute.Title) ?
			attribute.Title : Path.GetFileNameWithoutExtension(
				Assembly.CodeBase));

	/// <summary>
	/// Gets the version of the assembly.
	/// </summary>
	[Browsable(true)]
	public string Version =>
		version = version ?? (Assembly.GetName().Version.ToString());

	private bool GetAttribute<TAttribute>(out TAttribute attribute)
			where TAttribute : Attribute =>
		(attribute = Assembly
			.GetCustomAttributes<TAttribute>()
			.FirstOrDefault()) != null;
}
