namespace Lyt.Avalonia.UsersAdministration;

public sealed class User
{
    public static readonly User DefaultUser = new()
    {
        FirstName = "Laurent",
        LastName = "Testud",
        UserName = "Home",
        Password = "2142",
        Role = UserRole.Business,
    };

    public static readonly User DefaultAdministrator = new()
    {
        FirstName = "Admin",
        LastName = "Istrator",
        UserName = "Admin",
        Password = "4284",
        Role = UserRole.Administrator,
    };

    public static readonly User TestUser_01 = new()
    {
        FirstName = "Laurent",
        LastName = "Testud",
        UserName = "Master Laurent Y. Testud",
        Password = "2142",
        Role = UserRole.Business,
    };

    public static readonly User TestUser_02 = new()
    {
        FirstName = "Zhu",
        LastName = "Berger",
        UserName = "Dr. Zhu (孔鳳) Berger, PhD",
        Password = "2142",
        Role = UserRole.Training,
    };

    public static readonly User TestUser_03 = new()
    {
        FirstName = "Elisabeth",
        LastName = "Edwards",
        UserName = "Elisabeth Edwards, MP",
        Password = "2142",
        Role = UserRole.Developer,
    };

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Business;

    public string Initials => string.Concat(this.FirstName[0], this.LastName[0]);

    public bool IsAdministrator => this.Role == UserRole.Administrator;
}
