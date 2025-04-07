using System.Text.Json;
using AspBlackCore.Interfaces;

namespace AspBlackCore.Builders;

internal sealed class HttpResponseBuilder: HttpResponseBuilderBase
{
    public override IHttpResponseBuilder SetBody<T>(T body)
    {
        AddHeader("Content-Type", "application/json");
        Body = JsonSerializer.SerializeToUtf8Bytes(body);
        AddHeader("Content-Length", Body.Length.ToString());
        return this;
    }
}