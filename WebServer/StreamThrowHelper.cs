using WebServer.Exceptions;

namespace WebServer;

internal static class StreamThrowHelper
{
    public static void ThrowIfEndOfStream(int bytesRead)
    {
        if (bytesRead == 0) throw new ParseRequestException("Unexpected end of stream");
    }
}