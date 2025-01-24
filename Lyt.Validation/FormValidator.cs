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
        string MessagePropertyName = ""
    );

public sealed class FormValidator<T>(Bindable viewModel, FormValidatorParameters<T> parameters)
    where T : class, new()
{
    private readonly Bindable viewModel = viewModel;
    private readonly FormValidatorParameters<T> parameters = parameters;
    private readonly List<FieldValidator> fieldValidators = new(parameters.FieldValidators);

    public Type TargetType => typeof(T);

    public FormValidatorResults<T> Validate()
    {
        string ShowValidationMessage(string message)
            => this.viewModel.ShowValidationMessage(this.parameters.MessagePropertyName, message);

        // Step #1 : Validate field by field 
        List<FieldValidatorResults> results = [];
        foreach (var fieldValidator in this.fieldValidators)
        {
            var result = fieldValidator.Validate();
            if (!result.IsValid)
            {
                return new FormValidatorResults<T>(IsValid: false);
            }

            results.Add(result);
        }

        // Step #2: all fields valid, run the validator if one is provided 
        // 2-a Create object from validated fields, property names should match 
        T formValue = new();
        for (int i = 0; i < results.Count; ++i)
        {
            var fieldValidator = this.fieldValidators[i];
            var result = results[i];

            // Copy validated property value into new object 
            string propertyName = fieldValidator.Parameters.SourcePropertyName;
            object? propertyValue = result.InvokeGetProperty("Value");
            formValue.InvokeSetProperty(propertyName, propertyValue);
        }

        // 2-b Validate the resulting object if a validator is provided 
        var maybeValidator = this.parameters.FormValidator; 
        if (maybeValidator is not null)
        {
            bool isValid = maybeValidator.IsValid(formValue, out string message);
            if (!isValid)
            {
                ShowValidationMessage(message);
                return new FormValidatorResults<T>(IsValid: false, Message: message);
            }
        }

        // All passed, fully validated 
        return new FormValidatorResults<T>(IsValid: true, HasValue: true, Value: formValue);
    }
}