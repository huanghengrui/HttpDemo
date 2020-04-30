using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RenLianShiBie
{
    public partial class Manage : Form
    {
        public Manage()
        {
            InitializeComponent();
        }

        private void Manage_Load(object sender, EventArgs e)
        {
            MenuPanel.Visible = false;

            Animation.ShowControl(MenuPanel,true ,  AnchorStyles.Bottom);

            try
            {
                SheBeiView.BeginUpdate();
                ListViewItem lvi = new ListViewItem();
                lvi.Text = SheBeiView.Items.Count.ToString();
 
                lvi.SubItems.Add("正面们");
                lvi.SubItems.Add("192.168.3.3");
                SheBeiView.Items.Add(lvi);
                SheBeiView.EndUpdate();
                SheBeiView.BeginUpdate();
                ListViewItem lvi2 = new ListViewItem();
                lvi2.Text = SheBeiView.Items.Count.ToString();
                lvi2.SubItems.Add("正面们");
                lvi2.SubItems.Add("192.168.3.3");
                SheBeiView.Items.Add(lvi2);
                SheBeiView.EndUpdate();

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void BianJi_Click(object sender, EventArgs e)
        {

        }

        private void SheBeiTianJia_Click(object sender, EventArgs e)
        {

        }
    }
}
