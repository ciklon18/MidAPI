namespace MisAPI.Exceptions;

public class ForbiddenLeaveCommentException : Exception
{
    public ForbiddenLeaveCommentException(string message) : base(message)
    {
    }
}