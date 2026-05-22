using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.Common;

namespace TaskManagement.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IIdentityService _identity;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IIdentityService identity, ITokenService tokenService)
    {
        _identity = identity;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _identity.AuthenticateAsync(request.Email, request.Password, cancellationToken)
            ?? throw new ForbiddenAccessException("Invalid credentials.");

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.FullName),
            ExpiresAt = _tokenService.GetAccessTokenExpiration()
        };
    }
}
