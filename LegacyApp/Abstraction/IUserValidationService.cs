namespace LegacyApp.Abstraction
{
    public interface IUserValidationService
    {
        bool IsValidUser(User user);

        bool IsValidCreditLimit(User user);
    }
}
