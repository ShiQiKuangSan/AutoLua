using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace NLua.DynamicLua
{
    public class DynamicLuaTable : DynamicObject, IEnumerable, IEnumerable<KeyValuePair<object, object>>, IDisposable
    {
        private readonly LuaTable _table;
        private readonly Lua _state;
        private readonly string _path;

        internal DynamicLuaTable(LuaTable table, Lua state, string path = null)
            : base()
        {
            this._table = table;
            this._state = state;
            if (path == null)
                path = LuaHelper.GetRandomString(); //tables need a name, so we can access them
            this._path = path;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                _table.Dispose();
        }

        public ICollection Keys => _table.Keys;

        public ICollection Values => _table.Values;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<KeyValuePair<object, object>>).GetEnumerator();
        }

        IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
        {
            return _table.GetEnumerator() as IEnumerator<KeyValuePair<object, object>>;
        }

        public dynamic GetMetatable()
        {
            return LuaHelper.UnWrapObject(
                _state.DoString($"return getmetatable({_path})", "DynamicLua internal operation")[0], _state);
        }

        public void SetMetatable(DynamicLuaTable tab)
        {
            _state.DoString($"setmetatable({_path}, {tab._path})", "DynamicLua internal operation");
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetTableValue(binder.Name);
            return true;
        }

        private object GetTableValue(string name)
        {
            var val = _table[name];

            if (val != null)
                return LuaHelper.UnWrapObject(val, _state, _path + "." + name);

            //not found, check __index of metatable

            var func = GetMetaFunction("index");

            if (func != null) //metatable and metamethod set
                return LuaHelper.UnWrapObject(func.Call(_table, name)[0], _state, _path + "." + name);

            return null;
        }

        private LuaFunction GetMetaFunction(string name)
        {
            if ((bool) _state.DoString($"return debug.getmetatable({_path}) == nil", "DynamicLua internal operation")[0])
                return null; //Metatable not set

            //This is NO performace problem, according to my benchmarks, debug.gmt() is even faster than the normal gmt()! (But you need 1 billion operations to messuare it... on my PC)
            var funcOrNative = _state.DoString($"return debug.getmetatable({_path}).__{name}",
                "DynamicLua internal operation")[0];

            switch (funcOrNative)
            {
                //Either the metatable is not set, or the requested function is not found. Just return null.
                case null:
                    return null;
                case LuaFunction function:
                    return function;
                //We need to warp it manually
                default:
                    return (LuaFunction) typeof(LuaFunction)
                        .GetConstructor(new Type[] {funcOrNative.GetType(), typeof(Lua)})
                        ?.Invoke(new[] {funcOrNative, _state});
            }
        }


        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetTableMember(binder.Name, value);
            return true;
        }

        private void SetTableMember(string name, object value)
        {
            var tmp = LuaHelper.WrapObject(value, _state, _path + "." + name);

            if (tmp == null)
                return;

            var func = GetMetaFunction("newindex");
            if (func != null)
                func.Call(_table, name, value);
            else
                _table[name] = value;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetTableValue(indexes[0].ToString());
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            SetTableMember(indexes[0].ToString(), value);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            for (var i = 0; i < args.Length; i++)
                args[i] = LuaHelper.WrapObject(args[i], _state);

            var func = GetMetaFunction("call");
            if (func != null)
            {
                if (args.Length == 0)
                    result = new DynamicArray(func.Call(_table), _state);
                else
                    result = new DynamicArray(func.Call(new object[] {_table}.Concat(args).ToArray()), _state);

                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            string metamethodName;
            
            var switchOperands = false; //Lua has only comparison metamethods for less, so for greater the we have to switch the operands
            var negateResult = false; //Lua has only metamethods for equal, so we need this trick here.
            
            switch (binder.Operation)
            {
                //Math
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked: //TODO: Thing about __concat
                    metamethodName = "add";
                    break;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    metamethodName = "sub";
                    break;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                    metamethodName = "mul";
                    break;
                case ExpressionType.Divide:
                case ExpressionType.DivideAssign:
                    metamethodName = "div";
                    break;
                case ExpressionType.Modulo:
                case ExpressionType.ModuloAssign:
                    metamethodName = "mod";
                    break;
                case ExpressionType.Power:
                case ExpressionType.PowerAssign:
                    metamethodName = "pow";
                    break;
                //Logic Tests
                case ExpressionType.Equal:
                    metamethodName = "eq";
                    break;
                case ExpressionType.NotEqual:
                    metamethodName = "eq";
                    negateResult = true;
                    break;
                case ExpressionType.GreaterThan:
                    metamethodName = "lt";
                    switchOperands = true;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    metamethodName = "le";
                    switchOperands = true;
                    break;
                case ExpressionType.LessThan:
                    metamethodName = "lt";
                    break;
                case ExpressionType.LessThanOrEqual:
                    metamethodName = "le";
                    break;
                default: //This operation is not supported by Lua...
                    result = null;
                    return false;
            }

            var mtFunc = GetMetaFunction(metamethodName);

            if (mtFunc == null)
            {
                result = null;
                return false;
            }

            if (!switchOperands)
                //Metamethods just return one value, or the other will be ignored anyway
                result = mtFunc.Call(_table, LuaHelper.WrapObject(arg, _state)) [0]; 
            else
                result = mtFunc.Call(LuaHelper.WrapObject(arg, _state), _table)[0];

            //We can't negate if its not bool. If the metamethod returned someting other than bool and ~= is called there will be a bug. (But who would do this?)
            if (negateResult && result is bool b)
                result = !b;

            return true;
        }
        
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.Negate || binder.Operation == ExpressionType.NegateChecked)
            {
                result = GetMetaFunction("unm").Call(_table)[0];
                return true;
            }
            else
            {
                result = null; //Operation not supported
                return false;
            }
        }
        
        public override string ToString()
        {
            var func = GetMetaFunction("tostring");
            
            if (func != null)
                return func.Call(_table)[0].ToString();
            
            return _table.ToString();
        }
        
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(LuaTable))
            {
                result = _table;
                return true;
            }

            if (binder.Type == typeof(string))
            {
                result = ToString();
                return true;
            }

            result = null;
            return false;
        }
        
        public dynamic Power(object operand)
        {
            operand = LuaHelper.WrapObject(operand, _state);

            var func = GetMetaFunction("pow");
            
            if (func != null)
                return func.Call(_table, operand);
            
            throw new InvalidOperationException("Metamethod __pow not found");
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return _table.Equals(((DynamicLuaTable)obj)._table);
        }
        
        public override int GetHashCode()
        {
            return _table.GetHashCode();
        }
        
        public static bool operator ==(DynamicLuaTable a, DynamicLuaTable b)
        {
            return a != null && a.Equals(b);
        }

        public static bool operator !=(DynamicLuaTable a, DynamicLuaTable b)
        {
            return a != null && !a.Equals(b);
        }
    }
}