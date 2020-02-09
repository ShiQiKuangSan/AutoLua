using System;

namespace NLua
{
    /*
     * Structure to store a type and the return types of
     * its methods (the type of the returned value and out/ref
     * parameters).
     */
    internal struct LuaClassType
    {
        public Type Klass;
        public Type[][] ReturnTypes;
    }
}