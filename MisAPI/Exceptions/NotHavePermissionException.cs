namespace MisAPI.Exceptions;

public class NotHavePermissionException : Exception
{
    public NotHavePermissionException(string message) : base(message)
    {
    }
}