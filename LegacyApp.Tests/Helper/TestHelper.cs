namespace LegacyApp.Tests.Helper
{
    internal static class TestHelper
    {
        internal static User GetValidUser()
        {
            return new User
            {
                Firstname = "John",
                Surname = "Doe",
                EmailAddress = "john.doe@example.com",
                DateOfBirth = DateTime.Today.AddYears(-21)
            };
        }
    }
}
