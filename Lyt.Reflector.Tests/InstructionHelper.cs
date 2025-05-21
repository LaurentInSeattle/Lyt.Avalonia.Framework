namespace Lyt.Reflector.Tests;

/// <summary>
/// Helper class to generate a tighly controlled set of instructions to test
/// the various classes that are derived from <see cref="Instruction"/>.
/// </summary>
public class InstructionHelper
{
    /// <summary>
    /// The name of the test <see cref="Assembly"/> created by this class.
    /// </summary>
    public const string TestAssemblyName = "TestAssembly";

    /// <summary>
    /// The name of the test <see cref="MethodInfo"/> created by this class.
    /// </summary>
    public const string TestMethodName = "TestMethod";

    /// <summary>
    /// The name of the test <see cref="Module"/> created by this class.
    /// </summary>
    public const string TestModuleName = "TestModule.exe";

    /// <summary>
    /// The text displayed by <see cref="CreateHelloWorld"/>.
    /// </summary>
    public const string TestText = "Hello World!";

    /// <summary>
    /// The name of the test <see cref="Type"/> created by this class.
    /// </summary>
    public const string TestTypeName = "TestType";

    /// <summary>
    /// Create an assembly with the specified instructions and access.
    /// </summary>
    /// <param name="addInstructions">The method used to add instructions to the test method.</param>
    /// <param name="addMembers">The method used to add members.</param>
    /// <param name="access">The access allowed to the assembly.</param>
    /// <returns>The assembly that was created.</returns>
    public static AssemblyBuilder CreateAssembly(Action<ILGenerator> addInstructions,
        Action<TypeBuilder>? addMembers = null,
        AssemblyBuilderAccess access = AssemblyBuilderAccess.Run)
    {
        AssemblyBuilder assembly =
            AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(TestAssemblyName), access);
        ModuleBuilder module = assembly.DefineDynamicModule(TestModuleName);
        TypeBuilder type = module.DefineType(TestTypeName, TypeAttributes.Public);
        addMembers?.Invoke(type);

        MethodBuilder method = type.DefineMethod(TestMethodName,
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard, typeof(void), new Type[0]);

        addInstructions?.Invoke(method.GetILGenerator());
        type.CreateType();

