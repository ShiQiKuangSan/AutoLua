using System;

namespace NLua
{
    /// <summary>
    /// Base class to provide consistent disposal flow across lua objects. Uses code provided by Yves Duhoux and suggestions by Hans Schmeidenbacher and Qingrui Li 
    /// </summary>
    public abstract class LuaBase : IDisposable
    {
        private bool _disposed;
        protected readonly int Reference;
        private Lua _lua;

        protected bool TryGet(out Lua lua)
        {
            if (_lua.State == null)
            {
                lua = null;
                return false;
            }

            lua = _lua;
            return true;
        }

        protected LuaBase(int reference, Lua lua)
        {
            _lua = lua;
            Reference = reference;
        }

        ~LuaBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void DisposeLuaReference(bool finalized)
        {
            if (_lua == null)
                return;
            if (!TryGet(out var lua))
                return;

            lua.DisposeInternal(Reference, finalized);
        }

        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (_disposed)
                return;

            var finalized = !disposeManagedResources;

            if (Reference != 0)
            {
                DisposeLuaReference(finalized);
            }

            _lua = null;
            _disposed = true;
        }

        public override bool Equals(object o)
        {
            if (!(o is LuaBase reference))
                return false;

            if (!TryGet(out var lua))
                return false;

            return lua.CompareRef(reference.Reference, Reference);
        }

        public override int GetHashCode()
        {
            return Reference;
        }
    }
}