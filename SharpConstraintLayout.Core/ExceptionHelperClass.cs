using System;
using System.Collections.Generic;
using System.Text;


public class AssertionError : Exception
{
    public AssertionError() : base()
    {
    }

    public AssertionError(string message) : base(message)
    {
    }

    public AssertionError(string message, Exception innerException) : base(message, innerException)
    {
    }
}

