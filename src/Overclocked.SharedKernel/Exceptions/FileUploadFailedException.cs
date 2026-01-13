namespace Overclocked.SharedKernel.Exceptions;

public class FileUploadFailedException : Exception
{
    public FileUploadFailedException() { }

    public FileUploadFailedException(string? message)
        : base(message) { }

    public FileUploadFailedException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
