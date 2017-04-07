using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using Edward;

//using MySql.Data.MySqlClient;


namespace FICT_2._0_Duplex_Panel
{
    public partial class FICTDebug : Form
    {
        public FICTDebug()
        {
            InitializeComponent();
        }


        #region 参数定义
        //
        string FICT_PLC_Write_AddressA = "D4"; //写FICT的地址
        string FICT_PLC_Write_AddressB = "D7";
        string FICT_PLC_Read_AddressA = "D9"; //读FICT的地址
        string FICT_PLC_Read_AddressB = "D11";
        //
        //FICT
        string FICT_ReadValue = string.Empty; //从FICT寄存器中读到的值
        string FICT_Last_ReadValue = string.Empty; //上一次从FICT寄存器中读到的值

        //
        #endregion


        private void FICTDebug_Load(object sender, EventArgs e)
        {
            this.btnCLOSE.Enabled = false ;
        }
        #region 串口操作（SerialPort）


        /// <summary>
        /// 获取串口 
        /// </summary>
        /// <param name="combox"></param>
        private void getSerialPort(ComboBox combox)
        {
            combox.Items.Clear();
            combox.Text = string.Empty;
            foreach (string sp in System.IO.Ports.SerialPort.GetPortNames())
            {
                combox.Items.Add(sp);
            }

            if (combox.Items.Count > 0)
            {
                combox.SelectedIndex = 0;
            }

            //if (combox.Items.Count == 1)
            //{
            //    //結果存入ini
            //    IniFile.IniWriteValue("COM_Set", "PLC_COM", combox.SelectedItem.ToString(), Param.IniFilePath);
            //    portName = combox.SelectedItem.ToString();
            //}
        }


