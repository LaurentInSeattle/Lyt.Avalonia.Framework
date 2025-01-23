namespace Lyt.Validation;

public class FieldValidator(Bindable viewModel)
{
    public const string DefaultEmptyFieldMessage = "Validation.EmptyFieldMessageKey";
    public const string DefaultFailedToParseMessage = "Validation.FailedToParseMessageKey";

    protected readonly Bindable viewModel = viewModel;
}
