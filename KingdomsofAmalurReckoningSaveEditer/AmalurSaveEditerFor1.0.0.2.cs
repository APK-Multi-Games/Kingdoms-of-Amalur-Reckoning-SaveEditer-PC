using System;
using System.Collections.Generic;
using System.Text;
using ByteManager;

namespace KingdomsofAmalurReckoningSaveEditer
{
    /// <summary>
    /// 阿玛拉王国存档操作类(支持1.0.0.2)
    /// </summary>
    public class AmalurSaveEditer
    {
        /// <summary>
        /// 装备属性头部指示属性数量的数据相对装备数据头部的偏移量
        /// </summary>
        public static int WeaponAttHeadOffSet = 21;
        private ByteEditer br = null;

        /// <summary>
        /// 读取存档
        /// </summary>
        /// <param name="path">存档路径</param>
        public void ReadFile(string path)
        {
            br = new ByteEditer();
            try
            {
                br.ReadFile(path);
            }
            catch
            {
                br = null;
                throw new Exception("存档文件打开失败");
            }
        }

        /// <summary>
        /// 保存存档
        /// </summary>
        /// <param name="path">保存路径</param>
        public void SaveFile(string path)
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }

            try
            {
                br.SaveFile(path);
            }
            catch
            {
                throw new Exception("存档文件保存失败");
            }
        }

        /// <summary>
        /// 获取背包容量上限
        /// </summary>
        /// <returns></returns>
        public int GetMaxBagCount()
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }
            int index = br.FindIndexByString("current_inventory_count")[0] + 35;
            byte[] bt = br.GetBytsByIndexAndLength(index, 4);
            return BitConverter.ToInt32(bt,0);
        }

        /// <summary>
        /// 写入背包容量上限
        /// </summary>
        /// <param name="c"></param>
        public void EditMaxBagCount(int c)
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }
            int index = br.FindIndexByString("current_inventory_count")[0] + 35;
            byte[] bt = BitConverter.GetBytes(c);
            br.EditByIndex(index, bt);
        }

        /// <summary>
        /// 获取装备的属性列表(包含属性文字描述)
        /// </summary>
        /// <param name="weaponInfo">装备对象</param>
        /// <param name="attInfoList">属性描述类集合</param>
        /// <returns>属性列表</returns>
        public List<AttributeMemoryInfo> getAttList(WeaponMemoryInfo weaponInfo, List<AttributeInfo> attInfoList)
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }

            List<AttributeMemoryInfo> attList = weaponInfo.WeaponAttList;
            foreach(AttributeMemoryInfo attInfo in attList)
            {
                String text = "";
                foreach (AttributeInfo att in attInfoList)
                {
                    if (att.AttributeId == attInfo.Code)
                    {
                        text = att.AttributeText;
                    }
                }
                if (text == "")
                {
                    text = "未知";
                }
                attInfo.Detail = text;
            }
            return attList;
        }

        /// <summary>
        /// 检查某装备对象是否是一件有效装备
        /// </summary>
        /// <param name="weapon">装备对象</param>
        /// <returns></returns>
        public bool IsWeapon(WeaponMemoryInfo weapon)
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }

            byte[] bytes = new byte[9];
            bytes[0] = 11;
            bytes[1] = 0;
            bytes[2] = 0;
            bytes[3] = 0;
            bytes[4] = 104;
            bytes[5] = 213;
            bytes[6] = 36;
            bytes[7] = 0;
            bytes[8] = 3;
            try
            {
                return br.HasBytesByIndexAndLength(bytes, weapon.WeaponIndex+4, 17);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取所有装备
        /// </summary>
        /// <returns></returns>
        public List<WeaponMemoryInfo> GetAllWeapon()
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }

            List<WeaponMemoryInfo> weaponList = new List<WeaponMemoryInfo>();

            byte[] bytes = new byte[9];
            bytes[0] = 11;
            bytes[1] = 0;
            bytes[2] = 0;
            bytes[3] = 0;
            bytes[4] = 104;
            bytes[5] = 213;
            bytes[6] = 36;
            bytes[7] = 0;
            bytes[8] = 3;
            List<int> indexList = br.FindIndexList(bytes);
            
            for (int i = 0; i < indexList.Count; i++)
            {
                indexList[i] -=4;
            }

            for(int i=0;i<indexList.Count;i++)
            {
                if(i!=indexList.Count-1)
                {
                    if (indexList[i + 1] - indexList[i] < 44)
                    {
                        continue;
                    }
                }

                WeaponMemoryInfo weapon = new WeaponMemoryInfo();
                weapon.WeaponIndex = indexList[i];
                if (i != indexList.Count - 1)
                {
                    weapon.NextWeaponIndex = indexList[i + 1];
                    weapon.WeaponBytes = br.GetBytsByIndexAndLength(indexList[i], indexList[i + 1] - indexList[i]);

                    if (weapon.CurrentDurability != 100 && weapon.MaxDurability != -1 && weapon.MaxDurability != 100 && weapon.CurrentDurability != 0 && weapon.MaxDurability != 0)
                    {
                        weaponList.Add(weapon);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    int attHeadIndex = weapon.WeaponIndex + AmalurSaveEditer.WeaponAttHeadOffSet;
                    int attCount = BitConverter.ToInt32(br.BtList,attHeadIndex);
                    int endIndex = 0;
                    if (br.BtList[attHeadIndex + 22 + attCount * 8] != 1)
                    {
                        endIndex = attHeadIndex + 22 + attCount * 8;
                    }
                    else
                    {
                        int nameLength = 0;
                        nameLength = BitConverter.ToInt32(br.BtList, attHeadIndex + 22 + attCount * 8 + 1);
                        endIndex = attHeadIndex + 22 + attCount * 8 + nameLength + 4;
                    }
                    weapon.WeaponBytes = br.GetBytsByIndexAndLength(weapon.WeaponIndex, endIndex - weapon.WeaponIndex+1);
                    if (weapon.CurrentDurability != 100 && weapon.MaxDurability != -1 && weapon.MaxDurability != 100 && weapon.CurrentDurability != 0 && weapon.MaxDurability != 0)
                    {
                        weaponList.Add(weapon);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return weaponList;
        }

        /// <summary>
        /// 删除某件装备
        /// </summary>
        /// <param name="weapon"></param>
        public void DeleteWeapon(WeaponMemoryInfo weapon)
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }

            weapon.WeaponBytes = new byte[] {0,0,0,0 };
            WriteWeaponByte(weapon);
        }

        /// <summary>
        /// 将装备代码写入存档
        /// </summary>
        /// <param name="weapon">要写入的装备</param>
        public void WriteWeaponByte(WeaponMemoryInfo weapon)
        {
            if (br.BtList == null)
            {
                throw new Exception("存档未打开");
            }

            br.DeleteIntsByStartAndEnd(weapon.WeaponIndex, weapon.NextWeaponIndex - 1);
            br.AddByIndex(weapon.WeaponIndex, weapon.WeaponBytes);
        }
    }
}
