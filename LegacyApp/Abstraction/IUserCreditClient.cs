using System;

namespace LegacyApp.Abstraction;

public interface IUserCreditClient
{
    int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth);
}