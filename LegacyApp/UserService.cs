using LegacyApp.Abstraction;
using System;


namespace LegacyApp;

public class UserService
{
    private const string ClientWithoutCreditLimit = "VeryImportantClient";

    private readonly IUserValidationService _userValidationService;
    private readonly IUserCreditLimitService _userCreditLimitService;
    private readonly IClientRepository _clientRepository;
    private readonly IUserDataAccessService _userDataAccessService;

    public UserService()
    {
        _userValidationService = new UserValidationService();
        _userCreditLimitService = new UserCreditLimitService();
        _clientRepository = new ClientRepository();
        _userDataAccessService = new UserDataAccessService();
    }

    //Required for unit tests
    //It is rather a code smell, and I would prefer to use DI, but since we can't change Program.cs...
    public UserService(IUserValidationService userValidationService, 
        IUserCreditLimitService userCreditLimitService,
        IClientRepository clientRepository,
        IUserDataAccessService userDataAccessService)
    {
        _userValidationService = userValidationService;
        _userCreditLimitService = userCreditLimitService;
        _clientRepository = clientRepository;
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
       
        user.Client = _clientRepository.GetById(clientId);
        
        if (user.Client.Name != ClientWithoutCreditLimit)
        {
            _userCreditLimitService.UpdateCreditLimit(user);
        }

        if (!_userValidationService.IsValidCreditLimit(user))
        {
            return false;
        }

        _userDataAccessService.AddUser(user);

        return true;
    }
}