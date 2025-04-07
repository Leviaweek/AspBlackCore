namespace AspBlackCore.Interfaces;

public interface IHttpResponseBuilder
{
    public IHttpResponseBuilder SetBody<T>(T body);
    public IHttpResponseBuilder SetStatusCode(int statusCode);
}