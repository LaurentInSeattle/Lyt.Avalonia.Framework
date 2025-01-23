namespace Lyt.Validation;

public sealed record class FieldValidatorParameters<T>
    (
        string SourcePropertyName,
        AbstractValidator<T> Validator,
        bool AllowEmpty = false,
        string MessagePropertyName = "",
        string EmptyFieldMessage = "",
        string FailedToParseMessage = ""
    );

