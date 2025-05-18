namespace Lyt.Reflector.IL;

/// <summary>
/// A meta-data token, see:
/// https://docs.microsoft.com/en-us/dotnet/standard/metadata-and-self-describing-components.
/// </summary>
/// <remarks>
/// The unusually large number of methods and interfaces are actualy quite simple.  They
/// basically mirror what is available for the underyling <see cref="int"/> type for
/// this class.
/// </remarks>
[DebuggerDisplay(nameof(Type) + "={" + nameof(Type) + "}, " +
    nameof(Id) + "={" + nameof(Id) + "}, " + 
    nameof(Value) + "={" + nameof(Value) + "}")]
public struct Token 
{
    /// <summary> The mask for the type portion of the token. </summary>
    public const int TypeMask = 0xFF0000 << 8;

    /// <summary> The mask for the record/identifier (RID) postion of the token. </summary>
    public const int IdMask = 0xFFFFFF;

    /// <summary> The empty value for a token. </summary>
    public static readonly Token Empty = new (TokenType.Module, 0);

    /// <summary> Create an instance for the specified "raw" value. </summary>
    /// <param name="value">The "raw" value of this token.</param>
    public Token(int value) => this.Value = value;

    /// <summary> Create an instance for the specified token type and record/row identifier. </summary>
    /// <param name="type">The type of this token.</param>
    /// <param name="id">The record/row identifier (RID) for this token.</param>
    public Token(TokenType type, int id) => this.Value = (int)type | id;

    /// <summary> Gets the record/row identifier (RID) for this token. </summary>
    public readonly int Id => this.Value & IdMask;

    /// <summary> Gets the type of this token. </summary>
    public readonly TokenType Type => (TokenType)(this.Value & TypeMask);

    /// <summary> Gets the "raw" value of this token. </summary>
    public int Value { get; }
}
