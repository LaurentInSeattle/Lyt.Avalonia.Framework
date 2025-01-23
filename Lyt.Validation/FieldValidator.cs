namespace Lyt.Validation;

public abstract class FieldValidator(
    Bindable viewModel, Type targetType, FieldValidatorParameters parameters)
{
    public const string DefaultEmptyFieldMessage = "Validation.EmptyFieldMessageKey";
    public const string DefaultFailedToParseMessage = "Validation.FailedToParseMessageKey";

    protected readonly Bindable viewModel = viewModel;
    public FieldValidatorParameters Parameters { get; protected set; } = parameters; 
    public Type TargetType { get; protected set; } = targetType;

    public abstract FieldValidatorResults Validate(); 
}
