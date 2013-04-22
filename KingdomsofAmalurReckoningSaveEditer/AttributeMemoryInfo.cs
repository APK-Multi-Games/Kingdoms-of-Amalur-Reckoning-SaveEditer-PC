using System;
using System.Collections.Generic;
using System.Text;

namespace KingdomsofAmalurReckoningSaveEditer
{
    /// <summary>
    /// 属性在文件中的信息
    /// </summary>
    public class AttributeMemoryInfo
    {
        private int[] value;
        /// <summary>
        /// 属性在文件中的值
        /// </summary>
        public int[] Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private String code;
        /// <summary>
        /// 属性代码
        /// </summary>
        public String Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        private string detail;
        /// <summary>
        /// 属性文字描述
        /// </summary>
        public string Detail
        {
            get { return detail; }
            set { detail = value; }
        }
    }
}
