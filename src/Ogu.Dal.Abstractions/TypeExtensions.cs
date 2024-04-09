using System;
using System.Linq;
using System.Reflection;

namespace Ogu.Dal.Abstractions
{
    public static class TypeExtensions
    {
        public static bool IsTypeOfGeneric(this Type genericType, Type toCheck)
        {
            if (!toCheck.IsGenericType)
                return false;

            return genericType == toCheck.GetTypeInfo().GetGenericTypeDefinition();
        }

        public static bool IsSubTypeOfRawGeneric(this Type genericType, Type toCheck)
        {
            return genericType.GetTypeInfo().IsInterface ? (toCheck.GetTypeInfo().IsClass && IsInterfaceOfRawGeneric(genericType, toCheck)) : IsSubclassOfRawGeneric(genericType, toCheck);
        }

        public static bool IsInterfaceOfRawGeneric(this Type genericType, Type toCheck)
        {
            return genericType.GetInterfaces().Any(i => (i.GetTypeInfo().IsGenericType ? i.GetGenericTypeDefinition() : i) == toCheck);
        }

        public static bool IsSubclassOfRawGeneric(this Type genericType, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (genericType == cur)
                {
                    return true;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
            }
            return false;
        }
    }
}