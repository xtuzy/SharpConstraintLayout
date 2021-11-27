using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 一般参考Math.java
/// </summary>
public static class MathExtension
{
    /**
     * Constant by which to multiply an angular value in degrees to obtain an
     * angular value in radians.
     */
    private const double DEGREES_TO_RADIANS = 0.017453292519943295;

    /**
     * Converts an angle measured in degrees to an approximately
     * equivalent angle measured in radians.  The conversion from
     * degrees to radians is generally inexact.
     *
     * @param   angdeg   an angle, in degrees
     * @return  the measurement of the angle {@code angdeg}
     *          in radians.
     * @since   1.2
     */
    public static double toRadians(double angdeg)
    {
        return angdeg * DEGREES_TO_RADIANS;
    }

    /// <summary>
    /// https://www.dreamincode.net/forums/topic/273525-java-mathhypotab-equivalent-in-c%23/
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static double hypot(double x, double y)
    {
        return System.Math.Sqrt(x * x + y * y);
    }


    /**
     * Constant by which to multiply an angular value in radians to obtain an
     * angular value in degrees.
     */
    private const double RADIANS_TO_DEGREES = 57.29577951308232;
    
    /**
     * Converts an angle measured in radians to an approximately
     * equivalent angle measured in degrees.  The conversion from
     * radians to degrees is generally inexact; users should
     * <i>not</i> expect {@code cos(toRadians(90.0))} to exactly
     * equal {@code 0.0}.
     *
     * @param   angrad   an angle, in radians
     * @return  the measurement of the angle {@code angrad}
     *          in degrees.
     * @since   1.2
     */
    public static double toDegrees(double angrad)
    {
        return angrad * RADIANS_TO_DEGREES;
    }
}
