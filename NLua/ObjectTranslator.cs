using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.InteropServices;
using KeraLua;

using NLua.Method;
using NLua.Exceptions;
using NLua.Extensions;

#if __IOS__ || __TVOS__ || __WATCHOS__
    using ObjCRuntime;
#endif

using LuaState = KeraLua.Lua;
using LuaNativeFunction = KeraLua.LuaFunction;

namespace NLua
{
    public class ObjectTranslator
    {
        // Compare cache entries by exact reference to avoid unwanted aliases
        private class ReferenceComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                if (x != null && y != null && x.GetType() == y.GetType() && x.GetType().IsValueType && y.GetType().IsValueType)
                    return x.Equals(y); // Special case for boxed value types
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return obj.GetHashCode();
            }
        }

        private static readonly LuaNativeFunction RegisterTableFunction = RegisterTable;
        private static readonly LuaNativeFunction UnregisterTableFunction = UnregisterTable;
        private static readonly LuaNativeFunction GetMethodSigFunction = GetMethodSignature;
        private static readonly LuaNativeFunction GetConstructorSigFunction = GetConstructorSignature;
        private static readonly LuaNativeFunction ImportTypeFunction = ImportType;
        private static readonly LuaNativeFunction LoadAssemblyFunction = LoadAssembly;
        private static readonly LuaNativeFunction CtypeFunction = CType;
        private static readonly LuaNativeFunction EnumFromIntFunction = EnumFromInt;

        // object to object #
        private readonly Dictionary<object, int> _objectsBackMap = new Dictionary<object, int>(new ReferenceComparer());
        // object # to object (FIXME - it should be possible to get object address as an object #)
        private readonly Dictionary<int, object> _objects = new Dictionary<int, object>();

        private readonly ConcurrentQueue<int> _finalizedReferences = new ConcurrentQueue<int>();

        internal readonly EventHandlerContainer PendingEvents = new EventHandlerContainer();
        private readonly List<Assembly> _assemblies;
        internal readonly CheckType TypeChecker;
        internal readonly Lua _interpreter;
        /// <summary>
        /// We want to ensure that objects always have a unique ID
        /// </summary>
        private int _nextObj;

        public MetaFunctions MetaFunctionsInstance { get; }

        public Lua Interpreter => _interpreter;
        public IntPtr Tag { get; }

        public ObjectTranslator(Lua interpreter, LuaState luaState)
        {
            Tag = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
            this._interpreter = interpreter;
            TypeChecker = new CheckType(this);
            MetaFunctionsInstance = new MetaFunctions(this);
            _assemblies = new List<Assembly>();

            CreateLuaObjectList(luaState);
            CreateIndexingMetaFunction(luaState);
            CreateBaseClassMetatable(luaState);
            CreateClassMetatable(luaState);
            CreateFunctionMetatable(luaState);
            SetGlobalFunctions(luaState);

        }

        /*
         * Sets up the list of objects in the Lua side
         */
        private static void CreateLuaObjectList(LuaState luaState)
        {
            luaState.PushString("luaNet_objects");
            luaState.NewTable();
            luaState.NewTable();
            luaState.PushString("__mode");
            luaState.PushString("v");
            luaState.SetTable(-3);
            luaState.SetMetaTable(-2);
            luaState.SetTable((int)LuaRegistry.Index);
        }

        /*
         * Registers the indexing function of CLR objects
         * passed to Lua
         */
        private static void CreateIndexingMetaFunction(LuaState luaState)
        {
            luaState.PushString("luaNet_indexfunction");
            luaState.DoString(MetaFunctions.LuaIndexFunction);
            luaState.RawSet(LuaRegistry.Index);
        }

        /*
         * Creates the metatable for superclasses (the base
         * field of registered tables)
         */
        private static void CreateBaseClassMetatable(LuaState luaState)
        {
            luaState.NewMetaTable("luaNet_searchbase");
            luaState.PushString("__gc");
            luaState.PushCFunction(MetaFunctions.GcFunction);
            luaState.SetTable(-3);
            luaState.PushString("__tostring");
            luaState.PushCFunction(MetaFunctions.ToStringFunction);
            luaState.SetTable(-3);
            luaState.PushString("__index");
            luaState.PushCFunction(MetaFunctions.BaseIndexFunction);
            luaState.SetTable(-3);
            luaState.PushString("__newindex");
            luaState.PushCFunction(MetaFunctions.NewIndexFunction);
            luaState.SetTable(-3);
            luaState.SetTop(-2);
        }

        /*
         * Creates the metatable for type references
         */
        private static void CreateClassMetatable(LuaState luaState)
        {
            luaState.NewMetaTable("luaNet_class");
            luaState.PushString("__gc");
            luaState.PushCFunction(MetaFunctions.GcFunction);
            luaState.SetTable(-3);
            luaState.PushString("__tostring");
            luaState.PushCFunction(MetaFunctions.ToStringFunction);
            luaState.SetTable(-3);
            luaState.PushString("__index");
            luaState.PushCFunction(MetaFunctions.ClassIndexFunction);
            luaState.SetTable(-3);
            luaState.PushString("__newindex");
            luaState.PushCFunction(MetaFunctions.ClassNewIndexFunction);
            luaState.SetTable(-3);
            luaState.PushString("__call");
            luaState.PushCFunction(MetaFunctions.CallConstructorFunction);
            luaState.SetTable(-3);
            luaState.SetTop(-2);
        }

        /*
         * Registers the global functions used by NLua
         */
        private static void SetGlobalFunctions(LuaState luaState)
        {
            luaState.PushCFunction(MetaFunctions.IndexFunction);
            luaState.SetGlobal("get_object_member");
            luaState.PushCFunction(ImportTypeFunction);
            luaState.SetGlobal("import_type");
            luaState.PushCFunction(LoadAssemblyFunction);
            luaState.SetGlobal("load_assembly");
            luaState.PushCFunction(RegisterTableFunction);
            luaState.SetGlobal("make_object");
            luaState.PushCFunction(UnregisterTableFunction);
            luaState.SetGlobal("free_object");
            luaState.PushCFunction(GetMethodSigFunction);
            luaState.SetGlobal("get_method_bysig");
            luaState.PushCFunction(GetConstructorSigFunction);
            luaState.SetGlobal("get_constructor_bysig");
            luaState.PushCFunction(CtypeFunction);
            luaState.SetGlobal("ctype");
            luaState.PushCFunction(EnumFromIntFunction);
            luaState.SetGlobal("enum");
        }

        /*
         * Creates the metatable for delegates
         */
        private static void CreateFunctionMetatable(LuaState luaState)
        {
            luaState.NewMetaTable("luaNet_function");
            luaState.PushString("__gc");
            luaState.PushCFunction(MetaFunctions.GcFunction);
            luaState.SetTable(-3);
            luaState.PushString("__call");
            luaState.PushCFunction(MetaFunctions.ExecuteDelegateFunction);
            luaState.SetTable(-3);
            luaState.SetTop(-2);
        }

        /*
         * Passes errors (argument e) to the Lua interpreter
         */
        internal void ThrowError(LuaState luaState, object e)
        {
            // We use this to remove anything pushed by luaL_where
            var oldTop = luaState.GetTop();

            // Stack frame #1 is our C# wrapper, so not very interesting to the user
            // Stack frame #2 must be the lua code that called us, so that's what we want to use
            luaState.Where(1);
            var curlev = PopValues(luaState, oldTop);

            // Determine the position in the script where the exception was triggered
            var errLocation = string.Empty;

            if (curlev.Length > 0)
                errLocation = curlev[0].ToString();

            switch (e)
            {
                case string message:
                {
                    // Wrap Lua error (just a string) and store the error location
                    if (_interpreter.UseTraceback) 
                        message += Environment.NewLine + _interpreter.GetDebugTraceback();
                    e = new LuaScriptException(message, errLocation);
                    break;
                }
                case Exception ex:
                {
                    // Wrap generic .NET exception as an InnerException and store the error location
                    if (_interpreter.UseTraceback) ex.Data["Traceback"] = _interpreter.GetDebugTraceback();
                    e = new LuaScriptException(ex.Message, errLocation);
                    break;
                }
            }

            Push(luaState, e);
            //luaState.Error();
        }

        /*
         * Implementation of load_assembly. Throws an error
         * if the assembly is not found.
         */
