namespace UserService.Domain.Common;

/// <summary>
/// Represents the outcome of an operation that can succeed or fail,
/// containing information about success state and an optional <see cref="Error"/>.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error associated with a failed operation, if any.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">The error information, if the operation failed.</param>
    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance.
    /// </summary>
    /// <returns>A <see cref="Result"/> representing a successful operation.</returns>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the operation to fail.</param>
    /// <returns>A <see cref="Result"/> representing a failed operation.</returns>
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>
/// Represents the outcome of an operation that can succeed or fail
/// and may return a value of type <typeparamref name="T"/> when successful.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the value returned by a successful operation.
    /// If the operation failed, this value is <see langword="default"/>.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="value">The value of the result, if successful.</param>
    /// <param name="isSuccess">Indicates whether the operation succeeded.</param>
    /// <param name="error">The error information, if the operation failed.</param>
    private Result(T? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> with the specified value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A successful <see cref="Result{T}"/> instance.</returns>
    public static Result<T> Success(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the operation to fail.</param>
    /// <returns>A failed <see cref="Result{T}"/> instance.</returns>
    public new static Result<T> Failure(Error error) => new(default, false, error);
}
