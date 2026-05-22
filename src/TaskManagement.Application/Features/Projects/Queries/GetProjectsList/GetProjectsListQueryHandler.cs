using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Projects.Common;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectsList;

public class GetProjectsListQueryHandler : IRequestHandler<GetProjectsListQuery, PaginatedList<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProjectsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<ProjectDto>> Handle(
        GetProjectsListQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var query = _context.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(p => p.Name.Contains(search) ||
                                     (p.Description != null && p.Description.Contains(search)));
        }

        var projection = query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                OwnerId = p.OwnerId,
                CreatedAt = p.CreatedAt,
                TaskCount = p.Tasks.Count
            });

        return await PaginatedList<ProjectDto>.CreateAsync(
            projection, request.PageNumber, request.PageSize, cancellationToken);
    }
}
