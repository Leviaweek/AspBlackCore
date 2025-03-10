namespace WebServer.Exceptions;

public class ParseRequestException(string? message = null) : Exception(message);