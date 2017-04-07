using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FICT_2._0_Duplex_Panel
{
    public partial class MaintainRecord : Form
    {
        public MaintainRecord()
        {
            InitializeComponent();
        }

        private void MaintainRecord_Load(object sender, EventArgs e)
        {
           // this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmIOMonitor_FormClosing);
        }
        private void MaintainRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("aaaaaaaaaaaaa");
        }
    }
}
