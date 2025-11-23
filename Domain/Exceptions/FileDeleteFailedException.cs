namespace Domain.Exceptions;

public class FileDeleteFailedException(string message) : Exception(message);