        return assembly;
    }

    /// <summary>
    /// Create a "Hello World!" method.
    /// </summary>
    /// <returns>A "Hello World!" method.</returns>
    public static MethodInfo CreateHelloWorld() =>
        CreateMethod(il =>
        {
            il.EmitWriteLine(TestText);
            il.Emit(OpCodes.Ret);
        });

    /// <summary>
    /// Create a method with the specified instructions.
    /// </summary>
    /// <param name="addInstructions">The method used to add instructions to the test method.</param>
    /// <param name="addMembers">The method used to add members.</param>
    /// <returns>A method with the specified instructions.</returns>
    public static MethodInfo CreateMethod(Action<ILGenerator> addInstructions,
        Action<TypeBuilder> addMembers = null)
    {
        var assembly = CreateAssembly(addInstructions, addMembers);
        var modules = assembly.GetModules();
        var module = modules[0];
        var type = module.GetType(TestTypeName);
        var methodInfo = type.GetMethod(TestMethodName, BindingFlags.Public | BindingFlags.Static);
        return methodInfo; 
    }

    /// <summary>
    /// Test the specified instruction with an operand type of
    /// <see cref="OperandType.InlineNone"/>.
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    public static void TestInstruction(OpCode opCode, string expectedText,
        int expectedOffset = 0, bool expectedIsTarget = false,
        int expectedCount = 1, int index = 0, Type expectedType = null)
    {
        MethodInfo method = CreateMethod(opCode, OperandType.InlineNone, il =>
            il.Emit(opCode));
        TestInstruction(method.GetIL(), opCode, expectedText, expectedOffset,
            expectedIsTarget, expectedCount, index, expectedType);
    }

    /// <summary>
    /// Test the specified instruction with the specified operand type.
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> for the instruction.</param>
    /// <param name="arg">The argument for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOperandType">The expected <see cref="OperandType"/> for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    public static void TestInstruction(OpCode opCode, byte arg, string expectedText,
        OperandType expectedOperandType = OperandType.ShortInlineI,
        int expectedOffset = 0, bool expectedIsTarget = false, int expectedCount = 1,
        int index = 0, Type expectedType = null)
    {
        MethodInfo method = CreateMethod(opCode, OperandType.ShortInlineI, il =>
            il.Emit(opCode, arg));
        TestInstruction(method.GetIL(), opCode, arg, expectedText, expectedOffset,
            expectedIsTarget, expectedCount, index, expectedType);
    }

    /// <summary>
    /// Test the specified instruction with the specified operand type.
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> for the instruction.</param>
    /// <param name="arg">The argument for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOperandType">The expected <see cref="OperandType"/> for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    public static void TestInstruction(OpCode opCode, sbyte arg, string expectedText,
        OperandType expectedOperandType = OperandType.ShortInlineI,
        int expectedOffset = 0, bool expectedIsTarget = false, int expectedCount = 1,
        int index = 0, Type expectedType = null)
    {
        MethodInfo method = CreateMethod(opCode, OperandType.ShortInlineI, il =>
            il.Emit(opCode, arg));
        TestInstruction(method.GetIL(), opCode, arg, expectedText, expectedOffset,
            expectedIsTarget, expectedCount, index, expectedType);
    }

    /// <summary>
    /// Get the token for a specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <param name="method">The method containing the reference.</param>
    /// <param name="tokenType">The type of token.</param>
    /// <param name="value">The value for which a token is obtained.</param>
    /// <param name="testCandidate">The method used to test a candidate.</param>
    /// <returns>The token, if successful; otherwise, null.</returns>
    internal Token GetToken<TValue>(MethodInfo method, TokenType tokenType, TValue value,
        Func<Token, MethodInfo, TValue, bool> testCandidate)
    {
        for (int index = 1; index <= 0xFFFFFF; index++)
        {
            try
            {
                Token token = new Token(tokenType, index);
                if (testCandidate(token, method, value))
                    return token;
            }
            catch
            {
                break;
            }
        }

        return Token.Empty;
    }

    /// <summary>
    /// Create a method with the specified instructions.
    /// </summary>
    /// <param name="opCode">The <see cref="OpCode"/> for the instruction.</param>
    /// <param name="expectedOperandType">The expected <see cref="OperandType"/> for the instruction.</param>
    /// <param name="addInstructions">The method used to add instructions to the test method.</param>
    /// <param name="addMembers">The method used to add members.</param>
    /// <returns>A method with the specified instructions.</returns>
    protected static MethodInfo CreateMethod(OpCode opCode,
        OperandType expectedOperandType, Action<ILGenerator> addInstructions,
        Action<TypeBuilder> addMembers = null)
    {
        Assert.IsNotNull(opCode, "opCode failed");
        Assert.AreEqual(expectedOperandType, opCode.OperandType,
            $"{opCode.Name} expectedOperandType failed");

        MethodInfo method = CreateMethod(il => addInstructions(il), addMembers);
        Assert.IsNotNull(method, $"{opCode.Name} method failed");
        return method;
    }

    /// <summary>
    /// Test the specified instruction.
    /// </summary>
    /// <param name="instructions">The <see cref="Instruction"/> to test.</param>
    /// <param name="expectedOpCode">The <see cref="OpCode"/> to test.</param>
    /// <param name="expectedOperand">The expected operand for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    /// <typeparam name="TOperand">The type of the operand for the instruction.</typeparam>
    protected static void TestInstruction<TOperand>(MethodInstructionsList instructions,
        OpCode expectedOpCode, TOperand expectedOperand, string expectedText,
        int expectedOffset = 0, bool expectedIsTarget = false, int expectedCount = 1,
        int index = 0, Type expectedType = null)
            where TOperand : struct

        =>
        TestInstruction(instructions, expectedOpCode, expectedOperand,
            expectedOperand, expectedText, expectedOffset, expectedIsTarget,
            expectedCount, index, expectedType ?? typeof(Instruction<TOperand>));

    /// <summary>
    /// Test the specified instruction.
    /// </summary>
    /// <param name="instructions">The <see cref="Instruction"/> to test.</param>
    /// <param name="expectedOpCode">The <see cref="OpCode"/> to test.</param>
    /// <param name="expectedOperand">The expected operand for the instruction.</param>
    /// <param name="expectedValue">The expected value for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    /// <typeparam name="TOperand">The type of the operand for the instruction.</typeparam>
    /// <typeparam name="TValue">The type of the falue for the instruction.</typeparam>
    protected static void TestInstruction<TOperand, TValue>(
        MethodInstructionsList instructions,
        OpCode expectedOpCode, TOperand expectedOperand, TValue expectedValue,
        string expectedText, int expectedOffset = 0, bool expectedIsTarget = false,
        int expectedCount = 1, int index = 0, Type expectedType = null)
        where TOperand : struct
    {
        var instruction = TestInstruction(instructions, expectedOpCode, expectedText,
            expectedOffset, expectedIsTarget, expectedCount, index,
            expectedType ?? typeof(Instruction<TOperand, TValue>))
            as IInstruction<TOperand, TValue>;
        Assert.IsNotNull(instruction, $"{expectedOpCode.Name} instruction failed");
        Assert.IsNotNull(expectedOperand, $"{expectedOpCode.Name} expectedOperand failed");

        Type expectedOperandType = typeof(TOperand);
        Assert.IsInstanceOfType(instruction.Operand, expectedOperandType,
            $"{expectedOpCode.Name} Operand type failed");
        Assert.AreEqual(expectedOperand, instruction.Operand,
            $"{expectedOpCode.Name} Operand failed");

        Type expectedValueType = typeof(TValue);
        if (expectedValue != null)
            Assert.IsInstanceOfType(instruction.Value, expectedValueType,
                $"{expectedOpCode.Name} Value type failed");
        Assert.AreEqual(expectedValue, instruction.Value,
            $"{expectedOpCode.Name} Value failed");

        Assert.IsInstanceOfType(instruction.GetOperand(), expectedOperandType,
            $"{expectedOpCode.Name} GetOperand() type failed");
        Assert.AreEqual(expectedOperand, (TOperand)instruction.GetOperand(),
            $"{expectedOpCode.Name} GetOperand() failed");

        if (expectedValue != null)
            Assert.IsInstanceOfType(instruction.GetValue(), expectedValueType,
                $"{expectedOpCode.Name} GetValue() type failed");
        Assert.AreEqual(expectedValue, (TValue)instruction.GetValue(),
            $"{expectedOpCode.Name} GetValue() failed");
    }

    /// <summary>
    /// Test the specified instruction.
    /// </summary>
    /// <param name="instructions">The <see cref="Instruction"/> to test.</param>
    /// <param name="expectedOpCode">The expected <see cref="OpCode"/> for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    /// <returns>The instruction to test.</returns>
    protected static IInstruction TestInstruction(
        MethodInstructionsList instructions,
        OpCode expectedOpCode, string expectedText, int expectedOffset = 0,
        bool expectedIsTarget = false, int expectedCount = 1, int index = 0,
        Type expectedType = null)
    {
        IInstruction instruction = TestInstruction(instructions,
            expectedCount, index, expectedType);
        TestInstruction(instruction, instructions, expectedOpCode, expectedText,
            expectedOffset, expectedIsTarget);
        return instruction;
    }

    /// <summary>
    /// Test the specified instruction.
    /// </summary>
    /// <param name="instructions">The <see cref="Instruction"/> to test.</param>
    /// <param name="expectedCount">The expected number of instructions.</param>
    /// <param name="index">The index of the instruction.</param>
    /// <param name="expectedType">The expected type for the instruction (or null).</param>
    /// <returns>The instruction to test.</returns>
    protected static IInstruction TestInstruction(
        MethodInstructionsList instructions,
        int expectedCount = 1, int index = 0, Type? expectedType = null)
    {
        Assert.IsNotNull(instructions, "instructions failed");

        Assert.AreEqual(expectedCount, instructions.Count, "expectedCount failed");

        IInstruction instruction = instructions.Instructions[index];
        expectedType = expectedType ?? typeof(Instruction);
        var currentType = instruction.GetType();
        Assert.AreSame(expectedType, currentType);
        return instruction;
    }

    /// <summary>
    /// Test the specified instruction.
    /// </summary>
    /// <param name="instruction">The <see cref="IInstruction"/> to test.</param>
    /// <param name="expectedParent">The expected parent <see cref="IInstructionList"/> for the instruction.</param>
    /// <param name="expectedOpCode">The expected <see cref="OpCode"/> for the instruction.</param>
    /// <param name="expectedText">The expected text for the instruction.</param>
    /// <param name="expectedOffset">The zero-based, byte offset for the instruction.</param>
    /// <param name="expectedIsTarget">A value indicating if the instruction is the target of a branch instruction.</param>
    protected static void TestInstruction(
        IInstruction instruction,
        MethodInstructionsList expectedParent, 
        OpCode expectedOpCode, string expectedText,
        int expectedOffset = 0, bool expectedIsTarget = false)
    {
        Assert.IsNotNull(expectedOpCode, "expectedOpCode failed");
        Assert.IsNotNull(expectedText, $"{expectedOpCode.Name} expectedText failed");
        Assert.IsNotNull(instruction, $"{expectedOpCode.Name} instruction failed");

        string label = expectedText.Substring(0, expectedText.IndexOf(':'));

        Assert.AreEqual(expectedOpCode, instruction.OpCode,
            $"{expectedOpCode.Name} expectedOpCode failed");
        Assert.AreEqual(label, instruction.Label,
            $"{expectedOpCode.Name} label failed");
        Assert.AreEqual(expectedOffset, instruction.Offset,
            $"{expectedOpCode.Name} expectedOffset failed");
        Assert.AreEqual(expectedIsTarget, instruction.IsTarget,
            $"{expectedOpCode.Name} expectedIsTarget failed");
        Assert.AreEqual(expectedParent, instruction.Parent,
            $"{expectedOpCode.Name} expectedParent failed");
        Assert.AreEqual(expectedText, instruction.ToString(),
            $"{expectedOpCode.Name} expectedText failed");
    }
}
