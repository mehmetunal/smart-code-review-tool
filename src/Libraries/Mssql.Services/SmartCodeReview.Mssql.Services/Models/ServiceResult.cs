namespace SmartCodeReview.Mssql.Services.Models;

/// <summary>
/// Servis işlem sonucu
/// </summary>
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;

    public static ServiceResult<T> Success(T data, string? message = null)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = 200
        };
    }

    public static ServiceResult<T> Created(T data, string? message = null)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = 201
        };
    }

    public static ServiceResult<T> Fail(string message, int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Fail(List<string> errors, string? message = null, int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> NotFound(string message = "Kayıt bulunamadı")
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = 404
        };
    }
}

/// <summary>
/// Servis işlem sonucu (data olmayan)
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public int StatusCode { get; set; } = 200;

    public static ServiceResult Success(string? message = null)
    {
        return new ServiceResult
        {
            IsSuccess = true,
            Message = message,
            StatusCode = 200
        };
    }

    public static ServiceResult Fail(string message, int statusCode = 400)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ServiceResult Fail(List<string> errors, string? message = null, int statusCode = 400)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}

