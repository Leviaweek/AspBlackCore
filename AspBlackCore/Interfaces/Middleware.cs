using AspBlackCore.Delegates;

namespace AspBlackCore.Interfaces;

public abstract class Middleware
{
    protected Middleware(RequestDelegate next)
    {
        Next = next;
    }
    
    protected readonly RequestDelegate Next;
    public abstract Task InvokeAsync(HttpContext context);
}