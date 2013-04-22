using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ByteManager;
using System.Xml;
using System.IO;

namespace KingdomsofAmalurReckoningSaveEditer
{
    public partial class MainForm : Form
    {
        AmalurSaveEditer editer = null;
        List<AttributeInfo> attributeList = null;
        List<WeaponMemoryInfo> weaponList = null;
        string searchType = "";

        public MainForm()
        {
            InitializeComponent();
            lvMain.Columns[1].Width = 100;
            lvMain.Columns[2].Width = 100;
            lvMain.Columns[3].Width = 100;
            lvMain.Columns[4].Width = 100;
        }

        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            if (opfMain.ShowDialog() == DialogResult.OK)
            {
                lvMain.Items.Clear();
                String fileName = opfMain.FileName;
                editer = new AmalurSaveEditer();
                editer.ReadFile(fileName);
                tslblFileLocal.Text = fileName;
                btnSearchAll.PerformClick();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchType = "name";
            if (txtSearch.Text == "")
            {
                return;
            }
            lvMain.Items.Clear();

            foreach (WeaponMemoryInfo w in weaponList)
            {
                if (w.WeaponName.ToUpper().Contains(txtSearch.Text.ToUpper()))
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = w.WeaponIndex.ToString();
                    item.Text = w.WeaponIndex.ToString();
                    item.SubItems.Add(w.WeaponName);
                    item.SubItems.Add(w.CurrentDurability.ToString());
                    item.SubItems.Add(w.MaxDurability.ToString());
                    item.SubItems.Add(w.AttCount.ToString());
                    item.Tag = w;
                    lvMain.Items.Add(item);
                }
            }
            lvMain.SelectedItems.Clear();
        }

        private void btnSearchByDur_Click(object sender, EventArgs e)
        {
            if (txtCurrendDur.Text.Trim() == "" && txtMaxDur.Text.Trim() == "")
            {
                return;
            }
            float curDur = 0;
            float maxDur = 0;

            if (txtCurrendDur.Text.Trim() != "")
            {
                try
                {
                    curDur = float.Parse(txtCurrendDur.Text);
                }
                catch
                {
                    MessageBox.Show("输入耐久度不合法");
                    return;
                }
            }
            if (txtMaxDur.Text.Trim() != "")
            {
                try
                {
                    maxDur = float.Parse(txtMaxDur.Text);
                }
                catch
                {
                    MessageBox.Show("输入耐久度不合法");
                    return;
                }
            }

            searchType = "dur";
            lvMain.Items.Clear();

            foreach (WeaponMemoryInfo w in weaponList)
            {
                if (txtCurrendDur.Text.Trim() != "" && txtMaxDur.Text.Trim() == "")
                {
                    if (curDur != w.CurrentDurability)
                    {
                        continue;
                    }
                }
                else if (txtCurrendDur.Text.Trim() == "" && txtMaxDur.Text.Trim() != "")
                {
                    if (maxDur != w.MaxDurability)
                    {
                        continue;
                    }
                }
                else
                {
                    if (maxDur != w.MaxDurability || curDur!=w.CurrentDurability)
                    {
                        continue;
                    }
                }
                ListViewItem item = new ListViewItem();
                item.Name = w.WeaponIndex.ToString();
                item.Text = w.WeaponIndex.ToString();
                item.SubItems.Add(w.WeaponName);
                item.SubItems.Add(w.CurrentDurability.ToString());
                item.SubItems.Add(w.MaxDurability.ToString());
                item.SubItems.Add(w.AttCount.ToString());
                item.Tag = w;
                lvMain.Items.Add(item);
            }
            lvMain.SelectedItems.Clear();
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            lvMain.Items.Clear();
            if (editer == null)
            {
                MessageBox.Show("还未打开存档文件,点击确定打开存档");
                tsmiOpen.PerformClick();
            }
            else
            {
                List<WeaponMemoryInfo> weaponTemp = editer.GetAllWeapon();

                weaponList = new List<WeaponMemoryInfo>();
                foreach (WeaponMemoryInfo w in weaponTemp)
                {
                    if (w.WeaponName == "未知")
                    {
                        weaponList.Add(w);
                    }
                    else
                    {
                        weaponList.Insert(0, w);
                    }
                }
                foreach (WeaponMemoryInfo w in weaponList)
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = w.WeaponIndex.ToString();
                    item.Text = w.WeaponIndex.ToString();
                    item.SubItems.Add(w.WeaponName);
                    item.SubItems.Add(w.CurrentDurability.ToString());
                    item.SubItems.Add(w.MaxDurability.ToString());
                    item.SubItems.Add(w.AttCount.ToString());
                    item.Tag = w;
                    lvMain.Items.Add(item);
                }
                btnSearchByDur.Enabled = true;
                btnSearchByName.Enabled = true;
                btnPrint.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnSave.Enabled = false;
                lvMain.SelectedItems.Clear();
            }
        }

        private void AmalurEditer_Load(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            attributeList = new List<AttributeInfo>();
            try
            {
                doc.Load(Application.StartupPath + @"\Data\属性列表.xml");
                XmlNodeList nodes = doc.DocumentElement.ChildNodes;
                foreach (XmlNode n in nodes)
                {
                    AttributeInfo att = new AttributeInfo();
                    att.AttributeId = n.Attributes["id"].Value.ToUpper();
                    att.AttributeText = n.InnerText.ToUpper();
                    attributeList.Add(att);
                }
            }
            catch
            {
                MessageBox.Show("属性列表加载失败,请检查属性列表是否有效");
                Application.Exit();
            }
        }

        private void lvMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMain.SelectedItems.Count > 0)
            {
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                btnPrint.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnPrint.Enabled = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (editer != null)
            {
                File.Copy(opfMain.FileName, opfMain.FileName + ".bak", true);
                editer.SaveFile(opfMain.FileName);
                tslblEditState.Text = "未修改";
                btnSave.Enabled = false;
                MessageBox.Show("保存成功,原存档已备份到"+opfMain.FileName+".bak");
            }
        }

        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            HelpForm form = new HelpForm();
            form.ShowDialog();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            WeaponMemoryInfo weaponInfo = (WeaponMemoryInfo)lvMain.SelectedItems[0].Tag;

            EditForm form = new EditForm(editer, attributeList, weaponInfo);
            btnPrint.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            if (form.ShowDialog() == DialogResult.Yes)
            {
                CanSave();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("强制删除装备可能会导致某些BUG,删除已穿在身上的装备必然导致存档无效，如非必要，请勿使用此功能，确定要删除么?", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                WeaponMemoryInfo weaponInfo = (WeaponMemoryInfo)lvMain.SelectedItems[0].Tag;
                editer.DeleteWeapon(weaponInfo);
                CanSave();
            }
        }

        private void CanSave()
        {
            btnSearchAll.PerformClick();
            if (searchType == "name")
            {
                btnSearchByName.PerformClick();
            }
            else if (searchType == "dur")
            {
                btnSearchByDur.PerformClick();
            }
            tslblEditState.Text = "已修改";
            btnSave.Enabled = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            btnPrint.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            WeaponBytesForm form = new WeaponBytesForm(editer,lvMain.SelectedItems[0].Tag as WeaponMemoryInfo);
            if (form.ShowDialog() == DialogResult.Yes)
            {
                CanSave();
            }
        }

        private void btnBag_Click(object sender, EventArgs e)
        {
            BagEditForm form = new BagEditForm(editer);
            if (form.ShowDialog() == DialogResult.Yes)
            {
                btnSave.Enabled = true;
            }
        }
    }
}
