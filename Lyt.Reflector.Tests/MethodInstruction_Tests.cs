namespace Lyt.Reflector.Tests;

    [TestClass]
public class MethodInstruction_Tests : InstructionHelper
{
	[TestMethod]
	public void InlineTok_MethodInstruction()
	{
		foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.InlineTok))
			TestInstruction(opCode);
	}

	[TestMethod]
	public void InlineMethod_MethodInstruction()
	{
		foreach (OpCode opCode in AllOpCodes.Instance.OfType(OperandType.InlineMethod))
			TestInstruction(opCode);
	}

	private void TestInstruction(OpCode opCode)
	{
		Type type = typeof(SampleMethods<int>);
		string typeName = $"[{type.Assembly.GetName().Name}]{type.Namespace}.{type.Name}<int32>";
		string prefix = opCode.OperandType == OperandType.InlineTok ? "method " : "";

		TestInstruction(opCode, "Static", $"{prefix}int32 {typeName}::Static(string)");
		TestInstruction(opCode, "Instance", $"{prefix}instance int32 {typeName}::Instance(string)");
		TestInstruction(opCode, "Generic", $"{prefix}instance string {typeName}::Generic<string>(int32, string)");
	}

	private void TestInstruction(OpCode opCode, string methodName, string expectedText)
	{
		MethodInfo method = typeof(SampleMethods<int>).GetMethod(methodName,
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
		if (method.IsGenericMethod)
			method = method.MakeGenericMethod(typeof(string));

		TestInstruction(opCode, method, expectedText);
	}

	private Token GetToken(MethodInfo method, MethodInfo value)
	{
		Type[] genericTypeArguments = method.DeclaringType.GetGenericArguments();
		Type[] methodTypeArguments = method.GetGenericArguments();
		Module module = method.Module;

		Token result = GetToken(method, TokenType.MethodDef, value,
				(token, m, v) => module.ResolveMethod(token.Value, genericTypeArguments,
						methodTypeArguments) == value);
		if (!result.Equals(Token.Empty))
			return result;

		result = GetToken(method, TokenType.MethodSpec, value,
				(token, m, v) => module.ResolveMethod(token.Value, genericTypeArguments,
						methodTypeArguments) == value);
		if (!result.Equals(Token.Empty))
			return result;

		result = GetToken(method, TokenType.MemberRef, value,
				(token, m, v) => module.ResolveMember(token.Value, genericTypeArguments,
						methodTypeArguments) == value);
		if (!result.Equals(Token.Empty))
			return result;

		throw new ArgumentException(nameof(value));
	}

	private void TestInstruction(OpCode opCode, MethodInfo method, string expectedText)
	{
		MethodInfo containerMethod = CreateMethod(il => il.Emit(opCode, method));
		var instructions = containerMethod.GetIL();

		Token expectedToken = GetToken(containerMethod, method);

		TestInstruction(instructions, opCode, expectedToken,
			(MethodBase)method, $"IL_0000: {opCode.Name} {expectedText}",
			expectedType: typeof(MethodInstruction));
	}
}

public class SampleMethods<TValue1>
{
	public static int Static(string text) => 123;

	public int Instance(string text) => 123;

	public TValue2 Generic<TValue2>(TValue1 value1, TValue2 value2) => value2;
}
