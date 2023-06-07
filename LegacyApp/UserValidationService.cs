using System;
using System.Text.RegularExpressions;
using LegacyApp.Abstraction;

namespace LegacyApp;

public class UserValidationService : IUserValidationService
{
    private const int MinUserAge = 21;
    private const int MinAllowedCreditLimit = 500;

    public bool IsValidUser(User user)
    {
        return user != null
               && IsValidName(user.Firstname, user.Surname)
               && IsValidEmail(user.EmailAddress)
               && IsAppropriateAge(user.DateOfBirth);
    }

    public bool IsValidCreditLimit(User user)
    {
        return !user.HasCreditLimit || user.CreditLimit >= MinAllowedCreditLimit;
    }

    private static bool IsValidEmail(string email)
    {
        if (email == null) 
            return false;
        const string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                           + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                           + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        var regex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(email);
    }

    private static bool IsValidName(string firstName, string surname)
    {
        return !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(surname);
    }

    private static bool IsAppropriateAge(DateTime dateOfBirth)
    {
        var age = CalculateAge(dateOfBirth);
    
        return age >= MinUserAge;
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }
        return age;
    }
}
