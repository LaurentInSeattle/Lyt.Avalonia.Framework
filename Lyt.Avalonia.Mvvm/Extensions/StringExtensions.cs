namespace Lyt.Avalonia.Mvvm.Extensions;

public static class StringExtensions
{
    public static string Capitalize(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }

        return string.Concat(char.ToUpper(str[0]), str[1..]);
    }

    public static TextInfo? UsaTextInfo;

    public static string ToTitleCase(this string phrase)
    {
        StringExtensions.UsaTextInfo ??= new CultureInfo("en-US", false).TextInfo;
        return StringExtensions.UsaTextInfo.ToTitleCase(phrase);
    }

    public static string ToFancyString<T>(this T x)
    {
        string? ts = x?.ToString();
        if (string.IsNullOrEmpty(ts))
        {
            return string.Empty; 
        }

        string y = ts.Wordify().Trim();
        return y.Replace("  ", " ");
    }

    public static string BeautifyEnumString(this string enumString)
    {
        string eString = string.Empty;
        if (enumString.Contains("_", StringComparison.InvariantCultureIgnoreCase))
        {
            eString = enumString.ToLower().Replace("_", " ").ToTitleCase();
            eString.Capitalize().Wordify();
        }
        else
        {
            eString = enumString.Capitalize().Wordify();
        }

        return eString; 
    }

    public static string BeautifyEnumString(this object value, string prefixTrim = "", string postfixTrim = "" )
    {
        if (!value.GetType().IsEnum)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            return value.ToString()!;
        }

        string enumString = value.ToString()!;
        if (!string.IsNullOrWhiteSpace(prefixTrim) &&
            enumString.StartsWith(prefixTrim, StringComparison.InvariantCultureIgnoreCase))
        {
            enumString = enumString[prefixTrim.Length..];
        }

        if (!string.IsNullOrWhiteSpace(postfixTrim) &&
            enumString.EndsWith(postfixTrim, StringComparison.InvariantCultureIgnoreCase))
        {
            enumString = enumString[0..(enumString.Length - postfixTrim.Length)];
        }

        return enumString.BeautifyEnumString();
    }

    private static Regex? WordifyingRegex;

    /// <summary>
    /// Add spaces to separate the capitalized words in the string,  i.e. insert a space before
    /// each uppercase letter that is  either preceded by a lowercase letter or followed by a
    /// lowercase letter (but not for the first char in string).  This keeps groups of uppercase
    /// letters - e.g. acronyms - together.
    /// </summary>
    /// <param name="pascalCaseString">A string in PascalCase</param>
    /// <returns>A wordified string.</returns>
    public static string Wordify(this string pascalCaseString)
    {
        StringExtensions.WordifyingRegex ??= new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z])");
        if (string.IsNullOrWhiteSpace(pascalCaseString))
        {
            return string.Empty;
        }

        return StringExtensions.WordifyingRegex.Replace(pascalCaseString, " ${x}");
    }

    public static string ToMonthOrdinal(this int value)
    {
        // 1, 21, 31 get 'st'
        // 2, 22 get 'nd'
        // 3, 23 get 'rd'
        // all other get 'th'

        // ᵃ =0x1d43 ᵇ=0x1d47 ᶜ=0x1d9c ᵈ=0x1d48 ᵉ=0x1d49 ᶠ=0x1da0 ᵍ=0x1d4d ʰ=0x2b0 ⁱ=0x2071 ʲ=0x2b2
        // ᵏ =0x1d4f  ˡ=0x2e1 ᵐ=0x1d50 ⁿ=0x207f ᵒ=0x1d52 ᵖ=0x1d56  ʳ=0x2b3 ˢ=0x2e2 ᵗ=0x1d57 ᵘ=0x1d58
        // ᵛ =0x1d5b  ʷ=0x2b7  ˣ=0x2e3  ʸ=0x2b8 ᶻ=0x1dbb

        string result = value.ToString();
        string appendage;
        if ((value == 1) || (value == 21) || (value == 31))
        {
            appendage = "\u02E2\u1D57";
        }
        else if ((value == 2) || (value == 22))
        {
            appendage = "\u207F\u1D48";
        }
        else if ((value == 3) || (value == 23))
        {
            appendage = "\u02B3\u1D48";
        }
        else
        {
            appendage = "\u1D57\u02B0";
        }

        return string.Concat(result, appendage);
    }

    public static string FancyConcat(this IList<string> s)
    {
        int count = s.Count;
        if (count == 1)
        {
            return s[0];
        }
        else if (count == 2)
        {
            return string.Concat(s[0], " and ", s[1]);
        }
        else if (count > 5)
        {
            return string.Format("{0}, {1} and {2} more.", s[0], s[1], count - 2);
        }

        var sb = new StringBuilder(1024);
        for (int i = 0; i < count; ++i)
        {
            _ = sb.Append(s[i]);
            if (i < count - 2)
            {
                _ = sb.Append(", ");
            }
            else if (i == count - 2)
            {
                _ = sb.Append(" and ");
            }
            else
            {
                // last: add nothing
            }
        }

        return sb.ToString();
    }
}
