namespace Lyt.Validation;

public abstract class FieldValidator(Type targetType, FieldValidatorParameters parameters)
{
    public const string DefaultEmptyFieldMessageKey = "Validation.EmptyFieldMessageKey";
    public const string DefaultFailedToParseMessageKey = "Validation.FailedToParseMessageKey";

    public const string DefaultEmptyFieldMessage = "This field is required.";
    public const string DefaultFailedToParseMessage = "This entry is invalid.";

    public FieldValidatorParameters Parameters { get; protected set; } = parameters; 
    
    public Type TargetType { get; protected set; } = targetType;

    public abstract void Clear (Bindable viewModel);

    public abstract FieldValidatorResults Validate(Bindable viewModel); 
}
