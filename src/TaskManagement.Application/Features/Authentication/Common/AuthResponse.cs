namespace TaskManagement.Application.Features.Authentication.Common;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
