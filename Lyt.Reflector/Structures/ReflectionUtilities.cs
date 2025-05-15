namespace Lyt.Reflector.Structures;

public static class ReflectionUtilities
{
    private static List<string> excludedNamespaces = [];

    public static void SetExcludedNamespaces(List<string> excludedNamespaces)
        => ReflectionUtilities.excludedNamespaces = excludedNamespaces;

    public static List<string> SetExcludedNamespaces() => ReflectionUtilities.excludedNamespaces ;

    public static Tuple<bool, List<Type>> AnalyseType(this Type type)
    {
        // ==> BROKEN : Should recurse into Generic types 
        // 
        // Should be able to handle stuff like List<Tuple<bool,StuffClass>> and return StuffClass
        // 

        List<Type> dependantTypes = [];
        var ignoreType = Tuple.Create(false, new List<Type>());
        var relevantType = Tuple.Create(true, dependantTypes);
        if (type.IsPrimitive)
        {
            // Primitive types do not create dependencies, we ignore them 
            return ignoreType;
        }

        if (type.IsGenericType)
        {
            // Need to check generic types 
            Type[] typeParameters = type.GetGenericArguments();
            foreach (Type typeParameter in typeParameters)
            {
                if (typeParameter.ShouldBeIgnored() || (typeParameter == type) )
                {
                    // Example : 
                    // Class: Lyt.Avalonia.Controls.Progress.ProgressRing
                    //   Static Field: MaxSideLengthProperty
                    //   Type: Avalonia.DirectProperty`2[Lyt.Avalonia.Controls.Progress.ProgressRing, System.Double]
                    continue;
                }

                dependantTypes.Add(typeParameter);
            }

            if (dependantTypes.Count == 0)
            {
                // Example: Static Field: Throw Type: System.Action`1[System.Exception]
                if (type.HasExcludedNamespace())
                {
                    // Dependency to an ignore assembly: we can ignore 
                    return ignoreType;
                }
            }

            return relevantType;
        }
        else
        {
            if (type.ShouldBeIgnored())
            {
                return ignoreType;
            }

            // No dependant types, but still relevant by itself 
            return relevantType;
        }
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
        if (type.IsPrimitive  ||  type.HasNameWithSpecialCharacters() || type.HasExcludedNamespace())
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
        => type.FullName is null && !type.IsGenericType ; 

    public static string SafeFullName (this Type type)
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
            return string.Concat( assemblyName.Name , "." , type.Name );
        }

        if (type.FullName is null)
        {
            // Did you invoke HasNoSafeFullName ? 
            Debugger.Break();
            throw new Exception("Invoke HasNoSafeFullName");
        }

        return type.FullName; 
    }

    public static bool HasNameWithSpecialCharacters( this Type type )
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

    public static bool HasExcludedNamespace (this Type type)
    {
        if ( type.HasNoSafeFullName())
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
}