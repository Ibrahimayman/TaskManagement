using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Features.Tasks.Common;

public class TaskDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
