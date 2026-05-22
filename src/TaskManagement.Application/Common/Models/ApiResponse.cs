namespace TaskManagement.Application.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.", int statusCode = 200) =>
        new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Created(T data, string message = "Resource created successfully.") =>
        new()
        {
            Success = true,
            StatusCode = 201,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Fail(string message, int statusCode = 400, IEnumerable<string>? errors = null) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors?.ToList() ?? new List<string>()
        };
}

public class ApiResponse : ApiResponse<object?>
{
    public static ApiResponse Ok(string message = "Request completed successfully.", int statusCode = 200) =>
        new()
        {
            Success = true,
            StatusCode = statusCode,
            Message = message
        };

    public static new ApiResponse Fail(string message, int statusCode = 400, IEnumerable<string>? errors = null) =>
        new()
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Errors = errors?.ToList() ?? new List<string>()
        };
}