#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int LoadAssembly(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.LoadAssemblyInternal(state);
        }

        private int LoadAssemblyInternal(LuaState luaState)
        {
            try
            {
                var assemblyName = luaState.ToString(1, false);
                Assembly assembly = null;
                Exception exception = null;

                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch (BadImageFormatException)
                {
                    // The assemblyName was invalid.  It is most likely a path.
                }
                catch (FileNotFoundException e)
                {
                    exception = e;
                }

                if (assembly == null)
                {
                    try
                    {
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyName));
                    }
                    catch (FileNotFoundException e)
                    {
                        exception = e;
                    }
                    if (assembly == null)
                    {
                        var mscor = _assemblies[0].GetName();
                        var name = new AssemblyName
                        {
                            Name = assemblyName, 
                            CultureInfo = mscor.CultureInfo, 
                            Version = mscor.Version
                        };
                        
                        name.SetPublicKeyToken(mscor.GetPublicKeyToken());
                        name.SetPublicKey(mscor.GetPublicKey());
                        assembly = Assembly.Load(name);

                        if (assembly != null)
                            exception = null;
                    }
                    if (exception != null)
                        ThrowError(luaState, exception);
                }
                if (assembly != null && !_assemblies.Contains(assembly))
                    _assemblies.Add(assembly);
            }
            catch (Exception e)
            {
                ThrowError(luaState, e);
            }
            return 0;
        }

        internal Type FindType(string className)
        {
            return _assemblies.Select(assembly => assembly.GetType(className)).FirstOrDefault(klass => klass != null);
        }

        public bool TryGetExtensionMethod(Type type, string name, out MethodInfo method)
        {
            method = GetExtensionMethod(type, name);
            return method != null;
        }

        private MethodInfo GetExtensionMethod(Type type, string name)
        {
            return type.GetExtensionMethod(name, _assemblies);
        }

        /*
         * Implementation of import_type. Returns nil if the
         * type is not found.
         */
