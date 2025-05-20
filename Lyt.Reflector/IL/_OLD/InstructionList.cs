namespace Lyt.Reflector.IL;

/*
 
/// <summary> A list of instructions. </summary>
public abstract class InstructionList 
{
    /// <summary> Create an instance for the specified byte data. </summary>
    /// <param name="data">The byte data containing the instructions.</param>
    protected InstructionList(byte[] data) =>
        this.Data = new ReadOnlyCollection<byte>(data ??
            throw new ArgumentNullException(nameof(data)));

    private static readonly Dictionary<OpCode, byte> impliedParameters =
        new()
    {
        { OpCodes.Ldarg_0, 0 },
        { OpCodes.Ldarg_1, 1 },
        { OpCodes.Ldarg_2, 2 },
        { OpCodes.Ldarg_3, 3 }
    };

    private static readonly Dictionary<OpCode, byte> impliedVariables =
        new()
    {
        { OpCodes.Ldloc_0, 0 },
        { OpCodes.Ldloc_1, 1 },
        { OpCodes.Ldloc_2, 2 },
        { OpCodes.Ldloc_3, 3 },
        { OpCodes.Stloc_0, 0 },
        { OpCodes.Stloc_1, 1 },
        { OpCodes.Stloc_2, 2 },
        { OpCodes.Stloc_3, 3 }
    };

    private readonly List<IInstruction> instructions = [];

    /// <summary>
    /// Gets the instruction at the specified zero-based index.
    /// </summary>
    /// <param name="index">The zero-based index of the instruction.</param>
    /// <returns>The instruction at the specified zero-based index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or greater than or equal to
    /// the number of instructions.
    /// </exception>
    public IInstruction this[int index] => instructions[index];

    /// <summary>
    /// Gets the byte data for these instructions.
    /// </summary>
    public IReadOnlyList<byte> Data { get; }

    /// <summary>
    /// Gets the number of instructions.
    /// </summary>
    public int Count => instructions.Count;

    /// <summary>
    /// Gets a value indicating if any problems occurred decoding the byte
    /// data (<see cref="Data"/>) for these instructions.
    /// </summary>
    public bool IsInvalidData { get; private set; }

    /// <summary>
    /// Get an enumerator for these instructions.
    /// </summary>
    /// <returns>An enumerator for these instructions.</returns>
    public IEnumerator<IInstruction> GetEnumerator() =>
        instructions.GetEnumerator();

    /// <summary>
    /// Gets a value indicating if the specified instruction is contained
    /// within an instance method.
    /// </summary>
    /// <param name="instruction">The instruction to test.</param>
    /// <returns>True, if the instruction is contained within an instanc method; otherwise, false.</returns>
    public abstract bool HasThis(IInstruction instruction);

    /// <summary>
    /// Gets a value indicating if the specified assembly is the same
    /// as the one containing this instruction list.
    /// </summary>
    /// <param name="assembly">The assembly to test.</param>
    /// <returns>True, if the assembly is the same; otherwise, false.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="assembly"/> is null.
    /// </exception>
    public abstract bool IsSameAssembly(Assembly assembly);

    /// <summary>
    /// Resolve field information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The field information.</returns>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="token"/> is not a field within the scope of this method's module.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="token"/> is not a valid field within this method's module.
    /// </exception>
    public abstract FieldInfo ResolveField(Token token);

    /// <summary>
    /// Resolve an instruction for a byte offset.
    /// </summary>
    /// <param name="offset">The byte offset of an instruction within this method body.</param>
    /// <returns>The instruction.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> does not specify the valid byte offset of an
    /// instruction within this method body.
    /// </exception>
    public abstract IInstruction ResolveInstruction(int offset);

    /// <summary>
    /// Resolve member information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The member information.</returns>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="token"/> is not a member within the scope of this method's module.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="token"/> is not a valid member within this method's module.
    /// </exception>
    public abstract MemberInfo ResolveMember(Token token);

    /// <summary>
    /// Resolve method information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The method information.</returns>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="token"/> is not a method within the scope of this method's module.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="token"/> is not a valid method within this method's module.
    /// </exception>
    public abstract MethodBase ResolveMethod(Token token);

    /// <summary>
    /// Resolve parameter information from the index of a parameter.
    /// </summary>
    /// <param name="operand">The zero-based index of the parameter.</param>
    /// <returns>The parameter information.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="operand"/> does not identify a valid parameter for this method.
    /// </exception>
    public abstract ParameterInfo ResolveParameter(int operand);

    /// <summary>
    /// Resolve signature information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The signature information.</returns>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="token"/> is not a signature within the scope of this method's module.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="token"/> is not a valid signature within this method's module.
    /// </exception>
    public abstract byte[] ResolveSignature(Token token);

    /// <summary>
    /// Resolve string information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The string information.</returns>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="token"/> is not a string within the scope of this method's module.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="token"/> is not a valid string within this method's module.
    /// </exception>
    public abstract string ResolveString(Token token);

    /// <summary>
    /// Resolve type information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The type information.</returns>
    /// <exception cref="System.ArgumentException">
    /// <paramref name="token"/> is not a type within the scope of this method's module.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="token"/> is not a valid type within this method's module.
    /// </exception>
    public abstract Type ResolveType(Token token);

    /// <summary>
    /// Resolve local variable information from the specified index.
    /// </summary>
    /// <param name="operand">The zero-based index of the variable.</param>
    /// <returns>The local variable information.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="operand"/> does not identify a valid local variable for this method.
    /// </exception>
    public abstract LocalVariableInfo ResolveVariable(int operand);

    /// <summary>
    /// Decode the instructions.
    /// </summary>
    protected void DecodeInstructions()
    {
        int count = Data.Count;
        int offset = 0;

        while (offset < count)
        {
            if (TryCreate(ref offset, out IInstruction instruction))
            {
                instructions.Add(instruction);
            }
            else
            {
                IsInvalidData = true;
                break;
            }
        }

        Resolve();
    }

    // Create a switch instruction
    private IInstruction CreateSwitch(int offset, OpCode opCode, ref int operandOffset)
    {
        int length = Data.ReadInt32(operandOffset);
        if (length < 0)
            throw new ArgumentException(null, nameof(Data));

        int[] branches = new int[length];
        operandOffset += sizeof(int);

        for (int index = 0; index < length; index++)
        {
            branches[index] = Data.ReadInt32(operandOffset);
            operandOffset += sizeof(int);
        }

        return new SwitchInstruction(this, offset, opCode, length, branches);
    }

    // Try to create a token for the specified instruction
    private IInstruction CreateToken(int offset, OpCode opCode, int operandOffset)
    {
        Token token = Data.ReadToken(operandOffset);

        switch (token.Type)
        {
            case TokenType.TypeDef:
            case TokenType.TypeRef:
            case TokenType.TypeSpec:
                return new TypeInstruction(this, offset, opCode, token);

            case TokenType.MethodSpec:
            case TokenType.MethodDef:
                return new MethodInstruction(this, offset, opCode, token);

            case TokenType.FieldDef:
                return new FieldInstruction(this, offset, opCode, token);

            case TokenType.Signature:
                return new SignatureInstruction(this, offset, opCode, token);

            case TokenType.String:
                return new StringInstruction(this, offset, opCode, token);

            case TokenType.MemberRef:
                return MemberInstruction.Create(this, offset, opCode, token);

            default:
                return new Instruction<Token>(this, offset, opCode, token);
        }
    }

    // Resolve all of the instruction values
    private void Resolve()
    {
        foreach (IInstruction instruction in instructions)
        {
            try { instruction.Resolve(); }
            catch { IsInvalidData = true; }
        }
    }

    // Try to create an instruction from the byte data at the specified offset
    private bool TryCreate(ref int offset, out IInstruction instruction)
    {
        instruction = null;
        if (offset >= Data.Count)
            return false;

        int index = offset;
        short code = Data.ReadOpCode(ref index);

        if (!AllOpCodes.Instance.TryGetValue(code, out OpCode opCode))
            return false;

        try
        {
            switch (opCode.OperandType)
            {
                case OperandType.InlineBrTarget:
                    instruction = new BranchInstruction<int>(this, offset, opCode,
                        Data.ReadInt32(index), sizeof(int));
                    index += sizeof(int);
                    break;

                case OperandType.ShortInlineBrTarget:
                    instruction = new BranchInstruction<sbyte>(this, offset, opCode,
                        Data.ReadSByte(index), sizeof(sbyte));
                    index += sizeof(sbyte);
                    break;

                case OperandType.InlineField:
                    instruction = new FieldInstruction(this, offset, opCode,
                        Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineI:
                    instruction = new Instruction<int>(this, offset, opCode,
                        Data.ReadInt32(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineI8:
                    instruction = new Instruction<long>(this, offset, opCode,
                        Data.ReadInt64(index));
                    index += sizeof(long);
                    break;

                case OperandType.ShortInlineI:
                    instruction = opCode == OpCodes.Ldc_I4_S ?
                        (IInstruction)new Instruction<sbyte>(this, offset, opCode,
                            Data.ReadSByte(index)) :
                        new Instruction<byte>(this, offset, opCode,
                            Data.ReadByte(index));
                    index += sizeof(byte);
                    break;


                case OperandType.InlineMethod:
                    instruction = new MethodInstruction(this, offset, opCode,
                        Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineNone:
                    if (impliedParameters.TryGetValue(opCode, out byte operand))
                    {
                        instruction = new ParameterInstruction<byte>(
                            this, offset, opCode, operand);
                    }
                    else
                    {
                        if (impliedVariables.TryGetValue(opCode, out operand))
                        {
                            instruction = new VariableInstruction<byte>(
                                this, offset, opCode, operand);
                        }
                        else
                        {
                            instruction = new Instruction(this, offset, opCode);
                        }
                    }
                    break;

                case OperandType.InlineR:
                    instruction = new Instruction<double>(this, offset, opCode,
                        Data.ReadDouble(index));
                    index += sizeof(double);
                    break;

                case OperandType.ShortInlineR:
                    instruction = new Instruction<float>(this, offset, opCode,
                        Data.ReadSingle(index));
                    index += sizeof(float);
                    break;

                case OperandType.InlineSig:
                    instruction = new SignatureInstruction(this, offset, opCode,
                        Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineString:
                    instruction = new StringInstruction(this, offset, opCode,
                        Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineSwitch:
                    instruction = CreateSwitch(offset, opCode, ref index);
                    break;

                case OperandType.InlineTok:
                    instruction = CreateToken(offset, opCode, index);
                    index += sizeof(int);
                    break;

                case OperandType.InlineType:
                    instruction = new TypeInstruction(this, offset, opCode,
                        Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineVar:
                    instruction = opCode.Name.Contains("arg") ?
                        (IInstruction)new ParameterInstruction<ushort>(
                            this, offset, opCode, Data.ReadUInt16(index)) :
                        new VariableInstruction<ushort>(this, offset, opCode,
                            Data.ReadUInt16(index));
                    index += sizeof(ushort);
                    break;

                case OperandType.ShortInlineVar:
                    instruction = opCode.Name.Contains("arg") ?
                        (IInstruction)new ParameterInstruction<byte>(
                            this, offset, opCode, Data.ReadByte(index)) :
                        new VariableInstruction<byte>(this, offset, opCode,
                            Data.ReadByte(index));
                    index += sizeof(byte);
                    break;

                default:
                    throw new ArgumentException(null, nameof(Data));
            }

            offset = index;
            return true;
        }
        catch
        {
            instruction = null;
            return false;
        }
    }

}

*/