namespace Lyt.Validation;

public sealed record class FormValidatorResults<T>
    (
        T? Value = default,
        bool IsValid = false,
        bool HasValue = false,
        string Message = ""
    );

public sealed record class FormValidatorParameters<T>
    (
        AbstractValidator<T> Validator,
        IEnumerable<FieldValidator> FieldValidators, 
        string MessagePropertyName = ""
    );

public sealed class FormValidator<T>
    where T : class
{
    private readonly Bindable viewModel;
    private readonly FormValidatorParameters<T> parameters; 
    private readonly List<FieldValidator> fieldValidators;

    public FormValidator(Bindable viewModel, FormValidatorParameters<T> parameters)
    {
        this.viewModel = viewModel;
        this.parameters = parameters;
        this.fieldValidators = new List<FieldValidator>(parameters.FieldValidators);
    }

    public Type TargetType => typeof(T);

    public FieldValidatorResults<T> Validate()
    {
        return new FieldValidatorResults<T>();
    }
}