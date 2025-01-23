namespace Lyt.Validation;

public sealed class FieldValidator<T>(
    Bindable viewModel, FieldValidatorParameters<T> parameters)
    : FieldValidator(viewModel, typeof(T), parameters)
    where T : IParsable<T>
{
    private readonly FieldValidatorParameters<T> parameters = parameters;

    public new FieldValidatorParameters<T> Parameters => this.parameters;

    public override FieldValidatorResults<T> Validate()
    {
        string ShowValidationMessage(string message)
            => this.viewModel.ShowValidationMessage(this.parameters.MessagePropertyName, message);

        // Get property value 
        string propertyText = string.Empty;
        string? maybePropertyText = this.viewModel.Get<string>(this.parameters.SourcePropertyName);
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
            // Clear white space noise 
            this.viewModel.Set<string>(this.parameters.SourcePropertyName, string.Empty);

            if (this.parameters.AllowEmpty)
            {
                return
                    new FieldValidatorResults<T>(IsValid: true, HasValue: false, Value: default);
            }
            else
            {
                string emptyMessage = this.parameters.EmptyFieldMessage;
                emptyMessage = string.IsNullOrWhiteSpace(emptyMessage) ? DefaultEmptyFieldMessage : emptyMessage;
                emptyMessage = ShowValidationMessage(emptyMessage);
                return
                    new FieldValidatorResults<T>(IsValid: false, HasValue: false, Message: emptyMessage);
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
                var parseResults = new FieldValidatorResults<T>(IsValid: false, HasValue: false, Message: parseMessage);
                return parseResults;
            }

            propertyValue = value;
        }

        // Now we have a value: Run the Fluent Validator 
        bool isValid = propertyValue.IsValid<AbstractValidator<T>, T>(out string message);
        if (!isValid)
        {
            ShowValidationMessage(message);
        }

        return new FieldValidatorResults<T>(IsValid: isValid, HasValue: true, Value: propertyValue);
    }
}
