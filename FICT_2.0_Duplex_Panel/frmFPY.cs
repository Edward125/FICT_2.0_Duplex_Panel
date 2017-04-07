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
    public partial class frmFPY : Form
    {
        public frmFPY()
        {
            InitializeComponent();
        }

        private void frmFPY_Load(object sender, EventArgs e)
        {
            this.Text = "FPY 查看,查看时間:" + DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            
            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //SubFunction.updateFPY(txtTotal_B, txtPass_B, txtFail_B, txtFPY_B, Param.iPass_B, Param.iFail_B);
                //SubFunction.updateFPY(txtTotal, txtPass, txtFail, txtFPY, Param.iPass, Param.iFail);
                SubFunction.updateFPY(txtTotal_B, txtPass_B, txtFail_B, txtFPY_B, txtFBB, txtFPT_FPYB, Param.iPass_B, Param.iFail_B, Param.iFail_Pass_B);
                SubFunction.updateFPY(txtTotal, txtPass, txtFail, txtFPY, txtFB, txtFPT_FPY, Param.iPass, Param.iFail, Param.iFail_Pass);
                
            }
            //SubFunction.updateFPY(txtTotal_A, txtPass_A, txtFail_A, txtFPY_A, Param.iPass_A, Param.iFail_A); 
            SubFunction.updateFPY(txtTotal_A, txtPass_A, txtFail_A, txtFPY_A, txtFBA, txtFPT_FPYA, Param.iPass_A, Param.iFail_A, Param.iFail_Pass_A); 
        }

        private void timerUpdateFPY_Tick(object sender, EventArgs e)
        {
            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //SubFunction.updateFPY(txtTotal_B, txtPass_B, txtFail_B, txtFPY_B, Param.iPass_B, Param.iFail_B);
                //SubFunction.updateFPY(txtTotal, txtPass, txtFail, txtFPY, Param.iPass, Param.iFail);
                SubFunction.updateFPY(txtTotal_B, txtPass_B, txtFail_B, txtFPY_B, txtFBB, txtFPT_FPYB, Param.iPass_B, Param.iFail_B, Param.iFail_Pass_B);
                SubFunction.updateFPY(txtTotal, txtPass, txtFail, txtFPY, txtFB, txtFPT_FPY, Param.iPass, Param.iFail, Param.iFail_Pass);
            }
            //SubFunction.updateFPY(txtTotal_A, txtPass_A, txtFail_A, txtFPY_A, Param.iPass_A, Param.iFail_A); 
            SubFunction.updateFPY(txtTotal_A, txtPass_A, txtFail_A, txtFPY_A, txtFBA, txtFPT_FPYA, Param.iPass_A, Param.iFail_A, Param.iFail_Pass_A); 

        }

    }
}
