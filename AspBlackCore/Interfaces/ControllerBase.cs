namespace AspBlackCore.Interfaces;

public abstract class ControllerBase
{
    public required HttpContext Context { get; init; }
}