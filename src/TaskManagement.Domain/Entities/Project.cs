using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    public Guid OwnerId { get; private set; }

    private readonly List<TaskItem> _tasks = new();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    private Project() { }

    private Project(string name, string? description, Guid ownerId)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
    }

    public static Project Create(string name, string? description, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required.");

        if (ownerId == Guid.Empty)
            throw new DomainException("A project must have an owner.");

        return new Project(name.Trim(), description?.Trim(), ownerId);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required.");

        Name = name.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
    }

    public TaskItem AddTask(
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate)
    {
        var task = TaskItem.Create(Id, title, description, priority, dueDate);
        _tasks.Add(task);
        return task;
    }

    public void RemoveTask(TaskItem task)
    {
        if (task is null) throw new DomainException("Task is required.");
        _tasks.Remove(task);
    }
}
