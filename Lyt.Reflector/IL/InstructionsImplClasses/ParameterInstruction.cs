namespace Lyt.Reflector.IL;

/// <summary> An instruction that references parameter information  </summary>
/// <typeparam name="TOperand">The type of operand (token) for this instruction.</typeparam>
/// <remarks>
/// (<see cref="OperandType.InlineVar"/> or <see cref="OperandType.ShortInlineVar"/>).
/// Regrettably, references to both parameter and local variable references share a
/// operand types.  This code distinguishes between them by examining the name of the
/// operation code (opcode).  Operation codes (opcodes) containing the text "arg" are
/// assumed to be parameter references.  All others are assumed to be local variable
/// references.
/// </remarks>
/// <remarks> Create an instance for the specified byte offset and operation code (opcode). </remarks>
/// <param name="parent">The set of instructions containing this instruction.</param>
/// <param name="offset">The byte offset of this instruction.</param>
/// <param name="opCode">The operation code (opcode) for this instruction.</param>
/// <param name="operand">The operand for this instruction.</param>
public class ParameterInstruction<TOperand>(
    MethodInstructionsList parent, int offset, OpCode opCode, TOperand operand) 
    : Instruction<TOperand, ParameterInfo>(parent, offset, opCode, operand)
    where TOperand : struct, IBinaryInteger<TOperand>
{

    /// <summary> Gets a value indicating if the parameter is the "this" argument. </summary>
    public bool IsThis { get; private set; }

    /// <summary> Gets a value indicating if the operand and resulting value are implied (as opposed to explicit).</summary>
    public bool IsOperandImplied => this.OpCode.OperandType == OperandType.InlineNone;

    /// <summary> Resolve the parameter for this instructon. </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <see cref="Instruction{TOperand, TValue}.Operand"/> does not identify a valid
    /// parameter within the scope of <see cref="IInstruction.Parent"/>.
    /// </exception>
    public override void Resolve()
    {
        if (this.Value != null || this.IsThis)
        {
            return;
        }

        this.Value = this.Parent.ResolveParameter(this.Operand.ToInt32());
        if (this.Value == null)
        {
            this.IsThis = true;
        }
    }

    /// <summary> Returns the formatted value. </summary>
    protected override string FormatValue()
    {
        if (this.Value != null)
        {
            string? valueName = this.Value.Name; 
            if (string.IsNullOrWhiteSpace(valueName))
            {
                valueName = InvalidValue;
            }

            return this.IsOperandImplied ? $"// {valueName}" : valueName;
        }

        if (this.IsOperandImplied)
        {
            return this.IsThis ? "// this" : "// ?";
        }

        return this.IsThis ? $"{this.Operand} // this" : $"{this.Operand} // ?";
    }
}
