using LegacyApp.Abstraction;

namespace LegacyApp;

/// <summary>
/// Additional abstract wrapper allows us to decouple our code from the static UserDataAccess class, in order to mock it in unit tests
/// </summary>
public class UserDataAccessService : IUserDataAccessService
{
    public void AddUser(User user)
    {
        UserDataAccess.AddUser(user);
    }
}