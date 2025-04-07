using AspBlackCore.Builders;
using AspBlackCore.Interfaces;

namespace AspBlackCore.Results;

public static class BlackResults
{
    public static IHttpResponseBuilder Ok()
    {
        return new HttpResponseBuilder()
            .SetStatusCode(200);
    }
    
    public static IHttpResponseBuilder Ok<T>(T body)
    {
        return new HttpResponseBuilder()
            .SetStatusCode(200)
            .SetBody(body);
    }
    
    public static IHttpResponseBuilder BadRequest()
    {
        return new HttpResponseBuilder()
            .SetStatusCode(400);
    }
    
    public static IHttpResponseBuilder BadRequest<T>(T body)
    {
        return new HttpResponseBuilder()
            .SetStatusCode(400)
            .SetBody(body);
    }
}