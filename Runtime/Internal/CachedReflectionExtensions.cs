using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace UniMediator.Internal
{
    // Extension methods that cache the results of reflection operations.
    // Significantly improves performance if multiple instances of the 
    // same type need to be reflected
    
    internal static class CachedReflectionExtensions
    {
        static Dictionary<Type, Type[]> _interfaces = new Dictionary<Type, Type[]>();
        static Dictionary<Type, MethodInfo[]> _methods = new Dictionary<Type, MethodInfo[]>();
        static Dictionary<MethodInfo, ParameterInfo[]> _parameters = new Dictionary<MethodInfo, ParameterInfo[]>();
        static Dictionary<GenericMethodKey, MethodInfo> _genericMethods = new Dictionary<GenericMethodKey, MethodInfo>();
        
        public static Type[] GetCachedInterfaces(this Type type)
        {
            if (!_interfaces.TryGetValue(type, out var interfaces))
            {
                interfaces = type.GetInterfaces();
                _interfaces[type] = interfaces;
            }

            return interfaces;
        }
        
        public static MethodInfo[] GetCachedMethods(this Type type)
        {
            if (!_methods.TryGetValue(type, out var methods))
            {
                methods = type.GetMethods();
                _methods[type] = methods;
            }

            return methods;
        }

        public static ParameterInfo[] GetCachedParameters(this MethodInfo method)
        {
            if (!_parameters.TryGetValue(method, out var parameters))
            {
                parameters = method.GetParameters();
                _parameters[method] = parameters;
            }

            return parameters;
        }
        
        public static MethodInfo GetCachedGenericMethod(this Type type, string name, BindingFlags bindingFlags, Type typeArgument)
        {
            var key = new GenericMethodKey(type, name, typeArgument, null);
            return GetCachedGenericMethod(key, bindingFlags);
        }
        
        public static MethodInfo GetCachedGenericMethod(this Type type, string name, BindingFlags bindingFlags, Type typeArgument1, Type typeArgument2)
        {
            var key = new GenericMethodKey(type, name, typeArgument1, typeArgument2);
            return GetCachedGenericMethod(key, bindingFlags);
        }
        
        private static MethodInfo GetCachedGenericMethod(GenericMethodKey key, BindingFlags bindingFlags)
        {
            if (!_genericMethods.TryGetValue(key, out var genericMethod))
            {
                var method = key.InstanceType.GetMethod(key.MethodName, bindingFlags);
                genericMethod = method.MakeGenericMethod(key.GetTypeArguments());
                _genericMethods[key ] = genericMethod;
            }
            return genericMethod;
        }
        
        public static bool ImplementsGenericInterface(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetCachedInterfaces();
            
            for (int i = 0; i < interfaceTypes.Length; i++)
            {
                if (interfaceTypes[i].IsGenericType && 
                    interfaceTypes[i].GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}