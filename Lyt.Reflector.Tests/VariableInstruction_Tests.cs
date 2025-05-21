namespace Lyt.Reflector.Tests;

    [TestClass]
public class VariableInstruction_Tests : InstructionHelper
{
    [TestMethod]
    public void InlineNone_VariableInstruction() =>
        TestInstructions<byte>(AllOpCodes.Instance
            .OfType(OperandType.InlineNone)
            .Where(o => o.Name.StartsWith("ldloc") ||
                    o.Name.StartsWith("stloc")),
            (opCode, il) => il.Emit(opCode),
            GetExpectedValue);

    [TestMethod]
    public void InlineVar_VariableInstruction() =>
        TestInstructions<ushort>(AllOpCodes.Instance
            .OfType(OperandType.InlineVar)
            .Where(o => !o.Name.Contains("arg")),
            (opCode, il) => il.Emit(opCode, (short)0));

    [TestMethod]
    public void ShortInlineVar_VariableInstruction() =>
        TestInstructions<byte>(AllOpCodes.Instance
            .OfType(OperandType.ShortInlineVar)
            .Where(o => !o.Name.Contains("arg")),
            (opCode, il) => il.Emit(opCode, (byte)0));

    private byte GetExpectedValue(OpCode opCode)
    {
        string name = opCode.Name;
        int index = name.LastIndexOf('.');
        if (index < 0 || index >= name.Length - 1)
            return 0;

        return byte.TryParse(name.Substring(index + 1), out byte result) ?
            result : (byte)0;
    }

    private void TestInstructions<TOperand>(IEnumerable<OpCode> opCodes,
        Action<OpCode, ILGenerator> addInstructions, 
        Func<OpCode, TOperand> getExpectedValue = null)
        where TOperand : struct, IComparable, IFormattable, IConvertible,
            IComparable<TOperand>, IEquatable<TOperand>
    {
        getExpectedValue = getExpectedValue ?? (o => default(TOperand));

        foreach (OpCode opCode in opCodes)
        {
            TOperand expectedValue = getExpectedValue(opCode);
            TestInstruction(opCode, addInstructions, expectedValue);
        }
    }

    private void TestInstruction<TOperand>(OpCode opCode, 
        Action<OpCode, ILGenerator> addInstructions, TOperand expectedValue)
        where TOperand : struct, IComparable, IFormattable, IConvertible,
            IComparable<TOperand>, IEquatable<TOperand>
    {
        var method = CreateMethod(il =>
        {
            il.DeclareLocal(typeof(int)); // V_0
            il.DeclareLocal(typeof(int)); // V_1
            il.DeclareLocal(typeof(int)); // V_2
            il.DeclareLocal(typeof(int)); // V_3
            addInstructions(opCode, il);
        });

        int variableIndex = expectedValue.ToInt32(null);
        var instructions = method.GetIL();

        LocalVariableInfo variable = instructions.MethodBody.LocalVariables[variableIndex];
        string expectedText = opCode.OperandType == OperandType.InlineNone ?
            $"// V_{variableIndex} int32" : $"V_{variableIndex} // int32";

        TestInstruction(instructions, opCode, expectedValue,
            variable, $"IL_0000: {opCode.Name} {expectedText}",
            expectedType: typeof(VariableInstruction<TOperand>));
    }
}
