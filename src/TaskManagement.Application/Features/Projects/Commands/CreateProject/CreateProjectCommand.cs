using MediatR;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(string Name, string? Description) : IRequest<Guid>;
