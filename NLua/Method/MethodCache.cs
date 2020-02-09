using System;
using System.Reflection;
using NLua.Extensions;

namespace NLua.Method
{
    internal class MethodCache
    {
        public MethodCache()
        {
            Args = new object[0];
            ArgTypes = new MethodArgs[0];
            OutList = new int[0];
        }
        private MethodBase _cachedMethod;

        public MethodBase CachedMethod {
            get => _cachedMethod;
            set
            {
                _cachedMethod = value;
                var mi = value as MethodInfo;

                if (mi != null)
                {
                    IsReturnVoid = mi.ReturnType == typeof(void);
                }
            }
        }

        public bool IsReturnVoid;
        // List or arguments
        public object[] Args;
        // Positions of out parameters
        public int[] OutList;
        // Types of parameters
        public MethodArgs[] ArgTypes;
    }
}