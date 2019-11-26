using System;
using System.Collections.Generic;

namespace UniMediator.Internal
{
    internal struct GenericMethodKey : IEquatable<GenericMethodKey>
    {
        internal GenericMethodKey(
            Type instanceType, 
            string methodName, 
            Type typeArgument1, 
            Type typeArgument2)
        {
            InstanceType = instanceType;
            MethodName = methodName;
            TypeArgument1 = typeArgument1;
            TypeArgument2 = typeArgument2;
        }

        internal readonly Type InstanceType;
        internal readonly string MethodName;
        internal readonly Type TypeArgument1;
        internal readonly Type TypeArgument2;

        public Type[] GetTypeArguments()
        {
            return TypeArgument2 == null ? new[] {TypeArgument1} : new[] {TypeArgument1, TypeArgument2};
        }
        
        public bool Equals(GenericMethodKey other)
        {
            return InstanceType == other.InstanceType && 
                   Equals(MethodName, other.MethodName) && 
                   TypeArgument1 == other.TypeArgument1 && 
                   TypeArgument2 == other.TypeArgument2;
        }

        public override bool Equals(object obj)
        {
            return obj is GenericMethodKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = InstanceType != null ? InstanceType.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TypeArgument1 != null ? TypeArgument1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TypeArgument2 != null ? TypeArgument2.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(GenericMethodKey left, GenericMethodKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GenericMethodKey left, GenericMethodKey right)
        {
           return !left.Equals(right);
        }
    }
}