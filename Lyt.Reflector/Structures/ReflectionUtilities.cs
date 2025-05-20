namespace Lyt.Reflector.Structures;

public static class ReflectionUtilities
{
    public static readonly List<string> IgnorableMethodNames =
        [
            "ToString",
            "GetHashCode",
            "Equals",
            "Deconstruct",
            "GetType",
            "Finalize",
        ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32<T>(this T operand) => Convert.ToInt32(operand);

    public static List<string> GetExcludedNamespaces() => ReflectionUtilities.excludedNamespaces;

    private static List<string> excludedNamespaces = [];

    public static void SetExcludedNamespaces(List<string> excludedNamespaces)
        => ReflectionUtilities.excludedNamespaces = excludedNamespaces;

    public static Tuple<bool, List<Type>> Analyse(this Type sourceType)
    {
        List<Type> dependantTypes = [];
        var ignoreType = Tuple.Create(false, new List<Type>());
        var relevantType = Tuple.Create(true, dependantTypes);
        if (sourceType.IsPrimitive)
        {
            // Primitive types do not create dependencies, we ignore them 
            return ignoreType;
        }

        void RecurseAnalyseType(Type type)
        {
            if (type.IsPrimitive)
            {
                // Primitive types do not create dependencies, we ignore them 
                return;
            }

            if (type.IsArray)
            {
                Type? elementType = type.GetElementType();
                if (elementType is not null)
                {
                    RecurseAnalyseType(elementType);
                }

                return;
            }
            else if (type.IsGenericType)
            {
                if (!type.ShouldBeIgnored())
                {
                    dependantTypes.Add(type);
                }

                Type[] typeParameters = type.GetGenericArguments();
                foreach (Type typeParameter in typeParameters)
                {
                    if (typeParameter.ShouldBeIgnored() || (typeParameter == sourceType))
                    {
                        // Example : 
                        // Class: Lyt.Avalonia.Controls.Progress.ProgressRing
                        //   Static Field: MaxSideLengthProperty
                        //   Type: Avalonia.DirectProperty`2[Lyt.Avalonia.Controls.Progress.ProgressRing, System.Double]
                        continue;
                    }

                    RecurseAnalyseType(typeParameter);
                }

                return;
            }
        }

        RecurseAnalyseType(sourceType);

        if (sourceType.ShouldBeIgnored())
        {
            return ignoreType;
        }

        if (dependantTypes.Count == 0)
        {
            // Example: Static Field: Throw Type: System.Action`1[System.Exception]
            if (sourceType.HasExcludedNamespace())
            {
                // Dependency to an ignore assembly: we can ignore 
                return ignoreType;
            }
        }

        // No dependant types, but still relevant by itself 
        return relevantType;
    }

    public static Tuple<bool, List<Type>> Analyse(this MethodBody methodBody, Module module)
    {
        List<Type> dependantTypes = [];
        var ignoreType = Tuple.Create(false, new List<Type>());
        var relevantType = Tuple.Create(true, dependantTypes);

        // Find dependant types in local variables 
        var localVariables = methodBody.LocalVariables;
        foreach (var variable in localVariables)
        {
            Type type = variable.LocalType;

            // TODO ! 
        }

        // Analyse IL to figure out external calls 
        // First we need to extract out the raw IL
        byte[]? il = methodBody.GetILAsByteArray();
        if (il is not null)
        {
            // TODO 
            il.Analyse(module); 
        }


        return ignoreType;
    }

    public static bool ShouldBeIgnored(this Type type)
    {
        // IsPrimitive: The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64,
        // UInt64, IntPtr, UIntPtr, Char, Double, and Single.
        // If the current Type represents a generic type, or a type parameter in the definition of a generic type
        // or generic method, this IsPrimitive always returns false.
        // Primitive types do not create dependencies, we ignore them 
        //
        // HasNameWithSpecialCharacters: Most likely Compiler or tool generated 
        //
        // HasExcludedNamespace: Dependency to an ignore assembly: we can ignore 
        if (type.IsPrimitive || type.HasNameWithSpecialCharacters() || type.HasExcludedNamespace())
        {
            return true;
        }

        return false;
    }

    public static bool IsCompilerGenerated(this Type type)
    {
        if (type.GetCustomAttribute<CompilerGeneratedAttribute>() is not null ||
             type.GetCustomAttribute<GeneratedCodeAttribute>() is not null)
        {
            return true;
        }

        while (type.DeclaringType is Type declaringType)
        {
            if (declaringType.GetCustomAttribute<CompilerGeneratedAttribute>() is not null ||
                declaringType.GetCustomAttribute<GeneratedCodeAttribute>() is not null)
            {
                return true;
            }

            type = declaringType;
        }

        return false;
    }

    public static bool HasNoSafeFullName(this Type type)
        => type.FullName is null && !type.IsGenericType;

    public static string SafeFullName(this Type type)
    {
        // FullName return null if the current instance represents a generic type parameter, an array
        // type based on a type parameter, pointer type based on a type parameter, or byreftype based
        // on a type parameter, or a generic type that is not a generic type definition but contains
        // unresolved type parameters.
        // See:
        // https://stackoverflow.com/questions/34670901/in-c-when-does-type-fullname-return-null

        if (type.IsGenericType)
        {
            var assemblyName = type.Assembly.GetName();
            return string.Concat(assemblyName.Name, ".", type.Name);
        }

        if (type.FullName is null)
        {
            // Did you invoke HasNoSafeFullName ? 
            Debugger.Break();
            throw new Exception("Invoke HasNoSafeFullName");
        }

        return type.FullName;
    }

    public static bool HasNameWithSpecialCharacters(this Type type)
    {
        char[] specialChars = ['<', '!', '>', '+',];
        foreach (char special in specialChars)
        {
            if (type.Name.Contains(special))
            {
                // Computer generated class 
                Debug.WriteLine("Excluded: " + special + "  " + type.ToString());
                return true;
            }
        }

        if (!type.HasNoSafeFullName())
        {
            string fullName = type.SafeFullName();
            foreach (char special in specialChars)
            {
                if (fullName.Contains(special))
                {
                    // Computer generated class 
                    Debug.WriteLine("Excluded: " + special + "  " + type.ToString());
                    return true;
                }
            }
        }

        return false;
    }

    public static bool HasExcludedNamespace(this Type type)
    {
        if (type.HasNoSafeFullName())
        {
            return true;
        }

        string safeFullName = type.SafeFullName();
        return IsExcludedNamespace(safeFullName);
    }

    public static bool IsExcludedNamespace(this string namespaceString)
    {
        foreach (string excluded in ReflectionUtilities.excludedNamespaces)
        {
            if (namespaceString.StartsWith(excluded, StringComparison.InvariantCultureIgnoreCase))
            {
                // System assembly or spec'd to skip (Ex: Avalonia.) 
                return true;
            }
        }

        return false;
    }

    public static bool IsIgnorableMethodName(this string methodName)
    {
        foreach (string excluded in ReflectionUtilities.IgnorableMethodNames)
        {
            if (methodName.StartsWith(excluded, StringComparison.InvariantCultureIgnoreCase))
            {
                // Low level methods  
                return true;
            }
        }

        return false;
    }

    public static bool IsCompilerGenerated(this string methodName)
        // get or set: Parts of a property: ignore
        // op : Operators 
        // add remove : Events  
        // <clone> : ??? 
        => methodName.StartsWith("get_") ||
           methodName.StartsWith("set_") ||
           methodName.StartsWith("op_") ||
           methodName.StartsWith("add_") ||
           methodName.StartsWith("remove_") ||
           methodName.StartsWith("<Clone>");

    public static string Analyse(this byte[] il, Module module)
    {
        // For aggregating our response
        var sb = new StringBuilder();
        // We'll also need a full set of the IL opcodes so we
        // can remap them over our method body
        var opCodes = typeof(OpCodes)
            .GetFields()
            .Select(fi => (OpCode)fi.GetValue(null));

        //opCodes.Dump();

        // For each byte in our method body, try to match it to an opcode
        var mappedIL = il.Select(op =>
            opCodes.FirstOrDefault(opCode => opCode.Value == op));

        // OpCode/Operand parsing: 
        //     Some opcodes have no operands, some use ints, etc. 
        //  let's try to cover all cases
        var ilWalker = mappedIL.GetEnumerator();
        while (ilWalker.MoveNext())
        {
            var mappedOp = ilWalker.Current;
            if (mappedOp.OperandType != OperandType.InlineNone)
            {
                // For operand inference:
                // MOST operands are 32 bit, 
                // so we'll start there
                int byteCount = 4;
                long operand = 0;
                string token = string.Empty;

                // For metadata token resolution            
                Func<int, string> tokenResolver = tkn => string.Empty;
                switch (mappedOp.OperandType)
                {
                    // These are all 32bit metadata tokens
                    case OperandType.InlineMethod:
                        tokenResolver = tkn =>
                        {
                            var resMethod = module.SafeResolveMethod((int)tkn);
                            return string.Format("({0}())", resMethod == null ? "unknown" : resMethod.Name);
                        };
                        break;
                    case OperandType.InlineField:
                        tokenResolver = tkn =>
                        {
                            var field = module.SafeResolveField((int)tkn);
                            return string.Format("({0})", field == null ? "unknown" : field.Name);
                        };
                        break;
                    case OperandType.InlineSig:
                        tokenResolver = tkn =>
                        {
                            byte[]? sigBytes = module.SafeResolveSignature((int)tkn);
                            string catSig = string.Join(",", sigBytes);
                            return string.Format("(SIG:{0})", catSig == null ? "unknown" : catSig);
                        };
                        break;
                    case OperandType.InlineString:
                        tokenResolver = tkn =>
                        {
                            string? str = module.SafeResolveString((int)tkn);
                            return string.Format("('{0}')", str == null ? "unknown" : str);
                        };
                        break;
                    case OperandType.InlineType:
                        tokenResolver = tkn =>
                        {
                            var type = module.SafeResolveType((int)tkn);
                            return string.Format("(typeof({0}))", type == null ? "unknown" : type.Name);
                        };
                        break;
                    // These are plain old 32bit operands
                    case OperandType.InlineI:
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineSwitch:
                    case OperandType.ShortInlineR:
                        break;
                    // These are 64bit operands
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        byteCount = 8;
                        break;
                    // These are all 8bit values
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                        byteCount = 1;
                        break;
                }
                // Based on byte count, pull out the full operand
                for (int i = 0; i < byteCount; i++)
                {
                    ilWalker.MoveNext();
                    operand |= ((long)ilWalker.Current.Value) << (8 * i);
                }

                string? resolved = tokenResolver((int)operand);
                resolved = string.IsNullOrEmpty(resolved) ? operand.ToString() : resolved;
                sb.AppendFormat("{0} {1}",
                        mappedOp.Name,
                        resolved)
                    .AppendLine();
            }
            else
            {
                sb.AppendLine(mappedOp.Name);
            }
        }

        return sb.ToString();
    }
}