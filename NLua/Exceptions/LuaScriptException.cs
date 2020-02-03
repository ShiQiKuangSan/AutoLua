using System;

namespace NLua.Exceptions
{
    /// <summary>
    /// Exceptions thrown by the Lua runtime because of errors in the script
    /// </summary>
    /// 
    [Serializable]
    public class LuaScriptException : LuaException
    {
        private readonly string _source;

        /// <summary>
        /// The position in the script where the exception was triggered.
        /// </summary>
        public override string Source => _source;

        /// <summary>
        /// Creates a new Lua-only exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="source">The position in the script where the exception was triggered.</param>
        public LuaScriptException(string message, string source) : base(message)
        {
            _source = source;
        }

        public override string ToString()
        {
            // Prepend the error source
            return GetType().FullName + ": " + _source + Message;
        }
    }
}