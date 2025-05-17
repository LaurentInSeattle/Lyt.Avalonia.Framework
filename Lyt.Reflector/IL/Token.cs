namespace Lyt.Reflector.IL;

/// <summary>
/// A meta-data token, see:
/// https://docs.microsoft.com/en-us/dotnet/standard/metadata-and-self-describing-components.
/// </summary>
/// <remarks>
/// The unusually large number of methods and interfaces are actualy quite simple.  They
/// basically mirror what is available for the underyling <see cref="Int32"/> type for
/// this class.
/// </remarks>
[DebuggerDisplay(nameof(Type) + "={" + nameof(Type) + "}, " +
	nameof(Id) + "={" + nameof(Id) + "}, " + 
	nameof(Value) + "={" + nameof(Value) + "}")]
public struct Token : IComparable, IFormattable, IConvertible,
	IComparable<int>, IComparable<Token>, IEquatable<int>, IEquatable<Token>
{
	/// <summary>
	/// Create an instance for the specified "raw" value.
	/// </summary>
	/// <param name="value">The "raw" value of this token.</param>
	public Token(int value) =>
		Value = value;

	/// <summary>
	/// Create an instance for the specified token type and record/row identifier.
	/// </summary>
	/// <param name="type">The type of this token.</param>
	/// <param name="id">The record/row identifier (RID) for this token.</param>
	public Token(TokenType type, int id) =>
		Value = (int)type | id;

	/// <summary>
	/// The empty value for a token.
	/// </summary>
	public static readonly Token Empty = new Token(TokenType.Module, 0);

	/// <summary>
	/// The mask for the type portion of the token.
	/// </summary>
	public const int TypeMask = 0xFF0000 << 8;

	/// <summary>
	/// The mask for the record/identifier (RID) postion of the token.
	/// </summary>
	public const int IdMask = 0xFFFFFF;

	/// <summary>
	/// Gets the record/row identifier (RID) for this token.
	/// </summary>
	public int Id => Value & IdMask;

	/// <summary>
	/// Gets the type of this token.
	/// </summary>
	public TokenType Type => (TokenType)(Value & TypeMask);

	/// <summary>
	/// Gets the "raw" value of this token.
	/// </summary>
	public int Value { get; }

	/// <summary>
	/// Compare this token to another object.
	/// </summary>
	/// <param name="obj">The other object to which this token is compared.</param>
	/// <returns>
	/// -1, if less than <paramref name="obj"/>;
	/// 0, if equal to <paramref name="obj"/>;
	/// 1, if greater than <paramref name="obj"/>.
	/// </returns>
	public int CompareTo(object obj) =>
		obj is Token token ? CompareTo(token) : Value.CompareTo(obj);

	/// <summary>
	/// Compare this token to another integer.
	/// </summary>
	/// <param name="other">The other integer to which this token is compared.</param>
	/// <returns>
	/// -1, if less than <paramref name="other"/>;
	/// 0, if equal to <paramref name="other"/>;
	/// 1, if greater than <paramref name="other"/>.
	/// </returns>
	public int CompareTo(int other) =>
		Value.CompareTo(other);

	/// <summary>
	/// Compare this token to another token.
	/// </summary>
	/// <param name="other">The other token to which this token is compared.</param>
	/// <returns>
	/// -1, if less than <paramref name="other"/>;
	/// 0, if equal to <paramref name="other"/>;
	/// 1, if greater than <paramref name="other"/>.
	/// </returns>
	public int CompareTo(Token other) =>
		Value.CompareTo(other.Value);

	/// <summary>
	/// Get a value indicating if this token is equal to another object.
	/// </summary>
	/// <param name="obj">The other object to which this token is compared.</param>
	/// <returns>True, if this token is equal to the specified object; otherwise, false.</returns>
	public override bool Equals(object obj) =>
		obj is Token token ? Equals(token) : Value.Equals(obj);

	/// <summary>
	/// Get a value indicating if this token is equal to another integer.
	/// </summary>
	/// <param name="obj">The other integer to which this token is compared.</param>
	/// <returns>True, if this token is equal to the specified integer; otherwise, false.</returns>
	public bool Equals(int other) =>
		Value.Equals(other);

	/// <summary>
	/// Get a value indicating if this token is equal to another token.
	/// </summary>
	/// <param name="obj">The other token to which this token is compared.</param>
	/// <returns>True, if this token is equal to the specified token; otherwise, false.</returns>
	public bool Equals(Token other) =>
		Value.Equals(other.Value);

	/// <summary>
	/// Gets the hash code for this token.
	/// </summary>
	/// <returns>The hash code for this token.</returns>
	public override int GetHashCode() =>
		Value.GetHashCode();

	/// <summary>
	/// Gets the type code for the token value.
	/// </summary>
	/// <returns>The type code for the token value.</returns>
	public TypeCode GetTypeCode() =>
		Value.GetTypeCode();

	/// <summary>
	/// Get the textual representation of this token.
	/// </summary>
	/// <returns>The textual representation of this token.</returns>
	public override string ToString() =>
		Value.ToString();

	/// <summary>
	/// Get the textual representation of this token.
	/// </summary>
	/// <param name="provider">An object that provides culture-specific formatting information.</param>
	/// <returns>The textual representation of this token.</returns>
	public string ToString(IFormatProvider provider) =>
		Value.ToString(provider);

	/// <summary>
	/// Get the textual representation of this token.
	/// </summary>
	/// <param name="format">A standard or custom numeric format string.</param>
	/// <returns>The textual representation of this token.</returns>
	public string ToString(string format) =>
		Value.ToString(format);

	/// <summary>
	/// Get the textual representation of this token.
	/// </summary>
	/// <param name="format">A standard or custom numeric format string.</param>
	/// <param name="formatProvider">An object that provides culture-specific formatting information.</param>
	/// <returns>The textual representation of this token.</returns>
	public string ToString(string format, IFormatProvider formatProvider) =>
		Value.ToString(format, formatProvider);

	bool IConvertible.ToBoolean(IFormatProvider provider) =>
		((IConvertible) Value).ToBoolean(provider);

	byte IConvertible.ToByte(IFormatProvider provider) =>
		((IConvertible) Value).ToByte(provider);

	char IConvertible.ToChar(IFormatProvider provider) =>
		((IConvertible) Value).ToChar(provider);

	DateTime IConvertible.ToDateTime(IFormatProvider provider) =>
		((IConvertible) Value).ToDateTime(provider);

	decimal IConvertible.ToDecimal(IFormatProvider provider) =>
		((IConvertible) Value).ToDecimal(provider);

	double IConvertible.ToDouble(IFormatProvider provider) =>
		((IConvertible) Value).ToDouble(provider);

	short IConvertible.ToInt16(IFormatProvider provider) =>
		((IConvertible) Value).ToInt16(provider);

	int IConvertible.ToInt32(IFormatProvider provider) =>
		((IConvertible) Value).ToInt32(provider);

	long IConvertible.ToInt64(IFormatProvider provider) =>
		((IConvertible) Value).ToInt64(provider);

	sbyte IConvertible.ToSByte(IFormatProvider provider) =>
		((IConvertible) Value).ToSByte(provider);
	float IConvertible.ToSingle(IFormatProvider provider) =>
		((IConvertible) Value).ToSingle(provider);

	string IConvertible.ToString(IFormatProvider provider) =>
		((IConvertible) Value).ToString(provider);

	object IConvertible.ToType(Type conversionType, IFormatProvider provider) =>
		((IConvertible) Value).ToType(conversionType, provider);

	ushort IConvertible.ToUInt16(IFormatProvider provider) =>
		((IConvertible) Value).ToUInt16(provider);

	uint IConvertible.ToUInt32(IFormatProvider provider) =>
		((IConvertible) Value).ToUInt32(provider);

	ulong IConvertible.ToUInt64(IFormatProvider provider) =>
		((IConvertible) Value).ToUInt64(provider);
}
