using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KingdomsofAmalurReckoningSaveEditer
{
    public partial class BagEditForm : Form
    {
        private AmalurSaveEditer editer;
        bool isEdit = false;

        public BagEditForm(AmalurSaveEditer editer)
        {
            InitializeComponent();
            this.editer = editer;
            FormatAll();
        }

        private void FormatAll()
        {
            txtCurrentBag.Text = editer.GetMaxBagCount().ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int count = 0;
            if (int.TryParse(txtBag.Text, out count) && int.Parse(txtBag.Text) > 0 && int.Parse(txtBag.Text) <= 99999999)
            {
                count = int.Parse(txtBag.Text);
                editer.EditMaxBagCount(count);
                isEdit = true;
                MessageBox.Show("修改成功");
            }
            else
            {
                MessageBox.Show("输入的数据无效,不予修改");
            }
        }

        private void BagEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isEdit)
            {
                this.DialogResult = DialogResult.Yes;
            }
        }
    }
}
