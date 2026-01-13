namespace Overclocked.SharedKernel.Exceptions;

public class FileDeleteFailedException : Exception
{
    public FileDeleteFailedException() { }

    public FileDeleteFailedException(string? message)
        : base(message) { }

    public FileDeleteFailedException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
