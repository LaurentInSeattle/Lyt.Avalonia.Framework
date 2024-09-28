namespace Lyt.Avalonia.UsersAdministration;

public sealed class LoginParameters
{
    public LoginParameters() { /* Required for serialization */ }

    public LoginParameters(string username, string password)
    {
        this.Username = username;
        this.Password = password;
    }

    // Properties made public for serialization 
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

}
