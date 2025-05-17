namespace Lyt.Reflector.IL;

/// <summary> An enumeration of element types. </summary>
/// <remarks> Adapted from CorHdr.h (CorElementType). </remarks>
public enum ElementType
{
	/// <summary> A "void" type (<see cref="System.Void"/>). </summary>
	Void = 0x01, // ELEMENT_TYPE_VOID

	/// <summary>
	/// A Boolean type (<see cref="System.Boolean"/>).
	/// </summary>
	Boolean = 0x02, // ELEMENT_TYPE_BOOLEAN

	/// <summary>
	/// A character type (<see cref="System.Char"/>).
	/// </summary>
	Char = 0x03, // ELEMENT_TYPE_CHAR

	/// <summary>
	/// A signed 8 bit integer type (<see cref="System.SByte"/>).
	/// </summary>
	SByte = 0x04, // ELEMENT_TYPE_I1

	/// <summary>
	/// An unsigned 8 bit integer type (<see cref="System.Byte"/>).
	/// </summary>
	Byte = 0x05, // ELEMENT_TYPE_U1

	/// <summary>
	/// A signed 16 bit integer type (<see cref="System.Int16"/>).
	/// </summary>
	Int16 = 0x06, // ELEMENT_TYPE_I2

	/// <summary>
	/// An unsigned 16 bit integer type (<see cref="System.UInt16"/>).
	/// </summary>
	UInt16 = 0x07, // ELEMENT_TYPE_U2

	/// <summary>
	/// A signed 32 bit integer type (<see cref="System.Int32"/>).
	/// </summary>
	Int32 = 0x08, // ELEMENT_TYPE_I4

	/// <summary>
	/// An unsigned 32 bit integer type (<see cref="System.UInt32"/>).
	/// </summary>
	UInt32 = 0x09, // ELEMENT_TYPE_U4

	/// <summary>
	/// A signed 64 bit integer type (<see cref="System.Int64"/>).
	/// </summary>
	Int64 = 0x0a, // ELEMENT_TYPE_I8

	/// <summary>
	/// An unsigned 64 bit integer type (<see cref="System.UInt64"/>).
	/// </summary>
	UInt64 = 0x0b, // ELEMENT_TYPE_U8

	/// <summary>
	/// A 32 bit IEEE floating point type (<see cref="System.Single"/>).
	/// </summary>
	Single = 0x0c, // ELEMENT_TYPE_R4

	/// <summary>
	/// A 64 bit IEEE floating point type (<see cref="System.Double"/>).
	/// </summary>
	Double = 0x0d, // ELEMENT_TYPE_R8

	/// <summary>
	/// A character string type (<see cref="System.String"/>).
	/// </summary>
	String = 0x0e, // ELEMENT_TYPE_STRING

	/// <summary>
	/// A value type (followed by the metadata token for the type).
	/// </summary>
	ValueType = 0x11, // ELEMENT_TYPE_VALUETYPE <type token>

	/// <summary>
	/// A class type (followed by the metadata token for the type).
	/// </summary>
	Class = 0x12, // ELEMENT_TYPE_CLASS <type token>

	/// <summary>
	/// A typed reference type <see cref="System.TypedReference"/>.
	/// </summary>
	TypedReference = 0x16, // ELEMENT_TYPE_TYPEDBYREF

	/// <summary>
	/// A platform-specific signed integral type that is used to represent
	/// a pointer or a handle (<see cref="System.IntPtr"/>).
	/// </summary>
	IntPtr = 0x18, // ELEMENT_TYPE_I

	/// <summary>
	/// A platform-specific unsigned integral type that is used to represent
	/// a pointer or a handle (<see cref="System.UIntPtr"/>).
	/// </summary>
	UIntPtr = 0x19, // ELEMENT_TYPE_U

	/// <summary>
	/// An object type that can be used to pass any type <see cref="System.Object"/>.
	/// </summary>
	Object = 0x1c, // ELEMENT_TYPE_OBJECT

	/// <summary>
	/// A value that indicates this is not a type, but instead a value that
	/// modifies subsequent types.
	/// </summary>
	Modifier = 0x40, // ELEMENT_TYPE_MODIFIER

