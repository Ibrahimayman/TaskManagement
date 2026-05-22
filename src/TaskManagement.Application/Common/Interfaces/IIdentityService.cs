namespace TaskManagement.Application.Common.Interfaces;

public record AuthenticatedUser(Guid Id, string Email, string FullName);

public interface IIdentityService
{
    Task<AuthenticatedUser> RegisterAsync(
        string email,
        string fullName,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthenticatedUser?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
