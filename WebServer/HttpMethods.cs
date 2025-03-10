using HttpMethod = WebServer.Enums.HttpMethod;

namespace WebServer;

public static class HttpMethods
{
    public static string Get => HttpMethod.Get.ToString().ToUpper();
    public static string Post => HttpMethod.Post.ToString().ToUpper();
    public static string Put => HttpMethod.Put.ToString().ToUpper();
    public static string Delete => HttpMethod.Delete.ToString().ToUpper();
}