using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int LongRunningMilliseconds = 500;

    private readonly Stopwatch _timer = new();
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        if (elapsedMilliseconds > LongRunningMilliseconds)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogWarning(
                "Long running request: {RequestName} took {ElapsedMilliseconds} ms",
                requestName,
                elapsedMilliseconds);
        }

        return response;
    }
}
