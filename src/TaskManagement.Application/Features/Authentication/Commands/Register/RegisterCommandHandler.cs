using MediatR;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Authentication.Common;

namespace TaskManagement.Application.Features.Authentication.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IIdentityService _identity;
    private readonly ITokenService _tokenService;

    public RegisterCommandHandler(IIdentityService identity, ITokenService tokenService)
    {
        _identity = identity;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await _identity.RegisterAsync(
            request.Email,
            request.FullName,
            request.Password,
            cancellationToken);

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
