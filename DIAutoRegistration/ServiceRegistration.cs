using System.Reflection;
using BlackDependencyInjection;
using BlackDependencyInjection.Interfaces;
using DIAutoRegistration.Attributes;

namespace DIAutoRegistration;

public static class ServiceRegistration
{
    public static void RegisterAllServices(this IBlackServiceCollection services)
    {
        var serviceTypes = GetAllAssembliesServiceTypes();

        foreach (var serviceType in serviceTypes)
        {
            var attribute = serviceType.GetCustomAttribute<ServiceAttribute>()!;
            
            var baseType = GetBaseType(serviceType, attribute);

            var factoryAttribute = TryGetFactoryMethodAttribute(serviceType);

            var factory = TryGetFactory(factoryAttribute, serviceType);

            services[baseType] = new ServiceDescriptor(
                baseType,
                serviceType,
                attribute.Lifetime,
                factory);
        }
    }

    private static FactoryMethodAttribute? TryGetFactoryMethodAttribute(Type serviceType)
    {
        var method = serviceType.GetMethod(FactoryMethodAttribute.Name, BindingFlags.Static | BindingFlags.Public);

        var attribute = method?.GetCustomAttribute<FactoryMethodAttribute>();
        if (attribute is null) return null;
        
        if (method!.ReturnType == serviceType &&
            method.GetParameters() is
            [{
                    ParameterType: var parameterType 
            }] &&
            parameterType == typeof(IBlackServiceProvider))
        {
            return attribute;
        }
        
        throw new InvalidOperationException(
            $"The factory method {method.Name} must have one parameter of type {nameof(IBlackServiceProvider)}.");
    }

    private static Func<IBlackServiceProvider, object>? TryGetFactory(FactoryMethodAttribute? factoryAttribute, Type serviceType)
    {
        if (factoryAttribute is null) return null;
        
        var factoryMethod = serviceType.GetMethod(FactoryMethodAttribute.Name);
        if (factoryMethod is null) return null;
        var factoryDelegate = (Func<IBlackServiceProvider, object>)Delegate.CreateDelegate(
            typeof(Func<IBlackServiceProvider, object>),null ,factoryMethod);

        return factoryDelegate;

    }

    private static IEnumerable<Type> GetAllAssembliesServiceTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } &&
                        t.GetCustomAttribute<ServiceAttribute>() is not null);
    }

    private static Type GetBaseType(Type implementationType, ServiceAttribute attribute)
    {
        var serviceType = attribute.BaseType;

        if (attribute.GetType().IsGenericType)
        {
            serviceType = attribute.GetType().GetGenericArguments().First();
            
            if (!implementationType.IsAssignableTo(serviceType))
            {
                throw new InvalidOperationException(
                    $"The service type {serviceType} is not assignable to the implementation type {implementationType}");
            }

            return serviceType;
        }

        if (serviceType is null)
        {
            var interfaces = implementationType.GetInterfaces();

            if (interfaces.Length != 0)
            {
                return interfaces.First();
            }
            
            var baseType = implementationType.BaseType;
            
            if (baseType == typeof(object))
                return implementationType;
            
            throw new InvalidOperationException(
                $"The service type {serviceType} is not assignable to the implementation type {implementationType}");
        }
        
        if (!implementationType.IsAssignableTo(serviceType))
        {
            throw new InvalidOperationException(
                $"The service type {serviceType} is not assignable to the implementation type {implementationType}");
        }

        return serviceType;
    }
}