using Lyt.Avalonia.Interfaces.Localization;

namespace Lyt.Validation.Validators;

public static partial class Validators
{
    private static readonly Dictionary<Type, object> validators = new()
    {
        { typeof(Email), new Email() },
        { typeof(Password), new Password() },
    };

    public static ILocalizer? Localizer { get; private set; }

    public static bool IsValid<TValidator, TType>(this TType toValidate, out string message)
        where TValidator : AbstractValidator<TType>
        //where TType : class
    {
        if (!validators.TryGetValue(typeof(TValidator), out object? maybeValidator))
        {
            throw new Exception("No validator for type: " + typeof(TValidator).FullName);
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

    // Duplicated to avoid referencing another assembly 
    public static bool Is<T>(this Type type) => typeof(T) == type;

    public static bool DerivesFrom<TBase>(this Type type)
        where TBase : class
        => typeof(TBase).IsAssignableFrom(type);

    public static bool TryParse<T>(this string s, out T? value, IFormatProvider? provider = null) 
        where T : IParsable<T>
        => TryParse<T>(s, out value, provider);
}

public static partial class Validators
{
    public class Email : AbstractValidator<string>
    {
        public Email()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("Your email cannot be empty")
                .EmailAddress().WithMessage("This email is invalid or malformed.");
        }
    }

    public class Password : AbstractValidator<string>
    {
        public Password()
        {
            this.RuleFor(x => x)
                .NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(10).WithMessage("Your password length must be at least 10.")
                .MaximumLength(20).WithMessage("Your password length must not exceed 20.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
        }
    }
}
