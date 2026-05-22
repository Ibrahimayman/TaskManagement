using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskItem : AuditableEntity
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public DateTime? DueDate { get; set; }

    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
}
