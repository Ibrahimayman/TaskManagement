namespace TaskManagement.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string fullName);
    DateTime GetAccessTokenExpiration();
}
