using System.Reflection;

namespace Neighbor.Domain;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
