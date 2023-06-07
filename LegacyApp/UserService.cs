using System;


namespace LegacyApp;

public class UserService
{
    private const string ClientWithoutCreditLimit = "VeryImportantClient";

    private readonly UserValidationService _userValidationService;
    private readonly UserCreditLimitService _userCreditLimitService;
    private readonly ClientRepository _clientRepository;
    
    public UserService()
    {
        _userValidationService = new UserValidationService();
        _userCreditLimitService = new UserCreditLimitService();
        _clientRepository = new ClientRepository();
    }

    //Required for unit tests
    //It is rather a code smell, and I would prefer to use DI, but since we can't change Program.cs...
    public UserService(UserValidationService userValidationService, 
        UserCreditLimitService userCreditLimitService,
        ClientRepository clientRepository)
    {
        _userValidationService = userValidationService;
        _userCreditLimitService = userCreditLimitService;
        _clientRepository = clientRepository;
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
        
        UserDataAccess.AddUser(user);

        return true;
    }
}