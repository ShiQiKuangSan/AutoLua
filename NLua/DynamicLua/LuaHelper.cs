using System;
using System.Text;

namespace NLua.DynamicLua
{
    internal class LuaHelper
    {
        public const int RandomNameLength = 8;

        public static Random Random { get; }

        static LuaHelper() //static ctor
        {
            Random = new Random();
        }


        /// <summary>
        /// Returns a random string with the specified lenght. Use the overload
        /// without paramters for the default length.
        /// This can be safely used as a Lua Variable Name, but is not checked
        /// for collsions.
        /// </summary>
        /// <remarks>see http://dotnet-snippets.de/dns/passwort-generieren-SID147.aspx</remarks>
        public static string GetRandomString(int lenght = RandomNameLength)
        {
            var sb = new StringBuilder();
            const string content = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            for (var i = 0; i < lenght; i++)
                sb.Append(content[Random.Next(content.Length)]);
            return sb.ToString();
        }

        /// <summary>
        /// Unwaps an object comming from LuaInterface for use in DynamicLua.
        /// </summary>
        public static object UnWrapObject(object wrapped, Lua state, string name = null)
        {
            return wrapped switch
            {
                LuaTable table => new DynamicLuaTable(table, state, name),
                LuaFunction function => new DynamicLuaFunction(function, state),
                MulticastDelegate _ => new DynamicLuaFunction(state.GetFunction(name), state),
                _ => wrapped
            };
        }

        /// <summary>
        /// Wraps an object to prepare it for passing to LuaInterface. If no name is specified, a 
        /// random one with the default length is used.
        /// </summary>
        public static object WrapObject(object toWrap, Lua state, string name = null)
        {
            if (toWrap is DynamicArray)
            {
                //Someone passed an DynamicArray diretly back to Lua.
                //He wanted to pass the value in the array, so we extract it.
                //When there is more than one value, this method will ignore these extra value.
                //This could happen in a situation like this: lua.tmp = lua("return a,b");, but
                //that doesn't make sense.
                toWrap = ((dynamic) toWrap)[0];
            }

            if (toWrap is MulticastDelegate @delegate)
            {
                //We have to deal with a problem here: RegisterFunction does not really create
                //a new function, but a userdata with a __call metamethod. This works fine in all
                //except two cases: When Lua looks for an __index or __newindex metafunction and finds
                //a table or userdata, Lua tries to redirect the read/write operation to that table/userdata.
                //In case of our function that is in reality a userdata this fails. So we have to check
                //for these function and create a very thin wrapper arround this to make Lua see a function instead
                //the real userdata. This is no problem for the other metamethods, these are called independent
                //from their type. (If they are not nil ;))
                var function = @delegate;

                if (name != null && (name.EndsWith("__index") || name.EndsWith("__newindex")))
                {
                    var tmpName = GetRandomString();
                    state.RegisterFunction(tmpName, function.Target, function.Method);
                    state.DoString($"function {name}(...) return {tmpName}(...) end",
                        "DynamicLua internal operation");
                }
                else
                {
                    if (name == null)
                        name = GetRandomString();
                    state.RegisterFunction(name, function.Target, function.Method);
                }

                return null;
            }
            else if (toWrap is DynamicLuaTable wrap)
            {
                dynamic dlt = wrap;
                return (LuaTable) dlt;
            }
            else
                return toWrap;
        }
    }
}