﻿namespace MisAPI.Exceptions;

public class PatientNotFoundException : Exception
{
    public PatientNotFoundException(string message) : base(message)
    {
    }
}