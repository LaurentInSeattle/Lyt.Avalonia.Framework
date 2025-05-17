namespace Lyt.Reflector.IL;

/// <summary> An enumeration of values to describe a calling convention. </summary>
/// <remarks> Adapted from CorHdr.h (CorCallingConvention and CorUnmanagedCallingConvention). </remarks>
[Flags]
public enum CilCallingConvention
{
	/// <summary>
	/// The default managed calling convention (<see cref="CallingConventions.Standard"/>).
	/// </summary>
	Standard = 0x0, // IMAGE_CEE_CS_CALLCONV_DEFAULT from CorCallingConvention

	/// <summary>
	/// An unmanaged call where the caller cleans the stack (<see cref="CallingConvention.Cdecl"/>).
	/// </summary>
	/// <remarks>
	/// This enables calling functions with varargs, which makes it appropriate to use for
	/// methods that accept a variable number of parameters, such as Printf.
	/// </remarks>
	Cdecl = 0x1, // IMAGE_CEE_UNMANAGED_CALLCONV_C from CorUnmanagedCallingConvention

	/// <summary>
	/// An unmanaged call where the callee cleans the stack (<see cref="CallingConvention.StdCall"/>).
	/// </summary>
	/// <remarks>
	/// This is the default convention for calling unmanaged functions with platform invoke.
	/// </remarks>
	StdCall = 0x2, // IMAGE_CEE_UNMANAGED_CALLCONV_STDCALL from CorUnmanagedCallingConvention

	/// <summary>
	/// An unmanaged call where the first parameter is the "this" pointer and is stored in
	/// register ECX (<see cref="CallingConvention.ThisCall"/>).
	/// </summary>
	/// <remarks>
	/// Other parameters are pushed on the stack. This calling convention is used to call
	/// methods on classes exported from an unmanaged DLL.
	/// </remarks>
	ThisCall = 0x3, // IMAGE_CEE_UNMANAGED_CALLCONV_THISCALL from CorUnmanagedCallingConvention

	/// <summary>
	/// This unmanaged calling convention is not supported (<see cref="CallingConvention.FastCall"/>).
	/// </summary>
	FastCall = 0x4, // IMAGE_CEE_UNMANAGED_CALLCONV_FASTCALL from CorUnmanagedCallingConvention

	/// <summary>
	/// A managed call with a variable number of arguments (<see cref="CallingConventions.VarArgs"/>).
	/// </summary>
	VarArgs = 0x5, // IMAGE_CEE_CS_CALLCONV_VARARG from CorCallingConvention

	/// <summary>
	/// A mask to separate the calling convention (low order 4 bits) from
	/// calling convention modifiers (high order 4 bits).
	/// </summary>
	Mask = 0xf, // IMAGE_CEE_CS_CALLCONV_MASK from CorCallingConvention

	// No documentation here...despite its location in CorHdr.h, this seems like a different
	// type of signature
	Field = 0x6, // IMAGE_CEE_CS_CALLCONV_FIELD from CorCallingConvention

	// No documentation here...despite its location in CorHdr.h, this seems like a different
	// type of signature
	LocalSig = 0x7, // IMAGE_CEE_CS_CALLCONV_LOCAL_SIG from CorCallingConvention

	// No documentation here...despite its location in CorHdr.h, this seems like a different
	// type of signature
	Property = 0x8, // IMAGE_CEE_CS_CALLCONV_PROPERTY from CorCallingConvention

	// No documentation for this unmanaged calling convention, because I'm not sure if this is
	// truly equivalent to CallingConvention.WinApi?
	WinApi = 0x9, // IMAGE_CEE_CS_CALLCONV_UNMGD from CorCallingConvention (WinApi?)

	// No documentation here...not well understood...generic method instantiation
	GenericInst = 0xa, // IMAGE_CEE_CS_CALLCONV_GENERIC from CorCallingConvention

	// No documentation here...not well understood...used ONLY for 64bit vararg PInvoke calls
	NativeVarArg = 0xb, // IMAGE_CEE_CS_CALLCONV_NATIVEVARARG from CorCallingConvention

	// No documentation here...included for completeness...first invalid calling convention
	Max = 0xc, // IMAGE_CEE_CS_CALLCONV_MAX from CorCallingConvention

	/// <summary>
	/// A modifier indicating that a managed call expects a "this" parameter
	/// (<see cref="CallingConventions.HasThis"/>).
	/// </summary>
	HasThis = 0x20, // IMAGE_CEE_CS_CALLCONV_HASTHIS in CorCallingConvention

	/// <summary>
	/// A modifier indicating that a managed call has a "this" parameter that is explicitly
	/// in the signature (<see cref="CallingConventions.ExplicitThis"/>).
	/// </summary>
	ExplicitThis = 0x40, // IMAGE_CEE_CS_CALLCONV_EXPLICITTHIS in CorCallingConvention

	// No documentation here...not well enough understood.  A modifier indicating generic
	// method sig with explicit number of type arguments (precedes ordinary parameter count)
	Generic = 0x10  // IMAGE_CEE_CS_CALLCONV_GENERIC from CorCallingConvention
}
