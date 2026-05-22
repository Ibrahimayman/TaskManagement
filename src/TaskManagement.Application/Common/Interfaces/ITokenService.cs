using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    DateTime GetAccessTokenExpiration();
}
