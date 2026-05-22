namespace TaskManagement.Infrastructure.Identity;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Secret { get; set; } = default!;
    public int ExpirationInMinutes { get; set; } = 60;
}