	/// <summary>
	/// A "sentinel" that marks the start of optional arguments for varargs.
	/// </summary>
	Sentinel = 0x01 | Modifier, // ELEMENT_TYPE_SENTINEL

	// The remaining types (and modifiers) are included for completeness
	// Currently, they are not supported by this class library.

	// No documentation here...not well understood
	Pinned = 0x05 | Modifier, // ELEMENT_TYPE_PINNED

	// No documentation here...not well understood
	End = 0x00, // ELEMENT_TYPE_END

	// No documentation here...not well understood...PTR <type>
	Ptr = 0x0f, // ELEMENT_TYPE_PTR

	// No documentation here...not well understood...BYREF <type>
	ByRef = 0x10, // ELEMENT_TYPE_BYREF

	// No documentation here...not well understood...a class type variable VAR <number>
	Var = 0x13, // ELEMENT_TYPE_VAR

	// No documentation here...not well understood
	// MDARRAY <type> <rank> <bcount> <bound1> ... <lbcount> <lb1> ...
	Array = 0x14, // ELEMENT_TYPE_ARRAY

	// No documentation here...not well understood
	// GENERICINST <generic type> <argCnt> <arg1> ... <argn>
	GenericInst = 0x15, // ELEMENT_TYPE_GENERICINST

	// No documentation here...not well understood
	// FNPTR <complete sig for the function including calling convention>
	FnPtr = 0x1b, // ELEMENT_TYPE_FNPTR

	// No documentation here...not well understood
	// Shortcut for single dimension zero lower bound array SZARRAY <type>
	SzArray = 0x1d, // ELEMENT_TYPE_SZARRAY

	// No documentation here...not well understood
	// A method type variable MVAR <number>
	MVar = 0x1e, // ELEMENT_TYPE_MVAR

	// No documentation here...not well understood
	// This is only for binding...required C modifier : E_T_CMOD_REQD <mdTypeRef/mdTypeDef>
	CModReqd = 0x1f, // ELEMENT_TYPE_CMOD_REQD

	// No documentation here...not well understood
	// This is only for binding...optional C modifier : E_T_CMOD_OPT <mdTypeRef/mdTypeDef>
	CModOpt = 0x20, // ELEMENT_TYPE_CMOD_OPT

	// No documentation here...not well understood
	// This is for signatures generated internally (which will not be persisted in any way).
	// INTERNAL <typehandle>
	Internal = 0x21, // ELEMENT_TYPE_INTERNAL

	// No documentation here...not well understood
	// Note that this is the max of base type excluding modifiers...first invalid element type
	Max = 0x22 // ELEMENT_TYPE_MAX
}

/// <summary>
/// Extension methods for <see cref="ElementType"/>.
/// </summary>
public static class ElementTypeExtensions
{
	private static readonly Dictionary<ElementType, Type> simpleTypes = new()
	{
		{ ElementType.Void, typeof(void) },
		{ ElementType.Boolean, typeof(bool) },
		{ ElementType.Byte, typeof(byte) },
		{ ElementType.Char, typeof(char) },
		{ ElementType.Double, typeof(double) },
		{ ElementType.Int16, typeof(short) },
		{ ElementType.Int32, typeof(int) },
		{ ElementType.Int64, typeof(long) },
		{ ElementType.IntPtr, typeof(IntPtr) },
		{ ElementType.Object, typeof(object) },
		{ ElementType.SByte, typeof(sbyte) },
		{ ElementType.Single, typeof(float) },
		{ ElementType.String, typeof(string) },
		{ ElementType.TypedReference, typeof(TypedReference) },
		{ ElementType.UInt16, typeof(ushort) },
		{ ElementType.UInt32, typeof(uint) },
		{ ElementType.UInt64, typeof(ulong) },
		{ ElementType.UIntPtr, typeof(UIntPtr) }
	};

	/// <summary> Get a <see cref="System.Type"/> for this element type. </summary>
	/// <param name="elementType"></param>
	/// <returns> A <see cref="System.Type"/>, if successful; otherwise, null.</returns>
	public static Type? ToType(this ElementType elementType) =>
		simpleTypes.TryGetValue(elementType, out Type? type) ? type : null;
}
