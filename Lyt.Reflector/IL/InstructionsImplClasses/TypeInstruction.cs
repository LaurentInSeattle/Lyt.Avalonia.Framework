namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references type information 
/// (<see cref="OperandType.InlineType"/> or <see cref="OperandType.InlineTok"/>).
/// </summary>
public class TypeInstruction : Instruction<Token, Type>
{
	/// <summary>
	/// Create an instance for the specified byte offset and operation code (opcode).
	/// </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <param name="type">The (optional) type for this instruction.</param>
	public TypeInstruction(MethodInstructionsList parent, int offset, OpCode opCode,
		Token token, Type? type = null)
		: base(parent, offset, opCode, token) =>
        this.Value = type;

	/// <summary> Resolve the type for this instruction. </summary>
	public override void Resolve() => this.Value ??= this.Parent.ResolveType(this.Operand);

    /// <summary> Returns the formatted value. </summary>
	protected override string FormatValue() 
		=> this.Value == null ? InvalidValue : this.FormatType(this.Value);
}
