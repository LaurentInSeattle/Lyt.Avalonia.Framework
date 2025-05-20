namespace Lyt.Reflector.IL;

/// <summary> An instruction that references signature information (<see cref="OperandType.InlineString"/>). </summary>
/// <remarks> Create an instance for the specified byte offset and operation code. </remarks>
/// <param name="parent">The set of instructions containing this instruction.</param>
/// <param name="offset">The byte offset of this instruction.</param>
/// <param name="opCode">The operation code (opcode) for this instruction.</param>
/// <param name="token">The operand (token) for this instruction.</param>
public class StringInstruction(
    MethodInstructionsList parent, int offset, OpCode opCode, Token token) 
    : Instruction<Token, string>(parent, offset, opCode, token)
{
    /// <summary> Resolve the string for this instructon. </summary>
    public override void Resolve() => this.Value ??= this.Parent.ResolveString(this.Operand);

    /// <summary> Returns the formatted value. </summary>
    protected override string FormatValue()
    {
        if ( string.IsNullOrWhiteSpace(this.Value) )
        {
            return InvalidValue;
        }

        var builder = new StringBuilder(this.Value.Length << 1);
        builder.Append('\"');
        bool asByteArray = false;
        foreach (char chr in this.Value)
        {
            switch (chr)
            {
                // Escape sequences recognized by ILASM
                case '\\': builder.Append("\\\\"); break;
                case '\"': builder.Append("\\\""); break;
                case '\a': builder.Append("\\a"); break;
                case '\b': builder.Append("\\b"); break;
                case '\f': builder.Append("\\f"); break;
                case '\n': builder.Append("\\n"); break;
                case '\r': builder.Append("\\r"); break;
                case '\t': builder.Append("\\t"); break;
                case '\v': builder.Append("\\v"); break;

                default:
                    // Unescaped characters recognized by ILASM (we think?)
                    if (char.IsLetterOrDigit(chr) || char.IsPunctuation(chr) ||
                        char.IsSeparator(chr) || char.IsSymbol(chr))
                    {
                        builder.Append(chr);
                        break;
                    }

                    // Escaped octal sequences recognized by ILASM
                    if (chr > 0 && chr < 20)
                    {
                        builder.Append('\\');
                        builder.Append(Convert.ToString(chr, 8).PadLeft(3, '0'));
                        break;
                    }

                    // Characters where ILASM requires us to fall back to a byte array
                    asByteArray = true;
                    break;
            }

            if (asByteArray)
            {
                break;
            }
        }

        if (!asByteArray)
        {
            builder.Append('\"');
            return builder.ToString();
        }

        return this.FormatByteArray();
    }

    private string FormatByteArray()
    {
        if (string.IsNullOrWhiteSpace(this.Value))
        {
            return InvalidValue;
        }

        var builder = new StringBuilder(this.Value.Length << 2);
        builder.Append("bytearray(");
        bool isFirstByte = true;
        foreach (byte nextByte in Encoding.Unicode.GetBytes(this.Value))
        {
            if (isFirstByte)
            {
                builder.AppendFormat("{0:X2}", nextByte);
                isFirstByte = false;
            }
            else
            {
                builder.AppendFormat(" {0:X2}", nextByte);
            }
        }

        builder.Append(')');
        return builder.ToString();
    }
}
