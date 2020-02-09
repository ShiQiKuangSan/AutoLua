using System.Threading.Tasks;

namespace AutoLua.Services
{
    /// <summary>
    /// lua 脚本服务接口
    /// </summary>
    public interface ILuaScriptService
    {
        /// <summary>
        /// 运行文件。
        /// </summary>
        void RunFile(string path);

        /// <summary>
        /// 运行脚本。
        /// </summary>
        void RunProject(string path);
    }
}
