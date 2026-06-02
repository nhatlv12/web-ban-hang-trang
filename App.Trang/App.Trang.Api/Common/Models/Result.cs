namespace App.Trang.Api.Common.Models;

/// <summary>
/// Response wrapper chung cho tất cả API endpoints
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static Result<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static Result<T> Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

    public static Result<T> Fail(List<string> errors) => new()
    {
        Success = false,
        Errors = errors,
        Message = "Validation failed"
    };
}

/// <summary>
/// Response wrapper không có data (cho Delete, Update...)
/// </summary>
public class Result
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static Result Ok(string? message = null) => new()
    {
        Success = true,
        Message = message
    };

    public static Result Fail(string message) => new()
    {
        Success = false,
        Message = message
    };

    public static Result Fail(List<string> errors) => new()
    {
        Success = false,
        Errors = errors,
        Message = "Validation failed"
    };
}
