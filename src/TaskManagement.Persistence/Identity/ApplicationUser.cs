using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Persistence.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
