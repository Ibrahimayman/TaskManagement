using MediatR;
using TaskManagement.Application.Features.Authentication.Common;

namespace TaskManagement.Application.Features.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