#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int ImportType(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.ImportTypeInternal(state);
        }

        private int ImportTypeInternal(LuaState luaState)
        {
            var className = luaState.ToString(1, false);
            var klass = FindType(className);

            if (klass != null)
                PushType(luaState, klass);
            else
                luaState.PushNil();

            return 1;
        }

        /*
         * Implementation of make_object. Registers a table (first
         * argument in the stack) as an object subclassing the
         * type passed as second argument in the stack.
         */
#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int RegisterTable(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.RegisterTableInternal(state);
        }

        private int RegisterTableInternal(LuaState luaState)
        {
            if (luaState.Type(1) != LuaType.Table)
            {
                ThrowError(luaState, "register_table: 第一个arg不是表");
                return 0;
            }

            var luaTable = GetTable(luaState, 1);
            var superclassName = luaState.ToString(2, false);

            if (string.IsNullOrEmpty(superclassName))
            {
                ThrowError(luaState, "register_table: 父类名称不能为 null");
                return 0;
            }

            var klass = FindType(superclassName);

            if (klass == null)
            {
                ThrowError(luaState, "register_table: 找不到父类 '" + superclassName + "'");
                return 0;
            }

            // Creates and pushes the object in the stack, setting
            // it as the  metatable of the first argument
            var obj = CodeGeneration.Instance.GetClassInstance(klass, luaTable);
            PushObject(luaState, obj, "luaNet_metatable");
            luaState.NewTable();
            luaState.PushString("__index");
            luaState.PushCopy(-3);
            luaState.SetTable(-3);
            luaState.PushString("__newindex");
            luaState.PushCopy(-3);
            luaState.SetTable(-3);
            luaState.SetMetaTable(1);

            // Pushes the object again, this time as the base field
            // of the table and with the luaNet_searchbase metatable
            luaState.PushString("base");
            var index = AddObject(obj);
            PushNewObject(luaState, obj, index, "luaNet_searchbase");
            luaState.RawSet(1);

            return 0;
        }

        /*
         * Implementation of free_object. Clears the metatable and the
         * base field, freeing the created object for garbage-collection
         */
