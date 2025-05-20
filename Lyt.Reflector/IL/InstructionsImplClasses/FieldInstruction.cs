namespace Lyt.Reflector.IL;

/// <summary> An instruction that references field information  </summary>
/// <remarks>  (<see cref="OperandType.InlineField"/> or <see cref="OperandType.InlineTok"/>). </remarks>
public class FieldInstruction : Instruction<Token, FieldInfo>
{
	/// <summary> Create an instance for the specified byte offset and operation code (opcode). </summary>
	/// <param name="parent">The set of instructions containing this instruction.</param>
	/// <param name="offset">The byte offset of this instruction.</param>
	/// <param name="opCode">The operation code (opcode) for this instruction.</param>
	/// <param name="token">The operand (token) for this instruction.</param>
	/// <param name="field">The (optional) field for this instruction.</param>
	public FieldInstruction(
		MethodInstructionsList parent, 
		int offset, OpCode opCode, Token token, 
		FieldInfo? field = null)
		: base(parent, offset, opCode, token) 
		=> this.Value = field;

	/// <summary> Resolve the field for this instructon. </summary>
	public override void Resolve() => this.Value ??= this.Parent.ResolveField(this.Operand);

    /// <summary> Returns the formatted value. </summary>
    protected override string FormatValue()
	{
		if (this.Value == null)
        {
            return InvalidValue;
        }

        var builder = new StringBuilder(1024);
		if (this.OpCode.OperandType == OperandType.InlineTok)
        {
            builder.Append("field ");
        }

        this.AppendType(builder, this.Value.FieldType);
		builder.Append(' ');
		Type? maybeType = this.Value.DeclaringType;
		if (maybeType is Type declaringType)
		{
			this.AppendType(builder, declaringType);
		}
		else
		{
            _ = builder.Append(InvalidValue);
        }

        builder.Append("::");
		builder.Append(this.Value.Name);
		return builder.ToString();
	}
}
