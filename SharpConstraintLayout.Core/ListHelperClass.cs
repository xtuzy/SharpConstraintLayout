using System;
using System.Collections.Generic;
using System.Text;


internal static class ListHelperClass
{
    public static void addAll<T>(this List<T> list, IEnumerable<T> value)
    {
        foreach (var item in value)
            list.Add(item);
    }
}

