namespace UserService.Domain.Enums;

/// <summary>
/// Defines the logical categories of errors used throughout the application.
/// These values can be mapped to corresponding HTTP status codes for API responses.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Represents the absence of an error.
    /// Typically used as a default or placeholder value.
    /// </summary>
    None,

    /// <summary>
    /// Indicates a validation error — when input data does not meet business or format requirements.
    /// Commonly mapped to HTTP 400 (Bad Request).
    /// </summary>
    Validation,

    /// <summary>
    /// Indicates that the requested entity or resource was not found.
    /// Commonly mapped to HTTP 404 (Not Found).
    /// </summary>
    NotFound,

    /// <summary>
    /// Indicates a business or data conflict — for example, when a duplicate record exists.
    /// Commonly mapped to HTTP 409 (Conflict).
    /// </summary>
    Conflict,

    /// <summary>
    /// Indicates that authentication is required but missing or invalid.
    /// Commonly mapped to HTTP 401 (Unauthorized).
    /// </summary>
    Unauthorized,

    /// <summary>
    /// Indicates that the current user does not have permission to perform the requested action.
    /// Commonly mapped to HTTP 403 (Forbidden).
    /// </summary>
    Forbidden,

    /// <summary>
    /// Indicates an unexpected or unhandled error.
    /// Commonly mapped to HTTP 500 (Internal Server Error).
    /// </summary>
    Unexpected
}