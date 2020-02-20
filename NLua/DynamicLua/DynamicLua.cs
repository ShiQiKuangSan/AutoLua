using System;
using System.Dynamic;
using System.Text;

namespace NLua.DynamicLua
{
    public class DynamicLua : DynamicObject, IDisposable
    {
        public DynamicLua()
        {
            Lua = new Lua();
            Lua.State.Encoding = Encoding.UTF8;
            Lua.LoadCLRPackage();
        }

        internal Lua Lua { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Lua.Dispose();
        }
        
        public DynamicArray DoFile(string path)
        {
            return new DynamicArray(Lua.DoFile(path), Lua);
        }

        public DynamicLuaFunction LoadFile(string path)
        {
            return new DynamicLuaFunction(Lua.LoadFile(path), Lua);
        }

        public DynamicLuaFunction LoadString(string chunk, string name = "")
        {
            return new DynamicLuaFunction(Lua.LoadString(chunk, name), Lua);
        }

        public DynamicLuaTable NewTable(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The name cannot be null or emtpy.", "name");
            
            Lua.NewTable(name);
            return new DynamicLuaTable(Lua[name] as LuaTable, Lua, name);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetLuaValue(binder.Name);
            return true;
        }

        private object GetLuaValue(string name)
        {
            return LuaHelper.UnWrapObject(Lua[name], Lua, name);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetLuaMember(binder.Name, value);
            return true;
        }

        private void SetLuaMember(string name, object value)
        {
            var tmp = LuaHelper.WrapObject(value, Lua, name);
            
            if (tmp != null) //if a function was registered tmp is null, but we dont want to nil the function :P
                Lua[name] = tmp;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetLuaValue(indexes[0].ToString());
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            SetLuaMember(indexes[0].ToString(), value);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            result = new DynamicArray(Lua.DoString(args[0].ToString()), Lua);
            return true;
        }
    }
}