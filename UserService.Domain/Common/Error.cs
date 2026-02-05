using UserService.Domain.Enums;

namespace UserService.Domain.Common;

/// <summary>
/// Represents a standardized application or domain error.
/// This is a value object (immutable and comparable by value),
/// typically used to indicate validation, business, or infrastructure errors
/// without throwing exceptions.
/// </summary>
public sealed class Error : IEquatable<Error>
{
    /// <summary>
    /// Gets the unique error code that identifies the error type or source.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable description of the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the logical category of the error (for example, Validation, Conflict, or Unexpected).
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with the specified parameters.
    /// </summary>
    /// <param name="code">A unique code representing the error.</param>
    /// <param name="message">A descriptive message for the error.</param>
    /// <param name="type">The category of the error.</param>
    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    /// <summary>
    /// Creates a new <see cref="Error"/> instance.
    /// This is the primary factory method for error creation within the domain.
    /// </summary>
    /// <param name="code">A unique code representing the error.</param>
    /// <param name="message">A descriptive message for the error.</param>
    /// <param name="type">The category of the error.</param>
    /// <returns>A new <see cref="Error"/> instance.</returns>
    internal static Error Create(string code, string message, ErrorType type)
        => new(code, message, type);

    /// <summary>
    /// Returns a string representation of the error in the format
    /// <c>"{Code}: {Message}"</c>.
    /// </summary>
    /// <returns>A string combining the error code and message.</returns>
    public override string ToString() => $"{Code}: {Message}";

    /// <summary>
    /// Determines whether the specified <see cref="Error"/> instance
    /// is equal to the current instance (by comparing the <see cref="Code"/> values).
    /// </summary>
    /// <param name="other">The other <see cref="Error"/> to compare with.</param>
    /// <returns><c>true</c> if both have the same <see cref="Code"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(Error? other) =>
        other is not null && Code == other.Code;

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns><c>true</c> if the object is an <see cref="Error"/> with the same code; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => Equals(obj as Error);

    /// <summary>
    /// Returns a hash code for this instance based on the <see cref="Code"/> value.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Error"/>.</returns>
    public override int GetHashCode() => HashCode.Combine(Code);
}
