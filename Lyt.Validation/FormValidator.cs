namespace Lyt.Validation;

public sealed class FormValidator<T>(FormValidatorParameters<T> parameters)
    where T : class, new()
{
    private readonly FormValidatorParameters<T> parameters = parameters;
    private readonly List<FieldValidator> fieldValidators = new(parameters.FieldValidators);

    public Type TargetType => typeof(T);

    public void Clear(Bindable viewModel)
    {
        foreach (var fieldValidator in this.fieldValidators)
        {
            fieldValidator.Clear(viewModel);
        }

        viewModel.ClearValidationMessage(this.parameters.MessagePropertyName);
        this.SetFormValidProperty(viewModel, isValid: false);
    }

    public FormValidatorResults<T> Validate(Bindable viewModel)
    {
        string ShowValidationMessage(string message)
            => viewModel.ShowValidationMessage(this.parameters.MessagePropertyName, message);

        void SetFormValidProperty(bool isValid)
            => this.SetFormValidProperty(viewModel, isValid);

        // Step #1 : Validate field by field 
        List<FieldValidatorResults> results = [];
        foreach (var fieldValidator in this.fieldValidators)
        {
            var result = fieldValidator.Validate(viewModel);
            if (!result.IsValid)
            {
                SetFormValidProperty(isValid: false);
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
                SetFormValidProperty(isValid: false);
                ShowValidationMessage(message);
                return new FormValidatorResults<T>(IsValid: false, Message: message);
            }
        }

        // All passed, fully validated 
        SetFormValidProperty(isValid: true);
        return new FormValidatorResults<T>(IsValid: true, HasValue: true, Value: formValue);
    }

    private void SetFormValidProperty(Bindable viewModel, bool isValid)
    {
        string propertyName = this.parameters.FormValidPropertyName;
        if (!string.IsNullOrWhiteSpace(propertyName))
        {
            viewModel.InvokeSetProperty(propertyName, isValid);
        }
    }
}