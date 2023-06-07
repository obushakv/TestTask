namespace LegacyApp.Abstraction;

public interface IUserCreditService
{
    void SetUserCreditLimit(User user, int clientId);
}