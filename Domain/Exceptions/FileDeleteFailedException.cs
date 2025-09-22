namespace Domain.Exceptions;

public class FileDeleteFailedException : Exception
{
    public FileDeleteFailedException(string message) : base(message) { }
}
