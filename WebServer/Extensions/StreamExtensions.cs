using System.Text;
using WebServer.Exceptions;

namespace WebServer.Extensions;

internal static class StreamExtensions
{
    public static async Task<string> ReadLineAsync(this Stream stream, uint maxSize,
        CancellationToken cancellationToken = default)
    {
        var bytes = new byte[128];
        var foundR = false;
        var offset = 0;
        
        while (true)
        {
            if (offset == bytes.Length)
            {
                Array.Resize(ref bytes, bytes.Length * 2);
            }
            
            var bytesRead = await stream.ReadAsync(bytes.AsMemory(offset, 1), cancellationToken);
            
            StreamThrowHelper.ThrowIfEndOfStream(bytesRead);
            
            if (!foundR && bytes[offset] == '\r')
            {
                foundR = true;
                continue;
            }
            
            if (foundR)
            {
                if (bytes[offset] == '\n') break;
                 
                throw new ParseRequestException("Invalid line ending");
            }
    
            offset++;
            
            if (offset == maxSize)
            {
                throw new ParseRequestException("Line is too long");
            }
        }
        return offset == 0 ? string.Empty : Encoding.ASCII.GetString(bytes, 0, offset);
    }
}