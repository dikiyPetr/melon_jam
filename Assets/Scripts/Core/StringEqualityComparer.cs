using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

[Preserve]
public class StringEqualityComparer : IEqualityComparer<string>
{
    public static readonly StringEqualityComparer Default = new StringEqualityComparer();

    [Preserve]
    public bool Equals(string x, string y)
    {
        return string.Equals(x, y, StringComparison.Ordinal);
    }

    [Preserve]
    public int GetHashCode(string obj)
    {
        if (obj == null) return 0;
        return obj.GetHashCode();
    }
}
