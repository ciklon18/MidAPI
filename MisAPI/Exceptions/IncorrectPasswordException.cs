﻿namespace MisAPI.Exceptions;

public class IncorrectPasswordException : Exception
{
    public IncorrectPasswordException(string message) : base(message)
    {
    }
}