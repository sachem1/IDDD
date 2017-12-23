namespace Coralcode.Framework.Models
{
    public class DataSourceModel
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 字段值/子节点
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 字段的文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 是否当前步骤
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string ParentValue { get; set; }
    }
}
