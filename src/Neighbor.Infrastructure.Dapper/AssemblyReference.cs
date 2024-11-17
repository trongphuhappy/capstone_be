using System.Reflection;

namespace Neighbor.Infrastructure.Dapper;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
