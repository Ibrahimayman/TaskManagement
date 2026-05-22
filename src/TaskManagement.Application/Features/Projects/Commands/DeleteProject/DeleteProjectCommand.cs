using MediatR;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest;
