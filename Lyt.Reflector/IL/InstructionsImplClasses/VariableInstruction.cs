namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references local variable information (<see cref="OperandType.InlineVar"/> or
/// <see cref="OperandType.ShortInlineVar"/>).
/// </summary>
/// <typeparam name="TOperand">The type of operand (token) for this instruction.</typeparam>
/// <remarks>
/// Regrettably, references to both parameter and local variable references share a
/// operand types.  This code distinguishes between them by examining the name of the
/// operation code (opcode).  Operation codes (opcodes) containing the text "arg" are
/// assumed to be parameter references.  All others are assumed to be local variable
/// references.
/// </remarks>
/// <remarks>
/// Create an instance for the specified byte offset and operation code (opcode).
/// </remarks>
/// <param name="parent">The set of instructions containing this instruction.</param>
/// <param name="offset">The byte offset of this instruction.</param>
/// <param name="opCode">The operation code (opcode) for this instruction.</param>
/// <param name="operand">The operand for this instruction.</param>
public class VariableInstruction<TOperand>(MethodInstructionsList parent, int offset, OpCode opCode, TOperand operand) : Instruction<TOperand, LocalVariableInfo>(parent, offset, opCode, operand)
    where TOperand : struct, IComparable, IFormattable, IConvertible,
        IComparable<TOperand>, IEquatable<TOperand>
{
    /// <summary> Gets a value indicating if the operand and resulting value are implied (as opposed to explicit).</summary>
    public bool IsOperandImplied => this.OpCode.OperandType == OperandType.InlineNone;

    /// <summary> Resolve the variable for this instructon. </summary>
    public override void Resolve() 
        => this.Value ??= this.Parent.ResolveVariable(this.Operand.ToInt32(null));

    /// <summary> Returns the formatted value. </summary>
    protected override string FormatValue()
    {
        if (this.Value == null)
        {
            return InvalidValue;
        }

        string type = this.FormatType(this.Value.LocalType);
        return 
            this.IsOperandImplied ? 
                $"// V_{this.Value.LocalIndex} {type}" :
                $"V_{this.Value.LocalIndex} // {type}";
    }
}
