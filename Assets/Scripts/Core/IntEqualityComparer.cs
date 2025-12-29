using System.Collections.Generic;
using UnityEngine.Scripting;

[Preserve]
public class IntEqualityComparer : IEqualityComparer<int>
{
    public static readonly IntEqualityComparer Default = new IntEqualityComparer();

    [Preserve]
    public bool Equals(int x, int y)
    {
        return x == y;
    }

    [Preserve]
    public int GetHashCode(int obj)
    {
        return obj;
    }
}
    