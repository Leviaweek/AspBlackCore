using AspBlackCore.Interfaces;
using WebServer.Interfaces;

namespace AspBlackCore;

public sealed class HttpContext
{
    public IHttpRequest Request { get; set; }
    public IHttpResponseBuilder Response { get; set; }
    
    public HttpContext(IHttpRequest request, IHttpResponseBuilder response)
    {
        Request = request;
        Response = response;
    }
}

public sealed class BlackAppContext
{
    public BlackAppContext(CancellationTokenSource cancellationTokenSource)
    {
        CancellationTokenSource = cancellationTokenSource;
    }

    public CancellationTokenSource CancellationTokenSource { get; }
}