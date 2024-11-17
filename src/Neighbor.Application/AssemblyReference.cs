using System.Reflection;

namespace Neighbor.Application;

public class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
