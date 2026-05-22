using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Models;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Application.Features.Tasks.Common;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers;

[Authorize]
[Route("api/v1/tasks")]
public class TasksController : ApiControllerBase
{
    /// <summary>Returns all tasks for a specific project.</summary>
    [HttpGet("by-project/{projectId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<List<TaskDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetTasksByProjectQuery(projectId), cancellationToken);
        return Ok(ApiResponse<List<TaskDto>>.Ok(result));
    }

    /// <summary>Creates a new task inside a project.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var id = await Mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<Guid>.Created(id, "Task created successfully."));
    }

    /// <summary>Updates only the status of a task.</summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(new UpdateTaskStatusCommand(id, request.Status), cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes a task.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteTaskCommand(id), cancellationToken);
        return NoContent();
    }
}

public record UpdateTaskStatusRequest(TaskItemStatus Status);
