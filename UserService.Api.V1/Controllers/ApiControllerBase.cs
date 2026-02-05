using Microsoft.AspNetCore.Mvc;
using UserService.Domain.Common;
using UserService.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace UserService.Api.V1.Controllers;

/// <summary>
/// Base controller providing unified result handling for all API endpoints.
/// </summary>
/// <remarks>
/// This abstract class converts application-layer <see cref="Result"/> objects
/// into standardized HTTP responses according to REST conventions.
/// <para>
/// Use <see cref="ToActionResult(Result)"/> and <see cref="ToActionResult{T}(Result{T})"/>
/// to automatically translate success or error results into appropriate responses.
/// </para>
/// </remarks>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Converts a non-generic <see cref="Result"/> into an <see cref="IActionResult"/>.
    /// </summary>
    /// <param name="result">The operation result to convert.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item><description><c>204 No Content</c> if successful.</description></item>
    /// <item><description>Error <c>ProblemDetails</c> with appropriate status code if failed.</description></item>
    /// </list>
    /// </returns>
    protected IActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
            return NoContent();

        return Problem(
            detail: result.Error?.Message,
            statusCode: MapStatus(result.Error));
    }

    /// <summary>
    /// Converts a generic <see cref="Result{T}"/> into an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result object to convert.</param>
    /// <returns>
    /// <list type="bullet">
    /// <item><description><c>200 OK</c> with the result value if successful.</description></item>
    /// <item><description>Error <c>ProblemDetails</c> with appropriate status code if failed.</description></item>
    /// </list>
    /// </returns>
    protected IActionResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return Problem(
            detail: result.Error?.Message,
            statusCode: MapStatus(result.Error));
    }

    /// <summary>
    /// Maps a domain-level <see cref="Error"/> to an HTTP status code.
    /// </summary>
    /// <param name="error">The error to map.</param>
    /// <returns>An appropriate HTTP status code based on the error type.</returns>
    private static int MapStatus(Error? error) => error?.Type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };
}
