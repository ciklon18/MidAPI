namespace MisAPI.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}