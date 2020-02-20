using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace NLua.DynamicLua
{
    public class DynamicArray : DynamicObject, IEnumerable, IEnumerable<object>, IEnumerable<string>
    {
        private readonly object[] _array;
        private readonly Lua _state;
        
        internal DynamicArray(object[] array, Lua state)
        {
            _array = array ?? new object[0];
            _state = state;
        }
        
        public int Length => _array.Length;

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = null;
            
            if (!int.TryParse(indexes[0].ToString(), out var index))
                return false;
            
            if (index >= _array.Length)
                return false;

            result = LuaHelper.UnWrapObject(_array[index], _state);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            
            if (!int.TryParse(binder.Name, out var index))
                return false;
            
            if (index >= _array.Length)
                return false;

            result = LuaHelper.UnWrapObject(_array[index], _state);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            for (var i = 0; i < _array.Length; i++)
            {
                yield return i.ToString();
            }
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = null;
            if (binder.Type == typeof(object[]))
            {
                result = _array;
                return true;
            }

            if (_array.Length != 1)
                return false;

            if (_array[0].GetType() == typeof(LuaTable) || _array[0].GetType() == typeof(LuaFunction))
            {
                result = LuaHelper.UnWrapObject(_array[0], _state);
                return true;
            }

            if (_array[0].GetType() != binder.Type)
                return false;

            result = Convert.ChangeType(_array[0], binder.Type);
            
            return true;
        }

        public override string ToString()
        {
            return _array.Length == 1 ? _array[0].ToString() : _array.ToString();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return _array.Select(t => t.ToString()).GetEnumerator();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return ((IEnumerable<object>) _array).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }
    }
}