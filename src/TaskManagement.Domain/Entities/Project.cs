using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class Project : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
