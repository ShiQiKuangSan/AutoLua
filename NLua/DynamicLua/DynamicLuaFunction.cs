using System;
using System.Dynamic;

namespace NLua.DynamicLua
{
    public class DynamicLuaFunction: DynamicObject, IDisposable
    {
        private readonly LuaFunction _function;
        private readonly Lua _state;

        internal DynamicLuaFunction(LuaFunction function, Lua state)
            : base()
        {
            this._function = function;
            this._state = state;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                _function.Dispose();
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = new DynamicArray(_function.Call(args), _state);
            return true;
        }
    }
}