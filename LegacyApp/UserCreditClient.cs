using System;
using Grpc.Net.Client;
using LegacyApp.Abstraction;

namespace LegacyApp;

public class UserCreditClient : IUserCreditClient
{
    private readonly string _address;

    public UserCreditClient(string address = "http://totally-real-service.com/UserCreditGrpcServiceServer")
    {
        _address = address;
    }
    
    public int GetCreditLimit(string firstname, string surname, DateTime dateOfBirth)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new UserCreditGrpcService.UserCreditGrpcServiceClient(channel);
        var request = new UserCreditRequest
        {
            Firstname = firstname,
            Surname = surname,
            DateOfBirth = new Google.Protobuf.WellKnownTypes.Timestamp
            {
                Seconds = (long)dateOfBirth.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                Nanos = dateOfBirth.Millisecond * 1000000
            }
        };
        var response = client.GetCreditLimit(request);
        return response.CreditLimit;
    }
}