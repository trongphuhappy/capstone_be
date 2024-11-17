using System.Reflection;

namespace Neighbor.Contract;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