#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int UnregisterTable(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.UnregisterTableInternal(state);
        }

        private int UnregisterTableInternal(LuaState luaState)
        {

            if (!luaState.GetMetaTable(1))
            {
                ThrowError(luaState, "unregister_table: arg 不是一个有效的 table");
                return 0;
            }

            luaState.PushString("__index");
            luaState.GetTable(-2);
            var obj = GetRawNetObject(luaState, -1);

            if (obj == null)
                ThrowError(luaState, "unregister_table: arg 不是一个有效的 table");

            if (obj != null)
            {
                var luaTableField = obj.GetType().GetField("__luaInterface_luaTable");

                if (luaTableField == null)
                    ThrowError(luaState, "unregister_table: arg 不是一个有效的 table");

                // ReSharper disable once PossibleNullReferenceException
                luaTableField.SetValue(obj, null);
            }

            luaState.PushNil();
            luaState.SetMetaTable(1);
            luaState.PushString("base");
            luaState.PushNil();
            luaState.SetTable(1);

            return 0;
        }

        /*
         * Implementation of get_method_bysig. Returns nil
         * if no matching method is not found.
         */
#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int GetMethodSignature(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.GetMethodSignatureInternal(state);
        }

        private int GetMethodSignatureInternal(LuaState luaState)
        {
            ProxyType klass;
            object target;
            var udata = luaState.CheckUObject(1, "luaNet_class");

            if (udata != -1)
            {
                klass = (ProxyType)_objects[udata];
                target = null;
            }
            else
            {
                target = GetRawNetObject(luaState, 1);

                if (target == null)
                {
                    ThrowError(luaState, "get_method_bysig: 第一个 arg 不是类型或对象引用");
                    luaState.PushNil();
                    return 1;
                }

                klass = new ProxyType(target.GetType());
            }

            var methodName = luaState.ToString(2, false);
            var signature = new Type[luaState.GetTop() - 2];

            for (var i = 0; i < signature.Length; i++)
                signature[i] = FindType(luaState.ToString(i + 3, false));

            try
            {
                var method = klass.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static |
                    BindingFlags.Instance, signature);
                var wrapper = new LuaMethodWrapper(this, target, klass, method);
                var invokeDelegate = wrapper.InvokeFunction;
                PushFunction(luaState, invokeDelegate);
            }
            catch (Exception e)
            {
                ThrowError(luaState, e);
                luaState.PushNil();
            }

            return 1;
        }

        /*
         * Implementation of get_constructor_bysig. Returns nil
         * if no matching constructor is found.
         */
