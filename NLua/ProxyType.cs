using System;
using System.Reflection;

namespace NLua
{
    /// <summary>
    /// Summary description for ProxyType.
    /// </summary>
    public class ProxyType
    {
        public ProxyType(Type proxy)
        {
            UnderlyingSystemType = proxy;
        }

        /// <summary>
        /// Provide human readable short hand for this proxy object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ProxyType(" + UnderlyingSystemType + ")";
        }

        public Type UnderlyingSystemType { get; }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Type type:
                    return UnderlyingSystemType == type;
                case ProxyType proxyType:
                    return UnderlyingSystemType == proxyType.UnderlyingSystemType;
                default:
                    return UnderlyingSystemType.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return UnderlyingSystemType.GetHashCode();
        }

        public MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            return UnderlyingSystemType.GetMember(name, bindingAttr);
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Type[] signature)
        {
            return UnderlyingSystemType.GetMethod(name, bindingAttr, null, signature, null);
        }
    }
}