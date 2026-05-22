using MediatR;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public record UpdateTaskStatusCommand(Guid TaskId, TaskItemStatus Status) : IRequest;
