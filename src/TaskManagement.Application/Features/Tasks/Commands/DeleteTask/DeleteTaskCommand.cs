using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest;
