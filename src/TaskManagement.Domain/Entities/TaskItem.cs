using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Guid ProjectId { get; private set; }
    public Project? Project { get; private set; }

    private TaskItem() { }

    private TaskItem(
        Guid projectId,
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate)
    {
        ProjectId = projectId;
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        Status = TaskItemStatus.Todo;
    }

    public static TaskItem Create(
        Guid projectId,
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate)
    {
        if (projectId == Guid.Empty)
            throw new DomainException("A task must belong to a project.");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
            throw new DomainException("Due date cannot be in the past.");

        return new TaskItem(projectId, title.Trim(), description?.Trim(), priority, dueDate);
    }

    public void UpdateDetails(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        Title = title.Trim();
        Description = description?.Trim();
    }

    public void UpdateStatus(TaskItemStatus status) => Status = status;

    public void ChangePriority(TaskPriority priority) => Priority = priority;

    public void Reschedule(DateTime? dueDate)
    {
        if (dueDate.HasValue && dueDate.Value < DateTime.UtcNow)
            throw new DomainException("Due date cannot be in the past.");

        DueDate = dueDate;
    }
}
