namespace Lyt.Validation;

public sealed class FieldValidator<T>(FieldValidatorParameters<T> parameters)
    : FieldValidator(typeof(T), parameters)
    where T : IParsable<T>
{
    private readonly FieldValidatorParameters<T> parameters = parameters;

    public new FieldValidatorParameters<T> Parameters => this.parameters;

    public override void Clear(Bindable viewModel)
    { 
        // Clear: value comes first for Set) 
        viewModel.Set<string>(string.Empty, this.parameters.SourcePropertyName);
        viewModel.ClearValidationMessage(this.parameters.MessagePropertyName);
    }
    public override FieldValidatorResults<T> Validate(Bindable viewModel)
    {
        string ShowValidationMessage(string message)
            => viewModel.ShowValidationMessage(this.parameters.MessagePropertyName, message);

        // Get property value 
        string propertyText = string.Empty;
        string? maybePropertyText = viewModel.Get<string>(this.parameters.SourcePropertyName);
        bool isEmpty = string.IsNullOrWhiteSpace(maybePropertyText);
        if (!isEmpty)
        {
            // Trim 
            propertyText = maybePropertyText!;
            propertyText = propertyText.Trim();
            isEmpty = string.IsNullOrWhiteSpace(propertyText);
        }

        if (isEmpty)
        {
            // Clear white space noise, if any
            this.Clear(viewModel);

            if (this.parameters.AllowEmpty)
            {
                return
                    new FieldValidatorResults<T>(IsValid: true);
            }
            else
            {
                string emptyMessage = this.parameters.EmptyFieldMessage;
                emptyMessage = string.IsNullOrWhiteSpace(emptyMessage) ? DefaultEmptyFieldMessage : emptyMessage;
                emptyMessage = ShowValidationMessage(emptyMessage);
                return
                    new FieldValidatorResults<T>(Message: emptyMessage);
            }
        }

        // Check if parsing is needed 
        T propertyValue;
        if (typeof(string).Is<T>() && propertyText is T propertyString)
        {
            // Target type is string, no parsing needed
            propertyValue = propertyString;
        }
        else
        {
            // Need to parse  
            bool isParsed = propertyText.TryParse<T>(out T? maybeValue);
            if (!isParsed || maybeValue is not T value)
            {
                // failed to parse
                string parseMessage = this.parameters.FailedToParseMessage;
                parseMessage = string.IsNullOrWhiteSpace(parseMessage) ? DefaultFailedToParseMessage : parseMessage;
                parseMessage = ShowValidationMessage(parseMessage);
                var parseResults = new FieldValidatorResults<T>(Message: parseMessage);
                return parseResults;
            }

            propertyValue = value;
        }

        // Now we have a value: Run the Fluent Validator if we have one, 
        // If no validator is specified, it's all done and valid...
        var validator = this.parameters.Validator;
        if (validator != null) 
        {
            bool isValid = validator.IsValid(propertyValue, out string message);
            if (!isValid)
            {
                ShowValidationMessage(message);
                return
                    new FieldValidatorResults<T>(Message: message);
            }
        }

        viewModel.ClearValidationMessage(this.parameters.MessagePropertyName);
        return new FieldValidatorResults<T>(IsValid: true, HasValue: true, Value: propertyValue);
    }
}
