namespace Neighbor.Contract.Abstractions.Shared;

public class Success : IEquatable<Success>
{
    public static readonly Success None = new(string.Empty, string.Empty);
    public static readonly Success OperationCompleted = new("Success.OperationCompleted", "The operation was successfully completed.");

    public Success(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    public static implicit operator string(Success success) => success.Code;

    public static bool operator ==(Success? a, Success? b)
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

    public static bool operator !=(Success? a, Success? b) => !(a == b);

    public virtual bool Equals(Success? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }

    public override bool Equals(object? obj) => obj is Success success && Equals(success);

    public override int GetHashCode() => HashCode.Combine(Code, Message);

    public override string ToString() => Code;
}