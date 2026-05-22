using MediatR;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateTime? DueDate) : IRequest<Guid>;
