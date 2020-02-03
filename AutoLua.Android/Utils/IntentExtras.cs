using System.Collections.Generic;
using Android.Content;
using Android.Util;
using Java.IO;
using Java.Util.Concurrent.Atomic;

namespace AutoLua.Droid.Utils
{
    public class IntentExtras : Java.Lang.Object, ISerializable
    {
        public const string ExtraId = "AutoLua.Droid.Utils.IntentExtras.id";

        private static readonly AtomicInteger MaxId = new AtomicInteger(-1);

        private static readonly SparseArray<Dictionary<string, object>> ExtraStore =
            new SparseArray<Dictionary<string, object>>();

        private readonly Dictionary<string, object> _map;

        public int Id { get; private set; }

        private IntentExtras()
        {
            _map = new Dictionary<string, object>();
            Id = MaxId.IncrementAndGet();
            ExtraStore.Put(Id, _map);
        }

        private IntentExtras(int id, Dictionary<string, object> map)
        {
            Id = id;
            _map = map;
        }

        public static IntentExtras NewExtras()
        {
            return new IntentExtras();
        }

        public static IntentExtras FromIntentAndRelease(Intent intent)
        {
            var id = intent.GetIntExtra(ExtraId, -1);
            if (id < 0)
            {
                return null;
            }

            return FromIdAndRelease(id);
        }

        public static IntentExtras FromIdAndRelease(int id)
        {
            var map = ExtraStore.Get(id);
            if (map == null)
            {
                return null;
            }

            ExtraStore.Remove(id);
            return new IntentExtras(id, map);
        }

        public static IntentExtras FromId(int id)
        {
            var map = ExtraStore.Get(id);
            return map == null ? null : new IntentExtras(id, map);
        }

        public static IntentExtras FromIntent(Intent intent)
        {
            var id = intent.GetIntExtra(ExtraId, -1);
            return id < 0 ? null : FromId(id);
        }

        public T Get<T>(string key)
        {
            if (_map.ContainsKey(key))
            {
                return (T) _map[key];    
            }
            return default;
        }
        
        public IntentExtras Put(string key, object value) {
            _map.Add(key, value);
            return this;
        }
        
        public IntentExtras PutAll(IntentExtras extras) {
            foreach (var (key, value) in extras._map)
            {
                _map.Add(key,value);
            }
            return this;
        }
        
        public Intent PutInIntent(Intent intent) {
            intent.PutExtra(ExtraId, Id);
            return intent;
        }
        
        public void Release() {
            ExtraStore.Remove(Id);
            Id = -1;
        }
    }
}