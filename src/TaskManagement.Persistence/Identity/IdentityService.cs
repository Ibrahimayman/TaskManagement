using Microsoft.AspNetCore.Identity;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Persistence.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<AuthenticatedUser> RegisterAsync(
        string email,
        string fullName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var existing = await _userManager.FindByEmailAsync(normalizedEmail);
        if (existing is not null)
        {
            throw new ConflictException($"A user with email '{normalizedEmail}' already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            UserName = normalizedEmail,
            FullName = fullName.Trim(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            throw new ValidationException(
                result.Errors.Select(e => new FluentValidation.Results.ValidationFailure("Identity", e.Description)));
        }

        return new AuthenticatedUser(user.Id, user.Email!, user.FullName);
    }

    public async Task<AuthenticatedUser?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _userManager.FindByEmailAsync(normalizedEmail);
        if (user is null) return null;

        var passwordValid = await _userManager.CheckPasswordAsync(user, password);
        if (!passwordValid) return null;

        return new AuthenticatedUser(user.Id, user.Email!, user.FullName);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email.Trim().ToLowerInvariant());
        return user is not null;
    }
}
