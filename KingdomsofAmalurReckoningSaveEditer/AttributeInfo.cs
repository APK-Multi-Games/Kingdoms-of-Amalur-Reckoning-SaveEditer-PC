using System;
using System.Collections.Generic;
using System.Text;

namespace KingdomsofAmalurReckoningSaveEditer
{
    /// <summary>
    /// 属性信息
    /// </summary>
    public class AttributeInfo
    {
        private String attributeId;
        /// <summary>
        /// 属性代码
        /// </summary>
        public String AttributeId
        {
            get { return attributeId; }
            set { attributeId = value; }
        }
        private String attributeText;
        /// <summary>
        /// 属性描述
        /// </summary>
        public String AttributeText
        {
            get { return attributeText; }
            set { attributeText = value; }
        }
    }
}
