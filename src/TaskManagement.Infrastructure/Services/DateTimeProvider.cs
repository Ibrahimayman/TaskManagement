using TaskManagement.Application.Common.Interfaces;

namespace TaskManagement.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
