namespace Lyt.Reflector.Tests;

[TestClass]
public class ParemeterInstruction_Tests : InstructionHelper
{
    private const string MyMethodName = "MyMethod";
    private const string MyParameterName1 = "myParameter1";
    private const string MyParameterName2 = "myParameter1";
    private const string MyParameterName3 = "myParameter2";
    private const string MyParameterName4 = "myParameter3";

    [TestMethod]
    public void InlineNone_ParameterInstruction() =>
        TestInstructions<byte>(AllOpCodes.Instance.OfType(OperandType.InlineNone)
            .Where(o => o.Name.StartsWith("ldarg")),
            opCode => CreateMethod(il => il.Emit(opCode), true, MyParameterName1,
                MyParameterName2, MyParameterName3, MyParameterName4),
            GetExpectedValue);

    [TestMethod]
    public void InlineVar_ParameterInstruction() =>
        TestInstructions<ushort>(AllOpCodes.Instance.OfType(OperandType.InlineVar)
            .Where(opCode => opCode.Name.Contains("arg")),
            opCode => CreateMethod(il => il.Emit(opCode, (short)0), true, MyParameterName1));

    [TestMethod]
    public void ShortInlineVar_ParameterInstruction() =>
        TestInstructions<byte>(AllOpCodes.Instance.OfType(OperandType.ShortInlineVar)
            .Where(opCode => opCode.Name.Contains("arg")),
            opCode => CreateMethod(il => il.Emit(opCode, (byte)0), true, MyParameterName1));

    [TestMethod]
    public void InlineNone_This_ParameterInstruction()
    {
        TestInstruction(OpCodes.Ldarg_0, opCode => CreateMethod(il => il.Emit(opCode),
            false, MyParameterName1), (byte)0);
        TestInstruction(OpCodes.Ldarg_1, opCode => CreateMethod(il => il.Emit(opCode),
            false, MyParameterName1), (byte)1);
    }

    [TestMethod]
    public void InlineVar_This_ParameterInstruction()
    {
        TestInstruction(OpCodes.Ldarg, opCode => CreateMethod(il => il.Emit(opCode, (short)0),
            false, MyParameterName1), (ushort)0);
        TestInstruction(OpCodes.Ldarg, opCode => CreateMethod(il => il.Emit(opCode, (short)1),
            false, MyParameterName1), (ushort)1);
    }

    [TestMethod]
    public void ShortInlineVar_This_ParameterInstruction()
    {
        TestInstruction(OpCodes.Ldarg_S, opCode => CreateMethod(il => il.Emit(opCode, (byte)0),
            false, MyParameterName1), (byte)0);
        TestInstruction(OpCodes.Ldarg_S, opCode => CreateMethod(il => il.Emit(opCode, (byte)1),
            false, MyParameterName1), (byte)1);
    }

    private MethodInfo? CreateMethod(Action<ILGenerator> addInstructions,
        bool isStatic, params string[] parameterNames)
    {
        var assembly = CreateAssembly(il => il.Emit(OpCodes.Ret), type =>
            {
                var method = type.DefineMethod(MyMethodName, MethodAttributes.Public |
                    (isStatic ? MethodAttributes.Static : 0),
                    isStatic ? CallingConventions.Standard : CallingConventions.HasThis,
                    typeof(void), Enumerable.Repeat(typeof(int), parameterNames.Length)
                    .ToArray());

                for (int index = 0; index < parameterNames.Length; index++)
                    method.DefineParameter(index + 1, ParameterAttributes.None,
                        parameterNames[index]);

                addInstructions(method.GetILGenerator());
            }); 

        var modules = assembly.GetModules();
        var module = modules[0];
        var type = module.GetType(TestTypeName);
        var methodInfo = type.GetMethod(
            MyMethodName, 
            BindingFlags.Public | (isStatic ? BindingFlags.Static : BindingFlags.Instance));
        return methodInfo; 
    } 

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
        Func<OpCode, MethodInfo> createMethod, Func<OpCode, TOperand> getExpectedValue = null)
        where TOperand : struct, IBinaryInteger<TOperand>
    {
        getExpectedValue = getExpectedValue ?? (o => default(TOperand));

        foreach (OpCode opCode in opCodes)
            TestInstruction(opCode, createMethod, getExpectedValue(opCode));
    }

    private void TestInstruction<TOperand>(OpCode opCode,
        Func<OpCode, MethodInfo> createMethod, TOperand expectedValue)
        where TOperand : struct, IBinaryInteger<TOperand>
    {
        var method = createMethod(opCode);
        int parameterIndex = expectedValue.ToInt32();
        var instructions = method.GetIL();

        bool isThis = (method.CallingConvention & CallingConventions.HasThis) != 0 &&
            parameterIndex-- == 0;

        ParameterInfo parameter = isThis ? null : method.GetParameters()[parameterIndex];
        string expectedText;

        if (opCode.OperandType == OperandType.InlineNone)
            expectedText = isThis ? "// this" : $"// {parameter.Name}";
        else
            expectedText = isThis ? $"{expectedValue} // this" : parameter.Name;

        TestInstruction(instructions, opCode, expectedValue,
            parameter, $"IL_0000: {opCode.Name} {expectedText}",
            expectedType: typeof(ParameterInstruction<TOperand>));
    }
}
