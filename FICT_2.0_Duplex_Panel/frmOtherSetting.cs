using Edward;
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
    public partial class frmOtherSetting : Form
    {
        public frmOtherSetting()
        {
            InitializeComponent();
        }

        private void frmOtherSetting_Load(object sender, EventArgs e)
        {
            //加载数据
            loadData2UI();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private void loadData2UI()
        {
            //sysconfig
            this.txtMaxErrorCount.Text = Param.MaxErrorCount.ToString();
            this.txtMaxRetestCount.Text = Param.MaxRetestCount.ToString();
            checkSwitchStatus(Param.LeftInsert, this.btnLeftInsertSwitch);
            checkSwitchStatus(Param.RightInsert, this.btnRightInsertSwitch);
            checkSwitchStatus(Param.RTC_Use, this.btnRTC_UseSwtich);
            checkSwitchStatus(Param.UseCommand, this.btnUseCommand);
            checkSwitchStatus(Param.NG_Stop, this.btnNGStop);
            checkSwitchStatus(Param.ST_Flag, this.btnSTSwitch);
            checkSwitchStatus(Param.Upload_SFCS_type, this.btnSFCSUpload_S);  //上抛模式

            //CenterIP_DataBase set
            
            checkSwitchStatus(Param.Center_DataBase_Use, this.btnCenterDB_Use);
            this.txtDataBase_CenterIP.Text = Param.CenterIP_DataBase_IP;
            this.txtDataBase_CenterDB.Text = Param.CenterIP_DataBase_DB;
            this.txtDataBase_CenterTable.Text = Param.CenterIP_DataBase_Table;
            this.txtDataBase_CenterAccount.Text = Param.CenterIP_DataBase_Account;
            this.txtDataBase_CenterPassword.Text = Param.CenterIP_DataBase_Password;
            //RecordIP_Database set
            checkSwitchStatus(Param.Record_DataBase_Use, this.btnReCordDB_Use);
            this.txtDataBase_RecordIP.Text = Param.RecordIP_DataBase_IP;
            this.txtDataBase_RecordDB.Text = Param.RecordIP_DataBase_DB;
            this.txtDataBase_RecordTable.Text = Param.RecordIP_DataBase_TestInfo_Table;
            this.txtDataBase_RecordAccout.Text = Param.RecordIP_DataBase_Account;
            this.txtDataBase_RecordPassword.Text = Param.RecordIP_DataBase_Password;
            //comport_set
            checkSwitchStatus(Param.Scanner_Use, this.btnScanner_UseSwitch);
            checkSwitchStatus(Param.DUT_A_Use, this.btnDUT_A_UseSwitch);
            checkSwitchStatus(Param.DUT_B_Use, this.btnDUT_B_UseSwitch);
            checkSwitchStatus(Param.PLC_Use, this.btnPLC_UseSwitch);
            //timeout_set
            this.txtPowerONTimeOut.Text = Param.PowerONTimeOut.ToString();
            this.txtTestOKTimeOut.Text = Param.TestOKTimeOut.ToString();
            //SFCS_Set
            checkSwitchStatus(Param.CheckRouter, this.btnCheckRouterSwitch);
            checkSwitchStatus(Param.Web_Use, this.btnWeb_UseSwitch);
            this.txtWeb_Site.Text = Param.Web_Site;
            this.txtNet_Server.Text = Param.Net_Server;
            this.txtNet_ID.Text = Param.Net_ID;
            this.txtNet_Password.Text = Param.Net_Password;
            checkSwitchStatus(Param.LeftInsertRe, this.btnLeftInsertRe);
            checkSwitchStatus(Param.RightInserRe, this.btnRightInsertRe);

            this.txtDetectDelay.Text = Param.DetectDelay.ToString();
            this.txtShutDown.Text = Param.ShutDown.ToString();
            this.txtInsertDelay.Text = Param.InsertDelay.ToString();

            //ad 
            checkADModule(btnFx2n_4AD, btnFX3U_4AD);
            //check MB retry power
            checkSwitchStatus(Param.MB_A_Re, this.btnMB_A);
            checkSwitchStatus(Param.MB_B_Re, this.btnMB_B);

        }

        /// <summary>
        /// 根据switch状态更改button属性
        /// </summary>
        /// <param name="bstatus">bool量的status状态</param>
        /// <param name="switchbutton">switch button</param>
        private void checkSwitchStatus(bool bstatus, Button switchbutton)
        {
            if (bstatus)
            {
                switchbutton.Text = Param.swithStatus.OPEN.ToString();
                switchbutton.BackColor  = Color.FromArgb(255, 0, 192, 0);
            }
            else
            {
                switchbutton.Text = Param.swithStatus.CLOSE.ToString();
                switchbutton.BackColor  = Color.Red;
            }
        }

        /// <summary>
        /// check ad module name,change button color
        /// </summary>
        /// <param name="ad2n">fx2n-4ad</param>
        /// <param name="ad3u">fx3u-4ad</param>
        private void checkADModule(Button ad2n, Button ad3u)
        {
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                ad2n.BackColor = Color.FromArgb(255, 0, 192, 0);
                ad3u.BackColor = Color.Red;
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                ad3u.BackColor = Color.FromArgb(255, 0, 192, 0);
                ad2n.BackColor = Color.Red;
            }

        }


        /// <summary>
        /// 自动保存数据时，检查数据，禁止为空,为空则为false，不为空则为true
        /// </summary>
        /// <param name="textbox">check textbox的內容</param>
        /// <param name="paramdata">傳入的參數</param>
        /// <returns >为空则为false，不为空则为true</returns>
        private bool checkInputData(TextBox textbox, string paramdata)
        {
            if (string.IsNullOrEmpty(textbox.Text.Trim()))
            {
                MessageBox.Show("Data Can't be empty");
                textbox.Text = paramdata;
                textbox.SelectionStart = textbox.Text.Trim().Length;
                return false;
            }
            return true;
        }

        private void txtMaxErrorCount_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtMaxErrorCount, Param.MaxErrorCount.ToString()))
                return;

            Param.MaxErrorCount = Convert.ToInt16(this.txtMaxErrorCount.Text.Trim());
            IniFile.IniWriteValue("SysConfig", "MaxErrorCount", Param.MaxErrorCount.ToString(), @Param.IniFilePath);
        }

        private void txtMaxRetestCount_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtMaxRetestCount, Param.MaxRetestCount.ToString()))
                return;

            Param.MaxRetestCount = Convert.ToInt16(this.txtMaxRetestCount.Text.Trim());
            IniFile.IniWriteValue("SysConfig", "MaxRetestCount", Param.MaxRetestCount.ToString(), @Param.IniFilePath);
        }

        private void btnLeftInsertSwitch_Click(object sender, EventArgs e)
        {
            if (Param.LeftInsert)
            {
                Param.LeftInsert = false;
                IniFile.IniWriteValue("SysConfig", "LeftInsert", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.LeftInsert, this.btnLeftInsertSwitch);
                return;
            }

            if (!Param.LeftInsert)
            {
                Param.LeftInsert = true;
                IniFile.IniWriteValue("SysConfig", "LeftInsert", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.LeftInsert, this.btnLeftInsertSwitch);
                return;
            } 
        }

        private void btnRightInsertSwitch_Click(object sender, EventArgs e)
        {
            if (Param.RightInsert)
            {
                Param.RightInsert = false;
                IniFile.IniWriteValue("SysConfig", "RightInsert", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.RightInsert, this.btnRightInsertSwitch);
                return;
            }

            if (!Param.RightInsert)
            {
                Param.RightInsert = true;
                IniFile.IniWriteValue("SysConfig", "RightInsert", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.RightInsert, this.btnRightInsertSwitch);
                return;
            }
        }

        private void btnRTC_UseSwtich_Click(object sender, EventArgs e)
        {
            if (Param.RTC_Use)
            {
                Param.RTC_Use = false;
                IniFile.IniWriteValue("SysConfig", "RTC_Use", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.RTC_Use, this.btnRTC_UseSwtich);
                return;
            }

            if (!Param.RTC_Use)
            {
                Param.RTC_Use = true;
                IniFile.IniWriteValue("SysConfig", "RTC_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.RTC_Use, this.btnRTC_UseSwtich);
                return;
            }
        }

        private void txtDataBase_DB_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_CenterDB, Param.CenterIP_DataBase_DB))
                return;
            Param.CenterIP_DataBase_DB = this.txtDataBase_CenterDB.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_DB", Param.CenterIP_DataBase_DB, @Param.IniFilePath);
            Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
        }

        private void txtDataBase_Table_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_CenterTable, Param.CenterIP_DataBase_Table))
                return;
            Param.CenterIP_DataBase_Table = this.txtDataBase_CenterTable.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_Table", Param.CenterIP_DataBase_Table, @Param.IniFilePath);
            Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
        }

        private void txtDataBase_Account_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_CenterAccount, Param.CenterIP_DataBase_Account))
                return;
            Param.CenterIP_DataBase_Account = this.txtDataBase_CenterAccount.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_Account", Param.CenterIP_DataBase_Account, @Param.IniFilePath);
            Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
        }

        private void txtDataBase_Password_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_CenterPassword, Param.CenterIP_DataBase_Password))
                return;
            Param.CenterIP_DataBase_Password = this.txtDataBase_CenterPassword.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_Password", Param.CenterIP_DataBase_Password, @Param.IniFilePath);
            Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
        }

        private void btnScanner_UseSwitch_Click(object sender, EventArgs e)
        {
            if (Param.Scanner_Use)
            {
                Param.Scanner_Use = false;
                IniFile.IniWriteValue("ComPort_Set", "Scanner_Use", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.Scanner_Use, this.btnScanner_UseSwitch);
                return;
            }

            if (!Param.Scanner_Use)
            {
                Param.Scanner_Use = true;
                IniFile.IniWriteValue("ComPort_Set", "Scanner_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.Scanner_Use, this.btnScanner_UseSwitch);
                return;
            }
        }

        private void btnDUT_A_UseSwitch_Click(object sender, EventArgs e)
        {
            if (Param.DUT_A_Use)
            {
                Param.DUT_A_Use = false;
                IniFile.IniWriteValue("ComPort_Set", "DUT_A_Use", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.DUT_A_Use, this.btnDUT_A_UseSwitch);
                return;
            }

            if (!Param.DUT_A_Use)
            {
                Param.DUT_A_Use = true;
                IniFile.IniWriteValue("ComPort_Set", "DUT_A_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.DUT_A_Use, this.btnDUT_A_UseSwitch);
                return;
            }
        }

        private void btnDUT_B_UseSwitch_Click(object sender, EventArgs e)
        {
            if (Param.DUT_B_Use)
            {
                Param.DUT_B_Use = false;
                IniFile.IniWriteValue("ComPort_Set", "DUT_B_Use", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.DUT_B_Use, this.btnDUT_B_UseSwitch);
                return;
            }

            if (!Param.DUT_B_Use)
            {
                Param.DUT_B_Use = true;
                IniFile.IniWriteValue("ComPort_Set", "DUT_B_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.DUT_B_Use, this.btnDUT_B_UseSwitch);
                return;
            }
        }

        private void btnPLC_UseSwitch_Click(object sender, EventArgs e)
        {
            if (Param.PLC_Use)
            {
                Param.PLC_Use = false;
                IniFile.IniWriteValue("ComPort_Set", "PLC_Use", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.PLC_Use, this.btnPLC_UseSwitch);
                return;
            }

            if (!Param.PLC_Use)
            {
                Param.PLC_Use = true;
                IniFile.IniWriteValue("ComPort_Set", "PLC_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.PLC_Use, this.btnPLC_UseSwitch);
                return;
            }
        }

        private void txtPowerONTimeOut_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtPowerONTimeOut, Param.PowerONTimeOut.ToString()))
                return;
            Param.PowerONTimeOut = Convert.ToInt16(this.txtPowerONTimeOut.Text.Trim());
            IniFile.IniWriteValue("TimeOut_Set", "PowerONTimeOut", Param.PowerONTimeOut.ToString(), @Param.IniFilePath);
        }

        private void txtTestOKTimeOut_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtTestOKTimeOut, Param.TestOKTimeOut.ToString()))
                return;
            Param.TestOKTimeOut = Convert.ToInt16(this.txtTestOKTimeOut.Text.Trim());
            IniFile.IniWriteValue("TimeOut_Set", "TestOKTimeOut", Param.TestOKTimeOut.ToString(), @Param.IniFilePath);
        }

        private void btnWeb_UseSwitch_Click(object sender, EventArgs e)
        {
            if (Param.Web_Use)
            {
                DialogResult MsgBoxResult;//设置对话框返回值
                string MEG = string.Empty;
                MEG = "你是否确定要关闭SFCS？" + "\n";
                MEG += "\n";
                MEG += "注意，关闭后：" + "\n" + "1.不能获取双板另一面条码。" + "\n";
                MEG += "2.不能获取条码Model，UPN，MO，Mac等信息。" + "\n";
                MEG += "3.不能上抛MB测试结果及机台信息（相当于无效测试）。" + "\n";
                MEG +=  "\n";
                MEG += "是点击“Yes”，否点击“NO”" + "\n";
                MsgBoxResult = MessageBox.Show(MEG, "小心操作！！！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (MsgBoxResult == DialogResult.Yes)
                {
                    Param.Web_Use = false;
                    IniFile.IniWriteValue("SFCS_Set", "Web_Use", "0", @Param.IniFilePath);
                    checkSwitchStatus(Param.Web_Use, this.btnWeb_UseSwitch);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "设置窗口：执行关闭SFCS动作...");
                }
                if (MsgBoxResult == DialogResult.No)
                {
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "取消关闭SFCS动作...");
                }
                return;
            }

            if (!Param.Web_Use)
            {
                Param.Web_Use = true;
                IniFile.IniWriteValue("SFCS_Set", "Web_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.Web_Use, this.btnWeb_UseSwitch);
                return;
            }
        }

        private void btnCheckRouterSwitch_Click(object sender, EventArgs e)
        {
            if (Param.CheckRouter)
            {
                Param.CheckRouter = false;
                IniFile.IniWriteValue("SFCS_Set", "CheckRouter", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.CheckRouter, this.btnCheckRouterSwitch);
                return;
            }

            if (!Param.CheckRouter)
            {
                Param.CheckRouter = true;
                IniFile.IniWriteValue("SFCS_Set", "CheckRouter", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.CheckRouter, this.btnCheckRouterSwitch);
                return;
            }
        }

        private void txtWeb_Site_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtWeb_Site, Param.Web_Site))
                return;
            Param.Web_Site = this.txtWeb_Site.Text.Trim();
            IniFile.IniWriteValue("SFCS_Set", "Web_Site", @Param.Web_Site, @Param.IniFilePath);
        }

        private void txtNet_Server_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtNet_Server, Param.Net_Server))
                return;
            Param.Net_Server = this.txtNet_Server.Text.Trim();
            IniFile.IniWriteValue("SFCS_Set", "Net_Server", @Param.Net_Server, @Param.IniFilePath);
        }

        private void txtNet_ID_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtNet_ID, Param.Net_ID))
                return;
            Param.Net_ID = this.txtNet_ID.Text.Trim();
            IniFile.IniWriteValue("SFCS_Set", "Net_ID", Param.Net_ID, @Param.IniFilePath);
        }

        private void txtNet_Password_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtNet_Password, Param.Net_Password))
                return;
            Param.Net_Password = this.txtNet_Password.Text.Trim();
            IniFile.IniWriteValue("SFCS_Set", "Net_Password", Param.Net_Password, @Param.IniFilePath);
        }

        private void btnLeftInsertRe_Click(object sender, EventArgs e)
        {
            if (Param.LeftInsertRe )
            {               
                Param.LeftInsertRe = false;
                IniFile.IniWriteValue("SysConfig", "LeftInsertRe", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.LeftInsertRe, this.btnLeftInsertRe);
                return;
            }

            if (!Param.LeftInsertRe)
            {
                Param.LeftInsertRe = true;
                IniFile.IniWriteValue("SysConfig", "LeftInsertRe", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.LeftInsertRe, this.btnLeftInsertRe);
                return;
            }
        }

        private void btnRightInsertRe_Click(object sender, EventArgs e)
        {
            if (Param.RightInserRe) 
            {
                Param.RightInserRe  = false;
                IniFile.IniWriteValue("SysConfig", "RightInsertRe", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.RightInserRe, this.btnRightInsertRe);
                return;
            }

            if (!Param.RightInserRe)
            {
                Param.RightInserRe = true;
                IniFile.IniWriteValue("SysConfig", "RightInsertRe", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.RightInserRe, this.btnRightInsertRe);
                return;
            }
        }

        private void btnUseCommand_Click(object sender, EventArgs e)
        {
            if (Param.UseCommand )
            {
                Param.UseCommand = false;
                IniFile.IniWriteValue("SysConfig", "UseCommand", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.UseCommand, this.btnUseCommand);
                return;
            }

            if (!Param.UseCommand )
            {
                Param.UseCommand = true;
                IniFile.IniWriteValue("SysConfig", "UseCommand", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.UseCommand, this.btnUseCommand);
                return;
            }
        }

        private void txtDetectDelay_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDetectDelay  ,Param.DetectDelay .ToString()))
                return;      
            Param.DetectDelay = Convert.ToInt16(txtDetectDelay.Text.Trim());
            IniFile.IniWriteValue("SysConfig", "DetectDelay", Param.DetectDelay .ToString (), @Param.IniFilePath);
        }

        private void txtShutDown_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtShutDown , Param.ShutDown .ToString()))
                return;
            Param.ShutDown = Convert.ToInt16(txtShutDown.Text.Trim());
            IniFile.IniWriteValue("SysConfig", "ShutDown", Param.ShutDown.ToString(), @Param.IniFilePath);
        }

        private void btnSTSwitch_Click(object sender, EventArgs e)
        {
            if (Param.ST_Flag )
            {
                Param.ST_Flag = false;
                IniFile.IniWriteValue("SysConfig", "ST_Flag", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.ST_Flag, this.btnSTSwitch);
                return;
            }

            if (!Param.ST_Flag )
            {
                Param.ST_Flag  = true;
                IniFile.IniWriteValue("SysConfig", "ST_Flag", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.ST_Flag, this.btnSTSwitch);
                return;
            }
        }

        private void txtInsertDelay_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtInsertDelay , Param.InsertDelay .ToString ()))
                return;
            Param.InsertDelay = Convert.ToInt16(txtInsertDelay.Text.Trim());
            IniFile.IniWriteValue("SysConfig", "InsertDelay", Param.InsertDelay.ToString(), @Param.IniFilePath);

        }

        private void btnFx2n_4AD_Click(object sender, EventArgs e)
        {
            if (btnFx2n_4AD.BackColor == Color.Red)
            {
                //
                Param.AD_Module_Type = Param.AD_Module.FX2N_4AD.ToString();
            }
            else
            {
                Param.AD_Module_Type = Param.AD_Module.FX3U_4AD.ToString();
            }
            IniFile.IniWriteValue ("SysConfig", "AD_Module_Type", Param.AD_Module_Type, @Param.IniFilePath);
            checkADModule(btnFx2n_4AD, btnFX3U_4AD);
        }

        private void btnFX3U_4AD_Click(object sender, EventArgs e)
        {
            if (btnFX3U_4AD.BackColor == Color.Red)
            {
                //
                Param.AD_Module_Type = Param.AD_Module.FX3U_4AD.ToString();
            }
            else
            {
                Param.AD_Module_Type = Param.AD_Module.FX2N_4AD.ToString();
            }
            IniFile.IniWriteValue("SysConfig", "AD_Module_Type", Param.AD_Module_Type, @Param.IniFilePath);
            checkADModule(btnFx2n_4AD, btnFX3U_4AD);
        }

        private void btnNGStop_Click(object sender, EventArgs e)
        {
            if (Param.NG_Stop )
            {
                Param.NG_Stop = false;
                IniFile.IniWriteValue("SysConfig", "NG_Stop", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.NG_Stop, this.btnNGStop);
                return;
            }
            if (!Param.NG_Stop)
            {
                Param.NG_Stop = true;
                IniFile.IniWriteValue("SysConfig", "NG_Stop", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.NG_Stop, this.btnNGStop);
                return;
            }
            
        }

        private void txtDataBase_RecordDB_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_RecordDB, Param.RecordIP_DataBase_DB))
                return;
            Param.RecordIP_DataBase_DB = this.txtDataBase_RecordDB.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_DB", Param.RecordIP_DataBase_DB, @Param.IniFilePath);
            Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
        }

        private void txtDataBase_RecordTable_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_RecordTable, Param.RecordIP_DataBase_TestInfo_Table))
                return;
            Param.RecordIP_DataBase_TestInfo_Table = this.txtDataBase_RecordTable.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_Table", Param.RecordIP_DataBase_TestInfo_Table, @Param.IniFilePath);
            Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
        }

        private void txtDataBase_RecordAccout_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_RecordAccout, Param.RecordIP_DataBase_Account))
                return;
            Param.RecordIP_DataBase_Account = this.txtDataBase_RecordAccout.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_Account", Param.RecordIP_DataBase_Account, @Param.IniFilePath);
            Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
        }

        private void txtDataBase_RecordPassword_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_RecordPassword, Param.RecordIP_DataBase_Password))
                return;
            Param.RecordIP_DataBase_Password = this.txtDataBase_RecordPassword.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_Password", Param.RecordIP_DataBase_Password, @Param.IniFilePath);
            Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
        }

        private void txtDataBase_CenterIP_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_CenterIP, Param.CenterIP_DataBase_IP))
                return;
            Param.CenterIP_DataBase_IP = this.txtDataBase_CenterIP.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_IP", Param.CenterIP_DataBase_IP, @Param.IniFilePath);
            Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
        }

        private void txtDataBase_RecordIP_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtDataBase_RecordIP, Param.RecordIP_DataBase_IP))
                return;
            Param.RecordIP_DataBase_IP = this.txtDataBase_RecordIP.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_IP", Param.RecordIP_DataBase_IP, @Param.IniFilePath);
            Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
        }

        private void btnSFCSUpload_S_Click(object sender, EventArgs e)
        {

            checkSwitchStatus(Param.Upload_SFCS_type, this.btnSFCSUpload_S);
            MessageBox.Show("还在完善期间...");
            /*
            if (Param.Upload_SFCS_type)
            {
                Param.Upload_SFCS_type = false ;
                checkSwitchStatus(Param.Upload_SFCS_type, this.btnSFCSUpload_S);
                return;
            }
            if (!Param.Upload_SFCS_type)
            {
                Param.Upload_SFCS_type = true;
                checkSwitchStatus(Param.Upload_SFCS_type, this.btnSFCSUpload_S);
                return;
            }
            */


        }

        private void btnReCordDB_Use_Click(object sender, EventArgs e)
        {
            if (Param.Record_DataBase_Use)
            {
                DialogResult MsgBoxResult;//设置对话框返回值
                string MEG = string.Empty;
                MEG = "你是否确定要关闭记录数据库？" + "\n";
                MEG += "\n";
                MEG += "注意，关闭后：" + "\n" + "1.不能自动获取中控电脑IP地址。" + "\n";
                MEG += "2.不能向数据库写入TestLog。" + "\n";
                MEG += "3.远程无法监控本机台运行状况。" + "\n";
                MEG +=  "\n";
                MEG += "是点击“Yes”，否点击“NO”" + "\n";
                MsgBoxResult = MessageBox.Show(MEG, "小心操作！！！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (MsgBoxResult == DialogResult.Yes)
                {
                    Param.Record_DataBase_Use = false;
                    IniFile.IniWriteValue("DB_Set", "Record_DataBase_Use", "0", @Param.IniFilePath);
                    checkSwitchStatus(Param.Record_DataBase_Use, this.btnReCordDB_Use);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "设置窗口：执行关闭记录数据库动作...");
                }
                if (MsgBoxResult == DialogResult.No)
                {
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "取消关闭记录数据库动作...");
                }              
                return;
            }

            if (!Param.Record_DataBase_Use)
            {
                Param.Record_DataBase_Use = true;
                IniFile.IniWriteValue("DB_Set", "Record_DataBase_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.Record_DataBase_Use, this.btnReCordDB_Use);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "开启记录数据库...");
                return;
            }
        }

        private void btnCenterDB_Use_Click(object sender, EventArgs e)
        {
            if (Param.Center_DataBase_Use)
            {
                Param.Center_DataBase_Use = false;
                IniFile.IniWriteValue("DB_Set", "Center_DataBase_Use", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.Center_DataBase_Use, this.btnCenterDB_Use);
                return;
            }

            if (!Param.Center_DataBase_Use)
            {
                Param.Center_DataBase_Use = true;
                IniFile.IniWriteValue("DB_Set", "Center_DataBase_Use", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.Center_DataBase_Use, this.btnCenterDB_Use);
                return;
            }
        }

        private void btnMB_A_Click(object sender, EventArgs e)
        {
            if (Param.MB_A_Re )
            {
                Param.MB_A_Re  = false;
                IniFile.IniWriteValue("SysConfig", "MB_A_Re", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.MB_A_Re, this.btnMB_A);
                return;
            }

            if (!Param.MB_A_Re)
            {
                Param.MB_A_Re = true ;
                IniFile.IniWriteValue("SysConfig", "MB_A_Re", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.MB_A_Re, this.btnMB_A);
                return;
            }
 
        }

        private void btnMB_B_Click(object sender, EventArgs e)
        {
            if (Param.MB_B_Re)
            {
                Param.MB_B_Re = false;
                IniFile.IniWriteValue("SysConfig", "MB_B_Re", "0", @Param.IniFilePath);
                checkSwitchStatus(Param.MB_B_Re, this.btnMB_B);
                return;
            }

            if (!Param.MB_B_Re)
            {
                Param.MB_B_Re = true;
                IniFile.IniWriteValue("SysConfig", "MB_B_Re", "1", @Param.IniFilePath);
                checkSwitchStatus(Param.MB_B_Re, this.btnMB_B);
                return;
            }
        }




    }
}
