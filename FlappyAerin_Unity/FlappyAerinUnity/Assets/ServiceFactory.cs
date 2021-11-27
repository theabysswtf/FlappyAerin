using System.Collections;

public static class ServiceFactory
{
    static readonly Hashtable Services = new Hashtable();
    public static void AddService<T>(T t) where T:IService
    {
        Services.Add(typeof(T), t);
    }
    public static T GetService<T>()
    {
        return (T) Services[typeof(T)];
    }
}

public interface IService { }