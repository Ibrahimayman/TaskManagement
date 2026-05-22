using FluentValidation;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}
