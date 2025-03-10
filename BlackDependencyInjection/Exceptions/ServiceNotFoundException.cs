namespace BlackDependencyInjection.Exceptions;

public class ServiceNotFoundException : DependencyInjectionException
{
    public ServiceNotFoundException(Type serviceType) : base($"Service {serviceType.FullName} not found") { }
}