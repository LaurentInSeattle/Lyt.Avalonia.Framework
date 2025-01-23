namespace Lyt.Validation;

public sealed record class FieldValidatorParameters<T>
    (
        string SourcePropertyName,
        AbstractValidator<T> Validator,
        string MessagePropertyName ="" 
    )
{
}

public sealed record class FieldValidatorResults<T>
    (
        bool IsValid = false,
        bool HasValue =false, 
        T? Value = default, 
        string Message = ""
    )
{
}

public sealed class FieldValidator<T>(Bindable viewModel, FieldValidatorParameters<T> parameters)
{
    private readonly Bindable viewModel = viewModel;
    private readonly FieldValidatorParameters<T> parameters = parameters;

    public FieldValidatorResults<T> Validate ()
    {
        // Get property value 
        string propertyText = this.parameters.SourcePropertyName;
        // string propertyText = this.viewModel.Get(this.parameters.SourcePropertyName);

        // Trim 

        // Parse (if needed) 
        bool failedToParse = false;
        if (failedToParse)
        {
            // Localize message 
            string parseMessage = "Cant parse";
            var parseResults = new FieldValidatorResults<T>(IsValid: false, HasValue: false, Message: parseMessage);
            return parseResults;
        }

        T? propertyValue = default; 
        bool isValid = propertyValue!.IsValid<AbstractValidator<T>, T>( out string message);
        // Localize message 
        var results = new FieldValidatorResults<T>(IsValid: isValid, HasValue: true, Message: message);
        return results;
    }
}
