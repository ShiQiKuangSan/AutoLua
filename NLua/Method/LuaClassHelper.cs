using System;

namespace NLua.Method
{
    public class LuaClassHelper
    {
        /*
         *  Gets the function called name from the provided table,
         * returning null if it does not exist
         */
        public static LuaFunction GetTableFunction(LuaTable luaTable, string name)
        {
            if (luaTable == null)
                return null;

            if (luaTable.RawGet(name) is LuaFunction funcObj)
                return funcObj;
            return null;
        }

        /*
         * Calls the provided function with the provided parameters
         */
        public static object CallFunction(LuaFunction function, object[] args, Type[] returnTypes, object[] inArgs, int[] outArgs)
        {
            // args is the return array of arguments, inArgs is the actual array
            // of arguments passed to the function (with in parameters only), outArgs
            // has the positions of out parameters
            object returnValue;
            int iRefArgs;
            var returnValues = function.Call(inArgs, returnTypes);

            if (returnTypes[0] == typeof(void))
            {
                returnValue = null;
                iRefArgs = 0;
            }
            else
            {
                returnValue = returnValues[0];
                iRefArgs = 1;
            }

            foreach (var t in outArgs)
            {
                args[t] = returnValues[iRefArgs];
                iRefArgs++;
            }

            return returnValue;
        }
    }
}