using MediatR;
using TaskManagement.Application.Features.Authentication.Common;

namespace TaskManagement.Application.Features.Authentication.Commands.Register;

public record RegisterCommand(
    string Email,
    string FullName,
    string Password) : IRequest<AuthResponse>;
