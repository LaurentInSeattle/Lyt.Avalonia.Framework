namespace Lyt.Reflector.IL;

/// <summary>
/// An instruction that references method information 
/// (<see cref="OperandType.InlineMethod"/> or <see cref="OperandType.InlineTok"/>).
/// </summary>
public class MethodInstruction : Instruction<Token, MethodBase>
{
    /// <summary> Create an instance for the specified byte offset and operation code (opcode). </summary>
    /// <param name="parent">The set of instructions containing this instruction.</param>
    /// <param name="offset">The byte offset of this instruction.</param>
    /// <param name="opCode">The operation code (opcode) for this instruction.</param>
    /// <param name="token">The operand (token) for this instruction.</param>
    /// <param name="method">The (optional) method for this instruction.</param>
    public MethodInstruction(
        MethodInstructionsList parent, 
        int offset, OpCode opCode, Token token, 
        MethodBase? method = null)
        : base(parent, offset, opCode, token) 
        => this.Value = method;

    /// <summary> Resolve the method for this instructon. </summary>
    /// <exception cref="System.ArgumentException">
    /// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a method within the scope
    /// of <see cref="IInstruction.Parent"/>.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <see cref="Instruction{TOperand, TValue}.Operand"/> is not a valid method within
    /// the scope <see cref="IInstruction.Parent"/>.
    /// </exception>
    public override void Resolve() => this.Value ??= this.Parent.ResolveMethod(this.Operand);

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
            builder.Append("method ");
        }

        MethodSignature.AppendConventions(builder, this.Value.CallingConvention);
        this.AppendReturnType(builder);
        this.AppendFullName(builder);
        if (this.Value.IsGenericMethod)
        {
            this.AppendTypeParameters(builder, this.Value.GetGenericArguments());
        }

        this.AppendParameters(builder);
        return builder.ToString();
    }

    // Append the full name of the method including the namespace and declaring type
    private void AppendFullName(StringBuilder builder)
    {
        if (this.Value == null)
        {
            builder.Append(InvalidValue);
            return;
        }

        Type? maybeType = this.Value.DeclaringType;
        if (maybeType is Type declaringType)
        {
            this.AppendType(builder, declaringType);
        }
        else
        {
            builder.Append(InvalidValue);            
        }

        builder.Append("::");
        builder.Append(this.Value.Name);
    }

    // Append the return type of the method
    private void AppendReturnType(StringBuilder builder)
    {
        this.AppendType(
            builder, 
            this.Value is MethodInfo methodInfo ?
                methodInfo.ReturnType : 
                typeof(void), 
            true);
        builder.Append(' ');
    }

    // Append the parameters passed to the method
    private void AppendParameters(StringBuilder builder)
    {
        if (this.Value == null)
        {
            builder.Append(InvalidValue);
            return;
        }

        builder.Append('(');
        bool isFirstParameter = true;
        foreach (ParameterInfo parameter in this.Value.GetParameters())
        {
            if (isFirstParameter)
            {
                isFirstParameter = false;
            }
            else
            {
                builder.Append(", ");
            }

            this.AppendType(builder, parameter.ParameterType, true);
        }

        builder.Append(')');
    }
}
