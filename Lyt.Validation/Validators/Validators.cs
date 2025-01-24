namespace Lyt.Validation.Validators;

// TODO: Build that as a Service 
public static partial class Validators
{
    // TODO: Allow apps to populate this 
    // CONSIDER: Provide basic validators based on fluent 
    private static readonly Dictionary<Type, object> validators = [];

    public static ILocalizer? Localizer { get; private set; }

    public static bool IsValid<TValidator, TType>(this TType toValidate, out string message)
        where TValidator : AbstractValidator<TType>
    {
        if (!validators.TryGetValue(typeof(TValidator), out object? maybeValidator))
        {
            throw new Exception("No validator for type: " + typeof(TValidator).FullName);
        }

        return maybeValidator.IsValid(toValidate, out message);
    }

    public static bool IsValid<TValidator, TType>(this TValidator maybeValidator, TType toValidate, out string message)
    {
        if (maybeValidator is null)
        {
            throw new Exception("Null validator");
        }

        if (!maybeValidator.GetType().DerivesFrom<AbstractValidator<TType>>())
        {
            throw new Exception("Invalid validator type: " + typeof(TType).FullName);
        }

        if (maybeValidator is not AbstractValidator<TType> validator)
        {
            throw new Exception("Null validator");
        }

        message = string.Empty;
        var result = validator.Validate(toValidate);
        if (!result.IsValid && result.Errors.Count > 0)
        {
            var firstError = result.Errors[0];
            message = firstError.ErrorMessage;
        }

        return result.IsValid;
    }

    public static string ShowValidationMessage(this Bindable viewModel, string? messagePropertyName, string message)
    {
        if (!string.IsNullOrWhiteSpace(messagePropertyName) &&
            !string.IsNullOrWhiteSpace(message))
        {
            // Localize message if a localizer is available 
            var localizer = Localizer;
            if (localizer is not null)
            {
                message = localizer.Lookup(message);
            }

            // Set property: value comes first for Set
            viewModel.Set<string>(message, messagePropertyName);
        }

        return message;
    }

    public static void ClearValidationMessage(this Bindable viewModel, string? messagePropertyName)
    {
        if (!string.IsNullOrWhiteSpace(messagePropertyName))
        {
            // Nothing to Localize, Set property: value comes first for Set
            viewModel.Set<string>(string.Empty, messagePropertyName);
        }
    }

    // Duplicated to avoid referencing another assembly 
    public static bool Is<T>(this Type type) => typeof(T) == type;

    public static bool DerivesFrom<TBase>(this Type type)
        where TBase : class
        => typeof(TBase).IsAssignableFrom(type);

    public static bool TryParse<T>(this string s, out T? value, IFormatProvider? provider = null)
        where T : IParsable<T>
        => TryParse<T>(s, out value, provider);

    public static void InvokeSetProperty(this object target, string propertyName, object? value)
    {
        var propertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return;
        }

        var methodInfo = propertyInfo.GetSetMethod();
        if (methodInfo is null)
        {
            return;
        }

        methodInfo.Invoke(target, [value]);
    }

    public static object? InvokeGetProperty(this object target, string propertyName)
    {
        var propertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo is null)
        {
            return null;
        }

        var methodInfo = propertyInfo.GetGetMethod();
        if (methodInfo is null)
        {
            return null;
        }

        return methodInfo.Invoke(target, null);
    }
}
