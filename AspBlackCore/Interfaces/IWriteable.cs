namespace AspBlackCore.Interfaces;

public interface IWriteable
{
    public Task WriteAsync(Stream stream, CancellationToken cancellationToken);
}