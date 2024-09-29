namespace Lyt.Avalonia.Framework.TestApp.Models;

public sealed class UserAdministrationModel : ModelBase, IModel
{
    public UserAdministrationModel(IMessenger messenger, ILogger logger) 
        : base(messenger, logger)
    {
    }

    public List<User> Users { get; set; } = [];

    public User? LoggedInUser { get => this.Get<User?>(); set => this.Set(value); }

    public bool IsLoggedIn => this.LoggedInUser != null;

    public bool IsLoggedInAsUser => this.IsLoggedIn && !this.LoggedInUser!.IsAdministrator;

    public bool IsLoggedInAsAdministrator => this.IsLoggedIn && this.LoggedInUser!.IsAdministrator;

    public override Task Initialize()
    {
        this.CreateDefaultUsers();
        return Task.CompletedTask;
    }

    public override Task Shutdown()
    {
        this.Users.Clear();
        return Task.CompletedTask;
    }

    public bool TryLogin(LoginParameters loginParameters)
    {
        try
        {
            string username = loginParameters.Username;
            string password = loginParameters.Password;
            var user = (from u in this.Users select u).FirstOrDefault(user => user.UserName == username);
            if (user is null)
            {
                this.Logger.Error("Username not found: " + username);
                return false;
            }

            // if (BCrypt.Verify(password, user.PasswordHash))
            if (password == user.Password)
            {
                this.LoggedInUser = user;
                return true;
            }
        }
        catch (Exception ex)
        {
            this.Logger.Error("Login failed: " + ex);
        }

        return false;
    }

    public void Logout() => this.LoggedInUser = null;

    private void CreateDefaultUsers()
    {
        void AddUser(User user)
        {
            user.PasswordHash = user.Password;
            user.Password = user.Password;
            this.Users.Add(user);
        }

        AddUser(User.DefaultUser);
        AddUser(User.DefaultAdministrator);
        AddUser(User.TestUser_01);
        AddUser(User.TestUser_02);
        AddUser(User.TestUser_03);
    }
}
