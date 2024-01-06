namespace MisAPI.Exceptions;

public class DoctorAlreadyExistsException : Exception
{
    public DoctorAlreadyExistsException(string message) : base(message)
    {
    }
    
}