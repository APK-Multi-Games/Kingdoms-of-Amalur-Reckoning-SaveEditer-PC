using System;
using System.Collections.Generic;
using System.Text;

namespace KingdomsofAmalurReckoningSaveEditer
{
    /// <summary>
    /// 装备在内存中的信息
    /// </summary>
    public class WeaponMemoryInfo
    {
        private int weaponIndex;
        /// <summary>
        /// 装备头部所在索引(XX XX XX XX 0B 00 00 00 68 D5 24 00 03)
        /// </summary>
        public int WeaponIndex
        {
            get { return weaponIndex; }
            set { weaponIndex = value; }
        }

        private int nextWeaponIndex;
        /// <summary>
        /// 下一个装备头部所在索引
        /// </summary>
        public int NextWeaponIndex
        {
            get { return nextWeaponIndex; }
            set { nextWeaponIndex = value; }
        }

        /// <summary>
        /// 装备属性列表头部在存档中的索引(占4字节,指示该装备拥有属性数量)
        /// </summary>
        public int AttHeadIndex
        {
            get { return weaponIndex + AmalurSaveEditer.WeaponAttHeadOffSet; }
        }

        private byte[] weaponBytes;
        /// <summary>
        /// 装备数据
        /// </summary>
        public byte[] WeaponBytes
        {
            get { return weaponBytes; }
            set { weaponBytes = value; }
        }

        /// <summary>
        /// 装备名称
        /// </summary>
        public String WeaponName
        {
            get
            {
                if (weaponBytes[AmalurSaveEditer.WeaponAttHeadOffSet + 22 + AttCount * 8] != 1)
                {
                    return "未知";
                }
                else
                {
                    int count = BitConverter.ToInt32(weaponBytes,AmalurSaveEditer.WeaponAttHeadOffSet+22+AttCount*8+1);
                    return System.Text.Encoding.Default.GetString(weaponBytes, AmalurSaveEditer.WeaponAttHeadOffSet + 27 + 8 * AttCount, count);
                }
            }
            set
            {
                ByteManager.ByteEditer byteEditer = new ByteManager.ByteEditer(weaponBytes);
                byteEditer.DeleteToEnd(AmalurSaveEditer.WeaponAttHeadOffSet + 22 + AttCount * 8+1);
                if (value.Length != 0)
                {
                    byteEditer.EditByIndex(AmalurSaveEditer.WeaponAttHeadOffSet + 22 + AttCount * 8, new byte[] { 1 });
                    byteEditer.AddToEnd(BitConverter.GetBytes(value.Length));
                    byte[] nameList = System.Text.Encoding.Default.GetBytes(value);
                    byteEditer.AddToEnd(nameList);
                }
                else
                {
                    byteEditer.EditByIndex(AmalurSaveEditer.WeaponAttHeadOffSet + 22 + AttCount * 8, new byte[] { 0 });
                }
                weaponBytes = byteEditer.BtList;
            }
        }

        /// <summary>
        /// 属性数量
        /// </summary>
        public int AttCount
        {
            get { return BitConverter.ToInt32(weaponBytes, AmalurSaveEditer.WeaponAttHeadOffSet); }
        }

        /// <summary>
        /// 当前耐久度
        /// </summary>
        public float CurrentDurability
        {
            get { return BitConverter.ToSingle(weaponBytes, AmalurSaveEditer.WeaponAttHeadOffSet + 8 + 8 * AttCount); }
            set
            {
                byte[] bt = BitConverter.GetBytes(value);
                ByteManager.ByteEditer byteEditer = new ByteManager.ByteEditer(weaponBytes);
                byteEditer.EditByIndex(AmalurSaveEditer.WeaponAttHeadOffSet + 8 + 8 * AttCount, bt);
                weaponBytes = byteEditer.BtList;
            }
        }

        /// <summary>
        /// 最大耐久度
        /// </summary>
        public float MaxDurability
        {
            get { return BitConverter.ToSingle(weaponBytes, AmalurSaveEditer.WeaponAttHeadOffSet + 12 + 8 * AttCount); }
            set
            {
                byte[] bt = BitConverter.GetBytes(value);
                ByteManager.ByteEditer byteEditer = new ByteManager.ByteEditer(weaponBytes);
                byteEditer.EditByIndex(AmalurSaveEditer.WeaponAttHeadOffSet + 12 + 8 * AttCount, bt);
                weaponBytes = byteEditer.BtList;
            }
        }

        /// <summary>
        /// 装备属性列表
        /// </summary>
        public List<AttributeMemoryInfo> WeaponAttList
        {
            get
            {
                ByteManager.ByteEditer byteEditer = new ByteManager.ByteEditer(weaponBytes);
                List<AttributeMemoryInfo> attList = new List<AttributeMemoryInfo>();

                int attIndex = AmalurSaveEditer.WeaponAttHeadOffSet + 4;
                for (int i = 0; i < AttCount; i++)
                {
                    AttributeMemoryInfo att = new AttributeMemoryInfo();

                    att.Value = byteEditer.GetIntsByIndexAndLength(attIndex, 4);
                    String val1 = Convert.ToString(att.Value[2], 16).ToUpper();
                    String val2 = Convert.ToString(att.Value[1], 16).ToUpper();
                    String val3 = Convert.ToString(att.Value[0], 16).ToUpper();

                    att.Code = (val1.Length == 2 ? val1 : ("0" + val1)) + (val2.Length == 2 ? val2 : ("0" + val2)) + (val3.Length == 2 ? val3 : ("0" + val3));
                    attList.Add(att);

                    attIndex += 8;
                }
                return attList;
            }
            set
            {
                ByteManager.ByteEditer byteEditer = new ByteManager.ByteEditer(weaponBytes);
                byteEditer.DeleteIntsByIndexAndLength(AmalurSaveEditer.WeaponAttHeadOffSet + 4, 8 * AttCount);
                byteEditer.EditByIndex(AmalurSaveEditer.WeaponAttHeadOffSet, BitConverter.GetBytes(value.Count));
                foreach (AttributeMemoryInfo att in value)
                {
                    byte[] news = new byte[8];
                    news[0] = byte.Parse(att.Code.ToUpper().Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                    news[1] = byte.Parse(att.Code.ToUpper().Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    news[2] = byte.Parse(att.Code.ToUpper().Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    news[3] = 0;
                    news[4] = 255;
                    news[5] = 255;
                    news[6] = 255;
                    news[7] = 255;
                    byteEditer.AddByIndex(AmalurSaveEditer.WeaponAttHeadOffSet + 4, news);
                }
                weaponBytes = byteEditer.BtList;
            }
        }
    }
}
