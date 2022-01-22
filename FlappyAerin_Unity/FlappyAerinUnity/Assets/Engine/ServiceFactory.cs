using System.Collections;
using UnityEngine;

namespace Engine
{
    public static class ServiceFactory
    {
        static readonly Hashtable Services = new();
        public static void AddService<T>(T t) where T:IService
        {
            Services.Add(typeof(T), t);
        }
        public static T GetService<T>()
        {
            return (T) Services[typeof(T)];
        }
        
        // Method to remove service!
        // Each object should call this on their way out!
    }

    public interface IService { }
}