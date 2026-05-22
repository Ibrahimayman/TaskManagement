using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
