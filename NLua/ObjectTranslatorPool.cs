using System;
using System.Collections.Concurrent;

using LuaState = KeraLua.Lua;

namespace NLua
{
    internal class ObjectTranslatorPool
    {
        private static volatile ObjectTranslatorPool _instance = new ObjectTranslatorPool();

        private readonly ConcurrentDictionary<LuaState, ObjectTranslator> _translators = new ConcurrentDictionary<LuaState, ObjectTranslator>();

        public static ObjectTranslatorPool Instance => _instance;


        public void Add(LuaState luaState, ObjectTranslator translator)
        {
            if(!_translators.TryAdd(luaState, translator))
                throw new ArgumentException("An item with the same key has already been added. ", "luaState");
        }

        public ObjectTranslator Find(LuaState luaState)
        {
            if (_translators.TryGetValue(luaState, out var translator)) 
                return translator;
            
            var main = luaState.MainThread;

            if (!_translators.TryGetValue(main, out translator))
                return null;
            
            return translator;
        }

        public void Remove(LuaState luaState)
        {
            _translators.TryRemove(luaState, out _);
        }
    }
}

