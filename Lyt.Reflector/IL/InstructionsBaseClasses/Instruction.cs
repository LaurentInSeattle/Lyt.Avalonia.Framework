namespace Lyt.Reflector.IL;

/// <summary>
/// The base class for all instructions.  
/// Also, the sole class for instructions without an operand.
/// </summary>
/// <remarks> Create an instance for the specified byte offset and operation code (opcode). </remarks>
/// <param name="parent">The set of instructions containing this instruction.</param>
/// <param name="offset">The byte offset of this instruction.</param>
/// <param name="opCode">The operation code (opcode) for this instruction.</param>
public class Instruction(MethodInstructionsList parent, int offset, OpCode opCode) : IInstruction
{
    /// <summary> The text displayed for an unresolved value. </summary>
    protected const string InvalidValue = "-?-";

    /// <summary> Gets the set of instructions containing this instruction. </summary>
    public MethodInstructionsList Parent { get; } = parent;

    /// <summary> Gets the byte offset of this instruction. </summary>
    public int Offset { get; } = offset;

    /// <summary> Gets the operation code (opcode) for this instruction. </summary>
    public OpCode OpCode { get; } = opCode;

    /// <summary> Gets a value indicating if this instruction if the target of a branch or switch instruction.</summary>
    public bool IsTarget { get; internal set; }

    /// <summary> Gets a label for this instruction. </summary>
    public string Label => Instruction.FormatLabel(this.Offset);

    /// <summary> Get the operand for this instruction. </summary>
    /// <returns>The operand for this instruction.</returns>
    public virtual object? GetOperand() => null;

    /// <summary> Get the resolved value of the operand for this instruction. </summary>
    /// <returns>The resolved value of the operand for this instruction.</returns>
    public virtual object? GetValue() => null;

    /// <summary> Resolve the value for this instruction from the operand. </summary>
    public virtual void Resolve() { }

    /// <summary> Get a textual representation of this instruction. </summary>
    /// <returns>A textual representation of this instruction.</returns>
    public sealed override string ToString() => this.ToString(true);

    /// <summary> Get a textual representation of this instruction. </summary>
    /// <param name="includeLabel">A value indicating if a label should be included.</param>
    /// <returns>A textual representation of this instruction.</returns>
    public virtual string ToString(bool includeLabel) =>
        includeLabel ? $"{this.Label}: {this.OpCode.Name}" : $"{this.OpCode.Name}";

    /// <summary> Append the text for the specified type to the specified string builder. </summary>
    /// <param name="builder">The string builder to which the text is appended.</param>
    /// <param name="type">The type to format.</param>
    /// <param name="includeModifiers">A value indicating if type modifiers should be included.</param>
    protected void AppendType(StringBuilder builder, Type type, bool includeModifiers = false) 
        => CilTypes.Instance.AppendType(builder, this.Parent, type, includeModifiers);

    /// <summary> Append the text for the specified type parameters to the specified string builder. </summary>
    /// <param name="builder">The string builder to which the text is appended.</param>
    /// <param name="types">The type parameters.</param>
    protected void AppendTypeParameters(StringBuilder builder, IEnumerable<Type> types) =>
        CilTypes.Instance.AppendTypeParameters(builder, this.Parent, types);

    /// <summary> Format the label for the instruction at the specified offset. </summary>
    /// <param name="offset">The zero-based byte offset of the instruction.</param>
    /// <returns>The label for the instruction at the specified offset.</returns>
    protected static string FormatLabel(int offset) => $"IL_{offset:X4}";

    /// <summary> Format the specified type for this instruction. </summary>
    /// <param name="type">The type to format.</param>
    /// <param name="includeModifiers">A value indicating if type modifiers should be included.</param>
    /// <returns>The text for the specified type.</returns>
    protected string FormatType(Type type, bool includeModifiers = false) 
        => CilTypes.Instance.FormatType(this.Parent, type, includeModifiers);
}

