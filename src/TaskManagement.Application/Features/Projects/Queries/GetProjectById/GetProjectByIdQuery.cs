using MediatR;
using TaskManagement.Application.Features.Projects.Common;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;
