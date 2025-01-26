namespace Lyt.Validation;

public record class FieldValidatorResults
(
    bool IsValid = false,
    bool HasValue = false,
    string Message = ""
);

public sealed record class FieldValidatorResults<T> 
(
    T? Value = default,
    bool IsValid = false,
    bool HasValue = false,
    string Message = ""
) : FieldValidatorResults(IsValid , HasValue , Message )
{
    public T SafeValue 
        => this.HasValue ? this.Value! : throw new Exception("Should have checked 'HasValue'..."); 
}
