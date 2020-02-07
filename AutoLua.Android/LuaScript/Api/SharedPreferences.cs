using Android.Content;
using AutoLua.Droid.Utils;
using Java.Lang;
using Newtonsoft.Json;
using NLua;
using NLua.Exceptions;

namespace AutoLua.Droid.LuaScript.Api
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public class SharedPreferences
    {
        private ISharedPreferences _sharedPreferences;

        /// <summary>
        /// 创建一个本地存储并返回一个Storage对象。不同名称的本地存储的数据是隔开的，而相同名称的本地存储的数据是共享的。
        /// </summary>
        /// <param name="name">本地存储名称</param>
        public void create(string name)
        {
            _sharedPreferences = AppUtils.GetAppContext.GetSharedPreferences(
                AppUtils.GetAppContext.PackageName + name + "_preference",
                FileCreationMode.Private);
        }

        /// <summary>
        /// 删除一个本地存储以及他的全部数据。如果该存储不存在，返回false；否则返回true。
        /// </summary>
        /// <param name="key">本地存储名称</param>
        public void remove(string key)
        {
            _sharedPreferences?.Edit().Remove(key).Apply();
        }

        /// <summary>
        /// 如果该存储中不包含该数据，这时若指定了默认值参数则返回默认值，否则返回undefined。
        /// 返回的数据可能是任意数据类型，这取决于使用Storage.put保存该键值的数据时的数据类型。
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="defaultValue">可选，默认值</param>
        /// <returns></returns>
        public object get(string key, string defaultValue = "")
        {
            try
            {
                if (contains(key))
                {
                    // 普通类型
                    var str = _sharedPreferences.GetString(key, defaultValue);

                    if(str.StartsWith("{") || str.StartsWith("["))
                    {
                        //这是一个json
                        return str;
                    }

                    //这里会转换类型
                    return string.IsNullOrWhiteSpace(str) ? null : JsonConvert.DeserializeObject(str);
                }
            }
            catch (System.Exception e)
            {
                throw new LuaException(e.Message);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public SharedPreferences put(string key, object value = null)
        {
            try
            {
                string str;

                if (value.GetType() == typeof(LuaTable))
                {
                    var tab = value as LuaTable;
                    var tabDic = tab.Items;

                    //如果存入是是table
                    str = JsonConvert.SerializeObject(tabDic);
                }
                else
                {
                    str = JsonConvert.SerializeObject(value);
                    _sharedPreferences.Edit()
                        .PutString(key, str)
                        .Apply();
                }

                _sharedPreferences.Edit()
                    .PutString(key, str)
                    .Apply();

                return this;
            }
            catch (Exception e)
            {
                throw new LuaException(e.Message);
            }
        }

        /// <summary>
        /// 返回该本地存储是否包含键值为key的数据。是则返回true，否则返回false。
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public bool contains(string key)
        {
            return _sharedPreferences.Contains(key);
        }

        /// <summary>
        /// 移除该本地存储的所有数据。不返回任何值。
        /// </summary>
        public void clear()
        {
            _sharedPreferences.Edit().Clear().Apply();
        }
    }
}