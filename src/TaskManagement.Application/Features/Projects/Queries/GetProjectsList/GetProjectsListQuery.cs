using MediatR;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Projects.Common;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectsList;

public record GetProjectsListQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Search = null) : IRequest<PaginatedList<ProjectDto>>;
