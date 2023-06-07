using System.Collections.Generic;
using LegacyApp.Abstraction;

namespace LegacyApp;

public class UserCreditService : IUserCreditService
{
    public const string ClientWithoutCreditLimit = "VeryImportantClient";

    private static readonly Dictionary<string, int> CustomCreditLimitMultipliers = new(1)
    {
        { "ImportantClient", 2 }
    };

    private readonly IClientRepository _clientRepository;

    private readonly IUserCreditClient _userCreditClient;

    public UserCreditService()
    {
        _userCreditClient = new UserCreditClient();
        _clientRepository = new ClientRepository();
    }

    public UserCreditService(IUserCreditClient userCreditClient, IClientRepository clientRepository)
    {
        _userCreditClient = userCreditClient;
        _clientRepository = clientRepository;
    }

    public void SetUserCreditLimit(User user, int clientId)
    {
        var client = _clientRepository.GetById(clientId);

        if (client.Name == ClientWithoutCreditLimit)
        {
            return;
        }

        user.HasCreditLimit = true;
        var creditLimit = _userCreditClient.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);

        if (CustomCreditLimitMultipliers.TryGetValue(client.Name, out var limitMultiplier))
        {
            creditLimit *= limitMultiplier;
        }

        user.CreditLimit = creditLimit;
    }
}