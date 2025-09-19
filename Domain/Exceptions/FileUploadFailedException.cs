namespace Domain.Exceptions;

public class FileUploadFailedException : Exception
{
    public FileUploadFailedException(string message) : base(message) { }
}
