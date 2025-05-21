namespace Lyt.Reflector.Tests;

    [TestClass]
public class SignatureInstruction_Tests : InstructionHelper
{
	[TestMethod]
	public void InlineSig_Types_SignatureInstruction() =>
		TestInstruction("int32(bool, uint8, char, valuetype [System.Private.CoreLib]System.DateTime, class [System.Private.CoreLib]System.DBNull, valuetype [System.Private.CoreLib]System.Decimal, float64, int16, int32, int64, object, int8, float32, string, uint16, uint32, uint64, native int, native uint, typedref, class [Lyt.Reflector.Tests]Lyt.Reflector.Tests.SignatureInstruction_Tests)",
			CallingConventions.Standard, typeof(int),
			new Type[] { typeof(bool), typeof(byte), typeof(char), typeof(DateTime),
			typeof(DBNull), typeof(decimal), typeof(double), typeof(short),
			typeof(int), typeof(long), typeof(object), typeof(SByte), typeof(float),
			typeof(string), typeof(ushort), typeof(uint), typeof(ulong),
			typeof(IntPtr), typeof(UIntPtr), typeof(TypedReference),
			typeof(SignatureInstruction_Tests)}, null);

	[TestMethod]
	public void InlineSig_Standard_SignatureInstruction() =>
		TestInstruction("int32(int32)", CallingConventions.Standard,
			typeof(int), new[] { typeof(int) }, null);

	[TestMethod]
	public void InlineSig_Standard_Empty_SignatureInstruction() =>
		TestInstruction("void()", CallingConventions.Standard,
			typeof(void), new Type[0], null);

	[TestMethod]
	public void InlineSig_Standard_Instance_SignatureInstruction() =>
		TestInstruction("instance int32(int32)",
			CallingConventions.Standard | CallingConventions.HasThis,
			typeof(int), new[] { typeof(int) }, null);

	[TestMethod]
	public void InlineSig_VarArgs_SignatureInstruction() =>
		TestInstruction("vararg int32(int32, ..., int32)",
			CallingConventions.VarArgs,
			typeof(int), new[] { typeof(int) }, new[] { typeof(int) });

	[TestMethod]
	public void InlineSig_VarArgs_Empty_SignatureInstruction() =>
		TestInstruction("vararg void()",
			CallingConventions.VarArgs,
			typeof(void), new Type[0], new Type[0]);

	[TestMethod]
	public void InlineSig_VarArgs_Instance_SignatureInstruction() =>
		TestInstruction("instance vararg int32(int32, ..., int32)",
			CallingConventions.VarArgs | CallingConventions.HasThis,
			typeof(int), new[] { typeof(int) }, new[] { typeof(int) });

	[TestMethod]
	public void InlineSig_VarArgs_Optional_SignatureInstruction() =>
		TestInstruction("vararg int32(..., int32)",
			CallingConventions.VarArgs,
			typeof(int), new Type[0], new[] { typeof(int) });

	[TestMethod]
	public void InlineSig_Cdecl_SignatureInstruction() =>
		TestInstruction("unmanaged cdecl int32(int32)",
			CallingConvention.Cdecl,
			typeof(int), new[] { typeof(int) });

	[TestMethod]
	public void InlineSig_Fastcall_SignatureInstruction() =>
		TestInstruction("unmanaged fastcall int32(int32)",
			CallingConvention.FastCall,
			typeof(int), new[] { typeof(int) });

	[TestMethod]
	public void InlineSig_Stdcall_SignatureInstruction() =>
		TestInstruction("unmanaged stdcall int32(int32)",
			CallingConvention.StdCall,
			typeof(int), new[] { typeof(int) });

	[TestMethod]
	public void InlineSig_Thiscall_SignatureInstruction() =>
		TestInstruction("unmanaged thiscall int32(int32)",
			CallingConvention.ThisCall,
			typeof(int), new[] { typeof(int) });

	private void TestInstruction(string expectedText,
		CallingConvention convention, Type returnType, Type[] requiredParameters = null) =>
		TestInstruction(expectedText, true, convention, 0, returnType,
			requiredParameters, null);

	private void TestInstruction(string expectedText,
		CallingConventions conventions, Type returnType, Type[] requiredParameters = null,
		Type[] optionalParameters = null) =>
		TestInstruction(expectedText, false, 0, conventions, returnType,
			requiredParameters, optionalParameters);

	private void TestInstruction(string expectedText, bool isUnmanaged,
		CallingConvention convention, CallingConventions conventions,
		Type returnType, Type[] requiredParameters, Type[] optionalParameters)
	{
		OpCode opCode = OpCodes.Calli;

		var method = CreateMethod(il =>
		{
			if (isUnmanaged)
				il.EmitCalli(opCode, convention, returnType, requiredParameters);
			else
				il.EmitCalli(opCode, conventions, returnType, requiredParameters,
					optionalParameters);
		});

		Token expectedToken = new Token(TokenType.Signature, 1);
		byte[] expectedSignature = method.Module.ResolveSignature(expectedToken.Value);
		Assert.IsNotNull(expectedSignature, "ResolveSignature() failed");

		var instructions = method.GetIL();
		Assert.IsNotNull(instructions, "GetIL() failed");

		var instruction = TestInstruction(instructions, opCode,
			expectedText: $"IL_0000: {opCode.Name} {expectedText}",
			expectedType: typeof(SignatureInstruction)) as SignatureInstruction;
		Assert.IsNotNull(instruction, "Instruction failed");

		Assert.AreEqual(expectedToken, instruction.Operand, "Operand failed");
		Assert.AreEqual(expectedToken, instruction.GetOperand(), "GetOperand() failed");

		Assert.IsNotNull(instruction.Value, "Value failed");
		Assert.IsNotNull(instruction.Value.Data, "Value.Data failed");
		Assert.AreEqual(instruction.Value, instruction.GetValue(), "GetValue() failed");

		MethodSignature signature = instruction.Value;
		Assert.AreEqual(convention, signature.CallingConvention, "CallingConvention failed");
		Assert.AreEqual(conventions, signature.CallingConventions, "CallingConventions failed");
		Assert.AreEqual(isUnmanaged, signature.IsUnmanaged, "IsUnmanaged failed");

		requiredParameters = requiredParameters ?? new Type[0];
		optionalParameters = optionalParameters ?? new Type[0];

		CollectionAssert.AreEqual(expectedSignature, signature.Data.ToList(),
			"Value.Data failed");
		CollectionAssert.AreEqual(requiredParameters, signature.RequiredParameters.ToList(),
			"Value.RequiredParameters failed");
		CollectionAssert.AreEqual(optionalParameters, signature.OptionalParameters.ToList(),
			"Value.OptionalParameters failed");
	}
}
