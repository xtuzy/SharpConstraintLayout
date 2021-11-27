//----------------------------------------------------------------------------------------
//	Copyright ©  - 2017 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class includes methods to convert Java rectangular arrays (jagged arrays
//	with inner arrays of the same length).
//----------------------------------------------------------------------------------------
public static class RectangularArrays
{
    internal static double[][] ReturnRectangularDoubleArray(int size1, int size2)
    {
        double[][] newArray = new double[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new double[size2];
        }

        return newArray;
    }

    public static float[][] ReturnRectangularFloatArray(int size1, int size2)
    {
        float[][] newArray = new float[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new float[size2];
        }

        return newArray;
    }

    internal static object[][] ReturnRectangularObjectArray(int size1, int size2)
    {
        object[][] newArray = new object[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new object[size2];
        }

        return newArray;
    }

    public static float[][][][] ReturnRectangularFloatArray(int size1, int size2, int size3, int size4)
    {
        float[][][][] newArray = new float[size1][][][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new float[size2][][];
            if (size3 > -1)
            {
                for (int array2 = 0; array2 < size2; array2++)
                {
                    newArray[array1][array2] = new float[size3][];
                    if (size4 > -1)
                    {
                        for (int array3 = 0; array3 < size3; array3++)
                        {
                            newArray[array1][array2][array3] = new float[size4];
                        }
                    }
                }
            }
        }

        return newArray;
    }
}