        /// <summary>
        /// 获取串口 
        /// </summary>
        /// <param name="combox"></param>
        /// <param name="paramdate">存储串口的数据</param>
        private void getSerialPort(ComboBox combox, string paramdate)
        {
            combox.Items.Clear();
            combox.Text = string.Empty;
            foreach (string sp in System.IO.Ports.SerialPort.GetPortNames())
            {
                combox.Items.Add(sp);
            }

            if (combox.Items.Count > 0)
            {
                if (string.IsNullOrEmpty(paramdate))
                    combox.SelectedIndex = 0;
                else
                    combox.Text = paramdate;
            }

            //if (combox.Items.Count == 1)
            //{
            //    //結果存入ini
            //    IniFile.IniWriteValue("COM_Set", "PLC_COM", combox.SelectedItem.ToString(), Param.IniFilePath);
            //    portName = combox.SelectedItem.ToString();
            //}
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sp">控件</param>
        /// <param name="portname">串口名称</param>
        /// <returns>OK=true,NG=false</returns>
        private bool openSerialPort(SerialPort sp, string portname)
        {
            // bool result = true;

            if (!sp.IsOpen)
            {
                try
                {
                    sp.PortName = portname;
                    sp.Open();
                    SubFunction.updateMessage(lstCommandList, "机台调试:打开串口 <" + portname + "> 成功...");//Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:打开串口 <" + portname + "> 成功...\r\n");//Message:" + e.Message + "\r\n");
                }
                catch (Exception e)
                {
                    MessageBox.Show("机台调试:不能打开串口 <" + portname + ">,Message:" + e.Message);
                    SubFunction.updateMessage(lstCommandList, "机台调试:不能打开串口 <" + portname + "> ,Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:不能打开串口 <" + portname + "> ,Message:" + e.Message + "\r\n");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// close serial port
        /// </summary>
        /// <param name="sp">OK=true,NG=false</param>
        /// <returns></returns>
        private bool closeSerialPort(SerialPort sp)
        {
            if (sp.IsOpen)
            {
                try
                {
                    sp.Close();
                    SubFunction.updateMessage(lstCommandList, "机台调试:关闭串口 <" + sp.PortName.ToString() + "> 成功...");//Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:关闭串口 <" + sp.PortName.ToString() + "> 成功...\r\n");//Message:" + e.Message + "\r\n");
                }
                catch (Exception e)
                {
                    //MessageBox.Show("Can't close SerialPort=" + sp.PortName.ToString() + ",Message:" + e.Message);
                    SubFunction.updateMessage(lstCommandList, "机台调试:不能关闭串口 <" + sp.PortName.ToString() + "> ,Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:不能关闭串口 <" + sp.PortName.ToString() + "> ,Message:" + e.Message + "\r\n");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 发送数据到串口
        /// </summary>
        /// <param name="spport">串口控件</param>
        /// <param name="strdata">发送的数据</param>
        private void sendData(SerialPort spport, string strdata)
        {
            try
            {
                spport.Write(strdata);
                SubFunction.updateMessage(lstCommandList, "机台调试:向 <" + spport.PortName + "> 发送 '" + strdata + "' 成功...");//Message:" + e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(),"机台调试:向 <" + spport.PortName + "> 发送 '" + strdata + "' 成功...\r\n");//Message:" + e.Message + "\r\n");
            }
            catch (Exception e)
            {
                SubFunction.updateMessage(lstCommandList, "机台调试:向 <" + spport.PortName + "> 发送 '" + strdata + "' 失败...");
                SubFunction.updateMessage(lstCommandList, e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:向 <" + spport.PortName + "> 发送 ' " + strdata + "' 失败...," + e.Message);
            }
        }

        #endregion
        #region MX操作PLC

        /// <summary>
        /// 连接Robot PLC
        /// </summary>
        /// <param name="portname">Robot PLC IP</param>
        /// <returns></returns>
        private bool openPLC(string portname)
        {
            try
            {
                int iReturnCode = -1;//定義返回值
                int iMaxRetry = 3;
                //设置PLC属性            
                axActPLC.ActCpuType = 520;
                // axActPLC.ActHostAddress = portname;
                axActPLC.ActPortNumber = Convert.ToInt16(portname.Replace("COM", string.Empty));
                //axActPLC.ActSourceStationNumber = 5556;
                axActPLC.ActUnitType = 0x000F;
                axActPLC.ActProtocolType = 0x0004;
                axActPLC.ActCpuType = 0x0208;

                Stopwatch sw = new Stopwatch();
                TimeSpan ts = new TimeSpan();


                for (int i = 1; i <= iMaxRetry; i++)
                {
                    sw.Start();
                    SubFunction.updateMessage(lstCommandList, "第" + i + "次连接PLC");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次连接PLC");
                    Application.DoEvents();
                    try
                    {
                        iReturnCode = axActPLC.Open();
                    }
                    catch (Exception ex)
                    {
                        SubFunction.updateMessage(lstCommandList, "第" + i + "次连接PLC NG,Message:" + ex.Message);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次连接PLC NG,Message:" + ex.Message);
                    }
                    sw.Stop();
                    ts = sw.Elapsed;
                    if (iReturnCode == 0)
                    {
                        SubFunction.updateMessage(lstCommandList, "第" + i + "次连接Robot PLC,Used time(s):" + ts.Seconds);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次连接PLC OK,Used time(s):" + ts.Seconds);
                        return true;
                    }
                }
                // sw.Stop();
                SubFunction.updateMessage(lstCommandList, "第" + iMaxRetry + "次连接PLC NG,,已达到最大retry数，Errorcode=" + iReturnCode + ",Used time(s):" + ts.Seconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + iMaxRetry + "次连接PLC NG,已达到最大retry数，Errorcode=" + iReturnCode + ",Used time(s):" + ts.Seconds);
                return false;
            }
            catch (Exception exx)
            {
                SubFunction.updateMessage(lstCommandList,"连接PLC出错，Error：" +  exx.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "连接PLC出错，Error：" + exx.Message);
                return false;
            }
        }

        /// <summary>
        /// 断开Robot PLC
        /// </summary>   
        /// <returns></returns>
        private bool closePLC()
        {
            try
            {
                axActPLC.Close();
                SubFunction.updateMessage(lstCommandList, "clsoe PLC");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "close PLC");

            }
            catch (Exception e)
            {
                SubFunction.updateMessage(lstCommandList, "clsoe PLC error," + e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "close PLC," + e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从Robot PLC中读取指定寄存器的值
        /// </summary>
        /// <param name="readaddress">寄存器的地址</param>
        /// <param name="readdata">读出的值</param>
        /// <returns></returns>
        public bool readPLC(string readaddress, ref int readdata)
        {
            int iReturn = -1; //执行返回的值

            try
            {
                iReturn = axActPLC.GetDevice(readaddress, out  readdata);
            }
            catch (Exception e)
            {
                SubFunction.updateMessage(lstCommandList, "Read " + readaddress + " Fail.");
                SubFunction.updateMessage(lstCommandList, e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Read " + readaddress + " Fail," + e.Message);
                return false;
            }

            if (iReturn == 0)
            {
                return true;
            }
            else
            {
                SubFunction.updateMessage(lstCommandList, "Read " + readaddress + " Fail,errorcode = " + iReturn);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Read " + readaddress + " Fail,errorcode = " + iReturn);
                return true;
            }


        }

        /// <summary>
        /// 向PLC指定的寄存器写值
        /// </summary>
        /// <param name="devicename"></param>
        /// <param name="writedata"></param>
        private bool writePLC(string devicename, int writedata)
        {
            int iReturn = -1;//寄存器操作返回的值
            //先读要写入的寄存器的值，若值等于要写入的值，则不用写
            int dataread = -1;
            if (!readPLC(devicename, ref dataread))
                return false;
            if (dataread == writedata)
                return true; //值已相等
            //int retry = 0;
            //while (retry <= 1)
            //{
            try
            {
                iReturn = axActPLC.SetDevice(devicename, writedata);
            }
            catch (Exception e)
            {
                SubFunction.updateMessage(lstCommandList, "Write " + devicename + " Fail");
                SubFunction.updateMessage(lstCommandList, e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Write " + devicename + " Fail," + e.Message);
                return false;
            }

            if (iReturn != 0)
            {
                SubFunction.updateMessage(lstCommandList, "Write " + devicename + " Fail,errorcode = " + iReturn);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Write " + devicename + " Fail,errorcode = " + iReturn);
                return false;
            }
            else
            {
                return true;
            }
            //}

        }

        /// <summary>
        /// 向FICT PLC 发送命令
        /// </summary>
        /// <param name="commandStr">发送的命令</param>
        private void sendPLC(string commandStr)
        {
            string sA = commandStr.Substring(0, 1);
            string sB = commandStr.Substring(1, 1);

            if (writePLC(FICT_PLC_Write_AddressA, Other.Asc(sA)) && writePLC(FICT_PLC_Write_AddressB, Other.Asc(sB)))
            {
                SubFunction.updateMessage(lstCommandList, "PC->PLC:" + commandStr);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->PLC:" + commandStr);
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->PLC:" + commandStr);
            }
            else
            {
                SubFunction.updateMessage(lstCommandList, "PC->PLC:" + commandStr + " Fail");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->PLC:" + commandStr + " Fail");
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->PLC:" + commandStr);
            }

        }

        /// <summary>
        /// 从FICT PLC 接收数据
        /// </summary>
        /// <param name="outdata"></param>
        private void receivePLC(ref string outdata)
        {
            int iA = -1;
            int iB = -1;
            if (readPLC(FICT_PLC_Read_AddressA, ref iA) && readPLC(FICT_PLC_Read_AddressB, ref iB))
            {
                outdata = Other.Chr(iA) + Other.Chr(iB);
                SubFunction.updateMessage(lstCommandList, "FICT->PC:" + outdata);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata);
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata);
            }
            else
            {
                SubFunction.updateMessage(lstCommandList, "FICT->PC:" + outdata + " Fail");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata + " Fail");
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata + " Fail");
            }

        }

        /// <summary>
        /// 从FICT PLC 接收数据
        /// </summary>
        /// <param name="outdata"></param>
        /// <param name="msgflag">是否显示結果</param>
        private void receivePLC(ref string outdata, bool msgflag)
        {
            int iA = -1;
            int iB = -1;
            if (readPLC(FICT_PLC_Read_AddressA, ref iA) && readPLC(FICT_PLC_Read_AddressB, ref iB))
            {
                try
                {
                    outdata = Other.Chr(iA) + Other.Chr(iB);
                }
                catch (Exception)
                {

                }
                if (msgflag)
                {
                    SubFunction.updateMessage(lstCommandList, "FICT->PC:" + outdata);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata);
                    SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata);
                }
            }
            else
            {
                SubFunction.updateMessage(lstCommandList, "FICT->PC:" + outdata + " Fail");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata + " Fail");
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata + " Fail");
            }

        }



        #endregion

     

        private void btnIN_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击< 进板 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击< 进板 > 按钮");
            sendPLC("IN");
        }

        private void btnFresh_Click(object sender, EventArgs e)
        {
            getSerialPort(comboPLC);
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <刷新> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <刷新> 按钮...\r\n");
        }

        private void btnOPEN_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <开始> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <开始> 按钮......");
            if (!openPLC(Param.PLC))
                return;
            this.btnCLOSE.Enabled = true;
            this.comboPLC.Enabled = false;
            this.btnOPEN.Enabled = false;
            this.btnFresh.Enabled = false;

           // timerScanFICT.Start();
        }

        private void btnCLOSE_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <关闭> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <关闭> 按钮...");
            closePLC();
            this.btnOPEN.Enabled = true;
            this.btnCLOSE.Enabled = false;
            this.comboPLC.Enabled = true;
           // timerScanFICT.Stop();
        }

        private void timerScanFICT_Tick(object sender, EventArgs e)
        {

            //A面
            sendPLC("TA");
            string plcvalue_A = string.Empty;
            receivePLC(ref plcvalue_A, true);


            int plcread_A = 0;
            readPLC("D100", ref plcread_A);
            double mbV_A = 0;
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                mbV_A = 10.0 / 2000.0 * plcread_A;
                if (mbV_A >= 10) mbV_A = 0;//溢出
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                mbV_A = 10.0 / 4000.0 * plcread_A;
                if (mbV_A >= 10) mbV_A = 0;//溢出
            }
            TB_MB_A_Voltage.Text = mbV_A.ToString();


            //B面
            sendPLC("TB");
            string plcvalue_B = string.Empty;
            receivePLC(ref plcvalue_B, true);

            int plcread_B = 0;
            readPLC("D101", ref plcread_B);
            double mbV_B = 0;
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                mbV_B = 10.0 / 2000.0 * plcread_B;
                if (mbV_B >= 10) mbV_B = 0;//溢出
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                mbV_B = 10.0 / 4000.0 * plcread_B;
                if (mbV_B >= 10) mbV_B = 0;//溢出
            }
            TB_MB_B_Voltage.Text = mbV_B.ToString();



        }