#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int GetConstructorSignature(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.GetConstructorSignatureInternal(state);
        }

        private int GetConstructorSignatureInternal(LuaState luaState)
        {
            ProxyType klass = null;
            var udata = luaState.CheckUObject(1, "luaNet_class");

            if (udata != -1)
                klass = (ProxyType)_objects[udata];

            if (klass == null)
                ThrowError(luaState, "get_constructor_bysig: 第一个 arg 是无效的类型引用");

            var signature = new Type[luaState.GetTop() - 1];

            for (var i = 0; i < signature.Length; i++)
                signature[i] = FindType(luaState.ToString(i + 2, false));

            try
            {
                if (klass != null)
                {
                    var constructor = klass.UnderlyingSystemType.GetConstructor(signature);
                    var wrapper = new LuaMethodWrapper(this, null, klass, constructor);
                    var invokeDelegate = wrapper.InvokeFunction;
                    PushFunction(luaState, invokeDelegate);
                }
            }
            catch (Exception e)
            {
                ThrowError(luaState, e);
                luaState.PushNil();
            }
            return 1;
        }

        /*
         * Pushes a type reference into the stack
         */
        internal void PushType(LuaState luaState, Type t)
        {
            PushObject(luaState, new ProxyType(t), "luaNet_class");
        }

        /*
         * Pushes a delegate into the stack
         */
        internal void PushFunction(LuaState luaState, LuaNativeFunction func)
        {
            PushObject(luaState, func, "luaNet_function");
        }


        /*
         * Pushes a CLR object into the Lua stack as an userdata
         * with the provided metatable
         */
        private void PushObject(LuaState luaState, object o, string metatable)
        {
            var index = -1;

            // Pushes nil
            if (o == null)
            {
                luaState.PushNil();
                return;
            }

            // Object already in the list of Lua objects? Push the stored reference.
            var found = (!o.GetType().IsValueType || o.GetType().IsEnum) && _objectsBackMap.TryGetValue(o, out index);

            if (found)
            {
                luaState.GetMetaTable("luaNet_objects");
                luaState.RawGetInteger(-1, index);

                // Note: starting with lua5.1 the garbage collector may remove weak reference items (such as our luaNet_objects values) when the initial GC sweep 
                // occurs, but the actual call of the __gc finalizer for that object may not happen until a little while later.  During that window we might call
                // this routine and find the element missing from luaNet_objects, but collectObject() has not yet been called.  In that case, we go ahead and call collect
                // object here
                // did we find a non nil object in our table? if not, we need to call collect object
                var type = luaState.Type(-1);
                if (type != LuaType.Nil)
                {
                    luaState.Remove(-2);	 // drop the metatable - we're going to leave our object on the stack
                    return;
                }

                // MetaFunctions.dumpStack(this, luaState);
                luaState.Remove(-1);	// remove the nil object value
                luaState.Remove(-1);	// remove the metatable
                CollectObject(o, index);	// Remove from both our tables and fall out to get a new ID
            }

            index = AddObject(o);
            PushNewObject(luaState, o, index, metatable);
        }

        /*
         * Pushes a new object into the Lua stack with the provided
         * metatable
         */
        private void PushNewObject(LuaState luaState, object o, int index, string metatable)
        {
            if (metatable == "luaNet_metatable")
            {
                // Gets or creates the metatable for the object's type
                luaState.GetMetaTable(o.GetType().AssemblyQualifiedName);

                if (luaState.IsNil(-1))
                {
                    luaState.SetTop(-2);
                    luaState.NewMetaTable(o.GetType().AssemblyQualifiedName);
                    luaState.PushString("cache");
                    luaState.NewTable();
                    luaState.RawSet(-3);
                    luaState.PushLightUserData(Tag);
                    luaState.PushNumber(1);
                    luaState.RawSet(-3);
                    luaState.PushString("__index");
                    luaState.PushString("luaNet_indexfunction");
                    luaState.RawGet(LuaRegistry.Index);
                    luaState.RawSet(-3);
                    luaState.PushString("__gc");
                    luaState.PushCFunction(MetaFunctions.GcFunction);
                    luaState.RawSet(-3);
                    luaState.PushString("__tostring");
                    luaState.PushCFunction(MetaFunctions.ToStringFunction);
                    luaState.RawSet(-3);
                    luaState.PushString("__newindex");
                    luaState.PushCFunction(MetaFunctions.NewIndexFunction);
                    luaState.RawSet(-3);
                    // Bind C# operator with Lua metamethods (__add, __sub, __mul)
                    RegisterOperatorsFunctions(luaState, o.GetType());
                    RegisterCallMethodForDelegate(luaState, o);
                }
            }
            else
                luaState.GetMetaTable(metatable);

            // Stores the object index in the Lua list and pushes the
            // index into the Lua stack
            luaState.GetMetaTable("luaNet_objects");
            luaState.NewUData(index);
            luaState.PushCopy(-3);
            luaState.Remove(-4);
            luaState.SetMetaTable(-2);
            luaState.PushCopy(-1);
            luaState.RawSetInteger(-3, index);
            luaState.Remove(-2);
        }

        private static void RegisterCallMethodForDelegate(LuaState luaState, object o)
        {
            if (!(o is Delegate))
                return;

            luaState.PushString("__call");
            luaState.PushCFunction(MetaFunctions.CallDelegateFunction);
            luaState.RawSet(-3);
        }

        private static void RegisterOperatorsFunctions(LuaState luaState, Type type)
        {
            if (type.HasAdditionOperator())
            {
                luaState.PushString("__add");
                luaState.PushCFunction(MetaFunctions.AddFunction);
                luaState.RawSet(-3);
            }
            if (type.HasSubtractionOperator())
            {
                luaState.PushString("__sub");
                luaState.PushCFunction(MetaFunctions.SubtractFunction);
                luaState.RawSet(-3);
            }
            if (type.HasMultiplyOperator())
            {
                luaState.PushString("__mul");
                luaState.PushCFunction(MetaFunctions.MultiplyFunction);
                luaState.RawSet(-3);
            }
            if (type.HasDivisionOperator())
            {
                luaState.PushString("__div");
                luaState.PushCFunction(MetaFunctions.DivisionFunction);
                luaState.RawSet(-3);
            }
            if (type.HasModulusOperator())
            {
                luaState.PushString("__mod");
                luaState.PushCFunction(MetaFunctions.ModulosFunction);
                luaState.RawSet(-3);
            }
            if (type.HasUnaryNegationOperator())
            {
                luaState.PushString("__unm");
                luaState.PushCFunction(MetaFunctions.UnaryNegationFunction);
                luaState.RawSet(-3);
            }
            if (type.HasEqualityOperator())
            {
                luaState.PushString("__eq");
                luaState.PushCFunction(MetaFunctions.EqualFunction);
                luaState.RawSet(-3);
            }
            if (type.HasLessThanOperator())
            {
                luaState.PushString("__lt");
                luaState.PushCFunction(MetaFunctions.LessThanFunction);
                luaState.RawSet(-3);
            }

            if (!type.HasLessThanOrEqualOperator()) 
                return;
            
            luaState.PushString("__le");
            luaState.PushCFunction(MetaFunctions.LessThanOrEqualFunction);
            luaState.RawSet(-3);
        }

        /*
         * Gets an object from the Lua stack with the desired type, if it matches, otherwise
         * returns null.
         */
        internal object GetAsType(LuaState luaState, int stackPos, Type paramType)
        {
            var extractor = TypeChecker.CheckLuaType(luaState, stackPos, paramType);
            return extractor?.Invoke(luaState, stackPos);
        }

        /// <summary>
        /// Given the Lua int ID for an object remove it from our maps
        /// </summary>
        /// <param name = "udata"></param>
        internal void CollectObject(int udata)
        {
            var found = _objects.TryGetValue(udata, out var o);

            // The other variant of collectObject might have gotten here first, in that case we will silently ignore the missing entry
            if (found)
                CollectObject(o, udata);
        }

        /// <summary>
        /// Given an object reference, remove it from our maps
        /// </summary>
        /// <param name = "o"></param>
        /// <param name = "udata"></param>
        private void CollectObject(object o, int udata)
        {
            _objects.Remove(udata);
            if (!o.GetType().IsValueType || o.GetType().IsEnum)
                _objectsBackMap.Remove(o);
        }

        private int AddObject(object obj)
        {
            // New object: inserts it in the list
            var index = _nextObj++;
            _objects[index] = obj;

            if (!obj.GetType().IsValueType || obj.GetType().IsEnum)
                _objectsBackMap[obj] = index;

            return index;
        }

        /*
         * Gets an object from the Lua stack according to its Lua type.
         */
        internal object GetObject(LuaState luaState, int index)
        {
            var type = luaState.Type(index);

            switch (type)
            {
                case LuaType.Number:
                    {
                        if (luaState.IsInteger(index))
                            return luaState.ToInteger(index);

                        return luaState.ToNumber(index);
                    }
                case LuaType.String:
                        return luaState.ToString(index, false);
                case LuaType.Boolean:
                        return luaState.ToBoolean(index);
                case LuaType.Table:
                        return GetTable(luaState, index);
                case LuaType.Function:
                        return GetFunction(luaState, index);
                case LuaType.UserData:
                    {
                        var udata = luaState.ToNetObject(index, Tag);
                        return udata != -1 ? _objects[udata] : GetUserData(luaState, index);
                    }
                default:
                    return null;
            }
        }

        /*
         * Gets the table in the index positon of the Lua stack.
         */
        internal LuaTable GetTable(LuaState luaState, int index)
        {
            // Before create new tables, check if there is any finalized object to clean.
            CleanFinalizedReferences(luaState);

            luaState.PushCopy(index);
            var reference = luaState.Ref(LuaRegistry.Index);
            if (reference == -1)
                return null;
            return new LuaTable(reference, _interpreter);
        }

        /*
         * Gets the userdata in the index positon of the Lua stack.
         */
        internal LuaUserData GetUserData(LuaState luaState, int index)
        {
            // Before create new tables, check if there is any finalized object to clean.
            CleanFinalizedReferences(luaState);

            luaState.PushCopy(index);
            var reference = luaState.Ref(LuaRegistry.Index);
            if (reference == -1)
                return null;
            return new LuaUserData(reference, _interpreter);
        }

        /*
         * Gets the function in the index positon of the Lua stack.
         */
        internal LuaFunction GetFunction(LuaState luaState, int index)
        {
            // Before create new tables, check if there is any finalized object to clean.
            CleanFinalizedReferences(luaState);

            luaState.PushCopy(index);
            var reference = luaState.Ref(LuaRegistry.Index);
            if (reference == -1)
                return null;
            return new LuaFunction(reference, _interpreter);
        }

        /*
         * Gets the CLR object in the index positon of the Lua stack. Returns
         * delegates as Lua functions.
         */
        internal object GetNetObject(LuaState luaState, int index)
        {
            var idx = luaState.ToNetObject(index, Tag);
            return idx != -1 ? _objects[idx] : null;
        }

        /*
         * Gets the CLR object in the index position of the Lua stack. Returns
         * delegates as is.
         */
        internal object GetRawNetObject(LuaState luaState, int index)
        {
            var udata = luaState.RawNetObj(index);
            return udata != -1 ? _objects[udata] : null;
        }


        /*
         * Gets the values from the provided index to
         * the top of the stack and returns them in an array.
         */
        internal object[] PopValues(LuaState luaState, int oldTop)
        {
            var newTop = luaState.GetTop();

            if (oldTop == newTop)
                return null;

            var returnValues = new List<object>();
            for (var i = oldTop + 1; i <= newTop; i++)
                returnValues.Add(GetObject(luaState, i));

            luaState.SetTop(oldTop);
            return returnValues.ToArray();
        }

        /*
         * Gets the values from the provided index to
         * the top of the stack and returns them in an array, casting
         * them to the provided types.
         */
        internal object[] PopValues(LuaState luaState, int oldTop, Type[] popTypes)
        {
            var newTop = luaState.GetTop();

            if (oldTop == newTop)
                return null;

            var returnValues = new List<object>();

            var iTypes = popTypes[0] == typeof(void) ? 1 : 0;

            for (var i = oldTop + 1; i <= newTop; i++)
            {
                returnValues.Add(GetAsType(luaState, i, popTypes[iTypes]));
                iTypes++;
            }

            luaState.SetTop(oldTop);
            return returnValues.ToArray();
        }

        // The following line doesn't work for remoting proxies - they always return a match for 'is'
        // else if (o is ILuaGeneratedType)
        private static bool IsILua(object o)
        {
            if (!(o is ILuaGeneratedType)) 
                return false;
            
            // Make sure we are _really_ ILuaGenerated
            var typ = o.GetType();
            return typ.GetInterface("ILuaGeneratedType", true) != null;
        }

        /*
         * Pushes the object into the Lua stack according to its type.
         */
        internal void Push(LuaState luaState, object o)
        {
            switch (o)
            {
                case null:
                    luaState.PushNil();
                    break;
                case sbyte sb:
                    luaState.PushInteger(sb);
                    break;
                case byte bt:
                    luaState.PushInteger(bt);
                    break;
                case short s:
                    luaState.PushInteger(s);
                    break;
                case ushort us:
                    luaState.PushInteger(us);
                    break;
                case int i:
                    luaState.PushInteger(i);
                    break;
                case uint ui:
                    luaState.PushInteger(ui);
                    break;
                case long l:
                    luaState.PushInteger(l);
                    break;
                case ulong ul:
                    luaState.PushInteger((long)ul);
                    break;
                case char ch:
                    luaState.PushInteger(ch);
                    break;
                case float fl:
                    luaState.PushNumber(fl);
                    break;
                case decimal dc:
                    luaState.PushNumber((double)dc);
                    break;
                case double db:
                    luaState.PushNumber(db);
                    break;
                case string str:
                    luaState.PushString(str);
                    break;
                case bool b:
                    luaState.PushBoolean(b);
                    break;
                default:
                {
                    if (IsILua(o))
                        ((ILuaGeneratedType)o).LuaInterfaceGetLuaTable().Push(luaState);
                    else switch (o)
                    {
                        case LuaTable table:
                            table.Push(luaState);
                            break;
                        case LuaNativeFunction nativeFunction:
                            PushFunction(luaState, nativeFunction);
                            break;
                        case LuaFunction luaFunction:
                            luaFunction.Push(luaState);
                            break;
                        default:
                            PushObject(luaState, o, "luaNet_metatable");
                            break;
                    }
                    break;
                }
            }
        }

        /*
         * Checks if the method matches the arguments in the Lua stack, getting
         * the arguments if it does.
         */
        internal bool MatchParameters(LuaState luaState, MethodBase method, MethodCache methodCache, int skipParam)
        {
            return MetaFunctionsInstance.MatchParameters(luaState, method, methodCache, skipParam);
        }

        internal static Array TableToArray(LuaState luaState, ExtractValue extractValue, Type paramArrayType, int startIndex, int count)
        {
            return MetaFunctions.TableToArray(luaState, extractValue, paramArrayType, ref startIndex, count);
        }

        private Type TypeOf(LuaState luaState, int idx)
        {
            var udata = luaState.CheckUObject(idx, "luaNet_class");
            if (udata == -1)
                return null;

            var pt = (ProxyType)_objects[udata];
            return pt.UnderlyingSystemType;
        }

        private static int PushError(LuaState luaState, string msg)
        {
            luaState.PushNil();
            luaState.PushString(msg);
            return 2;
        }

