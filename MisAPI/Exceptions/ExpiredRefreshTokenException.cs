﻿namespace MisAPI.Exceptions;

public class ExpiredRefreshTokenException : Exception
{
    public ExpiredRefreshTokenException(string message) : base(message)
    {
    }
}