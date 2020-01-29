namespace AutoLua.Views.Scripts.Models
{
    public class ScriptFileModel
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileLength { get; set; }

        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public bool IsDir { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
    }
}
