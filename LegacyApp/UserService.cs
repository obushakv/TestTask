using System;
using LegacyApp.Abstraction;

namespace LegacyApp;

public class UserService
{
    private readonly IUserCreditService _userCreditService;
    private readonly IUserDataAccessService _userDataAccessService;
    private readonly IUserValidationService _userValidationService;

    public UserService()
    {
        _userValidationService = new UserValidationService();
        _userCreditService = new UserCreditService();
        _userDataAccessService = new UserDataAccessService();
    }

    //Required for unit tests
    //In a real world this constructor would be used for DI
    public UserService(IUserValidationService userValidationService,
        IUserCreditService userCreditService,
        IUserDataAccessService userDataAccessService)
    {
        _userValidationService = userValidationService;
        _userCreditService = userCreditService;
        _userDataAccessService = userDataAccessService;
    }

    public bool AddUser(string firstName, string surname, string email, DateTime dateOfBirth, int clientId)
    {
        var user = new User
        {
            DateOfBirth = dateOfBirth,
            EmailAddress = email,
            Firstname = firstName,
            Surname = surname
        };

        if (!_userValidationService.IsValidUser(user))
        {
            return false;
        }

        _userCreditService.SetUserCreditLimit(user, clientId);

        if (!_userValidationService.IsValidCreditLimit(user))
        {
            return false;
        }

        _userDataAccessService.AddUser(user);

        return true;
    }
}