using MediatR;
using TaskManagement.Application.Features.Tasks.Common;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public record GetTasksByProjectQuery(Guid ProjectId) : IRequest<List<TaskDto>>;
