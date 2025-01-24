namespace Lyt.Validation;

public record class FieldValidatorParameters
(
    string SourcePropertyName,
    bool AllowEmpty = false,
    string MessagePropertyName = "",
    string EmptyFieldMessage = "",
    string FailedToParseMessage = ""
);

public sealed record class FieldValidatorParameters<T>
(
    string SourcePropertyName,
    AbstractValidator<T>? Validator = null,
    bool AllowEmpty = false,
    string MessagePropertyName = "",
    string EmptyFieldMessage = "",
    string FailedToParseMessage = ""
) : FieldValidatorParameters(
    SourcePropertyName, AllowEmpty, MessagePropertyName, EmptyFieldMessage, FailedToParseMessage);

