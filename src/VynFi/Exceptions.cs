namespace VynFi;

/// <summary>Base exception for all VynFi API errors.</summary>
public class VynFiException : Exception
{
    public int? StatusCode { get; }
    public ErrorBody? Body { get; }

    public VynFiException(string message, int? statusCode = null, ErrorBody? body = null)
        : base(message)
    {
        StatusCode = statusCode;
        Body = body;
    }
}

public class AuthenticationException : VynFiException
{
    public AuthenticationException(string message, ErrorBody? body = null)
        : base(message, 401, body) { }
}

public class InsufficientCreditsException : VynFiException
{
    public InsufficientCreditsException(string message, ErrorBody? body = null)
        : base(message, 402, body) { }
}

public class NotFoundException : VynFiException
{
    public NotFoundException(string message, ErrorBody? body = null)
        : base(message, 404, body) { }
}

public class ConflictException : VynFiException
{
    public ConflictException(string message, ErrorBody? body = null)
        : base(message, 409, body) { }
}

public class ValidationException : VynFiException
{
    public ValidationException(string message, ErrorBody? body = null)
        : base(message, 422, body) { }
}

public class RateLimitException : VynFiException
{
    public RateLimitException(string message, ErrorBody? body = null)
        : base(message, 429, body) { }
}

public class ServerException : VynFiException
{
    public ServerException(string message, ErrorBody? body = null)
        : base(message, 500, body) { }
}

/// <summary>Structured error body from the VynFi API.</summary>
public class ErrorBody
{
    public string Detail { get; set; } = "";
    public string Message { get; set; } = "";
    public int Status { get; set; }
}
