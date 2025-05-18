namespace Lyt.Reflector.IL;

/// <summary> A method signature (for the <see cref="OpCodes.Calli"/> instruction). </summary>
public class MethodSignature
{
	/// <summary> Create an instance for the specified parent and signature data. </summary>
	/// <param name="parent">The instructions containing the signature data.</param>
	/// <param name="data">The signature data.</param>
	public MethodSignature(InstructionList parent, IReadOnlyList<byte> data)
	{
        this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        this.Data = data ?? throw new ArgumentNullException(nameof(data));

		int offset = 0;
        this.CilCallingConvention = (CilCallingConvention)data.ReadByte(offset++);

		if (!this.DecodeCallingConvention())
        {
            throw new ArgumentException(null, nameof(data));
        }

        int parameterCount = (int)data.ReadCompressedUInt32(offset, out int count);
		offset += count;

		bool isOptional = false;
        this.ReturnType = this.DecodeType(offset, ref isOptional, out count);
		offset += count;

		if (this.ReturnType == null)
        {
            throw new ArgumentException(nameof(data));
        }

        if (!this.DecodeParameters(offset, parameterCount))
        {
            throw new ArgumentException(nameof(data));
        }
    }

	/// <summary>
	/// Gets the calling convention, if <see cref="IsUnmanaged"/> is <see cref="true"/>;
	/// otherwise, (<see cref="System.Runtime.InteropServices.CallingConvention"/>)0.
	/// </summary>
	public CallingConvention CallingConvention { get; private set; }

	/// <summary>
	/// Gets the calling convention if <see cref="IsUnmanaged"/> is <see cref="false"/>;
	/// otherwise, (<see cref="System.Reflection.CallingConventions."/>)0;
	/// </summary>
	public CallingConventions CallingConventions { get; private set; }

	/// <summary>
	/// Gets the raw data for this method signature.
	/// </summary>
	public IReadOnlyList<byte> Data { get; }

	/// <summary>
	/// Gets a value indicating if the calling convention is managed or unmanaged.
	/// </summary>
	public bool IsUnmanaged { get; private set; }

	/// <summary>
	/// Gets the types for the optional parameters.
	/// </summary>
	public IReadOnlyList<Type> OptionalParameters { get; private set; }

	/// <summary>
	/// Gets the set of instructions containing this method signature.
	/// </summary>
	public InstructionList Parent { get; }

	/// <summary>
	/// Gets the Common Intermediate Language (CIL) calling convention.
	/// </summary>
	public CilCallingConvention CilCallingConvention { get; }

	/// <summary> Gets the return type. </summary>
	public Type ReturnType { get; }

	/// <summary> Gets the types for the required parameters. </summary>
	public IReadOnlyList<Type> RequiredParameters { get; private set; }

	/// <summary>
	/// Append the calling convention for an unmanaged call to the specified string builder.
	/// </summary>
	/// <param name="builder">The string builder to receive the calling convention.</param>
	/// <param name="convention">The calling convention to append.</param>
	public static void AppendConvention(StringBuilder builder, CallingConvention convention)
	{
		builder.Append("unmanaged ");
		switch (convention)
		{
			case CallingConvention.Cdecl:
				builder.Append("cdecl ");
				break;

			case CallingConvention.FastCall:
				builder.Append("fastcall ");
				break;

			case CallingConvention.ThisCall:
				builder.Append("thiscall ");
				break;

			case CallingConvention.StdCall:
			default:
				builder.Append("stdcall ");
				break;
		}
	}

	/// <summary>
	/// Append the calling conventions for a managed call to the specified string builder.
	/// </summary>
	/// <param name="builder">The string builder to receive the calling conventions.</param>
	/// <param name="conventions">The calling conventions to append.</param>
	public static void AppendConventions(StringBuilder builder, CallingConventions conventions)
	{
		if ((conventions & CallingConventions.HasThis) != 0)
        {
            builder.Append("instance ");
        }

        if ((conventions & CallingConventions.VarArgs) != 0)
        {
            builder.Append("vararg ");
        }
    }

