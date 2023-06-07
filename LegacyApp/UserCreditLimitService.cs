using System.Collections.Generic;

namespace LegacyApp;

    public class UserCreditLimitService
    {
        private static readonly Dictionary<string, int> CustomCreditLimitMultipliers = new(1)
        {
            { "ImportantClient", 2 }
        };

        //I would prefer to put it as private method in the UserService
        //but in order to use mock and still have userCreditService disposed after usage I put it here, in a separate service
        internal void UpdateCreditLimit(User user)
        {
            user.HasCreditLimit = true;
            using var userCreditService = new UserCreditServiceClient();
            var creditLimit = userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);

            if (CustomCreditLimitMultipliers.TryGetValue(user.Client.Name, out var limitMultiplier))
            {
                creditLimit *= limitMultiplier;
            }

            user.CreditLimit = creditLimit;
        }
    }