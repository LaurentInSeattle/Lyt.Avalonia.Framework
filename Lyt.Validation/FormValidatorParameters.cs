namespace Lyt.Validation;

public sealed record class FormValidatorResults<T>
    (
        T? Value = default,
        bool IsValid = false,
        bool HasValue = false,
        string Message = ""
    ) where T : class, new();

public sealed record class FormValidatorParameters<T>
    (
        IEnumerable<FieldValidator> FieldValidators,
        AbstractValidator<T>? FormValidator = null,
        string MessagePropertyName = "",
        string FormValidPropertyName = "", 
        string FocusFieldName = ""
    );
