namespace Neighbor.Contract.Abstractions.Shared;

public class Success<T> : IEquatable<Success<T>>
{
    public static readonly Success<T> None = new(string.Empty, string.Empty, default);
    public static readonly Success<T> OperationCompleted = new("Success.OperationCompleted", "The operation was successfully completed.", default);

    public Success(string code, string message, T? data)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    public string Code { get; }
    public string Message { get; }
    public T? Data { get; } // The data can be null

    public static implicit operator string(Success<T> success) => success.Code;

    public static bool operator ==(Success<T>? a, Success<T>? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Success<T>? a, Success<T>? b) => !(a == b);

    public virtual bool Equals(Success<T>? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message && EqualityComparer<T?>.Default.Equals(Data, other.Data);
    }

    public override bool Equals(object? obj) => obj is Success<T> success && Equals(success);

    public override int GetHashCode() => HashCode.Combine(Code, Message, Data);

    public override string ToString() => Code;
}
