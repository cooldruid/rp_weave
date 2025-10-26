namespace RpWeave.Server.Core.Startup;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ScopedServiceAttribute(Type? serviceType = null) : Attribute
{
    public Type? ServiceType { get; } = serviceType;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SingletonServiceAttribute(Type? serviceType = null) : Attribute
{
    public Type? ServiceType { get; } = serviceType;
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TransientServiceAttribute(Type? serviceType = null) : Attribute
{
    public Type? ServiceType { get; } = serviceType;
}