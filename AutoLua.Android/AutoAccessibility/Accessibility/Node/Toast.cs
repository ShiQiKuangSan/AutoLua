using System.Collections.Generic;
using System.Linq;

namespace AutoLua.Droid.AutoAccessibility.Accessibility.Node
{
    public class Toast
    {
        /// <summary>
        /// 包名。
        /// </summary>
        public string packageName { get; }

        /// <summary>
        /// Toast 集合
        /// </summary>
        private readonly string[] texts;

        /// <summary>
        /// Toast 文本
        /// </summary>
        public string text => texts.Any() ? texts.First() ?? string.Empty : string.Empty;

        public Toast(string packageName, List<string> texts)
        {
            this.packageName = packageName;
            this.texts = texts.ToArray();
        }

        public override string ToString()
        {
            return $"Toast{{ texts= {texts.ToString()} , packageName= {packageName}}}";
        }
    }
}