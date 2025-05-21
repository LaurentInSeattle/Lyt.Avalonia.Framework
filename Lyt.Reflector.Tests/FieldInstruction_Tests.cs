namespace Lyt.Reflector.Tests;

[TestClass]
public class FieldInstruction_Tests : InstructionHelper
{
	private const string TestFieldName = "TestField";

	[TestMethod]
	public void InlineField_FieldInstruction() =>
		TestInstructions(AllOpCodes.Instance.OfType(OperandType.InlineField));

	[TestMethod]
	public void InlineTok_FieldInstruction() =>
		TestInstructions(AllOpCodes.Instance.OfType(OperandType.InlineTok));

	private void TestInstructions(IEnumerable<OpCode> opCodes)
	{
		string expectedText = $"int32 {TestTypeName}::{TestFieldName}";

		foreach (OpCode opCode in opCodes)
			TestInstruction(opCode, typeof(int), expectedText,
				opCode.OperandType == OperandType.InlineTok ? "field " : string.Empty);
	}

	private void TestInstruction(OpCode opCode, Type fieldType, string expectedText,
		string prefix)
	{
		Token expectedToken = Token.Empty;

		MethodInfo method = CreateMethod(il =>
		{
			il.Emit(opCode, expectedToken.Value);
		}, type =>
		{
			var fieldBuilder = type.DefineField(TestFieldName, fieldType,
				FieldAttributes.Public | FieldAttributes.Static); 
            expectedToken = new Token(fieldBuilder.MetadataToken);
		});

		FieldInfo field = method.Module.ResolveField(expectedToken.Value,
			method.DeclaringType.GetGenericArguments(),
			method.GetGenericArguments());

		Assert.AreNotEqual(-1, expectedToken.Value);
		Assert.IsNotNull(field);
		Assert.AreEqual(TestFieldName, field.Name);

		TestInstruction(method.GetIL(), opCode, expectedToken,
			field, $"IL_0000: {opCode.Name} {prefix}{expectedText}",
			expectedType: typeof(FieldInstruction));
	}
}
