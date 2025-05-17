namespace Lyt.Reflector.IL;

/// <summary> A singleton class for converting types to Intermediate Language (IL). </summary>
public class CilTypes : IReadOnlyDictionary<Type, string>
{
	/// <summary> Prevents instances being created  </summary>
	private CilTypes() 	{ }

    /// <summary> Gets the singleton instance of this class. </summary>
    public static CilTypes Instance { get; } = new();

    private static readonly Dictionary<Type, string> types = new()
	{
		{ typeof(bool), "bool" },
		{ typeof(char), "char" },
		{ typeof(float), "float32" },
		{ typeof(double), "float64" },
		{ typeof(sbyte), "int8" },
		{ typeof(short), "int16" },
		{ typeof(int), "int32" },
		{ typeof(long), "int64" },
		{ typeof(IntPtr), "native int" },
		{ typeof(UIntPtr), "native uint" },
		{ typeof(object), "object" },
		{ typeof(string), "string" },
		{ typeof(TypedReference), "typedref" },
		{ typeof(byte), "uint8" },
		{ typeof(ushort), "uint16" },
		{ typeof(uint), "uint32" },
		{ typeof(ulong), "uint64" },
		{ typeof(void), "void" }
	};

	/// <summary> Gets the value with the specified key. </summary>
	/// <param name="key">The key of the value to get.</param>
	/// <returns>The value with the specified key.</returns>
	public string this[Type key] => types[key];

	/// <summary> Gets the number of items in this dictionary. </summary>
	public int Count => types.Count;

	/// <summary>
	/// Gets the keys for this dictionary.
	/// </summary>
	public IEnumerable<Type> Keys => types.Keys;

	/// <summary>
	/// Gets the values for this dictionary.
	/// </summary>
	public IEnumerable<string> Values => types.Values;

	/// <summary>
	/// Append the text for the specified type to the specified string builder.
	/// </summary>
	/// <param name="builder">The string builder to which the text is appended.</param>
	/// <param name="instructions">The list of instructions where the type is referenced.</param>
	/// <param name="type">The type to format.</param>
	/// <param name="includeModifiers">A value indicating if type modifiers should be included.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="builder"/>, <paramref name="instructions"/>, or
	/// <paramref name="type"/> is null.
	/// </exception>
	public void AppendType(
		StringBuilder builder, IInstructionList instructions,
		Type type, bool includeModifiers = false)
	{
		bool isByRef = TryGetUnrefType(ref type);
		bool isArray = TryGetArrayType(ref type, out int arrayRank);

		if (types.TryGetValue(type, out string? value))
			builder.Append(value);
		else
			AppendNonCilType(instructions, builder, type, includeModifiers);

		if (isArray)
			AppendArrayDimensions(builder, arrayRank);

		if (isByRef)
			builder.Append("&");
	}

	/// <summary>
	/// Append the text for the specified type parameters to the specified string builder.
	/// </summary>
	/// <param name="builder">The string builder to which the text is appended.</param>
	/// <param name="instructions">The list of instructions where the types are referenced.</param>
	/// <param name="types">The type parameters.</param>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="builder"/>, <paramref name="instructions"/>, or
	/// <paramref name="types"/> is null.
	/// </exception>
	public void AppendTypeParameters(
		StringBuilder builder, IInstructionList instructions, IEnumerable<Type> types)
	{
		builder.Append('<');

		bool isFirst = true;
		foreach (Type type in types)
		{
			if (isFirst)
				isFirst = false;
			else
				builder.Append(", ");

			AppendType(builder, instructions, type, true);
		}

		builder.Append('>');
	}

	/// <summary> Gets a value indicating if this dictionary contains the specified key. </summary>
	/// <param name="key">The key to test.</param>
	/// <returns>True, if this dictionary contains the specified key; otherwise, false.</returns>
	public bool ContainsKey(Type key) => types.ContainsKey(key);

