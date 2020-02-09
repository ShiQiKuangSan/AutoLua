
using System;

namespace NLua
{
    public class LuaUserData : LuaBase
    {
        public LuaUserData(int reference, Lua interpreter):base(reference, interpreter)
        {
        }

        /*
         * Indexer for string fields of the userdata
         */
        public object this[string field] {
            get
            {
                if (!TryGet(out var lua))
                    return null;

                return lua.GetObject(Reference, field);
            }
            set
            {
                if (!TryGet(out var lua))
                    return;

                lua.SetObject(Reference, field, value);
            }
        }

        /*
         * Indexer for numeric fields of the userdata
         */
        public object this[object field] {
            get
            {
                if (!TryGet(out var lua))
                    return null;

                return lua.GetObject(Reference, field);
            }
            set
            {
                if (!TryGet(out var lua))
                    return;

                lua.SetObject(Reference, field, value);
            }
        }

        /*
         * Calls the userdata and returns its return values inside
         * an array
         */
        public object[] Call(params object[] args)
        {
            if (!TryGet(out var lua))
                return null;

            return lua.CallFunction(this, args);
        }

        public override string ToString()
        {
            return "userdata";
        }
    }
}