        private void btnOU_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <退出> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <退出> 按钮...");
            sendPLC("OU");
        }

        private void btnUP_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <上升> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <上升> 按钮");
            sendPLC("UP");
        }

        private void btnDO_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <下降> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <下降> 按钮...");
            sendPLC("DO");
        }

        private void btnLC_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <插左侧插> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <插左侧插> 按钮...");
            sendPLC("LC");
        }

        private void btnOL_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <退左侧插> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <退左侧插> 按钮...");
            sendPLC("OL");
        }

        private void btnRC_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <插右侧插> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <插右侧插> 按钮...");
            sendPLC("RC");
        }

        private void btnOR_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <退右侧插> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <退右侧插> 按钮...");
            sendPLC("OR");
        }

        private void btnABU_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <B板通19V> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <B板通19V> 按钮...");
            sendPLC("AB");
        }

        private void btnabL_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <B板断开19V> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <B板断开19V> 按钮...");
            sendPLC("ab");
        }

        private void btnO2_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <B板开机> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <B板板开机> 按钮...");
            sendPLC("O2");
        }

        private void btnAaU_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <A板通19V> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <A板通19V> 按钮...");
            sendPLC("AA");
        }

        private void btnS2_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <B板关机> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <B板关机> 按钮...");
            sendPLC("S2");
        }

        private void btnTB_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <B板检测电压> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <B板检测电压> 按钮...");
            sendPLC("TB");

            string plcvalue = string.Empty;
            receivePLC(ref plcvalue, true);

            int plcread = 0;
            readPLC("D101", ref plcread);
            double mbV = 0;
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                mbV = 10.0 / 2000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                mbV = 10.0 / 4000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }
            TB_MB_B_Voltage.Text = mbV.ToString();

        }

        private void btnXB_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <B板查询电压> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <B板查询电压> 按钮...");
            sendPLC("XB");

            string plcvalue = string.Empty;
            receivePLC(ref plcvalue, true);

            int plcread = 0;
            readPLC("D101", ref plcread);
            double mbV = 0;
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                mbV = 10.0 / 2000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                mbV = 10.0 / 4000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }
            TB_MB_B_Voltage.Text = mbV.ToString();
        }

        private void btnO1_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <A板开机> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <A板开机> 按钮...");
            sendPLC("O1");
        }

        private void btnaaL_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <A板断开19V> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <A板断开19V> 按钮...");
            sendPLC("aa");
        }

        private void btnS1_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <A板关机> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <A板关机> 按钮...");
            sendPLC("S1");
        }

        private void btnTA_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <A板检测电压> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <A板检测电压> 按钮...");
            sendPLC("TA");
            string plcvalue = string.Empty;
            receivePLC(ref plcvalue, true);


            int plcread = 0;
            readPLC("D100", ref plcread);
            double mbV = 0;
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                mbV = 10.0 / 2000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                mbV = 10.0 / 4000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }
            TB_MB_A_Voltage.Text = mbV.ToString();
        }

        private void btnXA_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击 <A板查询电压> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击 <A板查询电压> 按钮...");
            sendPLC("XA");

            int plcread = 0;
            readPLC("D100", ref plcread);
            double mbV = 0;
            if (Param.AD_Module_Type == Param.AD_Module.FX2N_4AD.ToString())
            {
                mbV = 10.0 / 2000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }

            if (Param.AD_Module_Type == Param.AD_Module.FX3U_4AD.ToString())
            {
                mbV = 10.0 / 4000.0 * plcread;
                if (mbV >= 10) mbV = 0;//溢出
            }
            TB_MB_A_Voltage.Text = mbV.ToString();
        }

        private void FICTDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            SubFunction.updateMessage(lstCommandList, "机台调试:点击窗口右上角 <关闭> 按钮...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击窗口右上角 <关闭> 按钮...");
            closePLC();
            this.btnOPEN.Enabled = true;
            this.btnCLOSE.Enabled = false;
            this.comboPLC.Enabled = true;
        }


    }
}
