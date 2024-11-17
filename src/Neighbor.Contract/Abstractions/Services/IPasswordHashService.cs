namespace Neighbor.Contract.Abstractions.Services;

public interface IPasswordHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHashed);
}
