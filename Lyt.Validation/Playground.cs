
namespace Lyt.Validation;

public class Playground
{
    public void Test()
    {
        if (!Validate(this.DataEntry, out int value, out string message))
        { 
            this.Error = message;
            return; 
        }

        this.Error = string.Empty;

    }

    private bool Validate<T>(string dataEntry, out T? value , out string message  )
    {
        value = default;
        message = string.Empty ;
        return true;
    }

    public string DataEntry { get; set; } = "Blah";

    public string Error { get; set; } = "Blah";

}