#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int CType(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.CTypeInternal(state);
        }

        private int CTypeInternal(LuaState luaState)
        {
            var t = TypeOf(luaState,1);
            if (t == null)
                return PushError(luaState, "Not a CLR Class");

            PushObject(luaState, t, "luaNet_metatable");
            return 1;
        }

#if __IOS__ || __TVOS__ || __WATCHOS__
        [MonoPInvokeCallback(typeof(LuaNativeFunction))]
#endif
        private static int EnumFromInt(IntPtr luaState)
        {
            var state = LuaState.FromIntPtr(luaState);
            var translator = ObjectTranslatorPool.Instance.Find(state);
            return translator.EnumFromIntInternal(state);
        }

        private int EnumFromIntInternal(LuaState luaState)
        {
            var t = TypeOf(luaState, 1);
            if (t == null || !t.IsEnum)
                return PushError(luaState, "Not an Enum.");

            object res = null;
            var lt = luaState.Type(2);
            switch (lt)
            {
                case LuaType.Number:
                {
                    var ital = (int)luaState.ToNumber(2);
                    res = Enum.ToObject(t, ital);
                    break;
                }
                case LuaType.String:
                {
                    var flags = luaState.ToString(2, false);
                    string err = null;
                    try
                    {
                        res = Enum.Parse(t, flags, true);
                    }
                    catch (ArgumentException e)
                    {
                        err = e.Message;
                    }
                    if (err != null)
                        return PushError(luaState, err);
                    break;
                }
                default:
                    return PushError(luaState, "Second argument must be a integer or a string.");
            }
            PushObject(luaState, res, "luaNet_metatable");
            return 1;
        }

        internal void AddFinalizedReference(int reference)
        {
            _finalizedReferences.Enqueue(reference);
        }

        private void CleanFinalizedReferences(LuaState state)
        {
            if (_finalizedReferences.Count == 0)
                return;

            while (_finalizedReferences.TryDequeue(out var reference))
                state.Unref(LuaRegistry.Index, reference);
        }
    }
}
