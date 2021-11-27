using System;
using System.Collections.Generic;
using System.Text;


public static class ArrayHelperClass
{
    public static T[] Copy<T>(this T[] oldArray, int copyLength)
    {
        var newArray = new T[copyLength];
        Array.Copy(oldArray, newArray, Math.Min(oldArray.Length, copyLength));
        return newArray;
    }

    public static void Fill<T>(this T[] oldArray, T value) where T : class
    {
        for (int i = 0; i < oldArray.Length; i++)
            oldArray[i] = value;
    }

    public static void Fill(this double[] oldArray, double value)
    {
        for (int i = 0; i < oldArray.Length; i++)
            oldArray[i] = value;
    }

    public static void Fill(this float[] oldArray, float value)
    {
        for (int i = 0; i < oldArray.Length; i++)
            oldArray[i] = value;
    }

    public static void Fill(this int[] oldArray, int value)
    {
        for (int i = 0; i < oldArray.Length; i++)
            oldArray[i] = value;
    }
}

