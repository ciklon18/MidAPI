﻿namespace MisAPI.Exceptions;

public class NullEmailException : Exception
{
    public NullEmailException(string message) : base(message)
    {
    }
}