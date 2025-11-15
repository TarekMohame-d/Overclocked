using Application.Services.Authentication.DTOs.Request;
using Domain.Entities;
using Domain.StaticData;

namespace Application.Services.Authentication.Mapping;

public static class AuthenticationMapping
{
    public static User ToEntity(this RegisterRequest request, string passwordHash)
    {
        return new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.PhoneNumber,
            PasswordHash = passwordHash,
            RoleType = RoleType.Customer
        };
    }
}
