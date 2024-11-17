using System.Reflection;

namespace Neighbor.Infrastructure;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
