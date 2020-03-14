using System;
using System.Collections.Generic;
using AutoLua.Core.AutoAccessibility.Accessibility.Node;

namespace AutoLua.Droid.HttpServers.Models
{
    public class NodeModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 深度。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 节点id
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// 节点文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 节点描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 节点类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 是否可以勾选
        /// </summary>
        public bool Checkable { get; set; }

        /// <summary>
        /// 是否已经勾选
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 是否可以点击
        /// </summary>
        public bool Clickable { get; set; }

        /// <summary>
        /// 是否已启用
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 是否可以滑动
        /// </summary>
        public bool Scrollable { get; set; }

        /// <summary>
        /// 是否可以长按
        /// </summary>
        public bool LongClickable { get; set; }

        /// <summary>
        /// 是否是密码框
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// 是否已经选中
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 子集数量
        /// </summary>
        public int ChildCount { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        public Rect Rect { get; set; }

        /// <summary>
        /// 子节点集合。
        /// </summary>
        public List<NodeModel> Children { get; } = new List<NodeModel>();
    }

    public class Rect
    {
        public int Left { get; set; }

        public int Right { get; set; }

        public int Top { get; set; }

        public int Bottom { get; set; }

        public int CenterX { get; set; }
        
        public int CenterY { get; set; }
        
        public int Width { get; set; }

        public int Height { get; set; }
    }
}