	/// <summary> Format the specified type for the specified instruction list. </summary>
	/// <param name="instructions">The list of instructions where the type is referenced.</param>
	/// <param name="type">The type to format.</param>
	/// <param name="includeModifiers">A value indicating if type modifiers should be included.</param>
	/// <returns>The text for the specified type.</returns>
	/// <exception cref="System.ArgumentNullException">
	/// <paramref name="instructions"/> or <paramref name="type"/> is null.
	/// </exception>
	public string FormatType(IInstructionList instructions, Type type, bool includeModifiers = false)
	{

		if (types.TryGetValue(type, out string? value))
			return value;

		var builder = new StringBuilder(1024);
		AppendType(builder, instructions, type, includeModifiers);
		return builder.ToString();
	}

	/// <summary> Gets an enumerator for this dictionary. </summary>
	/// <returns> An enumerator for this dictionary.</returns>
	public IEnumerator<KeyValuePair<Type, string>> GetEnumerator() => types.GetEnumerator();

	/// <summary> Try to get the value for the specified key. </summary>
	/// <param name="key">The key to find.</param>
	/// <param name="value">The value that was found (or null).</param>
	/// <returns>True, if the key was found; otherwise, false.</returns>
	public bool TryGetValue(Type key, out string value) => types.TryGetValue(key, out value);

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// Append the array dimensions
	private void AppendArrayDimensions(StringBuilder builder, int arrayRank)
	{
		builder.Append('[');
		if (arrayRank > 1)
		{
			for (int index = 0; index < arrayRank; index++)
			{
				if (index > 0)
					builder.Append(',');
				builder.Append("0...");
			}
		}
		builder.Append(']');
	}

	// Append the assembly name if different from the assembly for the instructions
	private void AppendAssemblyName(IInstructionList instructions, StringBuilder builder,
		Type type)
	{
		Assembly assembly = type.Assembly;
		if (instructions.IsSameAssembly(assembly))
			return;

		builder.Append('[');
		builder.Append(assembly.GetName().Name);
		builder.Append(']');
	}

	// Append a type that has no direct CIL equivalent
	private void AppendNonCilType(IInstructionList instructions, StringBuilder builder,
		Type type, bool includeModifiers)
	{
		var assembly = type.Assembly;

		if (includeModifiers)
			builder.Append(type.IsValueType ? "valuetype " : "class ");

		AppendAssemblyName(instructions, builder, type);
		AppendTypeName(builder, type);

		if (type.IsGenericType)
			AppendTypeParameters(builder, instructions, type.GetGenericArguments());
	}

	// Append the names of nested types (if any)
	private void AppendNestedNames(StringBuilder builder, Stack<string> nestedNames)
	{
		if (nestedNames == null)
			return;

		while (nestedNames.Count > 0)
		{
			builder.Append('/');
			builder.Append(nestedNames.Pop());
		}
	}

	// Add the type name (including namespace)
	private void AppendTypeName(StringBuilder builder, Type type)
	{
		Stack<string> nestedNames = GetNestedNames(ref type);

		if (type.Namespace != null)
		{
			builder.Append(type.Namespace);
			builder.Append('.');
		}

		builder.Append(type.Name);

		AppendNestedNames(builder, nestedNames);
	}

	// Get the names of any nested types and advance to a global type
	private Stack<string> GetNestedNames(ref Type type)
	{
		Stack<string> nestedNames = null;

		while (type.DeclaringType != null)
		{
			if (nestedNames == null)
				nestedNames = new Stack<string>();

			nestedNames.Push(type.Name);
			type = type.DeclaringType;
		}

		return nestedNames;
	}

	// For IsByRef types, use the underlying non-IsByRef type
	private bool TryGetUnrefType(ref Type type)
	{
		if (!type.IsByRef)
			return false;

		Type unrefType = type.GetElementType();
		if (unrefType == null)
			return false;

		type = unrefType;
		return true;
	}

	// For IsArray types, use the underlying element type
	private bool TryGetArrayType(ref Type type, out int rank)
	{
		if (!type.IsArray)
		{
			rank = 0;
			return false;
		}

		rank = type.GetArrayRank();

		Type elementType = type.GetElementType();
		if (elementType == null)
			return false;

		type = elementType;
		return true;
	}
}
