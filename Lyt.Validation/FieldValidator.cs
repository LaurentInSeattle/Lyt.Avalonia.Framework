namespace Lyt.Validation;

public abstract class FieldValidator(
    Bindable viewModel, Type targetType, FieldValidatorParameters parameters)
{
    public const string DefaultEmptyFieldMessageKey = "Validation.EmptyFieldMessageKey";
    public const string DefaultFailedToParseMessageKey = "Validation.FailedToParseMessageKey";

    public const string DefaultEmptyFieldMessage = "This field is required.";
    public const string DefaultFailedToParseMessage = "This entry is invalid.";

    protected readonly Bindable viewModel = viewModel;

    public FieldValidatorParameters Parameters { get; protected set; } = parameters; 
    
    public Type TargetType { get; protected set; } = targetType;

    public abstract FieldValidatorResults Validate(); 
}
