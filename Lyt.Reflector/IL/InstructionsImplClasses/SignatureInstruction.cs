namespace Lyt.Reflector.IL;

/// <summary> An instruction that references signature information (<see cref="OperandType.InlineSig"/>). </summary>
/// <remarks> Create an instance for the specified byte offset and operation code.</remarks>
/// <param name="parent">The set of instructions containing this instruction.</param>
/// <param name="offset">The byte offset of this instruction.</param>
/// <param name="opCode">The operation code (opcode) for this instruction.</param>
/// <param name="token">The operand (token) for this instruction.</param>
public class SignatureInstruction(
    MethodInstructionsList parent, int offset, OpCode opCode, Token token) 
    : Instruction<Token, MethodSignature>(parent, offset, opCode, token)
{
    /// <summary> Resolve the signature for this instructon. </summary>
    public override void Resolve() =>
        this.Value ??= 
			new MethodSignature(this.Parent, this.Parent.ResolveSignature(this.Operand));

    /// <summary> Returns the formatted value. </summary>
	protected override string FormatValue() 
		=> this.Value == null ? InvalidValue : this.Value.ToString();
}