	/// <summary>
	/// Get a textual representation of this instance.
	/// </summary>
	/// <returns>A textual representation of this instance.</returns>
	public override string ToString()
	{
		var builder = new StringBuilder(1024);

		if (IsUnmanaged)
        {
            AppendConvention(builder, CallingConvention);
        }
        else
        {
            AppendConventions(builder, CallingConventions);
        }

        this.AppendType(builder, ReturnType);
		bool isFirstType = true;
		builder.Append('(');

        this.AppendTypes(builder, RequiredParameters, ref isFirstType);

		if (OptionalParameters.Count > 0)
		{
			if (isFirstType)
            {
                isFirstType = false;
            }
            else
            {
                builder.Append(", ");
            }

            builder.Append("...");
			AppendTypes(builder, OptionalParameters, ref isFirstType);
		}

		builder.Append(')');
		return builder.ToString();
	}

	// Append the text for a Type to the specified string builder
	private void AppendType(StringBuilder builder, Type type, bool includeModifiers = true) =>
		CilTypes.Instance.AppendType(builder, Parent, type, includeModifiers);

	// Append the text for a sequence of types to the specified string builder
	private void AppendTypes(StringBuilder builder, IEnumerable<Type> types,
		ref bool isFirstType)
	{
		foreach(Type type in types)
		{
			if (isFirstType)
            {
                isFirstType = false;
            }
            else
            {
                builder.Append(", ");
            }

            AppendType(builder, type);
		}
	}

	// Decode a Class or ValueType token from the signature data
	private bool DecodeToken(ElementType elementType, ref int offset, out Type type)
	{
		if (elementType == ElementType.Class || elementType == ElementType.ValueType)
		{
			try
			{
				type = Parent.ResolveType(Data.ReadCompressedTypeDefOrRef(offset, out int count));
				if (type != null)
				{
					offset += count;
					return true;
				}
			}
			catch
			{
			}
		}

		type = null;
		return false;
	}

	// Decode a Type from the signature data
	private Type DecodeType(int offset, ref bool isOptional, out int count)
	{
		int startOffset = offset;

		var elementType = (ElementType)Data.ReadByte(offset++);

		if (elementType == ElementType.Sentinel)
		{
			elementType = (ElementType)Data.ReadByte(offset++);
			isOptional = true;
		}

		if (!DecodeToken(elementType, ref offset, out Type type))
			type = elementType.ToType();

		count = type == null ? 0 : offset - startOffset;
		return type;
	}

	// Decode the parameters
	private bool DecodeParameters(int offset, int parameterCount)
	{
		var requiredParameters = new List<Type>();
		var optionalParameters = new List<Type>();
		bool isOptional = false;

		for(int index = 0;  index < parameterCount; index++)
		{
			Type type = DecodeType(offset, ref isOptional, out int count);
			if (type == null)
				return false;

			if (isOptional)
				optionalParameters.Add(type);
			else
				requiredParameters.Add(type);

			offset += count;
		}

		RequiredParameters = new ReadOnlyCollection<Type>(requiredParameters);
		OptionalParameters = new ReadOnlyCollection<Type>(optionalParameters);
		return true;
	}

	// Decode the Common Intermediate Language (CIL) calling convention into reflection-friendly
	// CallingConvention / CallingConventions types
	private bool DecodeCallingConvention()
	{
		CilCallingConvention type = CilCallingConvention & CilCallingConvention.Mask;

		switch (type)
		{
			case CilCallingConvention.Standard:
				CallingConventions = CallingConventions.Standard;
				break;

			case CilCallingConvention.WinApi:
				CallingConvention = CallingConvention.Winapi;
				return IsUnmanaged = true;

			case CilCallingConvention.Cdecl:
				CallingConvention = CallingConvention.Cdecl;
				return IsUnmanaged = true;

			case CilCallingConvention.StdCall:
				CallingConvention = CallingConvention.StdCall;
				return IsUnmanaged = true;

			case CilCallingConvention.ThisCall:
				CallingConvention = CallingConvention.ThisCall;
				return IsUnmanaged = true;

			case CilCallingConvention.FastCall:
				CallingConvention = CallingConvention.FastCall;
				return IsUnmanaged = true;

			case CilCallingConvention.VarArgs:
				CallingConventions = CallingConventions.VarArgs;
				break;

			case CilCallingConvention.Field:
			case CilCallingConvention.Property:
			case CilCallingConvention.GenericInst: // Should we decode this?
			case CilCallingConvention.NativeVarArg: // Should we decode this?
			default:
				return false;
		}

		if ((CilCallingConvention & CilCallingConvention.HasThis) != 0)
			CallingConventions |= CallingConventions.HasThis;

		if ((CilCallingConvention & CilCallingConvention.ExplicitThis) != 0)
			CallingConventions |= CallingConventions.ExplicitThis;

		return true;
	}
}
