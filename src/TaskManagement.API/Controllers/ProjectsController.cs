using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Application.Features.Projects.Common;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;
using TaskManagement.Application.Features.Projects.Queries.GetProjectsList;

namespace TaskManagement.API.Controllers;

[Authorize]
[Route("api/v1/projects")]
public class ProjectsController : ApiControllerBase
{
    /// <summary>Returns a paginated list of projects owned by the current user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(
            new GetProjectsListQuery(pageNumber, pageSize, search), cancellationToken);
        return Ok(ApiResponse<PaginatedList<ProjectDto>>.Ok(result));
    }

    /// <summary>Returns a single project by id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<ProjectDto>.Ok(result));
    }

    /// <summary>Creates a new project for the current user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var id = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id },
            ApiResponse<Guid>.Created(id, "Project created successfully."));
    }

    /// <summary>Updates an existing project owned by the current user.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest(ApiResponse.Fail("Route id and body id do not match."));
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes a project owned by the current user.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return NoContent();
    }
}
