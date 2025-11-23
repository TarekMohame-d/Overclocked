namespace Domain.Exceptions;

public class FileUploadFailedException(string message) : Exception(message);
