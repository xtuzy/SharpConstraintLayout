using System;
using System.Collections.Generic;
using System.Text;


public static class NumberHelperClass
{
    /// <summary>
    /// https://stackoverflow.com/a/26394670/13254773
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int floatToIntBits(this float value)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
    }

    public static float intBitsToFloat(this int value)
    {
        return (float)BitConverter.ToDouble(BitConverter.GetBytes(value),0);//TODO:验证正确性
    }
}

