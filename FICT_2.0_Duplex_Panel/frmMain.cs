using Edward;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace FICT_2._0_Duplex_Panel
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(0, 0);
            //this.ControlBox = false;
        }

        #region 参数定义

        string exeTitle = "FICT Auto Load Unload Duplex Panel Test（M/B_B延时5s后通电,MB_A/B可重复通电）,Ver:" + Application.ProductVersion + ",Author:edward_song@yeah.net" + "";
        string ipAdress = string.Empty;  //定义IP地址变量


        //database
        string dataBase_ReadAddress = string.Empty; //读数据库的地址
        string dataBase_ReadSNAddress = string.Empty;//读条码的地址
        string dataBase_WriteAddress = string.Empty;//写数据库的地址
        string dataBase_WriteData = string.Empty; //写的值
        string dataBase_ReadData = string.Empty;//读的值
        string dataBase_LastRead = "-1";//上一次读到的值

        //web
        //PcbWeb.WebService ws = new PcbWeb.WebService();

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
        string DUT_A_Status = "WAIT"; //MB的初始状态，WAIT 等待测试
        string DUT_B_Status = "WAIT"; //MB的初始状态，WAIT 等待测试

        int iTotalScan = 0;//总条码数
        string dataBaseStatus = "OK"; //数据库的状态，OK，正常测试，需等待FICT动作，NG，收到数据立即回傳，FICT无动作

        //web
        PcbWeb.WebService ws = new PcbWeb.WebService();
        string[] TrnDatas = new string[1];

        //
        string MB_A_Status = string.Empty;  //MB_A 的测试状态
        string MB_B_Status = string.Empty;  //MB_B 的测试状态
        string MB_A_Last_Status = string.Empty; //MB_A 上一次偵测到的测试状态
        string MB_B_Last_Status = string.Empty; //MB_B 上一次偵测到的测试状态
        bool DUT_A_Retest_Flag = false;// DUT_A 是否需要重复测试，不需要=false，需要=true
        bool DUT_B_Retest_Flag = false;// DUT_B 是否需要重复测试，不需要=false，需要=true
        bool DUT_Retest_Flag = false;//
        public static  int ErrCount = 0; //机台連續NG次数
        int icount_DUT_TestingTimeOut = 0; //DUT_A的计時器，用於和超時比对
        //int icount_DUT_B_TestingTimeOut = 0; //DUT_B的计時器，用於和超時比对
        int iCurrent_TestCount = 1; //当前测试计数
        //int iCurrent_B_TestCount = 1; //当前DUT_B测试计数
        string ngDUT_A_TestItem = string.Empty; //DUT_A当前测试項目
        string ngDUT_B_TestItem = string.Empty; //DUT_B当前测试項目
        string DUT_A_LastMO = string.Empty;//上一次的MO,双连板只判断A的
        string DUT_A_MO = string.Empty; //DUT_A 的MO
        string DUT_A_MAC = string.Empty; //DUT_A的MAC
        string DUT_B_MO = string.Empty; //DUT_B的MO
        string DUT_B_MAC = string.Empty; //DUT_B的MAC

        //MB 串口參数
        string DUT_A_Str = string.Empty; //DUT A 串口收到的数据
        string DUT_A_Last_Str = string.Empty; //DUT A 串口上次收到的数据
        string DUT_B_Str = string.Empty; //DUT B 串口收到的数据
        string DUT_B_Last_Str = string.Empty; //DUT B 串口收到的数据

        //_A arms
        string _ASYSID = string.Empty;
        string _ASRMMODEL = string.Empty;
        string _ADELLMODEL = string.Empty;
        //
        int ShutDownCount_A = 0; //断电次数计数
        int ShutDownCount_B = 0; //断电次数计数
        int i_Detect_A_Count = 0;//偵测电压B的计時
        int i_Detect_B_Count = 0;//偵测电压B的计時
       //  bool _b_V_show = true;//检测电压是否需要显示信息，防止重复显示   //屏蔽 by channing 20161214
        // bool _a_V_show = true;//检测电压是否需要显示信息，防止重复显示   //屏蔽 by channing 20161214

        bool _b_A_send_aa = false; //MB A是否已经发送aa断电命令
        bool _b_B_send_ab = false; //MB_B是否已经发送aa断电命令


        bool _b_Left_Insert_Re = false;//false:重复插拔还沒插;true，重复插拔已经插了
        bool _b_Right_Insert_Re = false;//false:重复插拔还沒插;true，重复插拔已经插了
        bool _b_MB_A_Re = false;//false:重复上电还没上电,true：重复上电已经上过了
        bool _b_MB_B_Re = false;//false:重复上电还没上电,true：重复上电已经上过了

        #endregion

        private void frmMain_Load(object sender, EventArgs e)
        {

            //初始化：
            //Param.bar_A = "TEST_CN041D5Y7620667P0BQCA00";
            //Param.bar_B = "TEST_CN041D5Y7620667P0BQDA00";

            //this.lblStationResult.Text = Param.StationStatus.离线.ToString();



            foreach (string ip in SubFunction.getIP(Dns.GetHostName(), Param.IPType.IPV4.ToString()))
            {
                this.Text = exeTitle + ",本地IP:" + ip;
                ipAdress = ip;
            }
            //检查文件夹
            SubFunction.updateMessage(lstStatusCommand, "检查系统文件夹");
            SubFunction.checkFolder();
            //检查配置档案
            SubFunction.updateMessage(lstStatusCommand, "检查配置文档");
            if (!File.Exists(Param.IniFilePath))
                SubFunction.creatInI(Param.IniFilePath);
            //加载配置档
            SubFunction.updateMessage(lstStatusCommand, "加载配置文档...");
            SubFunction.loadConfigData(Param.IniFilePath);

            //检查临时数据配置档案
            SubFunction.updateMessage(lstStatusCommand, "检查临时数据文档");
            if (!File.Exists(Param.TestDataTempini))
            {
                SubFunction.CreatTemp(Param.TestDataTempini);
            }
            //检查未上抛至记录数据库暂存文件
            if (!File.Exists(Param.MysqlTestDatatxt))
            {
                FileStream fs = File.Create(@Param.MysqlTestDatatxt);
                fs.Close();
            }

            //加载临时配置档
            SubFunction.updateMessage(lstStatusCommand, "加载临时文件文档...");
            SubFunction.loadTemp(Param.TestDataTempini);

            //加载串口
            SubFunction.updateMessage(lstStatusCommand, "加载串口");

            ///////////////////////////
            //条码枪串口
            if (Param.Scanner_Use)
            {
                getSerialPort(comboScannerPort, Param.Scanner);
            }
            else
            {
                comboScannerPort.Enabled = false;
            }
            //DUT串口A
            if (Param.DUT_A_Use)
            {
                getSerialPort(comboDUTPort_A, Param.DUT_A);
            }
            else
            {
                comboDUTPort_A.Enabled = false;
            }
            //DUT串口B
            if (Param.DUT_B_Use)
            {
                getSerialPort(comboDUTPort_B, Param.DUT_B);
            }
            else
            {
                comboDUTPort_B.Enabled = false;
            }
            //plc串口
            if (Param.PLC_Use)
            {
                getSerialPort(comboFICTPort, Param.PLC);
            }
            else
            {
                comboFICTPort.Enabled = false;
            }

            choseDoubleSigle();//初始化单板


            //getSerialPort(comboScannerPort, Param.Scanner);
            //getSerialPort(comboDUTPort_A, Param.DUT_A);
            //getSerialPort(comboDUTPort_B, Param.DUT_B);
            //getSerialPort(comboFICTPort, Param.PLC);
            //update data 2 ui
            loadData2UI();
           
        }


        #region 中小函数

        #region 加载数据库的地址

        /// <summary>
        /// 根据选择的站別，加载读写数据库的地址
        /// </summary>
        /// <param name="ficstage">fict的站別</param>
        private void loadDataBaseAddress(string ficstage)
        {
            switch (ficstage)
            {
                case "A":
                    dataBase_ReadAddress = "FICTA1";
                    dataBase_WriteAddress = "FICTA2";
                    dataBase_ReadSNAddress = "BarcodeSNA";
                    break;
                case "B":
                    dataBase_ReadAddress = "FICTB1";
                    dataBase_WriteAddress = "FICTB2";
                    dataBase_ReadSNAddress = "BarcodeSNB";
                    break;
                case "C":
                    dataBase_ReadAddress = "FICTC1";
                    dataBase_WriteAddress = "FICTC2";
                    dataBase_ReadSNAddress = "BarcodeSNC";
                    break;
                case "D":
                    dataBase_ReadAddress = "FICTD1";
                    dataBase_WriteAddress = "FICTD2";
                    dataBase_ReadSNAddress = "BarcodeSND";
                    break;
                case "E":
                    dataBase_ReadAddress = "FICTE1";
                    dataBase_WriteAddress = "FICTE2";
                    dataBase_ReadSNAddress = "BarcodeSNE";
                    break;
                case "F":
                    dataBase_ReadAddress = "FICTF1";
                    dataBase_WriteAddress = "FICTF2";
                    dataBase_ReadSNAddress = "BarcodeSNF";
                    break;
                case "G":
                    dataBase_ReadAddress = "FICTG1";
                    dataBase_WriteAddress = "FICTG2";
                    dataBase_ReadSNAddress = "BarcodeSNG";
                    break;
                case "H":
                    dataBase_ReadAddress = "FICTH1";
                    dataBase_WriteAddress = "FICTH2";
                    dataBase_ReadSNAddress = "BarcodeSNH";
                    break;
            }
        }

        #endregion

        #region 数据库操作

        /// <summary>
        /// 从数据库中读到的值，成功=true,失败=false
        /// </summary>
        /// <param name="address">address </param>
        /// <param name="outdata">都到的值</param>
        /// <returns>成功返回true，失败返回false</returns>
        private bool readDataBase(string address, ref string outdata)
        {
            string sql = "select * from " + Param.CenterIP_DataBase_Table;
            SubFunction.updateMessage(this.lstStatusCommand, "try to connect database");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "try to connect database");
            MySqlConnection objConnection = new MySqlConnection(Param.Center_DB_ConnStr);
            MySqlCommand objCommand = new MySqlCommand(sql, objConnection);
            MySqlDataReader objReader;
            try
            {
                objConnection.Open();
                SubFunction.updateMessage(this.lstStatusCommand, "connect database success");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), " connect database success");
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "connect database error," + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "connect database error," + ex.Message);
                return false;
            }

            try
            {
                objReader = objCommand.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        outdata = objReader[address].ToString();
                        SubFunction.updateMessage(lstStatusCommand, "read database " + address + "=" + outdata);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "read database " + address + "=" + outdata);
                        SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DataBase_" + address + "->PC=" + outdata);
                    }
                    objConnection.Close();
                }
                else
                {
                    SubFunction.updateMessage(this.lstStatusCommand, "can't find the database has rows");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "can't find the database has rows");
                    objConnection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "read data error," + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "read data error," + ex.Message);
                objConnection.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从数据库中读到的值，成功=true,失败=false
        /// </summary>
        /// <param name="address">address </param>
        /// <param name="outdata">都到的值</param>
        /// <param name="boollog">false</param>
        /// <returns>成功返回true，失败返回false</returns>
        private bool readDataBase(string address, ref string outdata, bool boollog)
        {
            string sql = "select * from " + Param.CenterIP_DataBase_Table;
            if (boollog)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "try to connect database");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "try to connect database");
            }
            MySqlConnection objConnection = new MySqlConnection(Param.Center_DB_ConnStr);
            MySqlCommand objCommand = new MySqlCommand(sql, objConnection);
            MySqlDataReader objReader;
            try
            {
                objConnection.Open();
                if (boollog)
                {
                    SubFunction.updateMessage(this.lstStatusCommand, "connect database success");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), " connect database success");
                }
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "connect database error," + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "connect database error," + ex.Message);
                return false;
            }

            try
            {
                objReader = objCommand.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        outdata = objReader[address].ToString();
                        if (boollog)
                        {
                            SubFunction.updateMessage(lstStatusCommand, "read database " + address + "=" + outdata);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "read database " + address + "=" + outdata);
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DataBase_" + address + "->PC=" + outdata);
                        }
                    }
                    objConnection.Close();
                }
                else
                {
                    SubFunction.updateMessage(this.lstStatusCommand, "can't find the database has rows");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "can't find the database has rows");
                    objConnection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "read data error," + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "read data error," + ex.Message);
                objConnection.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 写数据库的值，成功=true,失败=false，写之前check值是否已经是需要写入的值，是就不繼續写
        /// </summary>
        /// <param name="address">database address</param>
        /// <param name="indata">写入的值</param>
        /// <returns></returns>
        private bool writeDataBase(string address, string indata)
        {
            string tempValue = string.Empty;
            if (!readDataBase(address, ref tempValue, false))
            {
                SubFunction.updateMessage(lstStatusCommand, "before write check database fail");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "before write check database fail");
                return false;
            }
            else
            {
                if (tempValue == indata)
                    return true;
            }
            string sql = "update " + Param.CenterIP_DataBase_Table + " set " + address + "='" + indata + "'";

            MySqlConnection objConnection = new MySqlConnection(Param.Center_DB_ConnStr);
            MySqlCommand objCommand = new MySqlCommand(sql, objConnection);
            try
            {
                objConnection.Open();
                SubFunction.updateMessage(this.lstStatusCommand, "connect database success");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), " connect database success");
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "connect database error," + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "connect database error," + ex.Message);
                return false;
            }

            try
            {
                int i = objCommand.ExecuteNonQuery();
                if (i > 0)
                {
                    SubFunction.updateMessage(lstStatusCommand, "write database " + address + "=" + indata);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "write database " + address + "=" + indata);

                    SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DataBase_" + address + "=" + indata);
                }
                else
                {
                    SubFunction.updateMessage(lstStatusCommand, "write but impact " + i.ToString() + "row");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "write but impact " + i.ToString() + "row");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(this.lstStatusCommand, "read data error," + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "read data error," + ex.Message);
                objConnection.Close();
                return false;
            }

            return true;
        }

        #endregion

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
                    SubFunction.updateMessage(lstStatusCommand, "Open SerialPort=" + portname + " success.");//Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Open SerialPort=" + portname + " success.\r\n");//Message:" + e.Message + "\r\n");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Can't open SerialPort=" + portname + ",Message:" + e.Message);
                    SubFunction.updateMessage(lstStatusCommand, "Can't open SerialPort=" + portname + ",Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Can't open SerialPort=" + portname + ",Message:" + e.Message + "\r\n");
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
                }
                catch (Exception e)
                {
                    //MessageBox.Show("Can't close SerialPort=" + sp.PortName.ToString() + ",Message:" + e.Message);
                    SubFunction.updateMessage(lstStatusCommand, "Can't close SerialPort=" + sp.PortName.ToString() + ",Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Can't close SerialPort=" + sp.PortName.ToString() + ",Message:" + e.Message + "\r\n");
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
            }
            catch (Exception e)
            {
                SubFunction.updateMessage(lstStatusCommand, "Send " + spport.PortName + " " + strdata + "fail");
                SubFunction.updateMessage(lstStatusCommand, e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Send " + spport.PortName + " " + strdata + "fail," + e.Message);
            }
        }

        #endregion

        #region 动态选择串口（Dynamic detect serial port）

        // usb消息定义
        public const int WM_DEVICE_CHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICE_REMOVE_COMPLETE = 0x8004;
        public const UInt32 DBT_DEVTYP_PORT = 0x00000003;

        [StructLayout(LayoutKind.Sequential)]
        struct DEV_BROADCAST_HDR
        {
            public UInt32 dbch_size;
            public UInt32 dbch_devicetype;
            public UInt32 dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct DEV_BROADCAST_PORT_Fixed
        {
            public uint dbcp_size;
            public uint dbcp_devicetype;
            public uint dbcp_reserved;
            // Variable?length field dbcp_name is declared here in the C header file.
        }

        ///<summary>
        /// 检测USB串口的拔插
        ///</summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICE_CHANGE)        // 捕获USB设备的拔出消息WM_DEVICECHANGE
            {

                string portName = Marshal.PtrToStringUni((IntPtr)(m.LParam.ToInt32() + Marshal.SizeOf(typeof(DEV_BROADCAST_PORT_Fixed))));
                switch (m.WParam.ToInt32())
                {

                    case DBT_DEVICE_REMOVE_COMPLETE:    // USB拔出 
                        DEV_BROADCAST_HDR dbhdr0 = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr0.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                            try
                            {
                                // comboPortName.Items.Remove(portName);

                                if (btnStart.Enabled)         
                                {
                                    if (Param.Scanner_Use)
                                    {
                                        getSerialPort(comboScannerPort);
                                    }
                                    if (Param.DUT_A_Use)
                                    {
                                        getSerialPort(comboDUTPort_A);
                                    }
                                    if (Param.DUT_B_Use)
                                    {
                                        getSerialPort(comboDUTPort_B);
                                    }
                                    if (Param.PLC_Use)
                                    {
                                        getSerialPort(comboFICTPort);
                                    }
                                    //getSerialPort(comboScannerPort);
                                    //getSerialPort(comboDUTPort_A);
                                    //getSerialPort(comboDUTPort_B);
                                    //getSerialPort(comboFICTPort);
                                }
                                else
                                {
                                    //sp = new System.Media.SoundPlayer(global::SpotTestTester.Properties.Resources.ng);
                                    //sp.Play();

                                    //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口 '" + portName + "' 已被移除");
                                    //SubFunction.updateMessage(lstStatusCommand, "串口 '" + portName + "' 已被移除.");

                                    if (Param.Scanner == portName)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到《条码枪》串口《" + portName + "》被移除...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到《条码枪》串口《" + portName + "》被移除...");
                                        comboScannerPort.Text = "*****";
                                        closeSerialPort(spScanner);
                                    }
                                    else if (portName == Param.DUT_A)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到《DUT_A》串口《" + portName + "》被移除...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到《DUT_A》串口《" + portName + "》被移除...");
                                        comboDUTPort_A.Text = "*****";
                                        closeSerialPort(spDUT_A);
                                    }
                                    else if (portName == Param.DUT_B)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到《DUT_B》串口《" + portName + "》被移除...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到《DUT_B》串口《" + portName + "》被移除...");
                                        comboDUTPort_B.Text = "*****";
                                        closeSerialPort(spDUT_B);
                                    }
                                    else if (portName == Param.PLC)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到《PLC》串口《" + portName + "》被移除...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到《PLC》串口《" + portName + "》被移除...");
                                        comboFICTPort.Text = "*****";
                                        closePLC();
                                    }

                                    //SubFunction.updateMessage(lstStatusCommand, "偵测到串口丟失，請重新设置后點擊开始，若无法啟动，點擊Restart再點擊Start。");
                                    //closePort(spPLC);
                                    //closePort(spSN);
                                    //pressStopButton();
                                    //SubFunction.updateMessage(lstStatusCommand, "Port '" + portName + "' leaved.");
                                }


                            }
                            catch (Exception ex)
                            {
                                //sp = new System.Media.SoundPlayer(global::SpotTestTester.Properties.Resources.ng);
                                //sp.Play();
                                SubFunction.updateMessage(lstStatusCommand, "已侦测到串口 '" + portName + "' 被移除...");
                                SubFunction.updateMessage(lstStatusCommand, "Error:"+ ex.Message);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到串口 '" + portName + "' 被移除...");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Error:" + ex.Message);
                            }
                            Console.WriteLine("已侦测到串口 '" + portName + "' 被移除...");
                        }

                        break;
                    case DBT_DEVICEARRIVAL:             // USB插入获取对应串口名称
                        DEV_BROADCAST_HDR dbhdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                            if (Param.Scanner_Use)
                                getSerialPort(comboScannerPort);
                            else
                                IniFile.IniWriteValue("ComPort_Set", "Scanner", "NULL", @Param.IniFilePath);

                            if (Param.DUT_A_Use)
                                getSerialPort(comboDUTPort_A);
                            else
                                IniFile.IniWriteValue("ComPort_Set", "DUT_A", "NULL", @Param.IniFilePath);

                            if (Param.DUT_B_Use)
                                getSerialPort(comboDUTPort_B);
                            else
                                IniFile.IniWriteValue("ComPort_Set", "DUT_B", "NULL", @Param.IniFilePath);
                            if (Param.PLC_Use)
                                getSerialPort(comboFICTPort);
                            else
                                IniFile.IniWriteValue("ComPort_Set", "PLC", "NULL", @Param.IniFilePath);
                            //getSerialPort(comboScannerPort);
                            //getSerialPort(comboDUTPort_A);
                            //getSerialPort(comboDUTPort_B);
                            //getSerialPort(comboFICTPort);

                            //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到串口 '" + portName + "' 插入电脑");
                           // SubFunction.updateMessage(lstStatusCommand, "已侦测到串口 '" + portName + "' 插入电脑.");
                            Console.WriteLine("已侦测到串口 '" + portName + "' 插入电脑.");



                            if (!btnStart.Enabled)
                            {
                                //条码枪
                                if (Param.Scanner_Use)
                                {
                                    if (Param.Scanner == portName)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<条码枪>串口 <" + portName + "> 插入电脑...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到<条码枪>串口 <" + portName + "> 插入电脑...");
                                       // comboScannerPort.Text = Param.Scanner;
                                        if (!string.IsNullOrEmpty(Param.Scanner))
                                        {
                                            if (!openSerialPort(spScanner, Param.Scanner))
                                                return;
                                        }
                                        comboScannerPort.Text = Param.Scanner;

                                    }
                                }

                                if (Param.DUT_A_Use)
                                {
                                    if (portName == Param.DUT_A)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<DUT_A>串口 <" + portName + "> 插入电脑...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到<DUT_A>串口 <" + portName + "> 插入电脑...");
                                        //comboDUTPort_A.Text = Param.DUT_A;
                                        if (!string.IsNullOrEmpty(Param.DUT_A))
                                        {
                                            if (!openSerialPort(spDUT_A, Param.DUT_A))
                                                return;
                                        }
                                        comboDUTPort_A.Text = Param.DUT_A;
                                    }
                                }

                                if (Param.DUT_B_Use)
                                {
                                    if (portName == Param.DUT_B)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<DUT_B>串口 <" + portName + "> 插入电脑...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到<DUT_B>串口 <" + portName + "> 插入电脑...");
                                        //comboDUTPort_B.Text = Param.DUT_B;
                                        if (!string.IsNullOrEmpty(Param.DUT_B))
                                        {
                                            if (!openSerialPort(spDUT_B, Param.DUT_B))
                                                return;
                                        }
                                        comboDUTPort_B.Text = Param.DUT_B;
                                    }

                                }

                                if (Param.PLC_Use)
                                {
                                    if (portName == Param.PLC)
                                    {
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<PLC>串口 <" + portName + "> 插入电脑...");
                                        SubFunction.updateMessage(lstStatusCommand, "已侦测到<PLC>串口 <" + portName + "> 插入电脑...");
                                        comboFICTPort.Text = Param.PLC;
                                        if (!string.IsNullOrEmpty(Param.PLC))
                                        {
                                            if (!openPLC(Param.PLC))
                                                return;
                                        }
                                        comboFICTPort.Text = Param.PLC;
                                    }
                                }

                                ReviseSerialPort();//校正串口编号

                            }



/*













                            if (Param.Scanner == portName)
                            {
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<条码枪>串口 <" + portName + "> 插入电脑...");
                                SubFunction.updateMessage(lstStatusCommand, "已侦测到<条码枪>串口 <" + portName + "> 插入电脑...");
                                comboScannerPort.Text = Param.Scanner;
                                if (!btnStart.Enabled)
                                {
                                    if (!string.IsNullOrEmpty(Param.Scanner))
                                    {
                                        if (!openSerialPort(spScanner, Param.Scanner))
                                            return;
                                    }
                                   
                                }
                            }
                            else if (portName == Param.DUT_A)
                            {
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<DUT_A>串口 <" + portName + "> 插入电脑...");
                                SubFunction.updateMessage(lstStatusCommand, "已侦测到<DUT_A>串口 <" + portName + "> 插入电脑...");
                                comboDUTPort_A.Text = Param.DUT_A;
                                if (!btnStart.Enabled)
                                {
                                    if (!string.IsNullOrEmpty(Param.DUT_A))
                                    {
                                        if (!openSerialPort(spDUT_A, Param.DUT_A))
                                            return;
                                    }

                                }
                            }
                            else if (portName == Param.DUT_B)
                            {
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<DUT_B>串口 <" + portName + "> 插入电脑...");
                                SubFunction.updateMessage(lstStatusCommand, "已侦测到<DUT_B>串口 <" + portName + "> 插入电脑...");
                                comboDUTPort_B.Text = Param.DUT_B;
                                if (!btnStart.Enabled)
                                {
                                    if (!string.IsNullOrEmpty(Param.DUT_B))
                                    {
                                        if (!openSerialPort(spDUT_B, Param.DUT_B))
                                            return;
                                    }
                                }

                            }
                            else if (portName == Param.PLC)
                            {
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已侦测到<PLC>串口 <" + portName + "> 插入电脑...");
                                SubFunction.updateMessage(lstStatusCommand, "已侦测到<PLC>串口 <" + portName + "> 插入电脑...");
                                comboFICTPort.Text = Param.PLC;
                                if (!btnStart.Enabled)
                                {
                                    if (!string.IsNullOrEmpty(Param.PLC))
                                    {
                                        if (!openPLC(Param.PLC))
                                            return;
                                    }
                                }
                            }


                            ReviseSerialPort();//校正串口编号
 * */
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }



        /// <summary>
        /// 校正串口号
        /// </summary>
        private void ReviseSerialPort()
        {
            if (Param.Scanner_Use)
            {
                if (comboScannerPort.Text != Param.Scanner)
                {
                    comboScannerPort.Text = Param.Scanner;
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已检测<条码枪>串口编号发生改变，已校正...");
                    SubFunction.updateMessage(lstStatusCommand, "已检测<条码枪>串口编号发生改变，已校正...");
                }
            }

            if (Param.DUT_A_Use)
            {
                if (comboDUTPort_A.Text != Param.DUT_A)
                {
                    comboDUTPort_A.Text = Param.DUT_A;
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已检测<DUT_A>串口编号发生改变，已校正...");
                    SubFunction.updateMessage(lstStatusCommand, "已检测<DUT_A>串口编号发生改变，已校正...");
                }
            }

            if (Param.DUT_B_Use)
            {
                if (comboDUTPort_B.Text != Param.DUT_B)
                {
                    comboDUTPort_B.Text = Param.DUT_B;
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已检测<DUT_B>串口编号发生改变，已校正...");
                    SubFunction.updateMessage(lstStatusCommand, "已检测<DUT_B>串口编号发生改变，已校正...");
                }
            }

            if (Param.PLC_Use)
            {
                if (comboFICTPort.Text != Param.PLC)
                {
                    comboFICTPort.Text = Param.PLC;
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已检测<PLC>串口编号发生改变，已校正...");
                    SubFunction.updateMessage(lstStatusCommand, "已检测<PLC>串口编号发生改变，已校正...");
                }
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
                SubFunction.updateMessage(lstStatusCommand, "第" + i + "次连接PLC");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次连接PLC");
                Application.DoEvents();
                try
                {
                    iReturnCode = axActPLC.Open();
                }
                catch (Exception ex)
                {
                    SubFunction.updateMessage(lstStatusCommand, "第" + i + "次连接PLC NG,Message:" + ex.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次连接PLC NG,Message:" + ex.Message);
                }
                sw.Stop();
                ts = sw.Elapsed;
                if (iReturnCode == 0)
                {
                    SubFunction.updateMessage(lstStatusCommand, "第" + i + "次连接Robot PLC,Used time(s):" + ts.Seconds);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次连接PLC OK,Used time(s):" + ts.Seconds);
                    return true;
                }
            }
            // sw.Stop();
            SubFunction.updateMessage(lstStatusCommand, "第" + iMaxRetry + "次连接PLC NG,,已达到最大retry数，Errorcode=" + iReturnCode + ",Used time(s):" + ts.Seconds);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + iMaxRetry + "次连接PLC NG,已达到最大retry数，Errorcode=" + iReturnCode + ",Used time(s):" + ts.Seconds);
            return false;
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
                SubFunction.updateMessage(lstStatusCommand, "clsoe PLC");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "close PLC");

            }
            catch (Exception e)
            {
                SubFunction.updateMessage(lstStatusCommand, "clsoe PLC error," + e.Message);
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
                SubFunction.updateMessage(lstStatusCommand, "Read " + readaddress + " Fail.");
                SubFunction.updateMessage(lstStatusCommand, e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Read " + readaddress + " Fail," + e.Message);
                return false;
            }

            if (iReturn == 0)
            {
                return true;
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "Read " + readaddress + " Fail,errorcode = " + iReturn);
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
                SubFunction.updateMessage(lstStatusCommand, "Write " + devicename + " Fail");
                SubFunction.updateMessage(lstStatusCommand, e.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Write " + devicename + " Fail," + e.Message);
                return false;
            }

            if (iReturn != 0)
            {
                SubFunction.updateMessage(lstStatusCommand, "Write " + devicename + " Fail,errorcode = " + iReturn);
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
                SubFunction.updateMessage(lstStatusCommand, "PC->PLC:" + commandStr);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->PLC:" + commandStr);
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->PLC:" + commandStr);
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "PC->PLC:" + commandStr + " Fail");
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
                SubFunction.updateMessage(lstStatusCommand, "FICT->PC:" + outdata);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata);
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata);
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "FICT->PC:" + outdata + " Fail");
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
                    SubFunction.updateMessage(lstStatusCommand, "FICT->PC:" + outdata);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata);
                    SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata);
                }
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "FICT->PC:" + outdata + " Fail");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + outdata + " Fail");
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + outdata + " Fail");
            }

        }



        #endregion

        #region web_service操作

        /// <summary>
        /// 检测WebService的可連通性,可連通返回true，不可連通，返回false
        /// </summary>
        /// <param name="website">WebService的地址</param>
        /// <returns>可連通返回true，不可連通返回false</returns>
        private bool checkWebService(string website)
        {
            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            SubFunction.updateMessage(lstStatusCommand, "Check Web Service");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Check Web Service");
            ws.Url = Param.Web_Site;
            try
            {
                ws.Discover();
            }
            catch (Exception e)
            {
                sw.Stop();
                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "Check Web Service NG,Used time(ms):" + ts.Milliseconds);
                SubFunction.updateMessage(lstStatusCommand, e.Message);

                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Check Web Service NG,Used time(ms):" + ts.Milliseconds + "\r\n" + "Message:".PadLeft(24) + e.Message);
                // MessageBox.Show("Can't Connect to Web Service,\r\nMessage:" + e.Message);
                return false;
            }
            sw.Stop();
            ts = sw.Elapsed;
            SubFunction.updateMessage(lstStatusCommand, "Check Web Service OK,Used time(ms):" + ts.Milliseconds);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Check Web Service OK,Used time(ms):" + ts.Milliseconds);
            return true;
        }

        ///// <summary>
        ///// 从webservice上获取双连板的条码
        ///// </summary>
        ///// <param name="website">web service 地址</param>
        ///// <param name="queryusn">查詢時使用的条码</param>
        ///// <param name="barcodetype"></param>
        //private void getDynamicData(string website, string queryusn, string barcodetype)
        //{
        //    if (!checkWebService(website))
        //        return;
        //    /////////////////////////////
        //    Stopwatch sw = new Stopwatch();
        //    TimeSpan ts = new TimeSpan();
        //    sw.Start();
        //    string snlist = string.Empty;
        //    try
        //    {
        //        DataSet ds = ws.GetDynamicData("3", "USN", queryusn);
        //        for (int i = 0; i <= ds.Tables.Count - 1; i++)
        //        {
        //            //for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
        //            //{
        //                for (int k = 0; k <= ds.Tables[i].Columns.Count - 1; k++)
        //                {
        //                    if (ds.Tables[0].Columns[k].ToString().ToUpper () == "SNLIST")
        //                    {
        //                        snlist = ds.Tables[0].Rows[0][k].ToString();
        //                        splitBarcode(snlist, queryusn, Param.BarcodeType);         
        //                        sw.Stop();
        //                        ts = sw.Elapsed;
        //                        SubFunction.updateMessage(lstStatusCommand, "Get Dynamic OK,Used time(ms):" + ts.Milliseconds);
        //                        SubFunction.updateMessage(lstStatusCommand, "Bar_A=" + Param.bar_A);
        //                        SubFunction.updateMessage(lstStatusCommand, "Bar_B=" + Param.bar_B);
        //                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Get Dynamic OK,Used time(ms):" + ts.Milliseconds + "\r\n" + "Bar_A:".PadLeft(21) + Param.bar_A + "\r\n" + "Bar_B:".PadLeft(21) + Param.bar_B);
        //                    }
        //                }
        //            //}
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sw.Stop();
        //        ts = sw.Elapsed;
        //        SubFunction.updateMessage(lstStatusCommand, "Get Dynamic Error,Used Time(ms):" + ts.Milliseconds);
        //        SubFunction.updateMessage(lstStatusCommand, ex.Message);
        //        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Get Dynamic Error,Used Time(ms):" + ts.Milliseconds + "\r\n" + "Message:".PadLeft(21) + ex.Message);
        //    }

        //}

        /// <summary>
        /// 从webservice上获取双连板的条码
        /// </summary>
        /// <param name="website">web service 地址</param>
        /// <param name="queryusn">查詢時使用的条码</param>
        /// <param name="barcodetype"></param>
        private void getDynamicData(string website, string queryusn, string barcodetype)
        {
            if (!checkWebService(website))
                return;
            /////////////////////////////
            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            string snlist = string.Empty;
            try
            {
                DataSet ds = ws.GetDynamicData("3", "USN", queryusn);
                //DataSet ds= ws.GetDynamicData (

                // MessageBox.Show (ds.Tables[0].Columns[2].ToString());
                Console.WriteLine(ds.Tables[0].Rows.Count.ToString());
                Console.WriteLine(ds.Tables[0].Columns.Count.ToString());
                for (int i = 0; i <= ds.Tables.Count - 1; i++)
                {
                    //for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                    //{
                    for (int k = 0; k <= ds.Tables[i].Columns.Count - 1; k++)
                    {
                        if (ds.Tables[i].Columns[k].ToString() == "SNLIST")
                        {
                            snlist = ds.Tables[i].Rows[0][k].ToString();

                            splitBarcode(snlist, queryusn, barcodetype);
                            //if (barcodetype == barcode_type.A.ToString())
                            //{
                            //    bar_A = queryusn;
                            //    bar_B = snlist.Replace(bar_A, string.Empty);
                            //    if (bar_A.Contains(","))
                            //        bar_A.Replace(",", string.Empty);
                            //}
                            //if (barcodetype == barcode_type.B.ToString())
                            //{
                            //    bar_B = queryusn;
                            //    bar_A = snlist.Replace (bar_B ,string.Empty);
                            //    if (bar_B.Contains(","))
                            //        bar_B.Replace(",", string.Empty);
                            //}
                            sw.Stop();
                            ts = sw.Elapsed;
                            SubFunction.updateMessage(lstStatusCommand, "Get Dynamic OK,Used time(ms):" + ts.Milliseconds);
                            SubFunction.updateMessage(lstStatusCommand, "Bar_A=" + Param.bar_A);
                            SubFunction.updateMessage(lstStatusCommand, "Bar_B=" + Param.bar_B);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Get Dynamic OK,Used time(ms):" + ts.Milliseconds + "\r\n" + "Bar_A:".PadLeft(21) + Param.bar_A + "\r\n" + "Bar_B:".PadLeft(21) + Param.bar_B);

                        }
                        //  }
                    }
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "Get Dynamic Error,Used Time(ms):" + ts.Milliseconds);
                SubFunction.updateMessage(lstStatusCommand, ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Get Dynamic Error,Used Time(ms):" + ts.Milliseconds + "\r\n" + "Message:".PadLeft(21) + ex.Message);
            }

        }
        

        /// <summary>
        /// 从条码列中将条码分離出來
        /// </summary>
        /// <param name="usnlist">条码列</param>
        /// <param name="queryusn">查詢的条码</param>
        /// <param name="barcodetype">当前条码類型</param>
        private void splitBarcode(string usnlist, string queryusn, string barcodetype)
        {
            if (barcodetype == Param.barcodeType.A.ToString())
            {
                Param.bar_A = queryusn;
                Param.bar_B = usnlist.Replace(Param.bar_A, string.Empty);
                //  MessageBox.Show(bar_B.EndsWith(",").ToString());
                //  MessageBox.Show(bar_B.StartsWith(",").ToString());
                if ((Param.bar_B.EndsWith(",") || (Param.bar_B.StartsWith(","))))
                    Param.bar_B = Param.bar_B.Replace(",", string.Empty);
            }

            if (barcodetype == Param.barcodeType.B.ToString())
            {
                Param.bar_B = queryusn;
                Param.bar_A = usnlist.Replace(Param.bar_B, string.Empty);
                if ((Param.bar_A.EndsWith(",") || (Param.bar_A.StartsWith(","))))
                    Param.bar_A = Param.bar_A.Replace(",", string.Empty);
            }
        }



        /// <summary>
        /// 獲取系統內雙連板的A條碼和B條碼
        /// </summary>
        /// <param name="website">webservice site </param>
        /// <param name="queryusn">query sn </param>
        private void getDoubleMBSN(string website, string queryusn)
        {
           

        }



        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <param name="queryusn"></param>
        /// <param name="modelname"></param>
        /// <param name="mbpn"></param>
        private void getModelNameInfo( string queryusn, out string modelname, out string mbpn)
        {
            modelname = string.Empty;
            mbpn = string.Empty;
           

            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            SubFunction.updateMessage(lstStatusCommand, "SFCS:" + queryusn  + ",get ModelName & ModelFamily.");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SFCS:" + queryusn + ",get ModelName & ModelFamily.");
            PcbWeb.clsRequestData reqData = new PcbWeb.clsRequestData();
            reqData = ws.GetUUTData(queryusn, "TD", reqData, 1);
            
            sw.Stop();
            ts = sw.Elapsed;
            if (reqData != null )
            {
                modelname = reqData.Model;
                mbpn = reqData.CustomerUPN;
                SubFunction.updateMessage(lstStatusCommand, queryusn + "Get ModelName & ModelFamily ,Used time(ms):" + ts.Milliseconds);
                SubFunction.updateMessage(lstStatusCommand, queryusn + " Model:" + reqData.Model);
                SubFunction.updateMessage(lstStatusCommand, queryusn + " MBPN:" + reqData.CustomerUPN);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), queryusn + "Get ModelName & ModelFamily ,Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), queryusn + " Model:" + reqData.Model);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), queryusn + " MBPN:" + reqData.CustomerUPN);

                                
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand,"Get ModelName & ModelFamily Fail," + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Get ModelName & ModelFamily Fail," + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Get ModelName & ModelFamily Fail," + "Used time(ms):" + ts.Milliseconds);
                
            }


        }
        






        /// <summary>
        /// 检查USN站別是否在当前站別,在为true，不在为false
        /// </summary>
        /// <param name="usn">条码</param>
        /// <param name="stage">站別</param>
        /// <returns>在当前站別为true，不在当前站別为false</returns>
        private bool checkStage(string usn, string stage)
        {

            //  checkWebService(web_Site);

            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            SubFunction.updateMessage(lstStatusCommand, "SFCS:" + usn + ",Stage:" + stage);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SFCS:" + usn + ",Stage:" + stage);
            string result = ws.CheckRoute(usn, stage);
            sw.Stop();
            ts = sw.Elapsed;
            if (result.ToUpper() == "OK")
            {
                SubFunction.updateMessage(lstStatusCommand, result + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "usn:" + usn + "->" + stage);

                return true;
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, result + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), result + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), result + "Used time(ms):" + ts.Milliseconds);
                return false;
            }
        }

        /// <summary>
        /// 从webservice上获取MO和MAC地址
        /// </summary>
        /// <param name="website">website</param>
        /// <param name="usn"></param>
        /// <param name="stage"></param>
        /// <param name="mo"></param>
        /// <param name="mac"></param>
        private void getMOMAC(string website, string usn, string stage, ref  string mo, ref  string mac)
        {
            if (!checkWebService(website))
                return;

            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            string result = string.Empty;
            PcbWeb.clsInfoNameValue[] info = new PcbWeb.clsInfoNameValue[2];
            result = ws.GetKeyInfoFromView(usn, stage, "7", ref info);

            if (result != "OK")
            {
                sw.Stop();
                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "get MO_MAC fail,time(ms)" + ts.Milliseconds + "," + result);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "USN:" + usn + " get MO_MAC fail,time(ms)" + ts.Milliseconds + "," + result);
                return;
            }

            for (int i = 0; i <= info.Length - 1; i++)
            {
                if (info[i].InfoName.ToString() == "MO")
                {
                    mo = info[i].InfoValue.ToString();
                    SubFunction.updateMessage(lstStatusCommand, "USN:" + usn + ",MO:" + mo);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "USN:" + usn + ",MO:" + mo);
                }
                if (info[i].InfoName.ToString() == "MAC")
                {
                    mac = info[i].InfoValue.ToString();
                    SubFunction.updateMessage(lstStatusCommand, "USN:" + usn + ",MAC:" + mac);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "USN:" + usn + ",MAC:" + mac);
                }
            }
            sw.Stop();
            ts = sw.Elapsed;
            SubFunction.updateMessage(lstStatusCommand, "USN:" + usn + ",get mo_mac used time(ms):" + ts.Milliseconds);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "USN:" + usn + ",get mo_mac used time(ms):" + ts.Milliseconds);
        }

        /// <summary>
        ///  将fixtureid和usn綁定，上拋至sfcs
        /// </summary>
        /// <param name="website">web service地址</param>
        /// <param name="usn">条码</param>
        /// <param name="stage">站別</param>
        /// <param name="fixtureid">治具編號</param>
        private void uploadFixtureID(string website, string usn, string stage, string fixtureid)
        {
            if (!checkWebService(website))
                return;

            string result = string.Empty;
            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            result = ws.UploadFixtureID(usn, stage, fixtureid);
            if (result == "OK")
            {
                sw.Stop();
                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "upload FixtureID:" + fixtureid + " OK,time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "upload FixtureID:" + fixtureid + " OK,time(ms):" + ts.Milliseconds);
            }
            else
            {
                sw.Stop();
                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "upload FixtureID:" + fixtureid + " Fail,time(ms):" + ts.Milliseconds + "," + result);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "upload FixtureID:" + fixtureid + " Fail,time(ms):" + ts.Milliseconds + "," + result);
            }
        }

        /// <summary>
        /// 上拋信息到SFCS
        /// </summary>
        /// <param name="website">web serevice地址</param>
        /// <param name="usn">条码</param>
        /// <param name="bresult">测试結果，PASS=true，NG=false</param>
        /// <param name="ngitem">测试項目</param>
        /// <param name="bretest">MB重测的標註,重测過=true，沒重测過=false</param>
        private void updateResult2SFCS(string Bar_Type,string website, string usn, bool bresult, string ngitem, bool bretest)//,string testitem)
        {
            string[] trndata = new string[1]; //上拋SFCS的附件信息
            if (!checkWebService(website))
                return;
            string result = string.Empty; //上拋的結果
            string testresult = string.Empty;//测试的結果
            bool bsfcsresult = false;


            if (bresult)
            {
                trndata[0] = "PASS";
                testresult = Param.testResult.PASS.ToString();
                ngitem = "PASS";
            }
            else
            {
                trndata[0] = getErrorCode(ngitem);
                testresult = Param.testResult.FAIL.ToString();
            }
            result = ws.Complete(usn, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, bresult, trndata);
            if (result == "OK")
                bsfcsresult = true;
            else
                bsfcsresult = false;

            //save log
            //SubFunction.saveLog(Bar_Type + usn, testresult, bretest, bsfcsresult, ngitem);
            SubFunction.SaveTestLog(Bar_Type + usn, testresult, bretest, bsfcsresult, ngitem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
            TestLog_for_Record_DataBase(Bar_Type + usn, testresult, bretest, bsfcsresult, ngitem);
        }

        /// <summary>
        /// 根据测试項目获取对应的ErrorCode
        /// </summary>
        /// <param name="ngitem">测试項目</param>
        /// <returns></returns>
        private string getErrorCode(string ngitem)
        {
            string errorcode = string.Empty;

            switch (ngitem)
            {
                case "PASS":
                    errorcode = "0000";
                    break;
                case "MEFW":
                    errorcode = "FW06";
                    break;
                case "FFS":
                    errorcode = "HF18";
                    break;
                case "WLAN":
                    errorcode = "LP12";
                    break;
                case "BT":
                    errorcode = "FI06";
                    break;
                case "EQU":
                    errorcode = "CU02";
                    break;
                case "TPM":
                    errorcode = "TP00";
                    break;
                case "USB2":
                    errorcode = "USN20";
                    break;
                case "USN30":
                    errorcode = "USB30";
                    break;
                case "FAN":
                    errorcode = "FF00";
                    break;
                case "HDD":
                    errorcode = "HF00";
                    break;
                case "SPEAK":
                    errorcode = "SA00";
                    break;
                case "SD":
                    errorcode = "FC02"; //SD功能不良
                    break;
                case "MS":
                    errorcode = "FC03"; //MS Card功能不良
                    break;
                case "CPUCHK":
                    errorcode = "SF02"; //CPU型式錯誤
                    break;
                case "CPU":
                    errorcode = "CU01"; //CPU功能不良
                    break;
                case "HDDIRQ":
                    errorcode = "HF07"; //硬碟机中断测试不良
                    break;
                case "FLANID":
                    errorcode = "LP02"; //无法燒錄LANID
                    break;
                case "MEM":
                    errorcode = "CM00"; //快取記憶體功能不良
                    break;
                case "STNG":   //start ng
                    errorcode = "NO00"; //系統无法开机
                    break;
                case "TMOU": //time out
                    errorcode = "SH00"; //档机
                    break;
                default:
                    errorcode = ngitem;
                    break;
            }
            return errorcode;
        }

        #endregion

        #region 延時子程式

        /// <summary>
        /// 延時子程序
        /// </summary>
        /// <param name="interval">延時的時間，单位毫秒</param>
        private void Delay(double interval)
        {
            DateTime time = DateTime.Now;
            double span = interval * 10000;
            while (DateTime.Now.Ticks - time.Ticks < span)
            {
                Application.DoEvents();
            }

        }

        #endregion

        #region 电压检测

        private void timerDetect_V_A_Tick(object sender, EventArgs e)
        {
            txtMB_A_V.Text = "0";
            timerDetect_V_A.Stop();
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

            txtMB_A_V.Text = mbV.ToString();
            i_Detect_A_Count += 1;

            if (Param.UseCommand)
            {
                timerDetect_V_A.Start();
                return;
            }

            //开始偵测电压,並且一直偵测

            if (i_Detect_A_Count == Param.DetectDelay) //第一次检测
            {
                if (mbV >= 3.0)
                {
                    SubFunction.updateMessage(lstStatusCommand, "检测MB_A开机电压成功，实际电压:" + mbV + "V"); //ay
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测MB_A开机电压成功，实际电压:" + mbV + "V"); //ay
                }
                else
                {
                    SubFunction.updateMessage(lstStatusCommand, "检测MB_A开机电压失败,实际电压:" + mbV + "V");//an
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测MB_A开机电压失败,实际电压:" + mbV + "V");//an

                    SubFunction.updateMessage(lstStatusCommand, "当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount);

                    //判断是否需要重复上电
                     if (Param.MB_A_Re  && !_b_MB_A_Re  ) //MB_A需要重复上电且还未重复上电
                    {
                        SubFunction.updateMessage(lstStatusCommand, "MB_A需要重复上电，断电,再次上电");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A需要重复上电，断电,再次上电");
                        Delay(1000);
                        sendPLC("aa");
                        Delay(2000);
                        sendPLC("AA");
                        i_Detect_A_Count = 0; //重置检测断电次数
                        txtMB_A_V.Text = "0";
                        timerDetect_V_A.Start ();
                        return;//退出循环,继续下一次检测
                    }
                   




                    //判断是否需要重测
                    if (iCurrent_TestCount >= Param.MaxRetestCount)
                    {
                        //不重测
                        SubFunction.updateMessage(lstStatusCommand, "MB_A未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT不重复测试");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT不重复测试");
                        //SubFunction.saveLog(Param.bar_A, "FAIL", false, false, "STNG");

                        if (Param.Test_Log)
                        {
                            ngDUT_A_TestItem = "STNG";
                            //=============upload sfcs=============
                            if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                            {
                                ws.UploadFixtureID(Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                TrnDatas[0] = getErrorCode(ngDUT_A_TestItem);
                                string result = ws.Complete(Param.bar_A, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                                if (result == "OK")
                                {
                                    //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, true, ngDUT_A_TestItem);
                                    SubFunction.SaveTestLog("04---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, true, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("04---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, true, ngDUT_A_TestItem);

                                }
                                else
                                {
                                    //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, false, ngDUT_A_TestItem);
                                    SubFunction.SaveTestLog("05---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, false, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("05---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, false, ngDUT_A_TestItem);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + Param.bar_A + " Fail," + result);

                                }
                            }
                            else
                            {
                                //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", false, false, "STNG");
                                SubFunction.SaveTestLog("06---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, "STNG", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                TestLog_for_Record_DataBase("06---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, "STNG");
                            }
                            Param.Test_Log = false;
                        }

                        

                        lblTestResult_A.ForeColor = Color.Red;
                        lblTestResult_A.Text = "FAIL";

                        i_Detect_A_Count = 0;
                        timerDetectTimeOut.Stop();
                        ShutDownCount_A = 0;

                        //需要强制断开MB_B的电
                        //1,断
                        Console.WriteLine("双连板MB_A需要重测，MB_B需要强制开机");
                        SubFunction.updateMessage(lstStatusCommand, "双连板MB_A需要重测，MB_B需要强制开机");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板MB_A需要重测，MB_B需要强制开机");
                        sendPLC("S2");
                    }
                    else
                    {
                        DUT_A_Retest_Flag = true;
                        DUT_Retest_Flag = true;
                        i_Detect_A_Count = 0;
                        timerDetectTimeOut.Stop();
                        ShutDownCount_A = 0;
                        //要重测
                        SubFunction.updateMessage(lstStatusCommand, "MB_A未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");

                        //需要强制断开MB_B的电
                        //1,断
                        Console.WriteLine("双连板MB_A需要重测，MB_B需要强制开机");
                        SubFunction.updateMessage(lstStatusCommand, "双连板MB_A需要重测，MB_B需要强制开机");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板MB_A需要重测，MB_B需要强制开机");
                        sendPLC("S2");

                    }

                    //
                    //治具退出
                    i_Detect_A_Count = 0; //重置參数
                    //双板子
                    if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                    {
                        //1,断电
                        Console.WriteLine("双连板MB_A断19V");
                        SubFunction.updateMessage(lstStatusCommand, "MB_A断开19V");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A断开19V");
                        sendPLC("aa");
                        _b_A_send_aa = true;
#if DEBUG
                        SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                        SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);

#endif
                        if (_b_A_send_aa && _b_B_send_ab) //都发送了断电命令
                        {
                            _b_A_send_aa = false;

                            //----------可能会重复发送命令-----
                            //2,判断侧插
                            //左右都无
                            if (!Param.LeftInsert && !Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                sendPLC("DO");
                            }
                            //右侧插，先右后左
                            if (Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                sendPLC("OR");
                            }
                            else if (Param.LeftInsert) //右边沒有，判断左边
                            {
                                SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                sendPLC("OL");
                            }
                            timerDetectTimeOut.Stop();
                            return;
                        }

                        //开机断电只检测一次
                        return;

                    }
                }
            }
            //持續检测
            if (i_Detect_A_Count > Param.DetectDelay)
            {
                if (mbV < 3.0)
                {
                    if (Param.ShutDown == 0 | lblTestResult_A.Text == "PASS") //测试完毕断电---may be need complete
                    {
                        SubFunction.updateMessage(lstStatusCommand, "检测到MB_A已正常测试完毕开机");

                        //断开19V
                        //1,断电
                        SubFunction.updateMessage(lstStatusCommand, "MB_A断开19V");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A断开19V");
                        sendPLC("aa");
                        //--be care---may be need complete
                        i_Detect_A_Count = 0;
                        timerDetect_V_A.Stop();
                        _b_A_send_aa = true;
                        i_Detect_A_Count = 0; //重置參数

#if DEBUG
                        SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                        SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);

#endif

                        if (_b_A_send_aa && _b_B_send_ab)
                        {
                            _b_A_send_aa = false;
                            //----------可能会重复发送命令-----
                            //2,判断侧插
                            //左右都无
                            if (!Param.LeftInsert && !Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                sendPLC("DO");
                            }
                            //右侧插，先右后左
                            if (Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                sendPLC("OR");
                            }
                            else //右边沒有，判断左边
                            {
                                SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                sendPLC("OL");
                            }
                            timerDetectTimeOut.Stop();
                            return;
                        }
                        return;

                    }
                    else
                    {
                        //测试過程中断电                        
                        //重测被强制断电
                        if (DUT_Retest_Flag)
                        {
                            SubFunction.updateMessage(lstStatusCommand, "MB_A检测到断电,R/C:MB_B需要重复测试");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A检测到断电,R/C:MB_B需要重复测试");
                            sendPLC("aa");
                            _b_A_send_aa = true;
                            //1,断电
                            SubFunction.updateMessage(lstStatusCommand, "MB_A断开19V");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A断开19V");
#if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
#endif
                            if (_b_A_send_aa && _b_B_send_ab)
                            {
                                _b_B_send_ab = false;
                                //----------可能会重复发送命令-----
                                //2,判断侧插
                                //左右都无
                                if (!Param.LeftInsert && !Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                    sendPLC("DO");
                                }
                                //右侧插，先右后左
                                if (Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                    sendPLC("OR");
                                }
                                else if (Param.LeftInsert)//右边沒有，判断左边
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                    sendPLC("OL");
                                }

                            }
                            return;
                        }
                        else //正常测试中断电
                        {
                            ShutDownCount_A += 1;
                            if (ShutDownCount_A <= Param.ShutDown) //正常开机中
                            {
                                SubFunction.updateMessage(lstStatusCommand, "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                Delay(3000);
                            }
                            else                                //开机失败
                            {
  
                                CheckRetest("MB_A", "STNG", DUT_A_Retest_Flag, Param.bar_A, "");
 

                                //Add by channing Wang 20170112
                                /*
                                SubFunction.updateMessage(lstStatusCommand, "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");

                                // SubFunction.saveLog(Param.bar_A, "FAIL", false, false, "STNG");
                                ngDUT_A_TestItem = "STNG";
                                //=============upload sfcs=============
                                if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                                {
                                    ws.UploadFixtureID(Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                    TrnDatas[0] = getErrorCode(ngDUT_A_TestItem);
                                    string result = ws.Complete(Param.bar_A, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                                    if (result == "OK")
                                    {
                                        //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, true, ngDUT_A_TestItem);
                                        SubFunction.SaveTestLog("07---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, true, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("07---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, true, ngDUT_A_TestItem);
                                    }
                                    else
                                    {
                                        //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, false, ngDUT_A_TestItem);
                                        SubFunction.SaveTestLog("08---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, false, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("08---" + "Bar_A-" + Param.bar_A, "FAIL", DUT_A_Retest_Flag, false, ngDUT_A_TestItem);                      
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + Param.bar_A + " Fail," + result);
                                    }
                                }
                                else
                                {
                                    //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", false, false, "STNG");
                                    SubFunction.SaveTestLog("09---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, "STNG", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("09---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, "STNG"); 
                                }
                                lblTestResult_A.ForeColor = Color.Red;
                                lblTestResult_A.Text = "FAIL";
                                i_Detect_A_Count = 0; //重置參数

                                 * 
                                 * */

                                //断开19V
                                //1,断电
                                SubFunction.updateMessage(lstStatusCommand, "MB_A断开19V");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A断开19V");
                                sendPLC("aa");
                                //--be care---may be need complete
                                i_Detect_A_Count = 0;
                                timerDetect_V_A.Stop();
                                _b_A_send_aa = true;

#if DEBUG
                                SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                                SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);

#endif

                                if (_b_A_send_aa && _b_B_send_ab)
                                {
                                    _b_A_send_aa = false;
                                    //----------可能会重复发送命令-----
                                    //2,判断侧插
                                    //左右都无
                                    if (!Param.LeftInsert && !Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                        sendPLC("DO");
                                    }
                                    //右侧插，先右后左
                                    if (Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                        sendPLC("OR");
                                    }
                                    else if (Param.LeftInsert) //右边沒有，判断左边
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                        sendPLC("OL");
                                    }

                                    timerDetectTimeOut.Stop();
                                    return;
                                }
                                return;
                            }
                        }

                    }
                }
            }

            timerDetect_V_A.Start();

        }

        private void timerDetect_V_B_Tick(object sender, EventArgs e)
        {
            txtMB_B_V.Text = "0";
            timerDetect_V_B.Stop();
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

            txtMB_B_V.Text = mbV.ToString();
            i_Detect_B_Count += 1;


            if (Param.UseCommand)
            {
                timerDetect_V_B.Start();
                return;
            }

            if (i_Detect_B_Count == Param.DetectDelay) //第一次检测
            {
                if (mbV >= 3.0)
                {
                    SubFunction.updateMessage(lstStatusCommand, "检测MB_B开机电压成功，实际电压:" + mbV + "V"); //by
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测MB_B开机电压成功，实际电压:" + mbV + "V"); //by
                }
                else
                {
                    SubFunction.updateMessage(lstStatusCommand, "检测MB_B开机电压失败,实际电压:" + mbV + "V");//bn
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测MB_B开机电压失败,实际电压:" + mbV + "V");//bn

                    SubFunction.updateMessage(lstStatusCommand, "当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount);


                    //判断是否需要重复上电
                    if (Param.MB_B_Re && !_b_MB_B_Re) //MB_A需要重复上电且还未重复上电
                    {
                        SubFunction.updateMessage(lstStatusCommand, "MB_B需要重复上电，断电,再次上电");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B需要重复上电，断电,再次上电");
                        Delay(1000);
                        sendPLC("ab");
                        Delay(2000);
                        sendPLC("AB");
                        i_Detect_B_Count = 0; //重置检测断电次数
                        txtMB_B_V.Text = "0";
                        timerDetect_V_B.Start();
                        return;//退出循环,继续下一次检测
                    }



                    //判断是否需要重测
                    if (iCurrent_TestCount >= Param.MaxRetestCount)
                    {

                        //不重测
                        SubFunction.updateMessage(lstStatusCommand, "MB_B未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT不重复测试");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT不重复测试");
                        //SubFunction.saveLog(Param.bar_A, "FAIL", false, false, "STNG");
                        //SubFunction.saveLog(Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG");

                        if (Param.Test_Log)
                        {
                            ngDUT_B_TestItem = "STNG";
                            //=============upload sfcs=============
                            if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                            {
                                ws.UploadFixtureID(Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                TrnDatas[0] = getErrorCode(ngDUT_B_TestItem);
                                string result = ws.Complete(Param.bar_B, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                                if (result == "OK")
                                {
                                    //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem);
                                    SubFunction.SaveTestLog("10---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("10---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem);
                                }
                                else
                                {
                                    //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem);
                                    SubFunction.SaveTestLog("11---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("11---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + Param.bar_B + " Fail," + result);
                                }
                            }
                            else
                            {
                                //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG"); ////???????? 有疑问 
                                SubFunction.SaveTestLog("12---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                TestLog_for_Record_DataBase("12---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG");
                            }
                            Param.Test_Log = false;
                        }

                       

                        lblTestResult_B.ForeColor = Color.Red;
                        lblTestResult_B.Text = "FAIL";

                        i_Detect_B_Count = 0;
                        timerDetectTimeOut.Stop();
                        ShutDownCount_B = 0;
                        //需要强制断开MB_B的电
                        //1,断
                        Console.WriteLine("双连板MB_B需要重测，MB_A需要强制开机");
                        SubFunction.updateMessage(lstStatusCommand, "双连板MB_B需要重测，MB_A需要强制开机");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板MB_B需要重测，MB_A需要强制开机");
                        sendPLC("S1");
                    }
                    else
                    {
                        DUT_B_Retest_Flag = true;
                        DUT_Retest_Flag = true;
                        i_Detect_B_Count = 0;
                        timerDetectTimeOut.Stop();
                        ShutDownCount_B = 0;
                        //要重测
                        SubFunction.updateMessage(lstStatusCommand, "MB_B未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");

                        //需要强制断开MB_B的电
                        //1,断
                        Console.WriteLine("双连板MB_B需要重测，MB_A需要强制开机");
                        SubFunction.updateMessage(lstStatusCommand, "双连板MB_B需要重测，MB_A需要强制开机");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板MB_B需要重测，MB_A需要强制开机");
                        sendPLC("S1");

                    }
                    //
                    //治具退出
                    i_Detect_B_Count = 0; //重置參数

                    //双板子
                    if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                    {
                        //1,断电
                        SubFunction.updateMessage(lstStatusCommand, "MB_B断开19V");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B断开19V");
                        sendPLC("ab");
                        _b_B_send_ab = true;
                        i_Detect_B_Count = 0; //重置參数


#if DEBUG
                        SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                        SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
#endif
                        if (_b_A_send_aa && _b_B_send_ab)
                        {
                            _b_B_send_ab = false;
                            //----------可能会重复发送命令-----
                            //2,判断侧插
                            //左右都无
                            if (!Param.LeftInsert && !Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                sendPLC("DO");
                            }
                            //右侧插，先右后左
                            if (Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                sendPLC("OR");
                            }
                            else //右边沒有，判断左边
                            {
                                SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                sendPLC("OL");
                            }

                            // timerDetectTimeOut.Stop();
                            return;
                        }

                        //只检测一次，不要繼續检测
                        return;

                    }
                }
            }
            //持續检测
            if (i_Detect_B_Count > Param.DetectDelay)
            {
                if (mbV < 3.0)
                {
                    if (Param.ShutDown == 0 | lblTestResult_B.Text == "PASS") //测试完毕断电---may be need complete
                    {
                        SubFunction.updateMessage(lstStatusCommand, "检测到MB_B已正常测试完毕开机");

                        //1,断电
                        SubFunction.updateMessage(lstStatusCommand, "MB_B断开19V");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B断开19V");
                        sendPLC("ab");
                        _b_B_send_ab = true;
                        i_Detect_B_Count = 0; //重置參数

#if DEBUG
                        SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                        SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
#endif
                        if (_b_A_send_aa && _b_B_send_ab)
                        {
                            _b_B_send_ab = false;
                            //----------可能会重复发送命令-----
                            //2,判断侧插
                            //左右都无
                            if (!Param.LeftInsert && !Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                sendPLC("DO");
                            }
                            //右侧插，先右后左
                            if (Param.RightInsert)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                sendPLC("OR");
                            }
                            else //右边沒有，判断左边
                            {
                                SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                sendPLC("OL");
                            }
                            // timerDetectTimeOut.Stop();
                            return;
                        }
                        return;
                    }
                    else
                    {
                        //测试過程中断电
                        //重测被强制断电
                        if (DUT_Retest_Flag)
                        {
                            SubFunction.updateMessage(lstStatusCommand, "MB_B检测到断电,R/C:MB_A需要重复测试");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B检测到断电,R/C:MB_A需要重复测试");
                            //1,断电
                            SubFunction.updateMessage(lstStatusCommand, "MB_B断开19V");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B断开19V");
                            sendPLC("ab");
                            _b_B_send_ab = true;

#if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
#endif
                            if (_b_A_send_aa && _b_B_send_ab)
                            {
                                _b_B_send_ab = false;
                                //----------可能会重复发送命令-----
                                //2,判断侧插
                                //左右都无
                                if (!Param.LeftInsert && !Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                    sendPLC("DO");
                                }
                                //右侧插，先右后左
                                if (Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                    sendPLC("OR");
                                }
                                else //右边沒有，判断左边
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                    sendPLC("OL");
                                }
                            }
                            return;

                        }
                        else
                        {
                            //测试過程中断电
                            ShutDownCount_B += 1;
                            if (ShutDownCount_B <= Param.ShutDown) //正常开机中
                            {
                                SubFunction.updateMessage(lstStatusCommand, "MB_B检测到第" + ShutDownCount_B + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B检测到第" + ShutDownCount_B + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                Delay(3000);
                            }
                            else                                //非正常开机
                            {

                               CheckRetest("MB_B", "STNG", DUT_B_Retest_Flag, Param.bar_B, "");
 
                                //(string MB_Type, string NG_Type, bool Retest_Flag,string MB_Bar,string Remark

                                //Add by channing Wang 20170112
                                /*
                                SubFunction.updateMessage(lstStatusCommand, "MB_B检测到第" + ShutDownCount_B + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B检测到第" + ShutDownCount_B + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");

                                //SubFunction.saveLog(Param.bar_B, "FAIL", false, false, "STNG");

                                ngDUT_B_TestItem = "STNG";
                                //=============upload sfcs=============
                                if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                                {
                                    ws.UploadFixtureID(Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                    TrnDatas[0] = getErrorCode(ngDUT_B_TestItem);
                                    string result = ws.Complete(Param.bar_B, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                                    if (result == "OK")
                                    {
                                        //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem);
                                        SubFunction.SaveTestLog("13---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("13---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem);
                                    }
                                    else
                                    {
                                        //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem);
                                        SubFunction.SaveTestLog("14---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("14---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem);                                       
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + Param.bar_B + " Fail," + result);
                                    }
                                }
                                else
                                {
                                    //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG");
                                    SubFunction.SaveTestLog("15---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("15---" + "Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG"); 
                                }
                                lblTestResult_B.ForeColor = Color.Red;
                                lblTestResult_B.Text = "FAIL";
                                i_Detect_B_Count = 0; //重置參数

                                */



                                //1,断电
                                SubFunction.updateMessage(lstStatusCommand, "MB_B断开19V");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B断开19V");
                                sendPLC("ab");
                                _b_B_send_ab = true;

#if DEBUG
                                SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                                SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
#endif
                                if (_b_A_send_aa && _b_B_send_ab)
                                {
                                    _b_B_send_ab = false;
                                    //----------可能会重复发送命令-----
                                    //2,判断侧插
                                    //左右都无
                                    if (!Param.LeftInsert && !Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                        sendPLC("DO");
                                    }
                                    //右侧插，先右后左
                                    if (Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                        sendPLC("OR");
                                    }
                                    else //右边沒有，判断左边
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                        sendPLC("OL");
                                    }
                                    // timerDetectTimeOut.Stop();
                                    return;
                                }

                                return;
                            }
                        }

                    }
                }
            }

            timerDetect_V_B.Start();
        }

        #endregion


        private void checkMBType(string mbtype)//检查MB是单板还是双板,并进行相应的切换
        {
            //单板
            if (mbtype.ToUpper() == Param.mbType.Single.ToString().ToUpper())
            {
                this.comboBarcodeType.Enabled = false;
                this.comboDUTPort_B.Enabled = false;
                this.txtBar_B.ReadOnly = true;
                //this.grbMB_BSFCSInfo.Enabled = false;
                //this.grbMB_B_TestItem.Enabled = false;
                SubFunction.updateMessage(lstStatusCommand, "你所选择MB类型为 " + Param.mbType.Single.ToString());
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "你所选择MB类型为 " + Param.mbType.Single.ToString());
                return;
            }
            //双拼板
            if (mbtype.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //this.comboBarcodeType.Enabled = true;   //chage by channing 20161213
                //this.comboDUTPort_B.Enabled = true; //chage by channing 20161213
                this.txtBar_B.ReadOnly = true;
                this.grbMB_BSFCSInfo.Enabled = true;
                this.grbMB_B_TestItem.Enabled = true;
                SubFunction.updateMessage(lstStatusCommand, "你所选择MB类型为 " + Param.mbType.Panel.ToString());
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "你所选择MB类型为 " + Param.mbType.Panel.ToString());
                return;
            }
        }

        private void loadData2UI()// 将从配置档的读取的信息加载到UI上
        {


            this.txtCenterIP_DataBase_IP.Text = Param.CenterIP_DataBase_IP;
            this.txtRecordIP_DataBase_IP.Text = Param.RecordIP_DataBase_IP;
            this.comboTestingType.Text = Param.TestingType;
            this.comboRobotModule.Text = Param.RobotModule;
            this.comboMBType.Text = Param.MBType;
            this.comboFictStage.Text = Param.FICTStage;
            this.comboPCBLine.Text = Param.PCBLine;
            this.comboBarcodeType.Text = Param.BarcodeType;
            this.lblFICTStage.Text = Param.FixtrueID;
            //
            this.comboScannerPort.Text = Param.Scanner;
            this.comboDUTPort_A.Text = Param.DUT_A;
            this.comboDUTPort_B.Text = Param.DUT_B;
            this.comboFICTPort.Text = Param.PLC;
            //
            this.txtArmsPath.Text = Param.Arms_Path;
            //
            checkBool2Button(Param.OKBBBStatus, this.btnOKBBBSwitch);
            checkBool2Button(Param.NGBBBStatus, this.btnNGBBBSwitch);
            checkBool2Button(Param.Arms_Use, this.btnArmsSwith);
            //
            this.txtOPID.Text = Param.OPID;
            this.txtFixtureID.Text = Param.FixtrueID;
            //
            // Param.SFC_Stage;
            //
            checkMBType(Param.MBType);
            //
            //clear barcode
            lstBarcodeList.Items.Clear();
            this.txtBar_A.Text = string.Empty;
            this.txtBar_B.Text = string.Empty;
            //
            this.txtMB_A_ArmsVer.Text = Param.Arms_Version;
            this.txtMB_B_ArmsVer.Text = Param.Arms_Version;
            //timeout
            //this.lstStatusCommand.Text = icount_DUT_A_TestingTimeOut.ToString();
            this.txtPowerOnTimeOut.Text = Param.PowerONTimeOut.ToString();
            this.txtTestOKTimeOut.Text = Param.TestOKTimeOut.ToString();

            //是否启用侧插按钮
            if (Param.LeftInsert)
            {
                this.btnLC.Enabled = true;
                this.btnOL.Enabled = true;
            }
            else
            {
                this.btnLC.Enabled = false ;
                this.btnOL.Enabled = false;
            }

            if (Param.RightInsert)
            {
                this.btnRC.Enabled = true;
                this.btnOR.Enabled = true;
            }
            else
            {
                this.btnRC.Enabled = false;
                this.btnOR.Enabled = false;
            }

           //是否启用串口
            
            if ((Param.Scanner_Use) &&(this.btnStart.Enabled))
                this.comboScannerPort.Enabled = true ;
            else
                this.comboScannerPort.Enabled = false;
            if ((Param.DUT_A_Use) &&(this.btnStart.Enabled))
                this.comboDUTPort_A.Enabled = true;
            else
                this.comboDUTPort_A.Enabled = false;
            if ((Param.DUT_B_Use)&&(this.btnStart.Enabled))
                this.comboDUTPort_B.Enabled = true;
            else
                this.comboDUTPort_B.Enabled = false;
            if ((Param.PLC_Use)&&(this.btnStart.Enabled))
                this.comboFICTPort.Enabled = true;
            else
                this.comboFICTPort.Enabled = false;

           
            //是否开启上抛按钮
            if (this.comboRobotModule.Text == "1")
            {
                this.btnNGBBBSwitch.Enabled = false;
                checkBool2Button(false, btnNGBBBSwitch);
            }
            else
            {
                this.btnNGBBBSwitch.Enabled = true;
            }

            if (Param.ST_Flag)
            {
                this.LB_ST.Text = "...";
            }
            else
            {
                this.LB_ST.Text = "";
            }

        }

        private void checkBool2Button(bool inbool, Button outskinbutton)// 检查对应的bool的值，其对应的button对相应的变化
        {
            if (inbool)
                outskinbutton.BackColor = Color.Lime;
            if (!inbool)
                outskinbutton.BackColor = Color.Red;
        }

        private bool checkInputData(TextBox textbox, string paramdata)// 自动保存数据時，检查数据，禁止为空,为空则为false，不为空则为true
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

        #region Channing

        private bool checkStage(string usn, string stage, ref string returnstage)//检查USN条码是否在当前站别，在为true,不在为false
        {

            //  checkWebService(web_Site);

            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();
            SubFunction.updateMessage(lstStatusCommand, "SFCS:" + usn + ",站别:" + stage);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SFCS:" + usn + ",站别:" + stage);
            string result = ws.CheckRoute(usn, stage);
            //string result = ws.CheckRoute(usn, stage);
            // returnstage = getStage(result);
            sw.Stop();
            ts = sw.Elapsed;
            if (result.ToUpper() == "OK")
            {
                SubFunction.updateMessage(lstStatusCommand, result + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), usn + ":" + stage);
                returnstage = stage;
                //saveLog(usn, stage);// 分类-单独存stage.log信息
                return true;
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, result + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), result + "Used time(ms):" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), usn + ":" + getStage(result));
                //saveLog(usn, getStage(result)); // 分类-单独存stage.log信息
                returnstage = getStage(result);
                return false;
            }

        }
       
        private string getStage(string sfcsresult)//从SFCS返回胡结果里面抓取站别信息
        {
            string stage = string.Empty;

            // If message = "" Then GetStage = ""
            //Dim s() As String = message.Split(" ")
            //For i As Integer = 0 To s.Length - 1
            //    If s(i).ToLower.Trim = "to" Then
            //        GetStage = s(i + 1)
            //        Exit For
            //    End If
            //Next

            if (string.IsNullOrEmpty(sfcsresult))
                stage = string.Empty;
            string[] s = sfcsresult.Split(' ');
            for (int i = 0; i <= s.Length - 1; i++)
            {
                if (s[i].ToLower().Trim() == "to")
                {
                    stage = s[i + 1];
                    break;
                }
                if (s[i].ToLower().Trim() == "storein!")
                {
                    stage = "SA";
                    break;
                }
            }
            if (string.IsNullOrEmpty(stage))//检测出站别信息为空
                stage = "NULL";

            return stage;
        }

        private void Test_Error_Stop()//连续出现NG,停止测试
        {
            if (ErrCount < Param.MaxErrorCount)
            {
                ErrCount += 1;
                SubFunction.updateMessage(lstStatusCommand, "机台测试，连续第" + ErrCount.ToString() + "次NG。请注意！！！");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台测试，连续第" + ErrCount.ToString() + "次NG。请注意！！！");
            }
            else
            {
                this.BackColor = Color.Red;
                //this.BackColor = SystemColors.ActiveCaption;
                SubFunction.updateMessage(lstStatusCommand, "机台测试，连续出现NG" + Param.MaxErrorCount.ToString() + "次。现暂停测试！！！");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台测试，连续出现NG" + Param.MaxErrorCount.ToString() + "次。现暂停测试！！！");
                MessageBox.Show("因连续NG，目前暂停测试，请检查机台！！！");
                timerScanFICT.Stop();
            }
        }

        private void settime()//系统时间与记录数据库时间同步，误差为1秒
        {
            //Record_DataBase_IP = "127.0.0.1";
            //Record_DataBase_Account = "root";
            //Record_DataBase_Password = "123456";
            //Record_DataBase_DB = "fictsninfo";

            if (Param.Record_DataBase_Use)
            {
                string MyConString = string.Empty;
                MyConString = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB + ";connectiontimeout=3";
                MySqlConnection objConnection = new MySql.Data.MySqlClient.MySqlConnection(MyConString);
                MySqlCommand objCommand_getdatetime = new MySql.Data.MySqlClient.MySqlCommand("select sysdate()", objConnection);//获取数据库系统时间

                try
                {
                    //连接数据库
                    objConnection.Open();
                    SubFunction.updateMessage(lstStatusCommand, "获取 <记录数据库> 系统时间，连接成功");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "获取 <记录数据库> 系统时间，连接成功");
                }
                catch (Exception ex)
                {
                    SubFunction.updateMessage(lstStatusCommand, "获取 <记录数据库> 系统时间，但连接失败..., " + "Message:" + ex.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "获取 <记录数据库> 系统时间，但连接失败..., " + "Message:" + ex.Message);
                    return;
                }

                try
                {
                    MySqlDataReader reader_date_time = objCommand_getdatetime.ExecuteReader();
                    if (reader_date_time.Read())
                    {
                        string DBdt_date = ((DateTime)reader_date_time[0]).ToString("yyyy-MM-dd");//数据库日期;
                        string DBdt_time = ((DateTime)reader_date_time[0]).ToString("HH:mm:ss") + ".0";//数据库时间
                        string CLdt = (DateTime.Now).ToString("yyyyMMddHHmmss");//当前时间字符串
                        string DBdt = ((DateTime)reader_date_time[0]).ToString("yyyyMMddHHmmss");//当前时间字符串
                        objConnection.Close();

                        if (CLdt != DBdt)//时间校正
                        {

                            //DBdt_date = "2016-10-10";
                            //DBdt_time = "23:12:10.0";
                            //设置日期
                            Process pdate = new Process();
                            pdate.StartInfo.FileName = "cmd.exe";
                            pdate.StartInfo.Arguments = "/c date " + DBdt_date;
                            pdate.StartInfo.UseShellExecute = false;
                            pdate.StartInfo.RedirectStandardInput = true;
                            pdate.StartInfo.RedirectStandardOutput = true;
                            pdate.StartInfo.RedirectStandardError = true;
                            pdate.StartInfo.CreateNoWindow = true;
                            pdate.Start();
                            pdate.StandardOutput.ReadToEnd();

                            //设置时间
                            Process ptime = new Process();
                            ptime.StartInfo.FileName = "cmd.exe";
                            ptime.StartInfo.Arguments = "/c time " + DBdt_time;
                            ptime.StartInfo.UseShellExecute = false;
                            ptime.StartInfo.RedirectStandardInput = true;
                            ptime.StartInfo.RedirectStandardOutput = true;
                            ptime.StartInfo.RedirectStandardError = true;
                            ptime.StartInfo.CreateNoWindow = true;
                            ptime.Start();
                            ptime.StandardOutput.ReadToEnd();
                            SubFunction.updateMessage(lstStatusCommand, "校正系统时间：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "校正系统时间：" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    SubFunction.updateMessage(lstStatusCommand, "时间同步出错，Error" + ex.ToString());
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "时间同步出错，Error" + ex.ToString());
                    return;
                }
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "未使用记录数据库，不能完成时间同步...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未使用记录数据库，不能完成时间同步...");
            }
        }

        private bool updatesql(string sqlstr)//向数据库写入字符串sqlstr
        {
            string MyConString = string.Empty;
            MyConString = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB + ";connectiontimeout=3";
            MySqlConnection objConnection = new MySql.Data.MySqlClient.MySqlConnection(MyConString);
            MySqlCommand objCommand = new MySql.Data.MySqlClient.MySqlCommand(sqlstr, objConnection);
           //*********************************** add by channing 20161215
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), MyConString);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), sqlstr);

            int iCount = 1;
            int iCountMax = 4;
            while (iCount < iCountMax)
            {
                try
                {
                    objConnection.Open();
                    //SubFunction.updateMessage(lstStatusCommand, "第 "+ iCount + " 次连接 <记录数据库> 成功...");
                    //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第 " + iCount + " 次连接 <记录数据库> 成功...");
                    iCount = 5;
                }
                catch (Exception ex)
                {
                    SubFunction.updateMessage(lstStatusCommand, "第 " + iCount + " 次连接 <记录数据库> 失败...");
                    SubFunction.updateMessage(lstStatusCommand, "Message:" + ex.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第 " + iCount + " 次连接 <记录数据库> 失败...");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Message:" + ex.Message);
                    iCount += 1;
                }
            }
            if (iCount == 4) //连续三次未连接上数据库，退出本函数
            {
                SubFunction.updateMessage(lstStatusCommand, "已达最大连接数，退出重复连接 <记录数据库> ...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已达最大连接数，退出重复连接 <记录数据库> ...");
                return false;
            }

            iCount=1;

            while (iCount < iCountMax)
            {
                try
                {
                    int i = objCommand.ExecuteNonQuery();
                    SubFunction.updateMessage(lstStatusCommand, "第 " + iCount + " 次向 <记录数据库> 写成功...");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第 " + iCount + " 次向 <记录数据库> 写成功...");
                    iCount = 5;
                }
                catch (Exception ex)
                {
                    
                    SubFunction.updateMessage(lstStatusCommand, "第 " + iCount + " 次向 <记录数据库> 写失败,Message:" + ex.Message);
                    SubFunction.updateMessage(lstStatusCommand, "数据库操作命令： sql=" + sqlstr);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第 " + iCount + " 次向 <记录数据库> 写失败,Message:" + ex.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "数据库操作命令： sql=" + sqlstr);
                    iCount += 1;
                    objConnection.Close();
                }
            }

            if (iCount == 4) //连续三次未写入数据库，退出本函数
            {
                SubFunction.updateMessage(lstStatusCommand, "已达最大写入数，退出重复向 <记录数据库> 写入 ...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已达最大写入数，退出重复向 <记录数据库> 写入 ...");
                return false;
            }
            return true;
        }

        private void TestLog_for_Record_DataBase(string R_PPID, string R_TestResult, bool R_ReTestFlag, bool R_SFCSFlag, string R_TestInfo)//向数据库写入测试数据
        {

            if (Param.Record_DataBase_Use)//检查是否启用记录数据库
            {
                bool updatesqlResult;
                Param.sql = string.Empty;
                Param.sql = "insert into " + Param.RecordIP_DataBase_TestInfo_Table + " (RecordTime,PPID,TestResult,ReTestFlag,SFCSFlag,TestInfo,Model,ModelFamily,MO,UPN,TestTime,TestStation,Line,FICTModel,REMARK)";
                Param.sql += "values ('" + DateTime.Now.ToString("yyyyMMddHHmmss") + "','" + R_PPID + "','" + R_TestResult + "','" + R_ReTestFlag + "','" + R_SFCSFlag + "','" + R_TestInfo + "','" + Param.Model + "','" + Param.ModelFamily + "','" + Param.MO + "','" + Param.UPN + "','" + icount_DUT_TestingTimeOut.ToString() + "','" + Param.FixtrueID + "','" + Param.PCBLine + "','" + Param.RobotModule + "','" + "" + "')";
                updatesqlResult = updatesql(Param.sql);

                if (!updatesqlResult)   //未写入记录数据库的TestLog， 暂存缓存文档
                {
                    try
                    {
                        string NotWriteMysqlData = string.Empty;
                        NotWriteMysqlData = DateTime.Now.ToString("yyyyMMddHHmmss") + "," + R_PPID + "," + R_TestResult + "," + R_ReTestFlag + "," + R_SFCSFlag + "," + R_TestInfo + "," + Param.Model + "," + Param.ModelFamily + "," + Param.MO + "," + Param.UPN + "," + icount_DUT_TestingTimeOut + "," + Param.FixtrueID + "," + Param.PCBLine + "," + "" + "\n";
                        StreamWriter sw = File.AppendText(Param.MysqlTestDatatxt);
                        sw.WriteLine(NotWriteMysqlData);
                        sw.Flush();
                        sw.Close();
                        SubFunction.updateMessage(lstStatusCommand, "未上抛至记录数据库，现已写入暂存文档...");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未上抛至记录数据库，现已写入暂存文档...");
                    }
                    catch (Exception ex)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "未上抛至记录数据库，现写入暂存文档出错...");
                        SubFunction.updateMessage(lstStatusCommand, "Error:" + ex.Message);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未上抛至记录数据库，现写入暂存文档出错...");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Error:" + ex.Message);
                    }
                }
            }

        }

        private void getMOModelUPN(string website, string usn, ref string Model, ref string ModelFamily, ref string MO, ref string UPN)//从SFCS获取MB,MO,UPN等信息
        {
            if (!checkWebService(website))
                return;

            Stopwatch sw = new Stopwatch();
            TimeSpan ts = new TimeSpan();
            sw.Start();

            string result = string.Empty;
            PcbWeb.clsRequestData rd = new PcbWeb.clsRequestData();
            rd = ws.GetUUTData(usn, "TD", rd, 1);
            if (rd.Result != "OK")
            {
                sw.Stop();
                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "获取Model/ModelFamily/MO/UPN,time(ms)" + ts.Milliseconds + "," + result);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "获取Model/ModelFamily/MO/UPN,time(ms)" + ts.Milliseconds + "," + result);
                return;
            }
            if (rd.Result == "OK")
            {
                Model = rd.Model;
                ModelFamily = rd.ModelFamily;
                MO = rd.MO;
                UPN = rd.UPN;

                sw.Stop();
                ts = sw.Elapsed;

                ts = sw.Elapsed;
                SubFunction.updateMessage(lstStatusCommand, "获取Model/ModelFamily/MO/UPN" + result + " ,time(ms)" + ts.Milliseconds);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "获取Model/ModelFamily/MO/UPN" + result + " ,time(ms)" + ts.Milliseconds);
                SubFunction.updateMessage(lstStatusCommand, "Model = " + Model);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Model = " + Model);
                SubFunction.updateMessage(lstStatusCommand, "ModelFamily = " + ModelFamily);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "ModelFamily = " + ModelFamily);
                SubFunction.updateMessage(lstStatusCommand, "MO = " + MO);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MO = " + MO);
                SubFunction.updateMessage(lstStatusCommand, "UPN = " + UPN);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "UPN = " + UPN);
            }
        }

        private void choseDoubleSigle()//单板双板选择
        {
            //单板测试、双板测试初始化           //add 20170103
            if (Param.mbType.Panel.ToString() == this.comboMBType.Text)
            {
                Param.Test_MB_A = true; //启用A板测试
                Param.Test_MB_B = true; //启用B板测试
            }
            else if (Param.mbType.Single.ToString() == this.comboMBType.Text)
            {
                Param.Test_MB_A = true; //启用A板测试
                Param.Test_MB_B = false; //启用B板测试
            }
        }

        public static MySqlConnection getMySqlCon() //建立mysql数据库链接
        {
            // string Record_DataBase_IP = "127.0.0.1";
            // string Record_DataBase_Account = "root";
            // string Record_DataBase_Password = "123456";
            // string Record_DataBase_DB = "fictsninfo";
            string MyConString = string.Empty;
            MyConString = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB + ";connectiontimeout=3";
            MySqlConnection mysql = new MySqlConnection(MyConString);
            return mysql;
        }
        public static MySqlCommand getSqlCommand(String sql, MySqlConnection mysql)// 建立执行命令语句对象
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, mysql);
            //  MySqlCommand mySqlCommand = new MySqlCommand(sql);
            // mySqlCommand.Connection = mysql;
            return mySqlCommand;
        }
        public void getResultset(MySqlCommand mySqlCommand)// 查询并获得结果集并遍历
        {
            MySqlDataReader reader = mySqlCommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "编号:" + reader.GetInt32(0) + " | 日期：" + reader.GetInt32(1) + " | 条码：" + reader.GetInt32(2));
                        
                       // Console.WriteLine("编号:" + reader.GetInt32(0) + "|姓名:" + reader.GetString(1) + "|年龄:" + reader.GetInt32(2) + "|学历:" + reader.GetString(3));
                    }
                }
            }
            catch (Exception ex)
            {

                String message = ex.Message;
                SubFunction.updateMessage(lstStatusCommand, "获取数据失败！ Error:" + message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "获取数据失败！ Error:" + message);
            }
            finally
            {
                reader.Close();
            }
        }
        public void getInsert(MySqlCommand mySqlCommand)// 添加数据
        {
            try
            {
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                SubFunction.updateMessage(lstStatusCommand, "添加数据失败了！ Error:" + message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "添加数据失败了！ Error:" + message);
            }

        }
        public void getUpdate(MySqlCommand mySqlCommand)// 修改数据
        {
            try
            {
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                SubFunction.updateMessage(lstStatusCommand, "修改数据失败了！ Error:" + message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "修改数据失败了！ Error:" + message);
            }
        }
        public void getDel(MySqlCommand mySqlCommand)// 删除数据
        {
            try
            {
                mySqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                SubFunction.updateMessage(lstStatusCommand, "删除数据失败了！ Error:" + message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "删除数据失败了！ Error:" + message);
            }
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)//快捷键
        {
            if (e.Control && e.KeyCode == Keys.D)
            {
                e.Handled = true;
                MessageBox.Show("ssssssssssss");
            }
        }

        /// <summary>
        /// 检查是否需要重新测试
        /// </summary>
        /// <param name="MB_Type">MB板面,参数  MB_A,MB_B</param>
        /// <param name="NG_Type">NG现象，TMOU，STNG </param>
        /// <param name="Retest_Flag">重测标志</param>
        /// <param name="MB_Bar">MB板哪一面条码</param>
        /// <param name="Remark">条码标记</param>
        private void CheckRetest(string MB_Type, string NG_Type, bool Retest_Flag,string MB_Bar,string Remark)
        {

            //判断是否需要重测
            if (iCurrent_TestCount >= Param.MaxRetestCount)
            {
                //不重测
                SubFunction.updateMessage(lstStatusCommand, MB_Type + " 未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT不重复测试");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(),  MB_Type + " 未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT不重复测试");

                if (Param.Test_Log)
                {
                    // ngDUT_B_TestItem = "STNG";
                    //=============upload sfcs=============
                    if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                    {
                        ws.UploadFixtureID(MB_Bar, Param.SFC_Stage, Param.FixtrueID);
                        TrnDatas[0] = getErrorCode(NG_Type);
                        string result = ws.Complete(MB_Bar, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                        if (result == "OK")
                        {
                            //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, true, ngDUT_B_TestItem);
                            SubFunction.SaveTestLog(Remark + MB_Bar, "FAIL", Retest_Flag, true, NG_Type, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                            TestLog_for_Record_DataBase(Remark + MB_Bar, "FAIL", Retest_Flag, true, NG_Type);
                        }
                        else
                        {
                            //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_B_Retest_Flag, false, ngDUT_B_TestItem);
                            SubFunction.SaveTestLog(Remark + MB_Bar, "FAIL", Retest_Flag, false, NG_Type, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                            TestLog_for_Record_DataBase(Remark + MB_Bar, "FAIL", Retest_Flag, false, NG_Type);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + MB_Bar + " Fail," + result);
                        }
                    }
                    else
                    {
                        //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", DUT_A_Retest_Flag, false, "STNG"); ////???????? 有疑问 
                        SubFunction.SaveTestLog(Remark + MB_Bar, "FAIL", Retest_Flag, false, NG_Type, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                        TestLog_for_Record_DataBase(Remark + MB_Bar, "FAIL", Retest_Flag, false, NG_Type);
                    }
                    Param.Test_Log = false;
                }
               

                if (MB_Type == "MB_B")
                {
                    lblTestResult_B.ForeColor = Color.Red;
                    lblTestResult_B.Text = "FAIL";

                    i_Detect_B_Count = 0;
                    timerDetectTimeOut.Stop();
                    ShutDownCount_B = 0;
                    //需要强制断开MB_B的电
                    //1,断
                    //Console.WriteLine("双连板MB_B需要重测，MB_A需要强制开机");
                    SubFunction.updateMessage(lstStatusCommand, "B面测试失败，MB_B需要强制关机");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "B面测试失败，MB_B需要强制关机");
                    sendPLC("S2");

                }
                if (MB_Type == "MB_A")
                {
                    lblTestResult_A.ForeColor = Color.Red;
                    lblTestResult_A.Text = "FAIL";
                    i_Detect_A_Count = 0;
                    timerDetectTimeOut.Stop();
                    ShutDownCount_A = 0;
                    //需要强制断开MB_A的电
                    //1,断
                    //Console.WriteLine("双连板MB_B需要重测，MB_A需要强制开机");
                    SubFunction.updateMessage(lstStatusCommand, "A面测试失败，MB_A需要强制关机");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "A面测试失败，MB_A需要强制关机");
                    sendPLC("S1");
                }
            }
            else
            {
                if (MB_Type == "MB_B")
                {
                    DUT_B_Retest_Flag = true;
                    DUT_Retest_Flag = true;
                    i_Detect_B_Count = 0;
                    timerDetectTimeOut.Stop();
                    ShutDownCount_B = 0;

                    //要重测
                    SubFunction.updateMessage(lstStatusCommand, MB_Type + "未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), MB_Type + "未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");
                    SubFunction.updateMessage(lstStatusCommand, "双连板MB_B需要重测，MB_A需要强制开机");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板MB_B需要重测，MB_A需要强制开机");
                    sendPLC("S1");
                    sendPLC("S2");
                }
                if (MB_Type == "MB_A")
                {
                    DUT_A_Retest_Flag = true;
                    DUT_Retest_Flag = true;
                    i_Detect_A_Count = 0;
                    timerDetectTimeOut.Stop();
                    ShutDownCount_A = 0;
                    //要重测
                    SubFunction.updateMessage(lstStatusCommand, MB_Type + "未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), MB_Type + "未检测到开机电压，当前测试次数:" + iCurrent_TestCount + ",最大测试次数:" + Param.MaxRetestCount + ",FICT重复测试");
                    SubFunction.updateMessage(lstStatusCommand, "双连板MB_A需要重测，MB_B需要强制开机");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板MB_A需要重测，MB_B需要强制开机");
                    sendPLC("S1");
                    sendPLC("S2");
                }
            }  
        }



        /// <summary>
        ///     依据数据库表及其中地址查找字符串
        /// </summary>
        /// <param name="DataBase_Table">数据库表</param>
        /// <param name="Table_Address">表中地址</param>
        /// <param name="Search_Str">查找字符串</param>
        /// <param name="Output">查找字符串</param>
        /// <returns>返回结果，True，找到；false，未找到</returns>
        private bool Serch_Mysql_Str(string DataBase_Table, string Table_Address, string Search_Str, ref string Output)
        {
            //    Output = 11, 已打开数据库，并查询到相对应字符串
            //    Output = 12, 已打开数据库，未查询到相对应字符串
            //    Output = 13, 已打开数据库，查询出错

            //搜索数据库命令
            string outdata = string.Empty;
            string ConnStr = Param.Record_DB_ConnStr;
            string sqlSearch = "select * from " + DataBase_Table + " WHERE " + Table_Address + " LIKE  '" + Search_Str + "'";
            MySqlConnection objConnection = new MySqlConnection(ConnStr);
            MySqlCommand objCommand = new MySqlCommand(sqlSearch, objConnection); //查询
            MySqlDataReader objReader;
            int i = 1;
            int retrymax = 3;

            //打开记录数据库
            while (i <= retrymax)
            {
                try
                {
                    objConnection.Open();
                    //SubFunction.updateMessage(lstStatusCommand, "第" + i + "次打开<记录数据库>成功...");
                    //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次，打开<记录数据库>成功...");
                    i = 4;
                    Output = "1";
                }
                catch (Exception ex)
                {
                    SubFunction.updateMessage(lstStatusCommand, "第" + i + "次打开<记录数据库>失败...");
                    SubFunction.updateMessage(lstStatusCommand, "Error：" + ex.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次打开<记录数据库>失败...");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Error：" + ex.Message);
                    if (i == 3)
                    {
                        Output = "0";
                        return false;
                    }
                    i += 1;
                }
            }

            //查询记录数据库

            try
            {
                objReader = objCommand.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        outdata = objReader[Table_Address].ToString();
                        SubFunction.updateMessage(lstStatusCommand, "从<记录数据库>所读：<" + Table_Address + "> = " + outdata);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "从<记录数据库>所读：<" + Table_Address + "> = " + outdata);
                    }
                    Output += "1";
                    objConnection.Close();
                    return true;
                }
                else
                {
                    SubFunction.updateMessage(lstStatusCommand, "从<记录数据库>中<" + Table_Address + ">未搜索到“" + Search_Str + "”");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "从<记录数据库>中<" + Table_Address + ">未搜索到“" + Search_Str + "”");
                    objConnection.Close();
                    Output += "2";
                    return false;
                }
            }
            catch (Exception ex)
            {
                SubFunction.updateMessage(lstStatusCommand, "从<记录数据库>读取数据，Error：" + ex.Message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "从<记录数据库>取数据，Error：" + ex.Message);
                objConnection.Close();
                Output += "3";
                return false;
            }
        }

        private bool Insert_Mysql_Str(string DataBase_Table, string Table_Address, string MInsert_Str, ref string Output)
        {

            string sqlInsert = "insert into " + DataBase_Table + " (Line,Station,IPAdrress,State,Timer,TimeOut,Total,Pass,Fail,FPT,Model,MBNumber,UPN,MO,REMARK)";
            sqlInsert += "values ('" + Param.PCBLine + "','" + MInsert_Str + "','" + ipAdress + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "','" + "" + "')";
            // updatesql(sqlInsert);
            return updatesql(sqlInsert);
        }

        private bool Update_Mysql_Str(string DataBase_Table, string Update_Address, string Update_Str, string Serch_Address, string Serch_Str)
        {

            string outdata = string.Empty;
            string ConnStr = Param.Record_DB_ConnStr;
            string sqlUpdate = "update " + DataBase_Table + " set " + Update_Address + " = '" + Update_Str + "' where " + Serch_Address + " = '" + Serch_Str + "'";
            MySqlConnection objConnection = new MySqlConnection(ConnStr);
            MySqlCommand objCommand = new MySqlCommand(sqlUpdate, objConnection); //查询
            // MySqlDataReader objReader;
            int i = 1;
            int retrymax = 3;

            //打开记录数据库
            while (i <= retrymax)
            {
                try
                {
                    objConnection.Open();
                   //SubFunction.updateMessage(lstStatusCommand, "第" + i + "次，打开<记录数据库>成功...");
                   //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次，打开<记录数据库>成功...");
                    i = 4;
                }
                catch (Exception ex)
                {
                    SubFunction.updateMessage(lstStatusCommand, "第" + i + "次打开<记录数据库>库失败...");
                    SubFunction.updateMessage(lstStatusCommand, "Error：" + ex.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "第" + i + "次打开<记录数据库>失败...");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Error：" + ex.Message);
                    if (i == 3)
                    {
                        return false;
                    }
                    i += 1;
                }
            }

            try
            {
                objCommand.ExecuteNonQuery();
                SubFunction.updateMessage(lstStatusCommand, "Update <记录数据库>成功," + Update_Address + " = <" + Update_Str + ">...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Update <记录数据库>成功," + Update_Address + " = <" + Update_Str + ">...");
                objConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                String message = ex.Message;
                SubFunction.updateMessage(lstStatusCommand, "修改<记录数据库>失败了！"+ Update_Address + " = <" + Update_Str +", Error:" + message);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "修改<记录数据库>失败了！" + Update_Address + " = <" + Update_Str + ", Error:" + message);
                objConnection.Close();//009
                return false;           
            }
        }

        /// <summary>
        /// 更新测试统计信息
        /// </summary>
        private void Update_Test_Statistical()
        {
            if (Param.Record_DataBase_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "准向<记录数据库>上传统计信息...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准向<记录数据库>上传统计信息...");

                //上传总的MB测试数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "Total", (Param.iPass + Param.iFail).ToString(), "Station", Param.FixtrueID);
                //上传测试PASS数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "Pass", Param.iPass.ToString(), "Station", Param.FixtrueID);
                //上传测试FAIL数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "Fail", Param.iFail.ToString(), "Station", Param.FixtrueID);
                //上传测试FPT数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "FPT", Param.iFail_Pass.ToString(), "Station", Param.FixtrueID);
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "未启用<记录数据库>，不能上传统计信息...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未启用<记录数据库>，不能上传统计信息...");
            }
        }
        /// <summary>
        /// 更新机台所测试MB信息，Model，MO，UPN等
        /// </summary>
        private void Update_MB_Info()
        {
            if (Param.Record_DataBase_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "准向<记录数据库>上传MB信息...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准向<记录数据库>上传MB信息...");

                //上传总的MB测试数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "Model",Param.Model, "Station", Param.FixtrueID);
                //上传测试PASS数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "MBNumber", Param.MBNumber, "Station", Param.FixtrueID);
                //上传测试FAIL数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "UPN", Param.UPN, "Station", Param.FixtrueID);
                //上传测试FPT数量
                Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "MO", Param.MO, "Station", Param.FixtrueID);
            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "未启用<记录数据库>，不能上传MB信息...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未启用<记录数据库>，不能上传MB信息...");
            }
        }

        /// <summary>
        /// 更新治具编号信息
        /// </summary>
        private void Update_FixtrueID()
        {


            if (Param.Record_DataBase_Use)
            {
                foreach (string ip in SubFunction.getIP(Dns.GetHostName(), Param.IPType.IPV4.ToString()))
                {
                    this.Text = exeTitle + ",本地IP:" + ip;
                    ipAdress = ip;
                }

                string Serch_Outdata = string.Empty;
                string Insert_Outdata = string.Empty;
                SubFunction.updateMessage(lstStatusCommand, "准备从<记录数据库>中查找本机台站别信息...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备从<记录数据库>中查找本机台站别信息...");

                //从记录数据库中查找是否存在本机台
                Serch_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "Station", Param.FixtrueID, ref Serch_Outdata);

                if (Serch_Outdata == "12")//不存在机台治具编号
                {
                    SubFunction.updateMessage(lstStatusCommand, "未找到本机台站别信息，准备写入...");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未找到本机台站别信息，准备写入...");
                    //准备插入机台编号信息
                    Insert_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "Station", Param.FixtrueID, ref  Insert_Outdata);
                }

                if (Serch_Outdata == "11")//存在机台治具编号
                {
                    //准备插入机台编号信息
                    string IPinfo = string.Empty;
                    //准备核对IP地址
                    Serch_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "IPAdrress", ipAdress, ref IPinfo);
                    if (IPinfo == "11") //从记录数据库中找到本机IP地址
                    {
                        SubFunction.updateMessage(lstStatusCommand, "已核对IP，其地址未作变更...");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已核对IP，其地址未作变更...");
                    }
                    if (IPinfo == "12")//从记录数据库中未找到本机IP地址
                    {
                        SubFunction.updateMessage(lstStatusCommand, "存在机台编号，但IP地址不一致...");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "存在机台编号，但IP地址不一致...");


                        DialogResult MsgBoxResult;//设置对话框返回值
                        string MEG = string.Empty;
                        MEG = "<记录数据库>已存在机台编号，但IP地址不一致，请确认机台编号是否唯一？" + "\n";
                        MEG += "\n";
                        MEG += "注意：" + "\n" + "1.机台编号唯一，将更新IP地址" + "\n";
                        MEG += "2.不唯一，则表示FICT测试系统存在至少2两台同样编号的机台，请逐一排查！！！" + "\n";
                        MEG += "\n";
                        MEG += "是点击“Yes”，否点击“NO”" + "\n";
                        MsgBoxResult = MessageBox.Show(MEG, "小心操作！！！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                        if (MsgBoxResult == DialogResult.Yes)
                        {
                            Update_Mysql_Str(Param.RecordIP_DataBase_Station_Table, "IPAdrress", ipAdress, "Station", Param.FixtrueID);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "执行更新<记录数据库>IP地址...");
                        }
                        if (MsgBoxResult == DialogResult.No)
                        {
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "取消更新<记录数据库>IP地址...");
                        }
                    }

                }

            }
            else
            {
                SubFunction.updateMessage(lstStatusCommand, "未启用<记录数据库>，不能更新机台编号信息...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未启用<记录数据库>，不能更新机台编号信息...");
            }


        }
        /*

        //禁用窗体关闭按钮
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams mycp = base.CreateParams;
                mycp.ClassStyle = mycp.ClassStyle | CP_NOCLOSE_BUTTON;
                return mycp;
            }
        }
        */
        /// <summary>
        /// 测试数据自动清零
        /// </summary>
        private void Clear_TestData()
        {
            if (DateTime.Now.ToString("mm:ss") == "00:00")
            {
                SubFunction.saveLog(Param.logType.ANALOG.ToString(), "MB_Double:Total=" + (Param.iPass + Param.iFail).ToString() + ",Pass=" + Param.iPass.ToString() + ",Fail=" + Param.iFail.ToString() + ",FPT=" + Param.iFail_Pass.ToString ());
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已经到整点，存入测试统计信息");
                SubFunction.updateMessage(lstStatusCommand, "已经到整点，存入测试统计信息");
                
                if (DateTime.Now.ToString("hh:mm:ss") == "08:00:00")
                {
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "已经到换班时间，统计信息清零");
                    Param.iPass_A = 0;// MB_A Pass
                    Param.iFail_A = 0;// MB_A Fail 
                    Param.iFail_Pass_A = 0;// MB_A 强抛 

                    Param.iPass_B = 0; //MB_B Fail
                    Param.iFail_B = 0; //MB_B Fail
                    Param.iFail_Pass_B = 0;// MB_A 强抛 

                    Param.iPass = 0;// A and B OK
                    Param.iFail = 0;// A or B Fail
                    Param.iFail_Pass = 0;// A or B Fail 强抛 
                    
                    iTotalScan = 0;  //条码计数清零
                }
                Delay(1000);
            }
        }



        #endregion


        #endregion


        #region 复选框

        private void comboMBType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Param.MBType = this.comboMBType.Text.Trim();
            IniFile.IniWriteValue("SysConfig", "MBType", Param.MBType, Param.IniFilePath);
            checkMBType(Param.MBType);
        }

        private void comboTestingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Param.TestingType = this.comboTestingType.Text.Trim();
            IniFile.IniWriteValue("SysConfig", "TestingType", Param.TestingType, @Param.IniFilePath);
        }

        private void comboRobotModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            Param.RobotModule = comboRobotModule.SelectedItem.ToString();
            IniFile.IniWriteValue("SysConfig", "RobotModule", Param.RobotModule, Param.IniFilePath);
            setFixtureID(comboPCBLine, comboRobotModule, comboFictStage, txtFixtureID);
            lblFICTStage.Text = Param.FixtrueID;
        }

        /// <summary>
        /// auto set fixtureid 
        /// </summary>
        /// <param name="pcbline">combox pcbline</param>
        /// <param name="robotmodule">combox robotmodule</param>
        /// <param name="fictstage">combox fictstage</param>
        /// <param name="fixtureid">textbox fixtureid</param>
        private void setFixtureID(ComboBox pcbline, ComboBox robotmodule, ComboBox fictstage, TextBox fixtureid)
        {
            if (string.IsNullOrEmpty(pcbline.Text.Trim()))
                return;
            if (string.IsNullOrEmpty(robotmodule.Text.Trim()))
                return;
            if (string.IsNullOrEmpty(fictstage.Text.Trim()))
                return;

            Param.FixtrueID = pcbline.Text.ToUpper() + "-" + robotmodule.Text + "-" + fictstage.Text.ToUpper();
            fixtureid.Text = Param.FixtrueID;
            IniFile.IniWriteValue("SysConfig", "FixtureID", Param.FixtrueID, Param.IniFilePath);
        }
        private void comboFictStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Param.FICTStage = comboFictStage.SelectedItem.ToString();
            IniFile.IniWriteValue("SysConfig", "FICTStage", Param.FICTStage, Param.IniFilePath);
            setFixtureID(comboPCBLine, comboRobotModule, comboFictStage, txtFixtureID);
            loadDataBaseAddress(Param.FICTStage);
            lblFICTStage.Text = Param.FixtrueID;
        }

        private void comboPCBLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            Param.PCBLine = comboPCBLine.SelectedItem.ToString();
            IniFile.IniWriteValue("SysConfig", "PCBLine", Param.PCBLine, Param.IniFilePath);
            setFixtureID(comboPCBLine, comboRobotModule, comboFictStage, txtFixtureID);
            lblFICTStage.Text = Param.FixtrueID;
        }

        private void comboBarcodeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Param.BarcodeType = this.comboBarcodeType.Text.Trim();
            IniFile.IniWriteValue("SysConfig", "BarcodeType", Param.BarcodeType, @Param.IniFilePath);
        }

        private void comboScannerPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboScannerPort.Enabled)
            {
                Param.Scanner = this.comboScannerPort.SelectedItem.ToString();
                IniFile.IniWriteValue("ComPort_Set", "Scanner", Param.Scanner, @Param.IniFilePath);
            }
        }

        private void comboDUTPort_A_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboDUTPort_A.Enabled)    //add by channing Wang
            {
                Param.DUT_A = this.comboDUTPort_A.SelectedItem.ToString();
                IniFile.IniWriteValue("ComPort_Set", "DUT_A", Param.DUT_A, @Param.IniFilePath);
            }
        }


        private void comboFICTPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboFICTPort.Enabled)
            {
                Param.PLC = this.comboFICTPort.SelectedItem.ToString();
                IniFile.IniWriteValue("ComPort_Set", "PLC", Param.PLC, @Param.IniFilePath);
            }
        }

        private void comboDUTPort_B_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboDUTPort_B.Enabled)
            {
                Param.DUT_B = this.comboDUTPort_B.SelectedItem.ToString();
                IniFile.IniWriteValue("ComPort_Set", "DUT_B", Param.DUT_B, @Param.IniFilePath);
            }
        }


        #endregion

        #region  All Button

        private void btnRefresh_Click(object sender, EventArgs e)
        {

            //加载串口
            if (Param.Scanner_Use)
            {
                getSerialPort(comboScannerPort);
            }
            if (Param.DUT_A_Use)
            {
                getSerialPort(comboDUTPort_A);
            }
            if (Param.DUT_B_Use)
            {
                getSerialPort(comboDUTPort_B);
            }
            if (Param.PLC_Use)
            {
                getSerialPort(comboFICTPort);
            }    
            
        }
        private void btnDebug_Click(object sender, EventArgs e)
        {
           // getDynamicData(Param.Web_Site, "CN0CD08X7620665A0CPVA00", "A");
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <调试> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <调试> 按钮");
            if (this.Height == 580)
                this.Height = 680;
            else
                this.Height = 580;
        }


        /// <summary>
        /// 检测value是不是空值或者null，是为false，不是为true
        /// </summary>
        /// <param name="value">需要检测的value</param>
        /// <param name="message">附帶消息</param>
        /// <returns>空=false，非空=true</returns>
        private bool checkValueCannotEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                SubFunction.updateMessage(lstStatusCommand, message + " can't be empty.");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), message + " can't be empty.");
                return false;
            }
            return true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <开始> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <开始> 按钮");
            settime();//更新系统时间

            Update_FixtrueID();//更新记录数据库治具、IP信息

            //check empty value
            if (!checkValueCannotEmpty(this.txtOPID.Text.Trim(), "OPID"))
                return;
            if (!checkValueCannotEmpty(this.txtFixtureID.Text.Trim(), "FixtureID"))
                return;

            //check web service
            if (Param.Web_Use)
            {
                if (!checkWebService(Param.Web_Site))
                    return;
            }

            //check comport
            if (Param.Scanner_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "check scanner,ready to open scanner");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "check scanner,ready to open scanner");
                if (!string.IsNullOrEmpty(Param.Scanner))
                {
                    if (!openSerialPort(spScanner, Param.Scanner))
                        return;
                }
            }


            //MB_A 共同的
            if (Param.DUT_A_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "check DUT_A,ready to open DUT_A");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "check DUT_A,ready to open DUT_A");
                if (!string.IsNullOrEmpty(Param.DUT_A))
                {
                    if (!openSerialPort(spDUT_A, Param.DUT_A))
                        return;
                }
            }

            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //双板需要判断MB_B
                if (Param.DUT_B_Use)
                {
                    SubFunction.updateMessage(lstStatusCommand, "check DUT_B,ready to open DUT_B");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "check DUT_B,ready to open DUT_B");
                    if (!string.IsNullOrEmpty(Param.DUT_B))
                    {
                        if (!openSerialPort(spDUT_B, Param.DUT_B))
                            return;
                    }
                }
            }

            //mx 连接
            if (Param.PLC_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "check PLC,ready to connect PLC");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "check PLC,ready to connect PLC");
                if (!openPLC(Param.PLC))
                    return;
                //timerMonitor.Start();
            }

            //
            //判断测试方式
            if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
            {
                timerScanDatabase.Enabled = true; //打开连接数据库
                timerScanDatabase.Start();
            }

            PressStartButtonUI();
            choseDoubleSigle();  //单板双板选择初始化
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <停止> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <停止> 按钮");
            //check comport
            if (Param.Scanner_Use)
                closeSerialPort(spScanner);
            //MB_A 共同的
            if (Param.DUT_A_Use)
                closeSerialPort(spDUT_A);
            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //双板需要判断MB_B
                if (Param.DUT_B_Use)
                    closeSerialPort(spDUT_B);
            }

            //mx 连接
            if (Param.PLC_Use)
                closePLC();

            //判断测试方式
            if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                timerScanDatabase.Stop();

            timerDetect_V_A.Stop();
            timerDetect_V_B.Stop();
            timerScanFICT.Stop();           
            PressStopButtonUI();
            
            loadData2UI();

        }

        private void btnResetDatabase_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <重置数据库> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <重置数据库> 按钮");
            writeDataBase(dataBase_WriteAddress, "10");
        }

        private void btnResetFICT_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <重置FICT> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <重置FICT> 按钮");
            sendPLC("RE");
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 重启 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 重启 > 按钮");
            Application.Restart();
        }


        private void btn10_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“10”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“10”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "10");
        }

        private void btn11_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“11”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“11”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "11");
        }

        private void btn12_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“12”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“12”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "12");
        }

        private void btn13_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“13”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“13”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "13");
        }

        private void btn15_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“15”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“15”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "15");
        }

        private void btn98_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“98”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“98”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "98");
        }

        private void btn14_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 向中控数据库写入“14”信号 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击< 向中控数据库写入“14”信号 > 按钮");
            writeDataBase(dataBase_WriteAddress, "14");
        }


        private void btnIN_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击< 进板 > 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击进板< 进板 >按钮");
            sendPLC("IN");
        }

        private void btnUP_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <上升> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <上升> 按钮");
            ShutDownCount_A = 0;
            ShutDownCount_B = 0;
            sendPLC("UP");
        }

        private void btnLC_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <插左侧插> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <插左侧插> 按钮");
            sendPLC("LC");
        }

        private void btnRC_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <插右侧插> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <插右侧插> 按钮");
            sendPLC("RC");
        }

        private void btnOU_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <退出> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <退出> 按钮");
            sendPLC("OU");
            DUT_Retest_Flag = false;
            DUT_A_Retest_Flag = false;
            DUT_B_Retest_Flag = false;
            iCurrent_TestCount = 1;
        }

        private void btnDO_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <下降> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <下降> 按钮");
            sendPLC("DO");
        }

        private void btnOL_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <退左侧插> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <退左侧插> 按钮");
            sendPLC("OL");
        }

        private void btnOR_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <退右侧插> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <退右侧插> 按钮");
            sendPLC("OR");
        }

        private void btnMB_A_AA_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <A板通19V> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <A板通19V> 按钮");
            sendPLC("AA");
            _b_A_send_aa = false;
        }

        private void btnMB_A_O1_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <A板开机> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <A板开机> 按钮");
            sendPLC("O1");
        }

        private void btnMB_A_TA_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <A板检测电压> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <A板检测电压> 按钮");
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
            txtMB_A_V.Text = mbV.ToString();
        }

        private void btnMB_A_aaL_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <A板断开19V> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <A板断开19V> 按钮");
            sendPLC("aa");
            _b_A_send_aa = true;
        }

        private void btnMB_A_S1_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <A板关机> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <A板关机> 按钮");
            sendPLC("S1");
            if (Param.ShutDown > 0)
                ShutDownCount_A = Param.ShutDown;

            string plcvalue = string.Empty;
            receivePLC(ref plcvalue, true);

        }

        private void btnMB_A_XA_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <A板查询电压> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <A板查询电压> 按钮");
            sendPLC("XA");

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
            txtMB_A_V.Text = mbV.ToString();
        }

        private void btnMB_B_AB_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <B板19V电压> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <B板19V电压> 按钮");
            sendPLC("AB");
            _b_B_send_ab = false;
        }

        private void btnMB_B_O2_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <B板开机> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <B板开机> 按钮");
            sendPLC("O2");
        }

        private void btnMB_B_TB_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <B板检测电压> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <B板检测电压> 按钮");
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
            txtMB_B_V.Text = mbV.ToString();

        }

        private void btnMB_B_abL_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <断开B板19V> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <断开B板19V> 按钮");
            sendPLC("ab");
            _b_B_send_ab = true;
        }

        private void btnMB_B_S2_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <B板关机> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <B板关机> 按钮");
            sendPLC("S2");
            if (Param.ShutDown > 0)
                ShutDownCount_B = Param.ShutDown;
        }

        private void btnMB_B_XB_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <B板查询电压> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <B板查询电压> 按钮");
            sendPLC("XB");

            string plcvalue = string.Empty;
            receivePLC(ref plcvalue, true);

            int plcread = 0;
            readPLC("D101", ref plcread);
            double  mbV = 0;
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
            txtMB_B_V.Text = mbV.ToString();
        }

        private void btnOKBBBSwitch_Click(object sender, EventArgs e)
        {
            if (!btnStart.Enabled)
                return;

            if (Param.OKBBBStatus)
            {
                SubFunction.updateMessage(lstStatusCommand, "手动点击 <关闭OK BBB上抛> 按钮");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <关闭OK BBB上抛> 按钮");
                Param.OKBBBStatus = false;
                IniFile.IniWriteValue("SFCS_Set", "OKBBBStatus", "0", @Param.IniFilePath);
                checkBool2Button(Param.OKBBBStatus, this.btnOKBBBSwitch);
                return;
            }

            if (!Param.OKBBBStatus)
            {
                SubFunction.updateMessage(lstStatusCommand, "手动点击 <开启OK BBB上抛> 按钮");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <开启OK BBB上抛> 按钮");
                Param.OKBBBStatus = true;
                IniFile.IniWriteValue("SFCS_Set", "OKBBBStatus", "1", @Param.IniFilePath);
                checkBool2Button(Param.OKBBBStatus, this.btnOKBBBSwitch);
                return;
            }
        }

        private void btnNGBBBSwitch_Click(object sender, EventArgs e)
        {
            if (!btnStart.Enabled)
                return;
            if (Param.NGBBBStatus)
            {
                SubFunction.updateMessage(lstStatusCommand, "手动点击 <关闭NG BBB上抛> 按钮");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <关闭NG BBB上抛> 按钮");
                Param.NGBBBStatus = false;
                IniFile.IniWriteValue("SFCS_Set", "NGBBBStatus", "0", @Param.IniFilePath);
                checkBool2Button(Param.NGBBBStatus, this.btnNGBBBSwitch);
                return;
            }
            if (!Param.NGBBBStatus)
            {
                SubFunction.updateMessage(lstStatusCommand, "手动点击 <开启NG BBB上抛> 按钮");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <开启NG BBB上抛> 按钮");
                Param.NGBBBStatus = true;
                IniFile.IniWriteValue("SFCS_Set", "NGBBBStatus", "1", @Param.IniFilePath);
                checkBool2Button(Param.NGBBBStatus, this.btnNGBBBSwitch);
                return;
            }
        }

        private void btnArmsSwith_Click(object sender, EventArgs e)
        {
            if (!btnStart.Enabled)
                return;
            if (Param.Arms_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "手动点击 <关闭Arms上抛> 按钮");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <关闭Arms上抛> 按钮");
                Param.Arms_Use = false;
                IniFile.IniWriteValue("SFCS_Set", "Arms_Use", "0", @Param.IniFilePath);
                checkBool2Button(Param.Arms_Use, this.btnArmsSwith);
                return;
            }
            if (!Param.Arms_Use)
            {
                SubFunction.updateMessage(lstStatusCommand, "手动点击 <开启Arms上抛> 按钮");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <开启Arms上抛> 按钮");
                Param.Arms_Use = true;
                IniFile.IniWriteValue("SFCS_Set", "Arms_Use", "1", @Param.IniFilePath);
                checkBool2Button(Param.Arms_Use, this.btnArmsSwith);
                return;
            }
        }

        private void btnOtherSetting_Click(object sender, EventArgs e)
        {
            SubFunction.updateMessage(lstStatusCommand, "手动点击 <其他设置> 按钮");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "手动点击 <其他设置> 按钮");
            Form f = new frmOtherSetting();
            f.ShowDialog();
        }

        #endregion

        #region 文本框

        private void txtDataBase_IP_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtCenterIP_DataBase_IP, Param.CenterIP_DataBase_IP))
                return;
            Param.CenterIP_DataBase_IP = this.txtCenterIP_DataBase_IP.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_IP", Param.CenterIP_DataBase_IP, @Param.IniFilePath);
            Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
        }

        private void txtRecordIP_DataBase_IP_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtRecordIP_DataBase_IP, Param.RecordIP_DataBase_IP))
                return;
            Param.RecordIP_DataBase_IP = this.txtRecordIP_DataBase_IP.Text.Trim();
            IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_IP", Param.RecordIP_DataBase_IP, @Param.IniFilePath);
            Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
        }

        private void txtArmsPath_TextChanged(object sender, EventArgs e)
        {
            if (this.txtArmsPath.Enabled)
                return;
            if (!checkInputData(this.txtArmsPath, Param.Arms_Path))
                return;
            Param.Arms_Path = this.txtArmsPath.Text.Trim();
            IniFile.IniWriteValue("SFCS_Set", "Arms_Path", @Param.Arms_Path, @Param.IniFilePath);
        }

        private void txtOPID_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtOPID, Param.OPID))
                return;
            Param.OPID = this.txtOPID.Text.Trim().ToUpper();
            IniFile.IniWriteValue("SFCS_Set", "OPID", Param.OPID, @Param.IniFilePath);
        }

        private void txtFixtureID_TextChanged(object sender, EventArgs e)
        {
            if (!checkInputData(this.txtFixtureID, Param.FixtrueID))
                return;
            Param.FixtrueID = this.txtFixtureID.Text.Trim().ToUpper();
            IniFile.IniWriteValue("SysConfig", "FixtureID", Param.FixtrueID, @Param.IniFilePath);
        }

        #endregion

        #region UI change


        private void PressStartButtonUI()
        {
            //sysconfig
            this.txtCenterIP_DataBase_IP.Enabled = false;
            this.txtRecordIP_DataBase_IP.Enabled = false;
            this.comboTestingType.Enabled = false;
            this.comboRobotModule.Enabled = false;
            this.comboMBType.Enabled = false;
            this.comboFictStage.Enabled = false;
            this.comboPCBLine.Enabled = false;
            this.comboBarcodeType.Enabled = false;
            //serialport
            this.comboScannerPort.Enabled = false;
            this.comboDUTPort_A.Enabled = false;
            this.comboFICTPort.Enabled = false;
            this.comboDUTPort_B.Enabled = false;
            this.btnRefresh.Enabled = false;
            //
            this.txtArmsPath.ReadOnly = true;
            this.btnOKBBBSwitch.Enabled = false;
            this.btnNGBBBSwitch.Enabled = false;
            this.btnArmsSwith.Enabled = false;
            this.btnOtherSetting.Enabled = false;
            this.txtOPID.Enabled = false;
            this.txtFixtureID.Enabled = false;
            //button
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;
            this.btnResetDatabase.Enabled = false;
            this.btnResetFICT.Enabled = false;
            this.btnRestart.Enabled = false;
            //
            txtScanTotal.ReadOnly = true;
            txtBar_A.ReadOnly = true;
            txtBar_B.ReadOnly = true;
            //
            //grbMB_ASFCSInfo.Enabled = false;
            //grbMB_BSFCSInfo.Enabled = false;
            txtMB_A_ArmsVer.ReadOnly = true;
            txtMB_B_ArmsVer.ReadOnly = true;

            if (Param.LeftInsert)
            {
                this.btnLC.Enabled = true;
                this.btnOL.Enabled = true;
            }
            else
            {
                this.btnLC.Enabled = false;
                this.btnOL.Enabled = false;
            }

            if (Param.RightInsert)
            {
                this.btnRC.Enabled = true;
                this.btnOR.Enabled = true;
            }
            else
            {
                this.btnRC.Enabled = false;
                this.btnOR.Enabled = false;
            }



        }

        private void PressStopButtonUI()
        {
            //sysconfig
            this.txtCenterIP_DataBase_IP.Enabled = true;
            this.txtRecordIP_DataBase_IP.Enabled = true;
            this.comboTestingType.Enabled = true;
            this.comboRobotModule.Enabled = true;
            this.comboMBType.Enabled = true;
            this.comboFictStage.Enabled = true;
            this.comboPCBLine.Enabled = true;
            this.comboBarcodeType.Enabled = true;
            //serialport
            this.comboScannerPort.Enabled = true;
            this.comboDUTPort_A.Enabled = true;
            this.comboFICTPort.Enabled = true;
            this.comboDUTPort_B.Enabled = true;
            this.btnRefresh.Enabled = true;
            //
            this.txtArmsPath.ReadOnly = false;
            this.btnOKBBBSwitch.Enabled = true;
            this.btnNGBBBSwitch.Enabled = true;
            this.btnArmsSwith.Enabled = true;
            this.btnOtherSetting.Enabled = true;
            this.txtOPID.Enabled = true;
            this.txtFixtureID.Enabled = true;
            //button
            this.btnStart.Enabled = true;
            this.btnStop.Enabled = false;
            this.btnResetDatabase.Enabled = true;
            this.btnResetFICT.Enabled = true;
            this.btnRestart.Enabled = true;
            //
            txtScanTotal.ReadOnly = false;
            txtBar_A.ReadOnly = false;
            txtBar_B.ReadOnly = false;
            //
            //grbMB_ASFCSInfo.Enabled = true;
            //grbMB_BSFCSInfo.Enabled = true;
            txtMB_A_ArmsVer.ReadOnly = false;
            txtMB_B_ArmsVer.ReadOnly = false;
        }


        #endregion

        #region Arms

        /// <summary>
        /// 从DUT串口发送的数据获取ARMS的相關信息
        /// </summary>
        /// <param name="dutstring">串口发送的ARMS数据</param>
        /// <param name="sysid">SYSID</param>
        /// <param name="armsmodel">ARMSDODEL</param>
        /// <param name="dellmodel">DELLMODEL</param>
        private void getArmsInfoByDUT(string dutstring, ref string sysid, ref string armsmodel, ref string dellmodel)
        {
            string[] tempstr = dutstring.Split('+');
            if (tempstr.Length == 4)
            {
                sysid = tempstr[1];
                armsmodel = tempstr[2];
                if (tempstr[3].Contains("*"))
                    dellmodel = tempstr[3].Replace("*", string.Empty);
            }
        }

        /// <summary>
        /// 从DUT返回的bios信息中读取BIOS Version & SFCMAC
        /// </summary>
        /// <param name="dutstring">串口傳送的数据</param>
        /// <param name="mbflag">MB的标记,A or B</param>
        private void getBIOSInfo(string dutstring, string mbflag)
        {
            string[] tempstr = dutstring.Split('+');

            if (mbflag == "A")
            {
                txtMB_A_BIOS.Text = tempstr[0];
                txtMB_A_BIOS.ForeColor = Color.Green;
                txtMB_A_SFCMAC.Text = tempstr[1];
                txtMB_A_SFCMAC.ForeColor = Color.Green;
            }

            if (mbflag == "B")
            {
                txtMB_B_BIOS.Text = tempstr[0];
                txtMB_B_BIOS.ForeColor = Color.Green;
                txtMB_B_SFCMAC.Text = tempstr[1];
                txtMB_B_SFCMAC.ForeColor = Color.Green;
            }
        }


        #endregion

        private void TestReInitUI()
        {
            this.Invoke((EventHandler)(delegate
            {
                this.txtMB_A_SFCMAC.Text = string.Empty;
                this.txtMB_A_MO.Text = string.Empty;
                this.txtMB_A_BIOS.Text = string.Empty;
                this.txtMB_A_DellPN.Text = string.Empty;
                this.txtMB_A_LogMAC.Text = string.Empty;
                this.txtMB_A_DellMode.Text = string.Empty;
                //
                this.txtMB_B_SFCMAC.Text = string.Empty;
                this.txtMB_B_MO.Text = string.Empty;
                this.txtMB_B_BIOS.Text = string.Empty;
                this.txtMB_B_DellPN.Text = string.Empty;
                this.txtMB_B_LogMAC.Text = string.Empty;
                this.txtMB_B_DellMode.Text = string.Empty;
                //
                this.lstMB_ATestItems.Items.Clear();
                //
                this.lstMB_BTestItems.Items.Clear();
            }));
            //


        }


        #region 测试动作命令块函数
        /// <summary>
        /// 退侧插函数块
        /// </summary>
        private void exit_Left_Right_Insert() // 断电命令"aa" "ab"执行过后，退侧插函数块
        {
            //----------可能会重复发送命令-----
            //2,判断侧插
            //左右都无
            if (!Param.LeftInsert && !Param.RightInsert)
            {
                SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                sendPLC("DO");
            }
            //右侧插，先右后左
            if (Param.RightInsert)
            {
                SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                sendPLC("OR");
            }
            else //右边沒有，判断左边
            {
                SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                sendPLC("OL");
            }
        }


        #endregion

        #region 实时函数块

        private void timerScanDatabase_Tick(object sender, EventArgs e)
        {
            timerScanDatabase.Stop();
            //读数据库的值
            if (!readDataBase(dataBase_ReadAddress, ref dataBase_ReadData, false))
            {
                timerScanDatabase.Start();
                return;
            }

            //已经读到值退出
            if (dataBase_LastRead == dataBase_ReadData)
            {
                timerScanDatabase.Start();
                return;
            }

            //update message,并赋值
            SubFunction.updateMessage(lstStatusCommand, "DataBase_" + dataBase_ReadAddress + "->PC:" + dataBase_ReadData);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DataBase_" + dataBase_ReadAddress + "->PC:" + dataBase_ReadData);
            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DataBase_" + dataBase_ReadAddress + "->PC:" + dataBase_ReadData);
            dataBase_LastRead = dataBase_ReadData;

            //对值进行判断
            switch (dataBase_ReadData)
            {
                case "0":  // 剛开机，Robot PLC的值      
                    break;
                case "10": //准备OK,写Robot PLC为10
                    //給数据库回傳10
                    writeDataBase(dataBase_WriteAddress, dataBase_ReadData);
                    //SubFunction.updateMessage(txtCurrentTime, "PC->DataBase_" + dataBase_WriteAddress + ":" + dataBase_ReadData);
                    //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DataBase_" + dataBase_WriteAddress + ":" + dataBase_ReadData);

                    break;
                case "11":
                    //1,給数据库回写11

                    writeDataBase(dataBase_WriteAddress, dataBase_ReadData);
                    TestReInitUI();
                    //iCurrent_TestCount = 1;
                    icount_DUT_TestingTimeOut = 0;
                    this.txtCurrentTime.Text = icount_DUT_TestingTimeOut.ToString();
                    _b_Left_Insert_Re = false;
                    _b_Right_Insert_Re = false;
                    _b_B_send_ab =false ;
                    _b_A_send_aa = false;
                    i_Detect_B_Count = 0; //重置參数
                    i_Detect_A_Count = 0; //重置參数
                    _b_MB_A_Re = false;
                    _b_MB_B_Re = false;
                    ShutDownCount_A = 0;
                    ShutDownCount_B = 0;
                    DUT_Retest_Flag = false;
                    DUT_A_Retest_Flag = false;
                    DUT_B_Retest_Flag = false;
                    iCurrent_TestCount = 1;
                    dataBaseStatus = Param.dataBaseStatus.OK.ToString();

                    timerDetect_V_A.Stop();
                    timerDetect_V_B.Stop();
                    choseDoubleSigle();  //每次循环测试MB_A MB_B初始化

                   
               
#if DEBUG
                    SubFunction.updateMessage(lstStatusCommand ,"A断电次数:" + ShutDownCount_A);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "A断电次数:" + ShutDownCount_A);
                    SubFunction.updateMessage(lstStatusCommand, "B断电次数:" + ShutDownCount_B);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "B断电次数:" + ShutDownCount_B);
                    SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                    SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
#endif

                    //SubFunction.updateMessage(txtCurrentTime, "PC->DataBase_" + dataBase_WriteAddress + ":" + dataBase_ReadData);
                    //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DataBase_" + dataBase_WriteAddress + ":" + dataBase_ReadData);
                    //2,获取条码(se1,从条码枪获取,se2,从数据库获取)
                    if (Param.Scanner_Use)
                    {
                        //使用机台內部的条码枪

                    }
                    else
                    {
                        //获取条码
                        //单板
                        //if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                        if (Param.MBType.ToString() == Param.mbType.Single .ToString().ToUpper())
                        {
#if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "執行的是单板");
#endif
                            //使用轨道的条码枪，从数据库读取
                            string USN = string.Empty;
                            readDataBase(dataBase_ReadSNAddress, ref USN);
                            SubFunction.updateMessage(lstStatusCommand, "Read Barcode=" + USN);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Read Barcode=" + USN);
                            Param.bar_A = USN;
                            this.txtBar_A.Text = Param.bar_A;
                            this.txtMB_A_DellPN.Text = Param.bar_A.Substring(3, 5);
                            this.txtMB_A_DellPN.ForeColor = Color.Green;
                        }

                        //双板 
                        if (Param.MBType.ToUpper () == Param.mbType.Panel.ToString().ToUpper())
                        {
 #if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "执行的是双板");
#endif
                            //使用轨道条码枪，从数据库中读取
                            string USN = string.Empty;
                            readDataBase(dataBase_ReadSNAddress, ref USN);
                            SubFunction.updateMessage(lstStatusCommand, "Read Barcode=" + USN);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Read Barcode=" + USN);

                            if (Param.BarcodeType == Param.barcodeType.A.ToString()) Param.bar_A = USN;
                            if (Param.BarcodeType == Param.barcodeType.B.ToString()) Param.bar_B = USN;

                            //Channing 20161105
                            Param.Model = string.Empty; //清空
                            Param.ModelFamily = string.Empty;//清空
                            Param.MO = string.Empty;//清空
                            Param.UPN = string.Empty;//清空
                            getMOModelUPN(Param.Web_Site, USN, ref Param.Model,ref  Param.ModelFamily,ref  Param.MO, ref Param.UPN);
                            
                            Update_MB_Info();  //上传MB信息至记录数据库；
                            Param.Test_Log = true;  //TestLog 写入标志位，True：还未记录，Flase：已完成记录

                            //从SFCS获取条码 
                           
                            // if (Param.MBType.ToString() == Param.mbType.Panel.ToString().ToUpper())
                            // {
                            getDynamicData(Param.Web_Site, USN, Param.BarcodeType);
                            //}
                            this.txtBar_A.Text = Param.bar_A;
                            this.txtBar_B.Text = Param.bar_B;

                            //arms info
                            this.txtMB_B_DellPN.Text = Param.bar_B.Substring(3, 5);
                            this.txtMB_B_DellPN.ForeColor = Color.Green;
                        }
                    }

                    SubFunction.updateMessage(lstStatusCommand, "Bar_A:" + Param.bar_A);
                    SubFunction.updateMessage(lstStatusCommand, "Bar_B:" + Param.bar_B);
                    iTotalScan += 1;
                    this.txtScanTotal.Text = iTotalScan.ToString();
                    //arms info
                    this.txtMB_A_DellPN.Text = Param.bar_A.Substring(3, 5);
                    this.txtMB_A_DellPN.ForeColor = Color.Green;

                    // check是否是要check 流程
                    if (Param.CheckRouter)
                    {
                        //单板
                        if (Param.MBType.ToString() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            if (!(checkStage(Param.bar_A, Param.SFC_Stage) && checkStage(Param.bar_B, Param.SFC_Stage)))
                            {
                                //查詢流程錯誤
                                SubFunction.updateMessage(lstStatusCommand, "check stage error,FICT won't test");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "check stage error,FICT won't test");
                                dataBaseStatus = Param.dataBaseStatus.NG.ToString();
                                break;
                            }
                        }
                        else
                        {
                            if (!(checkStage(Param.bar_A, Param.SFC_Stage)))
                            {
                                //查詢流程錯誤
                                SubFunction.updateMessage(lstStatusCommand, "check stage error,FICT won't test");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "check stage error,FICT won't test");
                                dataBaseStatus = Param.dataBaseStatus.NG.ToString();
                                break;
                            }
                        }
                    }
                    //从SFCS获取数据
                    getMOMAC(Param.Web_Site, Param.bar_A, Param.SFC_Stage, ref DUT_A_MO, ref DUT_A_MAC);
                    this.txtMB_A_MO.ForeColor = Color.Green;
                    this.txtMB_A_MO.Text = DUT_A_MO;
                    this.txtMB_A_SFCMAC.ForeColor = Color.Green;
                    this.txtMB_A_SFCMAC.Text = DUT_A_MAC;

                    //双板
                    if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                    {
                        SubFunction.updateMessage(lstBarcodeList , "Bar_A:" + Param.bar_A);
                        SubFunction.updateMessage(lstBarcodeList , "Bar_B:" + Param.bar_B);

                        //从SFCS获取数据
                        getMOMAC(Param.Web_Site, Param.bar_B, Param.SFC_Stage, ref DUT_B_MO, ref DUT_B_MAC);
                        this.txtMB_B_MO.ForeColor = Color.Green;
                        this.txtMB_B_MO.Text = DUT_B_MO;
                        this.txtMB_B_SFCMAC.ForeColor = Color.Green;
                        this.txtMB_B_SFCMAC.Text = DUT_B_MAC;
                    }

                    //单板
                    if (Param.MBType.ToUpper() == Param.mbType.Single.ToString().ToUpper())
                    {
                        SubFunction.updateMessage(lstBarcodeList , "Bar_A:" + Param.bar_A);
                        //SubFunction.updateMessage(lstBarcode, "Bar_B:" + Param.bar_B);
                    }
                    
                    break;
                case "12": //接驳台开始转动,WCD不需要做动作，当接驳台将载板送入FICT時，FICT的PLC会自动发送yy,当载板进入到位后，改写值，讓接驳台停止转动

#if DEBUG
                    SubFunction.updateMessage(lstStatusCommand, "Database Status:" + dataBaseStatus);
#endif
                    
                    if (dataBaseStatus == Param.dataBaseStatus.NG.ToString())
                    {
                        //直接回写，并退出
                        writeDataBase(dataBase_WriteAddress, dataBase_ReadData);
                        break;
                    }
                    //
                    //屏蔽YY，使用軟件发出
                    Delay(2000);
                    sendPLC("IN");

                    if (Param.PLC_Use)
                    {
                        timerScanFICT.Start();
                    }
                    dataBase_LastRead = dataBase_ReadData;
                    break;

                case "13": //'送板完成接驳台链条停止

                    if (dataBaseStatus == Param.dataBaseStatus.NG.ToString())
                    {
                        //直接回写，并退出
                        writeDataBase(dataBase_WriteAddress, dataBase_ReadData);
                        break;
                    }

                    break;

                case "14": //接驳台链条運轉开始接收F/T载板 

                    if (dataBaseStatus == Param.dataBaseStatus.NG.ToString())
                    {
                        //直接回写，并退出
                        writeDataBase(dataBase_WriteAddress, dataBase_ReadData);   
                        break;
                    }

                    break;

                case "15": //接收F/T载板完成，接驳台链条停止,wks 写15，FICT链条停止转动，WCD不需要动作
                    writeDataBase(dataBase_WriteAddress, dataBase_ReadData);
                    break;
                case "98":
                    //update message,并赋值
                    SubFunction.updateMessage(lstStatusCommand, "Robot PLC BY PASS,Address:" + dataBase_ReadAddress + "->" + dataBase_ReadData);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Robot PLC BY PASS,Address:" + dataBase_ReadAddress + "->" + dataBase_ReadData);
                    //
                    string readPC = "-1";
                    readDataBase(dataBase_ReadAddress, ref readPC);
                    switch (readPC)
                    {
                        default:
                            writeDataBase(dataBase_WriteAddress, dataBase_ReadData);
                            //SubFunction.updateMessage(lstStatusCommand, "PC->DataBase_" + dataBase_WriteAddress + ":" + dataBase_ReadData);
                            //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DataBase_" + dataBase_WriteAddress + ":" + dataBase_ReadData);
                            break;
                        case "0":
                            break;
                        case "-1":
                            break;
                    }
                    break;
            }

            timerScanDatabase.Start();
        }

        private void timerScanFICT_Tick(object sender, EventArgs e)
        {
            //
            string FICT_PLC_Read_Value = string.Empty;
          
            //

            receivePLC(ref FICT_ReadValue, false);

            if (FICT_Last_ReadValue == FICT_ReadValue) //已经读到了
            {
                timerScanFICT.Start();
                return;
            }
            //还沒读到，赋值給上一次的值
            SubFunction.updateMessage(lstStatusCommand, "FICT->PC:" + FICT_ReadValue);
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FICT->PC:" + FICT_ReadValue);
            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "FICT->PC:" + FICT_ReadValue);
            FICT_Last_ReadValue = FICT_ReadValue;
            //根据值做相对应的动作
            //switch (FICT_PLC_Read_Value )
           switch (FICT_ReadValue)
            {
                //机台上升
                case "yy":

                    if (Param.TestingType.ToUpper() == Param.testingType.Manual.ToString().ToUpper())
                    {
                        sendPLC("IN");
                        SubFunction.updateMessage(lstStatusCommand, "载板即将开始进入");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "载板即将开始进入");
                        //如果需要使用条码枪,打开条码枪
                        if (Param.Scanner_Use)
                        {
                            sendData(spScanner, "LON\r\n");
                            SubFunction.updateMessage(lstStatusCommand, "打开条码枪");
                            SubFunction.updateMessage(lstStatusCommand, "PC->Scanner:LON");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "打开条码枪");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->Scanner:LON");
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->Scanner:LON");
                        }
                    }
                    break;

                case "in": //载板进入到位,触发载板到位Sensor
                    //判断条码是否刷成功,无论是单还是双，检测A条码即可  
                
                    if (string.IsNullOrEmpty(Param.bar_A))
                    {
                        //条码刷取不成功
                        //need complete
                        SubFunction.updateMessage(lstStatusCommand, "载板进入到位，但条码为空，治具将退出");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "载板进入到位，但条码为空，治具将退出");
                        //1.写12給数据库
                        if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                        {
                            writeDataBase(dataBase_WriteAddress, "12");
                            dataBaseStatus = Param.dataBaseStatus.NG.ToString(); //数据库异常标记
                            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                            {           
                                //CNG12345678901234567890
                                lblTestResult_A.Text = "FAIL";
                                lblTestResult_B.Text = "FAIL";
                                SubFunction.SaveTestLog("00---" + "Bar_A-" + "***********************", "NO", false, false, "MB Not Read PPID", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                TestLog_for_Record_DataBase("00---" + "Bar_A-" + "***********************", "NO", false, false, "MB Not Read PPID");
                                SubFunction.SaveTestLog("01---" + "Bar_B-" + "***********************", "NO", false, false, "MB Not Read PPID", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                TestLog_for_Record_DataBase("01---" + "Bar_B-" + "***********************", "NO", false, false, "MB Not Read PPID");
                            }
                            else
                            {
                                lblTestResult_A.Text = "FAIL";
                                SubFunction.SaveTestLog("00---" + "Bar_A-" + "***********************", "NO", false, false, "MB Not Read PPID", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                TestLog_for_Record_DataBase("00---" + "Bar_A-" + "***********************", "NO", false, false, "MB Not Read PPID");
                            }
                        }
                        //2.OU給FICT，讓治具退出
                        sendPLC("OU");

                    }
                    else
                    {
                        //条码刷取成功
                        SubFunction.updateMessage(lstStatusCommand, "载板进入到位，治具即将开始上升");
                        //1.写12給数据库
                        if (writeDataBase(dataBase_WriteAddress, "12"))
                        {
                            //2.发UP給FICT
                            sendPLC("UP");
                        }
                    }
                    //FICT_Last_ReadValue = FICT_PLC_Read_Value;
                    break;

                case "up": //载板上升到位，判断侧插，通电，开始检测电压

                    //判断侧插，插入時先左后右，退出時先右后左
                    //1无侧插
                    if (!Param.LeftInsert && !Param.RightInsert)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "治具上升到位，无左右侧插，准备开始通电");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "治具上升到位，无左右侧插，准备开始通电");
                        Delay(5000);

                        //判断测试MB的類型
                        //1 双连板
                        if (Param.MBType == Param.mbType.Panel.ToString())
                        {
                            SubFunction.updateMessage(lstStatusCommand, "检测到为双连板，准备开始为MB_A通电");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到为双连板，准备开始为MB_A通电");
                            sendPLC("AA");
                            lblTestResult_A.ForeColor  = Color.Yellow;
                            lblTestResult_A.Text = "TEST";                            
                            timerDetect_V_A.Start();
                            SubFunction.updateMessage(lstStatusCommand, "准备开始为MB_B通电");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始为MB_B通电");
                            Delay(5000);//5s通电
                            sendPLC("AB");
                            lblTestResult_B.ForeColor = Color.Yellow;
                            lblTestResult_B.Text = "TEST";      
                            timerDetect_V_B.Start();
                            timerDetectTimeOut.Start();
#if DEBUG
                            SubFunction.updateMessage( lstStatusCommand,"Detect_A:" + timerDetect_V_A.Enabled);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Detect_A:" + timerDetect_V_A.Enabled);
                            SubFunction.updateMessage(lstStatusCommand ,"Detect_B:" + timerDetect_V_B.Enabled);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Detect_B:" + timerDetect_V_B.Enabled);
#endif
 
                            //
                            //判断是否使用RTC电池
                            if (Param.RTC_Use)
                            {
                                //使用了RTC电池，需要按下开机按鈕
                                Delay(2000);
                                sendPLC("O1");
                                SubFunction.updateMessage(lstStatusCommand, "MB_A Press Power Button");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A Press Power Button");
                                sendPLC("O2");
                                SubFunction.updateMessage(lstStatusCommand, "MB_B Press Power Button");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B Press Power Button");
                            }
                            //准备开始检测电压
                            if (Param.UseCommand)
                            {
                                //准备开始检测电压
                                Delay(Param.DetectDelay * 1000);
                                SubFunction.updateMessage(lstStatusCommand, "双连板，准备开始检测MB_A开机电压");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始检测MB_A开机电压");
                                sendPLC("TA");
                                SubFunction.updateMessage(lstStatusCommand, "准备开始检测MB_A开机电压");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始检测MB_A开机电压");
                                sendPLC("TB");
                            }       

                        } 
                    }
                    //2有左边侧插
                    if (Param.LeftInsert)
                    {
                        Delay(Param.InsertDelay * 1000);
                        sendPLC("LC");
                        SubFunction.updateMessage(lstStatusCommand, "左侧插插入");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插插入");            
                    }
                    else
                    {
                        //沒左边的，判断是右右边的
                        if (Param.RightInsert)
                        {
                            Delay(Param.InsertDelay * 1000);
                            sendPLC("RC");
                            SubFunction.updateMessage(lstStatusCommand, "右侧插插入");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插插入");
                        }
                    }
                    break;

                case "nb"://未检测到板子，载板退出
                    SubFunction.updateMessage(lstStatusCommand, "治具上升過程中未检测到MB,By PASS");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "治具上升過程中未检测到MB,By PASS");
                    if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                    {
                        writeDataBase(dataBase_WriteAddress, "98");;
                    }
                    //sendPLC("DO");
                    //重置重测標誌
                    DUT_A_Retest_Flag = false;
                    DUT_B_Retest_Flag = false;
                    lblTestResult_A.ForeColor = Color.Red;
                    lblTestResult_A.Text = "FAIL";

                   
                    //need wait  
                    //单、双區別
                    if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                    {
                        MB_A_Status = Param.mbStatus.FAIL.ToString();
                        MB_B_Status = Param.mbStatus.FAIL.ToString();

                        //SubFunction.saveLog("Bar_B-" + Param.bar_B, "NO", false, false, "MB Not IN while UP");
                        SubFunction.SaveTestLog("16---" + "Bar_B-" + Param.bar_B, "NO", false, false, "MB Not IN while UP", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                        TestLog_for_Record_DataBase("16---" + "Bar_B-" + Param.bar_B, "NO", false, false, "MB Not IN while UP");
                        Param.iFail_B += 1;
                        Param.iFail += 1;
                        lblTestResult_B.ForeColor = Color.Red;
                        lblTestResult_B.Text = "FAIL";
                    }
                    if (Param.MBType.ToUpper() == Param.mbType.Single.ToString().ToUpper())
                    {
                        MB_A_Status = Param.mbStatus.FAIL.ToString();
                        Param.iFail_A += 1;
                    }
                    //SubFunction.saveLog("Bar_A-" + Param.bar_A, "NO", false, false, "MB Not IN while UP");
                    SubFunction.SaveTestLog("17---" + "Bar_A-" + Param.bar_A, "NO", false, false, "MB Not IN while UP", Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                    TestLog_for_Record_DataBase("17---" + "Bar_A-" + Param.bar_A, "NO", false, false, "MB Not IN while UP");
     
                    break;

                case "nl"://左侧插插入超時，机台停止
                    SubFunction.updateMessage(lstStatusCommand, "左侧插插入超時，PLC 停止，請检查机台");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插插入超時，PLC 停止，請检查机台");
                    sendPLC("OL");
                    writeDataBase(dataBase_WriteAddress, "98");
                    break;

                case "lc": //左侧插插入成功，繼續判断侧插，通电，检测电压

                    SubFunction.updateMessage(lstStatusCommand, "左侧插插入成功");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插插入成功");
                    
                   //判断是否需要重复插左侧插，如果需要退出，如果不需要，繼續下一步
                    if (Param.LeftInsertRe && !_b_Left_Insert_Re ) //左侧插需要重复插,並且还沒有插
                    {
                        SubFunction.updateMessage(lstStatusCommand, "左侧插需要重复插，退出，再次插入");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插需要重复插，退出，再次插入");
                        Delay(1000);
                        sendPLC("OL");
                    }
                    else
                    {
                        if (Param.RightInsert) //判断是否有右侧插
                        {
                            Delay(Param.InsertDelay * 1000);
                            sendPLC("RC");
                            SubFunction.updateMessage(lstStatusCommand, "右侧插插入");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插插入");
                        }
                        else
                        {
                            //沒有右侧插,通电
                            Delay(5000);
                            //判断测试MB的類型
                            //1 双连板
                            if (Param.MBType == Param.mbType.Panel.ToString())
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到为双连板，准备开始为MB_A通电");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到为双连板，准备开始为MB_A通电");
                                sendPLC("AA");
                                lblTestResult_A.ForeColor  = Color.Yellow;
                                lblTestResult_A.Text = "TEST";
                                timerDetect_V_A.Start();
                                SubFunction.updateMessage(lstStatusCommand, "准备开始为MB_B通电");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始为MB_B通电");
                                Delay(5000); //5s
                                sendPLC("AB");
                                lblTestResult_B.ForeColor = Color.Yellow;
                                lblTestResult_B.Text = "TEST";
                                timerDetect_V_B.Start();
                                timerDetectTimeOut.Start();
#if DEBUG
                                SubFunction.updateMessage(lstStatusCommand, "Detect_A:" + timerDetect_V_A.Enabled);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Detect_A:" + timerDetect_V_A.Enabled);
                                SubFunction.updateMessage(lstStatusCommand, "Detect_B:" + timerDetect_V_B.Enabled);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Detect_B:" + timerDetect_V_B.Enabled);
#endif

                                //
                                //判断是否使用RTC电池
                                if (Param.RTC_Use)
                                {
                                    //使用了RTC电池，需要按下开机按鈕
                                    Delay(2000);
                                    sendPLC("O1");
                                    SubFunction.updateMessage(lstStatusCommand, "MB_A Press Power Button");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A Press Power Button");
                                    sendPLC("O2");
                                    SubFunction.updateMessage(lstStatusCommand, "MB_B Press Power Button");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B Press Power Button");
                                }
                                if (Param.UseCommand)
                                {
                                    //准备开始检测电压
                                    Delay(Param.DetectDelay);
                                    SubFunction.updateMessage(lstStatusCommand, "双聯板，准备开始检测MB_A开机电压");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板，准备开始检测MB_A开机电压");
                                    sendPLC("TA");

                                    SubFunction.updateMessage(lstStatusCommand, "准备开始检测MB_B开机电压");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始检测MB_B开机电压");
                                    sendPLC("TB");
                                }


                            }
                        }
                    }

                    break;

                case "ol": //左侧插退出成功(退测查先右后左，左侧插退出为最後一個),治具下降

                    SubFunction.updateMessage(lstStatusCommand, "左侧插退出成功");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插退出成功");
                    //判断是否需要重复插左侧插，如果需要退出，如果不需要，繼續下一步
                    if (Param.LeftInsertRe && !_b_Left_Insert_Re) //左侧插需要重复插,並且还沒有插
                    {
                        SubFunction.updateMessage(lstStatusCommand, "左侧插需要重复插，第二次插入");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插需要重复插，第二次插入");
                        Delay(Param.InsertDelay * 1000);
                        sendPLC("LC");
                        _b_Left_Insert_Re = true;
                    }
                    else //不需要
                    {
                        Delay(1000);
                        sendPLC("DO");
                        SubFunction.updateMessage(lstStatusCommand, "治具开始下降");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "治具开始下降");

                    }                    

                    break;

                case "nr": //右侧插插入超時
                    SubFunction.updateMessage(lstStatusCommand, "右侧插插入超時，PLC 停止，請检查机台");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插插入超時，PLC 停止，請检查机台");
                    sendPLC("OR");
                    writeDataBase(dataBase_WriteAddress, "98");
                    break;

                case "rc": //右侧插插入成功,准备开始通电
                    SubFunction.updateMessage(lstStatusCommand, "右侧插插入成功");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插插入成功");

                    //判断是否需要重复插右侧插，如果需要退出，如果不需要，繼續下一步
                    if (Param.RightInserRe  && !_b_Right_Insert_Re) //左侧插需要重复插,並且还沒有插
                    {
                        SubFunction.updateMessage(lstStatusCommand, "右侧插需要重复插，退出，再次插入");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插需要重复插，退出，再次插入");
                        Delay(1000);
                        sendPLC("OR");
                    }
                    else
                    {
                        Delay(5000);
                        //判断测试MB的類型
                        //1 双连板
                        if (Param.MBType == Param.mbType.Panel.ToString())
                        {
                            SubFunction.updateMessage(lstStatusCommand, "检测到为双连板，准备开始为MB_A通电");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到为双连板，准备开始为MB_A通电");
                            sendPLC("AA");
                            lblTestResult_A.ForeColor = Color.Yellow;
                            lblTestResult_A.Text = "TEST";
                            timerDetect_V_A.Start();
                            SubFunction.updateMessage(lstStatusCommand, "准备开始为MB_B通电");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始为MB_B通电");
                            Delay(5000);//5s
                            sendPLC("AB");
                            lblTestResult_B.ForeColor = Color.Yellow;
                            lblTestResult_B.Text = "TEST";
                            timerDetect_V_B.Start();
                            timerDetectTimeOut.Start();
#if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "Detect_A:" + timerDetect_V_A.Enabled);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Detect_A:" + timerDetect_V_A.Enabled);
                            SubFunction.updateMessage(lstStatusCommand, "Detect_B:" + timerDetect_V_B.Enabled);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Detect_B:" + timerDetect_V_B.Enabled);
#endif

                            //
                            //判断是否使用RTC电池
                            if (Param.RTC_Use)
                            {
                                //使用了RTC电池，需要按下开机按鈕
                                Delay(2000);
                                sendPLC("O1");
                                SubFunction.updateMessage(lstStatusCommand, "MB_A Press Power Button");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A Press Power Button");
                                sendPLC("O2");
                                SubFunction.updateMessage(lstStatusCommand, "MB_B Press Power Button");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B Press Power Button");
                            }

                            if (Param.UseCommand)
                            {
                                //准备开始检测电压
                                Delay(Param.DetectDelay * 1000);
                                SubFunction.updateMessage(lstStatusCommand, "双连板，准备开始检测MB_A开机电压");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始检测MB_A开机电压");
                                sendPLC("TA");
                                SubFunction.updateMessage(lstStatusCommand, "准备开始检测MB_A开机电压");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "准备开始检测MB_A开机电压");
                                sendPLC("TB");
                            }
                        }
                    }

                    break;

                case "or"://右侧插退出成功

                    SubFunction.updateMessage(lstStatusCommand, "右侧插退出成功");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插退出成功");

                    //判断是否需要重复插右侧插，如果需要退出，如果不需要，繼續下一步
                    if (Param.RightInserRe && !_b_Right_Insert_Re) //左侧插需要重复插,並且还沒有插
                    {
                        SubFunction.updateMessage(lstStatusCommand, "右侧插需要重复插，第二次插入");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插需要重复插，第二次插入");
                        Delay(Param.InsertDelay * 1000);
                        sendPLC("RC");
                        _b_Right_Insert_Re = true;
                    }
                    else
                    {
                        if (Param.LeftInsert)
                        {
                            //左侧插存在，退左边的
                            SubFunction.updateMessage(lstStatusCommand, "左侧插存在，准备退出左侧插");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，准备退出左侧插");
                            sendPLC("OL");
                        }
                        else
                        {
                            //左侧插不存在，治具下降
                            SubFunction.updateMessage(lstStatusCommand, "左侧插不存在，治具准备开始下降");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插不存在，治具准备开始下降");
                            sendPLC("DO");
                        }
                    }
                    break;

                case "ay":
                    if (Param.UseCommand)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "MB_A检测到开机电压成功，实际电压:" + txtMB_A_V.Text + "(V)");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A检测到开机电压成功，实际电压:" + txtMB_A_V.Text + "(V)");
                    }
                    break;

                case "an":
                    if (Param.UseCommand)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "MB_A未检测到开机电压");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A未检测到开机电压");
                        //双板子
                        if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            //1,断电
                            Console.WriteLine("双连板MB_A断19V");
                            SubFunction.updateMessage(lstStatusCommand, "双连板,MB_A断开19V");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板,MB_A断开19V");
                            sendPLC("aa");
                            _b_A_send_aa = true;
#if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                            SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
#endif
                            if (_b_A_send_aa && _b_B_send_ab) //都发送了断电命令
                            {
                                _b_A_send_aa = false;
                                //----------可能会重复发送命令-----
                                //2,判断侧插
                                //左右都无
                                if (!Param.LeftInsert && !Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                    sendPLC("DO");
                                }
                                //右侧插，先右后左
                                if (Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                    sendPLC("OR");
                                }
                                else //右边沒有，判断左边
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                    sendPLC("OL");
                                }
                            }
                        } 

                    }
                    break;

                case "a3":
                    if (Param.UseCommand)
                    {
                        if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            if (Param.ShutDown == 0 | lblTestResult_A.Text == "PASS")
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到MB_A正常测试完毕断电");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到MB_A正常测试完毕断电");

                                //断电

                                //断开19V
                                //1,断电
                                SubFunction.updateMessage(lstStatusCommand, "双连板，MB_A断开19V");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板，MB_A断开19V");
                                sendPLC("aa");
                                //--be care---may be need complete  
                                _b_A_send_aa = true;

#if DEBUG
                                SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                                SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
#endif

                                if (_b_A_send_aa && _b_B_send_ab)
                                {
                                    _b_A_send_aa = false;
                                    //----------可能会重复发送命令-----
                                    //2,判断侧插
                                    //左右都无
                                    if (!Param.LeftInsert && !Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                        sendPLC("DO");
                                    }
                                    //右侧插，先右后左
                                    if (Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                        sendPLC("OR");
                                    }
                                    else //右边沒有，判断左边
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                        sendPLC("OL");
                                    }
                                    timerDetectTimeOut.Stop();
                                }

                            }
                            else
                            {
                                ShutDownCount_A += 1;
                                if (ShutDownCount_A <= Param.ShutDown)
                                {
                                    //开机過程中的正常断电
                                    SubFunction.updateMessage(lstStatusCommand, "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                    break;
                                }
                                else
                                {
                                    //断电次数超過
                                    SubFunction.updateMessage(lstStatusCommand, "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");

                                    lblTestResult_A.ForeColor = Color.Red;
                                    lblTestResult_A.Text = "FAIL";
                                    ShutDownCount_A = 0;

                                    //断电
                                    //断开19V
                                    //1,断电
                                    SubFunction.updateMessage(lstStatusCommand, "双连板，MB_A断开19V");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板，MB_A断开19V");
                                    sendPLC("aa");
                                    //--be care---may be need complete  
                                    _b_A_send_aa = true;

#if DEBUG
                                    SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
                                    SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
#endif

                                    if (_b_A_send_aa && _b_B_send_ab)
                                    {
                                        _b_A_send_aa = false;
                                        //----------可能会重复发送命令-----
                                        //2,判断侧插
                                        //左右都无
                                        if (!Param.LeftInsert && !Param.RightInsert)
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                            sendPLC("DO");
                                        }
                                        //右侧插，先右后左
                                        if (Param.RightInsert)
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                            sendPLC("OR");
                                        }
                                        else //右边沒有，判断左边
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                            sendPLC("OL");
                                        }
                                        timerDetectTimeOut.Stop();
                                    }
                                }
                            }
                        }
                    }

                   
                    break;
                case "a1":

                    break;
                case "a0":

                    break;
                case "by":
                    if (Param.UseCommand)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "MB_B检测到开机电压成功，实际电压:" + txtMB_B_V.Text + "(V)");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B检测到开机电压成功，实际电压:" + txtMB_B_V.Text + "(V)");
                    }

                    break;
                case "bn":
                    if (Param.UseCommand)
                    {
                        SubFunction.updateMessage(lstStatusCommand, "MB_B未检测到开机电压");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B未检测到开机电压");
                        //双板子
                        if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            //1,断电
                            Console.WriteLine("双连板MB_B断19V");
                            SubFunction.updateMessage(lstStatusCommand, "双连板,MB_B断开19V");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板,MB_B断开19V");
                            sendPLC("ab");
                            _b_B_send_ab = true;
#if DEBUG
                            SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                            SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);
#endif
                            if (_b_A_send_aa && _b_B_send_ab) //都发送了断电命令
                            {
                                _b_B_send_ab = false;
                                //----------可能会重复发送命令-----
                                //2,判断侧插
                                //左右都无
                                if (!Param.LeftInsert && !Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                    sendPLC("DO");
                                }
                                //右侧插，先右后左
                                if (Param.RightInsert)
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                    sendPLC("OR");
                                }
                                else //右边沒有，判断左边
                                {
                                    SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                    sendPLC("OL");
                                }
                            }
                        } 
                    }

                                     
                    break;
                case "b3":
                    if (Param.UseCommand)
                    {
                        if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            if (Param.ShutDown == 0 | lblTestResult_B.Text == "PASS")
                            {
                                SubFunction.updateMessage(lstStatusCommand, "检测到MB_B正常测试完毕断电");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到MB_B正常测试完毕断电");

                                //断电

                                //断开19V
                                //1,断电
                                SubFunction.updateMessage(lstStatusCommand, "双连板，MB_B断开19V");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板，MB_B断开19V");
                                sendPLC("ab");
                                //--be care---may be need complete  
                                _b_B_send_ab = true;

#if DEBUG
                                SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                                SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);

#endif
                                if (_b_A_send_aa && _b_B_send_ab)
                                {
                                    _b_B_send_ab = false;
                                    //----------可能会重复发送命令-----
                                    //2,判断侧插
                                    //左右都无
                                    if (!Param.LeftInsert && !Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                        sendPLC("DO");
                                    }
                                    //右侧插，先右后左
                                    if (Param.RightInsert)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                        sendPLC("OR");
                                    }
                                    else //右边沒有，判断左边
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                        sendPLC("OL");
                                    }
                                    timerDetectTimeOut.Stop();
                                }

                            }
                            else
                            {
                                ShutDownCount_B += 1;
                                if (ShutDownCount_B <= Param.ShutDown)
                                {
                                    //开机過程中的正常断电
                                    SubFunction.updateMessage(lstStatusCommand, "MB_B检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次");
                                    break;
                                }
                                else
                                {
                                    //断电次数超過
                                    SubFunction.updateMessage(lstStatusCommand, "MB_B检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B检测到第" + ShutDownCount_A + "次断电，开机共计需要断电:" + Param.ShutDown + "次，开机失败!");

                                    lblTestResult_B.ForeColor = Color.Red;
                                    lblTestResult_B.Text = "FAIL";
                                    ShutDownCount_B = 0;

                                    //断电
                                    //断开19V
                                    //1,断电
                                    SubFunction.updateMessage(lstStatusCommand, "双连板，MB_B断开19V");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双连板，MB_B断开19V");
                                    sendPLC("ab");
                                    //--be care---may be need complete  
                                    _b_B_send_ab = true;

#if DEBUG
                                    SubFunction.updateMessage(lstStatusCommand, "_b_B_send_ab = " + _b_B_send_ab);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_B_send_ab = " + _b_B_send_ab);
                                    SubFunction.updateMessage(lstStatusCommand, "_b_A_send_aa = " + _b_A_send_aa);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "_b_A_send_aa = " + _b_A_send_aa);

#endif

                                    if (_b_A_send_aa && _b_B_send_ab)
                                    {
                                        _b_B_send_ab = false;
                                        //----------可能会重复发送命令-----
                                        //2,判断侧插
                                        //左右都无
                                        if (!Param.LeftInsert && !Param.RightInsert)
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "检测到无侧插，治具开始下降.");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到无侧插，治具开始下降.");
                                            sendPLC("DO");
                                        }
                                        //右侧插，先右后左
                                        if (Param.RightInsert)
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "右侧插存在，右侧插退出.");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "右侧插存在，右侧插退出.");
                                            sendPLC("OR");
                                        }
                                        else //右边沒有，判断左边
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "左侧插存在，左侧插退出.");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "左侧插存在，左侧插退出.");
                                            sendPLC("OL");
                                        }
                                        timerDetectTimeOut.Stop();
                                    }
                                }
                            }
                        }
                    }
       

                    break;
                case "b1:":

                    break;
                case "b0":
                    break;

                case "do":
                    SubFunction.updateMessage(lstStatusCommand, "治具下降到位");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "治具下降到位");
                    //判断是否需要重测

                    if ( iCurrent_TestCount < Param.MaxRetestCount )
                    {
                        SubFunction.updateMessage(lstStatusCommand, "检测到需要重测，治具将上升");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到需要重测，治具将上升");
                        sendPLC("UP");
                        //重新计時
                        timerDetectTimeOut.Start();
                        icount_DUT_TestingTimeOut = 0;
                        iCurrent_TestCount += 1;
                        i_Detect_A_Count = 0;
                        i_Detect_B_Count = 0;
                        DUT_Retest_Flag = false;
                        ShutDownCount_B = 0;
                        ShutDownCount_A = 0;
                        _b_A_send_aa = false;
                        _b_B_send_ab = false;
                       
                    }
                    else//不重测
                    {
                        TestReInitUI();

                        if (lblTestResult_A.Text == "PASS") Param.iPass_A += 1;
                        if (lblTestResult_B.Text == "PASS") Param.iPass_B += 1;
                        if (lblTestResult_A.Text == "FAIL") Param.iFail_A += 1;
                        if (lblTestResult_B.Text == "FAIL") Param.iFail_B += 1;
                        if (lblTestResult_A.Text == "PASS" && lblTestResult_B.Text == "PASS")
                            Param.iPass += 1;
                        else
                            Param.iFail += 1;


                        Update_FixtrueID();//更新记录数据库治具ID
                        Update_Test_Statistical();//上传测试统计信息至记录数据库

                        SubFunction.updateMessage(lstStatusCommand, "检测到不需要重测，治具准备退出");
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到不需要重测，治具准备退出");

                        SubFunction.WriteTemp(Param.TestDataTempini); //记录临时储存数据
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "记录临时测试数据");

                        sendPLC("OU");
                        //
                        DUT_Retest_Flag = false;
                        DUT_A_Retest_Flag = false;
                        DUT_B_Retest_Flag = false;
                        iCurrent_TestCount = 1;
                        //

                        //自动测试，需要些数据库
                        if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                        {

                            //单板
                            if (Param.MBType.ToUpper() == Param.mbType.Single.ToString().ToUpper())
                            {
                                if (lblTestResult_A.Text == "PASS")
                                    writeDataBase(dataBase_WriteAddress, "13");
                                else
                                {                                //fail
                                    //module1写23，module2写13
                                    if (Param.RobotModule == "1")
                                        writeDataBase(dataBase_WriteAddress, "23");
                                    else
                                        writeDataBase(dataBase_WriteAddress, "13");
                                }
                            }
                            //双板
                            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                            {
                                //兩片板子都PASS才算是PASS
                                if (lblTestResult_A.Text == "PASS" && lblTestResult_B.Text == "PASS")
                                {
                                    //writeDataBase(dataBase_WriteAddress, "13");
                                    ErrCount = 0;
                                    for (Param.i = 0; Param.i < 100; Param.i++)//add by channing 20161213
                                    {
                                        writeDataBase(dataBase_WriteAddress, "13");
                                    }
                                }
                                else
                                {
                                    if (Param.NG_Stop)
                                    {
                                        Test_Error_Stop();//add by channing Wang
                                    }
                                    //其中有1pcs fail
                                    //module1写23，module2写13
                                    if (Param.RobotModule == "1")
                                        writeDataBase(dataBase_WriteAddress, "23");
                                    else
                                    {
                                        //writeDataBase(dataBase_WriteAddress, "13");
                                        for (Param.i = 0; Param.i < 100; Param.i++)//add by channing 20161213
                                        {
                                            writeDataBase(dataBase_WriteAddress, "13");
                                        }

                                    }
                                }
                            }
                        }
                    }                
                    break;

                case "ou":

                    SubFunction.updateMessage(lstStatusCommand, "载板退出到位");
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "载板退出到位");
                    if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                        writeDataBase(dataBase_WriteAddress, "14");
                    sendPLC("RE");
                    break;
                default:
                    break;        
                
            }
           Clear_TestData();  //测试数据处理
        }

        private void timerDetectTimeOut_Tick(object sender, EventArgs e)
        {
            timerDetectTimeOut.Stop();
            icount_DUT_TestingTimeOut += 1;
            //iCurrent_TestCount += 1;
            txtCurrentTime.Text = icount_DUT_TestingTimeOut.ToString();

            if (icount_DUT_TestingTimeOut  >= Param.TestOKTimeOut)
            {
                SubFunction.updateMessage(lstStatusCommand, "测试超時，超時時間:" + Param.TestOKTimeOut + ",A B准备强制开机并退出");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "测试超時，超時時間:" + Param.TestOKTimeOut + ",A B准备强制开机并退出");
                sendPLC("S1");
                if (Param.ShutDown > 0)
                    ShutDownCount_A = Param.ShutDown;
                Delay(2000);
                sendPLC("S2");
                if (Param.ShutDown > 0)
                    ShutDownCount_B = Param.ShutDown;

                //iCurrent_TestCount += 1; //重测计数
                //iCurrent_TestCount = Param.MaxRetestCount; //不重测    //Add by channing Wang 20161224

                //////////////////////
                if (iCurrent_TestCount == Param.MaxRetestCount)          //Add by channing Wang 20161224
                {
                    if (lblTestResult_B.Text != "PASS" && Param.ST_Flag)
                    {
                    this.Invoke((EventHandler)(delegate
                    {
                        Param.iFail_Pass_B += 1;  //B面强抛计数
                        lstMB_BTestItems.Items.Clear();
                        foreach (string st in lstMB_ATestItems.Items)
                        {
                            string it = st.Substring(st.LastIndexOf(' '), st.Length - st.LastIndexOf(' '));
                            SubFunction.updateMessage(lstMB_BTestItems, it);
                            Delay(1000);
                        }
                        lblTestResult_B.ForeColor = Color.Green;
                        lblTestResult_B.Text = "PASS";
                        ngDUT_A_TestItem = "PASS";
                        ngDUT_B_TestItem = "PASS";
                    }));
                }

                if (lblTestResult_A.Text != "PASS" && Param.ST_Flag)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        Param.iFail_Pass_A += 1;  //A面强抛计数
                        lstMB_ATestItems.Items.Clear();
                        foreach (string st in lstMB_BTestItems.Items)
                        {
                            string it = st.Substring(st.LastIndexOf(' '), st.Length - st.LastIndexOf(' '));
                            SubFunction.updateMessage(lstMB_ATestItems, it);
                        }
                        lblTestResult_A.ForeColor = Color.Green;
                        lblTestResult_A.Text = "PASS";
                        ngDUT_B_TestItem = "PASS";
                        ngDUT_A_TestItem = "PASS";
                    }));

                }

                if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                {
                    if (Param.Web_Use )
                    {
                        //要兩片都PASS才上拋
                        if (lblTestResult_A.Text == "PASS" && lblTestResult_B.Text == "PASS")
                        {
                            if (Param.Test_Log)
                            {
                                this.Invoke((EventHandler)(delegate
                                {
                                    Param.iFail_Pass += 1;  //A面 or B强抛计数 
                                    SubFunction.updateMessage(lstStatusCommand, "检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                    SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                                    SubFunction.updateMessage(lstMB_BTestItems, "UPLOADARMS");
                                    uploadFixtureID(Param.Web_Site, Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                    updateResult2SFCS("011-ST-" + "Bar_A-", Param.Web_Site, Param.bar_A, true, ngDUT_A_TestItem, DUT_B_Retest_Flag);
                                    uploadFixtureID(Param.Web_Site, Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                    updateResult2SFCS("012-ST-" + "Bar_B-", Param.Web_Site, Param.bar_B, true, ngDUT_B_TestItem, DUT_B_Retest_Flag);
                                }));
                                Param.Test_Log = false; //TestLog已完成记录
                            }
                            icount_DUT_TestingTimeOut = 0;
                            iCurrent_TestCount = Param.MaxRetestCount;
                            timerDetectTimeOut.Stop();
                        }
                    }

                  
                }
                }
                /*
                if (lblTestResult_B.Text != "PASS" && Param.ST_Flag)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        Param.iFail_Pass_B += 1;  //B面强抛计数
                        lstMB_BTestItems.Items.Clear();
                        foreach (string st in lstMB_ATestItems.Items)
                        {
                            string it = st.Substring(st.LastIndexOf(' '), st.Length - st.LastIndexOf(' '));
                            SubFunction.updateMessage(lstMB_BTestItems, it);
                            Delay(1000);
                        }
                        lblTestResult_B.ForeColor = Color.Green;
                        lblTestResult_B.Text = "PASS";
                        ngDUT_A_TestItem = "PASS";
                        ngDUT_B_TestItem = "PASS";
                    }));
                }

                if (lblTestResult_A.Text != "PASS" && Param.ST_Flag)
                {
                    this.Invoke((EventHandler)(delegate
                    {
                        Param.iFail_Pass_A += 1;  //A面强抛计数
                        lstMB_ATestItems.Items.Clear();
                        foreach (string st in lstMB_BTestItems.Items)
                        {
                            string it = st.Substring(st.LastIndexOf(' '), st.Length - st.LastIndexOf(' '));
                            SubFunction.updateMessage(lstMB_ATestItems, it);
                        }
                        lblTestResult_A.ForeColor = Color.Green;
                        lblTestResult_A.Text = "PASS";
                        ngDUT_B_TestItem = "PASS";
                        ngDUT_A_TestItem = "PASS";
                    }));

                }

                if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                {
                    if (Param.Web_Use)
                    {
                        //要兩片都PASS才上拋
                        if (lblTestResult_A.Text == "PASS" && lblTestResult_B.Text == "PASS")
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                Param.iFail_Pass += 1;  //A面 or B强抛计数 
                                SubFunction.updateMessage(lstStatusCommand, "检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                                SubFunction.updateMessage(lstMB_BTestItems, "UPLOADARMS");
                                uploadFixtureID(Param.Web_Site, Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                updateResult2SFCS("011---" + "Bar_A-",Param.Web_Site, Param.bar_A, true, ngDUT_A_TestItem, DUT_B_Retest_Flag);
                                uploadFixtureID(Param.Web_Site, Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                updateResult2SFCS("012---" + "Bar_B-", Param.Web_Site, Param.bar_B, true, ngDUT_B_TestItem, DUT_B_Retest_Flag);
                            }));

                            icount_DUT_TestingTimeOut = 0;
                            iCurrent_TestCount = Param.MaxRetestCount;
                            timerDetectTimeOut.Stop();
                        }
                    }
                }
*/
                DUT_A_Str = string.Empty;
                DUT_B_Str = string.Empty;
                return;
            }

            timerDetectTimeOut.Start();
        }

        private void spDUT_A_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (spDUT_A.BytesToRead == 0)
                return;
            Delay(50);
            string temp_A = string.Empty;            
            DUT_A_Str = spDUT_A.ReadExisting().Trim();
            temp_A = DUT_A_Str;
            string spString = DUT_A_Str;
            spDUT_A.DiscardInBuffer();//清空buffer
            this.Invoke((EventHandler)(delegate
            {
                SubFunction.updateMessage(lstStatusCommand, "DUT_A->PC:" + DUT_A_Str);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DUT_A->PC:" + DUT_A_Str);
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DUT_A->PC:" + DUT_A_Str);
            }));

            //判断命令

            switch (spString.Substring(0, 1).ToUpper())
            {
                case "D":
                    //1.DTESTOK 测试結束
                    if (spString == "DTESTOK")
                    {
                        this.Invoke ((EventHandler )(delegate
                        {
                            sendData(spDUT_A, "OK");
                            SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:OK");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:OK");
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:OK");

                            SubFunction.updateMessage(lstStatusCommand, "MB_A Test OK");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A Test OK");
                            DUT_A_Status = Param.mbStatus.PASS.ToString();
                            lblTestResult_A.ForeColor = Color.Green;
                            lblTestResult_A.Text = "PASS";

                            if (Param.Upload_SFCS_type)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "单抛模式：MB_A已测试PASS,准备上拋SFCS");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "单抛模式：MB_A已测试PASS,准备上拋SFCS");
                                SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                                uploadFixtureID(Param.Web_Site, Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                updateResult2SFCS("S01---" + "Bar_A-", Param.Web_Site, Param.bar_A, true, ngDUT_A_TestItem, DUT_B_Retest_Flag);
                            }
                        }));



                        ////单板,直接拋
                        //if (Param.MBType.ToUpper() == Param.mbType.Single .ToString().ToUpper())
                        //{
                        //    if (Param.Web_Use)
                        //    {
                        //        //是否上拋BBB
                        //        if (Param.OKBBBStatus)
                        //        {
                        //            this.Invoke ((EventHandler )(delegate 
                        //            {
                        //                SubFunction.updateMessage(lstMB_ATestItems, "UPLOADSFCS");
                        //                SubFunction.updateMessage(lstStatusCommand, "upload test result to sfcs & save log");
                        //                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "upload test result to sfcs & save log");
                        //                uploadFixtureID(Param.Web_Site, Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                        //                updateResult2SFCS(Param.Web_Site, Param.bar_A, true, ngDUT_A_TestItem, DUT_A_Retest_Flag);
                        //            }));
                                    
                        //        }
                        //        //是否上拋BBB
                        //        if (Param.Arms_Use)
                        //        {
                        //            //need complete
                        //            this.Invoke((EventHandler)(delegate
                        //            {
                        //                SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                        //                SubFunction.updateMessage(lstStatusCommand, "upload arms to network drive");
                        //                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "upload arms to network drive");
                        //            }));
                        //        }
                        //    }
                        //}


                        //if (lblTestResult_B.Text != "PASS" && Param.ST_Flag)
                        //{
                        //    this.Invoke((EventHandler)(delegate
                        //    {
                        //        lstMB_BTestItems.Items.Clear();
                        //        foreach (string st in lstMB_ATestItems.Items)
                        //        {
                        //            string it = st.Substring(st.LastIndexOf(' '), st.Length - st.LastIndexOf(' '));
                        //            SubFunction.updateMessage(lstMB_BTestItems, it);
                        //        }
                        //        lblTestResult_B.ForeColor = Color.Green;
                        //        lblTestResult_B.Text = "PASS";
                        //        ngDUT_A_TestItem = "PASS";
                        //        ngDUT_B_TestItem = "PASS";
                        //    }));
                        //}
                            
           

                        if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            if (Param.Web_Use)
                            {
                                //要兩片都才上拋
                                if (lblTestResult_A.Text == "PASS" && lblTestResult_B.Text == "PASS")
                                {
                                    this.Invoke((EventHandler)(delegate
                                    {
                                        if (!Param.Upload_SFCS_type && Param.Test_Log)
                                        {
                                            SubFunction.updateMessage(lstStatusCommand, "双抛模式：检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双抛模式：检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                            SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                                            SubFunction.updateMessage(lstMB_BTestItems, "UPLOADARMS");
                                            uploadFixtureID(Param.Web_Site, Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                            updateResult2SFCS("021---" + "Bar_A-", Param.Web_Site, Param.bar_A, true, ngDUT_A_TestItem, DUT_B_Retest_Flag);
                                            uploadFixtureID(Param.Web_Site, Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                            updateResult2SFCS("022---" + "Bar_B-", Param.Web_Site, Param.bar_B, true, ngDUT_B_TestItem, DUT_B_Retest_Flag);
                                           
                                            Param.Test_Log = false;
                                        }
                                       
                                    }));
                                    
                                    icount_DUT_TestingTimeOut = 0;
                                    iCurrent_TestCount = Param.MaxRetestCount;
                                    timerDetectTimeOut.Stop();
                                }                                
                            }

                              
                        }    

                        //重置參数 
                        //need complete
                        DUT_A_Str = string.Empty;
                        //iCurrent_TestCount = Param.MaxRetestCount;                       
                        DUT_A_Retest_Flag = false;

                       // Param.iPass_A += 1; //良率计算

                       // testingReInitUI();
                    }


                    //特使的D开頭含+號的命令
                    //===========DSYS+651+AramModel+DellModel*======================
                    if (spString.Contains("+")) //包含+
                    {
                        //判断是否接收完毕
                        if (!spString.EndsWith("*"))
                        {
                            //接收不完毕
                            this.Invoke((EventHandler)(delegate
                            {
                                sendData(spDUT_A, "SYSNG");
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:SYSNG");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:SYSNG");
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:SYSNG");
                                SubFunction.updateMessage(lstStatusCommand, "SYS 信息接收不完全");                                
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SYS 信息接收不完全");

                            }));  
                        }
                        else
                        {
                            //接收完全
                            this.Invoke((EventHandler)(delegate
                            {
                                sendData(spDUT_A, "SYSOK");
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:SYSOK");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:SYSOK");
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:SYSOK");
                                SubFunction.updateMessage(lstStatusCommand, "SYS 信息接收完全");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SYS 信息接收完全");
                                getArmsInfoByDUT(spString, ref _ASYSID, ref _ASRMMODEL, ref _ADELLMODEL);
                                this.txtArmsPath.ForeColor = Color.Green;
                                 string temp_Arms = string.Empty ;
                                try
                                {
                                   temp_Arms = txtArmsPath.Text.Substring(0, 15);
                                }
                                catch (Exception ex)
                                {

                                    SubFunction.updateMessage(lstStatusCommand, "ReGet Arms Path Fail," + ex.Message);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "ReGet Arms Path Fail," + ex.Message);
                                }
                                
                                this.txtArmsPath.Text = temp_Arms  + @"\" + @_ADELLMODEL + @"\";
                                this.txtMB_A_DellMode.ForeColor = Color.Green;
                                this.txtMB_A_DellMode.Text = _ADELLMODEL;
                            }));   
                        }
                    }
                    else
                    {
                        //其他通用的項目
                        spString = spString.Substring(1, spString.Length - 1);
                        string sEnd = spString.Substring(spString.Length - 2, 2);
                        ngDUT_A_TestItem = spString.Substring(0, spString.Length - 2);
                        if (sEnd == "OK")
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                SubFunction.updateMessage(lstMB_ATestItems, ngDUT_A_TestItem + "->OK");
                                SubFunction.updateMessage(lstStatusCommand, "MB_A " + ngDUT_A_TestItem + " test OK");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A " + ngDUT_A_TestItem + " test OK");
                            }));

                        }
                        else if (sEnd == "NG")
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                SubFunction.updateMessage(lstMB_ATestItems, ngDUT_A_TestItem + "->NG");
                                SubFunction.updateMessage(lstStatusCommand, "MB_A " + ngDUT_A_TestItem + " test NG");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A " + ngDUT_A_TestItem + " test NG");
                                lblTestResult_A.ForeColor = Color.Red;
                                lblTestResult_A.Text = "FAIL";
                            }));
                           
                            //Param.iFail_A += 1;
                            if (Param.Test_Log)
                            {
                                //=============upload sfcs=============
                                if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                                {
                                    ws.UploadFixtureID(Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                    TrnDatas[0] = getErrorCode(ngDUT_A_TestItem);
                                    string result = ws.Complete(Param.bar_A, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                                    if (result == "OK")
                                    {
                                        //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", false, true, ngDUT_A_TestItem);
                                        SubFunction.SaveTestLog("18---" + "Bar_A-" + Param.bar_A, "FAIL", false, true, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("18---" + "Bar_A-" + Param.bar_A, "FAIL", false, true, ngDUT_A_TestItem);
                                    }
                                    else
                                    {
                                        //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", false, false, ngDUT_A_TestItem);
                                        SubFunction.SaveTestLog("19---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("19---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, ngDUT_A_TestItem);
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + "Bar_A-" + Param.bar_A + " Fail," + result);
                                    }
                                }
                                else
                                {
                                    //SubFunction.saveLog("Bar_A-" + Param.bar_A, "FAIL", false, false, ngDUT_A_TestItem);
                                    SubFunction.SaveTestLog("20---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, ngDUT_A_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("20---" + "Bar_A-" + Param.bar_A, "FAIL", false, false, ngDUT_A_TestItem);
                                }
                                Param.Test_Log = false;
                            }
                            

                        }
                        else
                        {
                            //测试項目
                            ngDUT_A_TestItem = spString;
                            this.Invoke((EventHandler)(delegate
                            {
                                SubFunction.updateMessage(lstMB_ATestItems, ngDUT_A_TestItem);
                                SubFunction.updateMessage(lstStatusCommand, "MB_A Test " + ngDUT_A_TestItem);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_A Test " + ngDUT_A_TestItem);
                            }));
                        }
                    }
                    break;

                case "C": //C开頭的命令
                    spString = spString.Substring(1, spString.Length - 1);//去掉开頭的C
                    //SubFunction.updateMessage(lstStatusCommand, "DUT_A->PC:" + spString);
                    this.Invoke((EventHandler)(delegate
                    {
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DUT_A->PC:" + spString);
                        SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DUT_A->PC:" + spString);
                    }));

                    if (spString == "GETPPID") //TE获取条码
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_ATestItems, spString);
                            if (!string.IsNullOrEmpty(Param.bar_A))
                            {
                                sendData(spDUT_A, Param.bar_A);
                                SubFunction.updateMessage(lstStatusCommand, "Bar_A:" + Param.bar_A);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Bar_A:" + Param.bar_A);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "Bar_A is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Bar_A is null");
                            }
                        }));    
                    }
                    else if (spString == "GETSTAGE") //TE获取治具编号
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_ATestItems, spString);
                            if (!string.IsNullOrEmpty(Param.FixtrueID))
                            {
                                sendData(spDUT_A, Param.FixtrueID);                                
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:" + Param.FixtrueID);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:" + Param.FixtrueID);
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:" + Param.FixtrueID);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "FixtrueID is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "FixtrueID is null");
                            }
                        }));
           
    
                    }
                    else if (spString == "GETOPID") //TE获取工号
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_ATestItems, spString);
                            if (!string.IsNullOrEmpty(Param.OPID))
                            {
                                sendData(spDUT_A, Param.OPID);
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:" + Param.OPID);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:" + Param.OPID);
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:" + Param.OPID);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "OPID is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "OPID is null");
                            }
                        }));


                    }
                    else if (spString == "GETLANID") //TE获取GETLANID
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_ATestItems, spString);
                            if (!string.IsNullOrEmpty(DUT_A_MAC))
                            {
                                sendData(spDUT_A, DUT_A_MAC);
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:" + DUT_A_MAC);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:" + DUT_A_MAC);
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:" + DUT_A_MAC);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "DUT_A_MAC is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DUT_A_MAC is null");
                            }
                        }));
        
                    }
                    else
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstStatusCommand, "Unknown Command：" + "C" + spString);
                        }));
                        
                    }
                    break;

                default:
                    //BIOS+MAC+*
                    if (spString.Contains("+") && spString.EndsWith("*"))
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            getBIOSInfo(spString, "A");
                            sendData(spDUT_A, "BIOSOK");
                            SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:BIOSOK");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:BIOSOK");
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:BIOSOK");
                            SubFunction.updateMessage(lstStatusCommand, "BIOS 信息获取正確");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "BIOS 信息获取正確");
 
                        }));

                    }
                    else
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstStatusCommand, "Check BIOS info incomplete.");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Check BIOS info incomplete.");
                            sendData(spDUT_A, "BOISNG");
                            SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:BOISNG");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:BOISNG");
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:BOISNG");
                        }));

                    }

                    if (!spString.Contains("+") || !spString.EndsWith("*"))
                    {
                        this.Invoke ((EventHandler )(delegate 
                        {
                            SubFunction.updateMessage(lstStatusCommand, "Unknown Command：" + spString);
                        }));                        
                    }
                    break;
            }  
        }

        private void spDUT_B_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (spDUT_B.BytesToRead == 0)
                return;
            Delay(50);
            string temp_B = string.Empty;
            DUT_B_Str = spDUT_B.ReadExisting().Trim();
            temp_B = DUT_B_Str;
            string spString = DUT_B_Str;
            spDUT_B.DiscardInBuffer();//清空buffer

            this.Invoke((EventHandler)(delegate
            {
                SubFunction.updateMessage(lstStatusCommand, "DUT_B->PC:" + DUT_B_Str);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DUT_B->PC:" + DUT_B_Str);
                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DUT_B->PC:" + DUT_B_Str);
            }));

            //判断命令
             
            switch (spString.Substring(0, 1).ToUpper())
            {
                case "D":
                    //1.DTESTOK 测试結束
                    if (spString == "DTESTOK")
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            sendData(spDUT_B, "OK");
                            SubFunction.updateMessage(lstStatusCommand, "PC->DUT_B:OK");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_B:OK");
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_B:OK");

                            SubFunction.updateMessage(lstStatusCommand, "MB_B Test OK");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B Test OK");
                            DUT_B_Status = Param.mbStatus.PASS.ToString();
                            lblTestResult_B.ForeColor = Color.Green;
                            lblTestResult_B.Text = "PASS";

                            if (Param.Upload_SFCS_type)
                            {
                                SubFunction.updateMessage(lstStatusCommand, "单抛模式：MB_B已测试PASS,准备上拋SFCS");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "单抛模式：MB_B已测试PASS,准备上拋SFCS");
                                SubFunction.updateMessage(lstMB_BTestItems, "UPLOADARMS");
                                uploadFixtureID(Param.Web_Site, Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                updateResult2SFCS("S02---" + "Bar_B-", Param.Web_Site, Param.bar_B, true, ngDUT_B_TestItem, DUT_B_Retest_Flag);
                            }

                        }));

                        ////单板,直接拋--->单板是A板
                        //if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        //{
                        //    if (Param.Web_Use)
                        //    {
                        //        //是否上拋BBB
                        //        if (Param.OKBBBStatus)
                        //        {
                        //            this.Invoke((EventHandler)(delegate
                        //            {
                        //                SubFunction.updateMessage(lstMB_BTestItems, "UPLOADSFCS");
                        //                SubFunction.updateMessage(lstStatusCommand, "upload test result to sfcs & save log");
                        //                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "upload test result to sfcs & save log");
                        //            }));
                        //            uploadFixtureID(Param.Web_Site, Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                        //            updateResult2SFCS(Param.Web_Site, Param.bar_B, true, ngDUT_B_TestItem, DUT_B_Retest_Flag);
                        //        }
                        //        //是否上拋BBB
                        //        if (Param.Arms_Use)
                        //        {
                        //            //need complete
                        //            this.Invoke((EventHandler)(delegate
                        //            {
                        //                SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                        //                SubFunction.updateMessage(lstStatusCommand, "upload arms to network drive");
                        //                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "upload arms to network drive");
                        //            }));
                        //        }
                        //    }
                        //}

                        //if (lblTestResult_A.Text != "PASS" && Param.ST_Flag)
                        //{
                        //    this.Invoke((EventHandler)(delegate
                        //    {
                        //        lstMB_ATestItems.Items.Clear();
                        //        foreach (string st in lstMB_BTestItems.Items)
                        //        {
                        //            string it = st.Substring(st.LastIndexOf(' '), st.Length - st.LastIndexOf(' '));
                        //            SubFunction.updateMessage(lstMB_ATestItems, it);
                        //        }
                        //        lblTestResult_A.ForeColor = Color.Green;
                        //        lblTestResult_A.Text = "PASS";
                        //        ngDUT_B_TestItem = "PASS";
                        //        ngDUT_A_TestItem = "PASS";
                        //    }));
                            
                        //}

                        if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
                        {
                            //要兩片都PASS才上拋
                            if (lblTestResult_A.Text == "PASS" && lblTestResult_B.Text == "PASS")
                            {
                                this.Invoke((EventHandler)(delegate
                                {
                                    if (!Param.Upload_SFCS_type && Param.Test_Log)
                                    {
                                        SubFunction.updateMessage(lstStatusCommand, "双抛模式：检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "双抛模式：检测到MB_A & MB_B 都已测试PASS,准备上拋SFCS");
                                        SubFunction.updateMessage(lstMB_ATestItems, "UPLOADARMS");
                                        SubFunction.updateMessage(lstMB_BTestItems, "UPLOADARMS");
                                        uploadFixtureID(Param.Web_Site, Param.bar_A, Param.SFC_Stage, Param.FixtrueID);
                                        updateResult2SFCS("031---" + "Bar_A-", Param.Web_Site, Param.bar_A, true, ngDUT_A_TestItem, DUT_B_Retest_Flag);
                                        uploadFixtureID(Param.Web_Site, Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                        updateResult2SFCS("032---" + "Bar_B-", Param.Web_Site, Param.bar_B, true, ngDUT_B_TestItem, DUT_B_Retest_Flag);
                                    }
                                    Param.Test_Log = false; //TestLog已完成记录
                                }));
                                // TestReInitUI();
                                icount_DUT_TestingTimeOut = 0;
                                iCurrent_TestCount = Param.MaxRetestCount;
                                timerDetectTimeOut.Stop();
                            }
                        }
                      
                        //重置參数 
                        //need complete
                        DUT_B_Str = string.Empty;
                       // iCurrent_B_TestCount = 1;                        
                        DUT_B_Retest_Flag = false;
                        //Param.iPass_B += 1; //良率计算
                        // testingReInitUI();
                    }


                    //特使的D开頭含+號的命令
                    //===========DSYS+651+AramModel+DellModel*======================
                    if (spString.Contains("+")) //包含+
                    {
                        //判断是否接收完毕
                        if (!spString.EndsWith("*"))
                        {
                            //接收不完毕
                            this.Invoke((EventHandler)(delegate
                            {
                                sendData(spDUT_B, "SYSNG");
                                SubFunction.updateMessage(lstStatusCommand, "SYS 信息接收不完全");
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_B:SYSNG");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SYS 信息接收不完全");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_B:SYSNG");
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_B:SYSNG");
                            }));
                        }
                        else
                        {
                            //接收完全
                            this.Invoke((EventHandler)(delegate
                            {
                                sendData(spDUT_B, "SYSOK");
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_B:SYSOK");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_B:SYSOK");
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_B:SYSOK");
                                SubFunction.updateMessage(lstStatusCommand, "SYS 信息接收完全");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "SYS 信息接收完全");
                                getArmsInfoByDUT(spString, ref _ASYSID, ref _ASRMMODEL, ref _ADELLMODEL);
                                this.txtArmsPath.ForeColor = Color.Green;                                
                                string temp_Arms = string.Empty;
                                try
                                {
                                    temp_Arms = txtArmsPath.Text.Substring(0, 15);
                                }
                                catch (Exception ex)
                                {

                                    SubFunction.updateMessage(lstStatusCommand, "ReGet Arms Path Fail," + ex.Message);
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "ReGet Arms Path Fail," + ex.Message);
                                }

                                this.txtArmsPath.Text = temp_Arms + @"\" + @_ADELLMODEL + @"\";                                
                                this.txtMB_B_DellMode.ForeColor = Color.Green;
                                this.txtMB_B_DellMode.Text = _ADELLMODEL;
                            }));
                        }
                    }
                    else
                    {
                        //其他通用的項目
                        spString = spString.Substring(1, spString.Length - 1);
                        string sEnd = spString.Substring(spString.Length - 2, 2);
                        ngDUT_B_TestItem = spString.Substring(0, spString.Length - 2);
                        if (sEnd == "OK")
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                SubFunction.updateMessage(lstMB_BTestItems, ngDUT_B_TestItem + "->OK");
                                SubFunction.updateMessage(lstStatusCommand, "MB_B " + ngDUT_B_TestItem + " test OK");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B " + ngDUT_B_TestItem + " test OK");
                            }));

                        }
                        else if (sEnd == "NG")
                        {
                            this.Invoke((EventHandler)(delegate
                            {
                                SubFunction.updateMessage(lstMB_ATestItems, ngDUT_B_TestItem + "->NG");
                                SubFunction.updateMessage(lstStatusCommand, "MB_B " + ngDUT_B_TestItem + " test NG");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B " + ngDUT_B_TestItem + " test NG");
                                lblTestResult_B.ForeColor = Color.Red;
                                lblTestResult_B.Text = "FAIL";
                            }));                            
                            //Param.iFail_B += 1;
                           
                            if(Param.Test_Log)
                            {
                            //=============upload sfcs=============
                                if ((Param.RobotModule == "2") && Param.NGBBBStatus)
                                {
                                    ws.UploadFixtureID(Param.bar_B, Param.SFC_Stage, Param.FixtrueID);
                                    TrnDatas[0] = getErrorCode(ngDUT_B_TestItem);
                                    string result = ws.Complete(Param.bar_B, Param.PCBLine, Param.SFC_Stage, Param.SFC_Stage, Param.OPID, false, TrnDatas);
                                    if (result == "OK")
                                    {
                                        //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", false, true, ngDUT_B_TestItem);
                                        SubFunction.SaveTestLog("21---" + "Bar_B-" + Param.bar_B, "FAIL", false, true, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("21---" + "Bar_B-" + Param.bar_B, "FAIL", false, true, ngDUT_B_TestItem);
                                    }
                                    else
                                    {
                                        //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", false, false, ngDUT_B_TestItem);
                                        SubFunction.SaveTestLog("22---" + "Bar_B-" + Param.bar_B, "FAIL", false, false, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                        TestLog_for_Record_DataBase("22---" + "Bar_B-" + Param.bar_B, "FAIL", false, false, ngDUT_B_TestItem);
                                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Upload " + "Bar_B-" + Param.bar_B + " Fail," + result);
                                    }
                                }
                                else
                                {
                                    //SubFunction.saveLog("Bar_B-" + Param.bar_B, "FAIL", false, false, ngDUT_B_TestItem);
                                    SubFunction.SaveTestLog("23---" + "Bar_B-" + Param.bar_B, "FAIL", false, false, ngDUT_B_TestItem, Param.Model, Param.ModelFamily, Param.MO, Param.UPN, icount_DUT_TestingTimeOut.ToString(), Param.FixtrueID);
                                    TestLog_for_Record_DataBase("23---" + "Bar_B-" + Param.bar_B, "FAIL", false, false, ngDUT_B_TestItem);
                                }                           
                                Param.Test_Log = false;
                            }
                        }
                        else
                        {
                            //测试項目
                            ngDUT_B_TestItem = spString;
                            this.Invoke((EventHandler)(delegate
                            {
                                SubFunction.updateMessage(lstMB_BTestItems, ngDUT_B_TestItem);
                                SubFunction.updateMessage(lstStatusCommand, "MB_B Test " + ngDUT_B_TestItem);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "MB_B Test " + ngDUT_B_TestItem);
                            }));
                        }
                    }
                    break;

                case "C": //C开頭的命令
                    spString = spString.Substring(1, spString.Length - 1);//去掉开頭的C
                    //SubFunction.updateMessage(lstStatusCommand, "DUT_B->PC:" + spString);
                    this.Invoke((EventHandler)(delegate
                    {
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DUT_B->PC:" + spString);
                        SubFunction.saveLog(Param.logType.COMLOG.ToString(), "DUT_B->PC:" + spString);
                    }));

                    if (spString == "GETPPID")//TE获取PPID 
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_BTestItems, spString);
                            if (!string.IsNullOrEmpty(Param.bar_B))
                            {
                                sendData(spDUT_B, Param.bar_B);
                                SubFunction.updateMessage(lstStatusCommand, "Bar_B:" + Param.bar_B);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Bar_B:" + Param.bar_B);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "Bar_B is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Bar_B is null");
                            }
                        }));

                    }
                    else if (spString == "GETSTAGE")   //TE获取测试站别     
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_BTestItems, spString);
                            if (!string.IsNullOrEmpty(Param.FixtrueID))
                            {
                                sendData(spDUT_B, Param.FixtrueID);
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_B:" + Param.FixtrueID + " ,Keystone 13");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_B:" + Param.FixtrueID + " ,Keystone 13");
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_B:" + Param.FixtrueID + " ,Keystone 13");
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "OPID is null" + " ,Keystone 13");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "OPID is null" + " ,Keystone 13");
                            }
                        }));


                    }
                    else if (spString == "GETOPID")//TE获取工号
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_BTestItems, spString);
                            if (!string.IsNullOrEmpty(Param.OPID))
                            {
                                sendData(spDUT_B, Param.OPID);
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_B:" + Param.OPID);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_B:" + Param.OPID);
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_B:" + Param.OPID);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "OPID is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "OPID is null");
                            }
                        }));


                    }
                    else if (spString == "GETLANID")//TE获取GETLANID
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstMB_BTestItems, spString);
                            if (!string.IsNullOrEmpty(DUT_B_MAC))
                            {
                                sendData(spDUT_B, DUT_B_MAC);
                                SubFunction.updateMessage(lstStatusCommand, "PC->DUT_B:" + DUT_B_MAC);
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_B:" + DUT_B_MAC);
                                SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_B:" + DUT_B_MAC);
                            }
                            else
                            {
                                SubFunction.updateMessage(lstStatusCommand, "DUT_B_MAC is null");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "DUT_B_MAC is null");
                            }
                        }));

                    }
                    else
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstStatusCommand, "Unknown Command：" + "C" + spString);
                        }));

                    }
                    break;

                default:
                    //BIOS+MAC+*
                    if (spString.Contains("+") && spString.EndsWith("*"))
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            getBIOSInfo(spString, "B");
                            sendData(spDUT_A, "BIOSOK");
                            SubFunction.updateMessage(lstStatusCommand, "BIOS 信息获取正確");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "BIOS 信息获取正確");
                            SubFunction.updateMessage(lstStatusCommand, "PC->DUT_A:BIOSOK");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "PC->DUT_A:BIOSOK");
                            SubFunction.saveLog(Param.logType.COMLOG.ToString(), "PC->DUT_A:BIOSOK");
                        }));                      

                    }
                    else
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstStatusCommand, "Check BIOS info incomplete.");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Check BIOS info incomplete.");
                            sendData(spDUT_B, "BOISNG");
                        }));

                    }

                    if (!spString.Contains("+") || !spString.EndsWith("*"))
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            SubFunction.updateMessage(lstStatusCommand, "Unknown Command：" + spString);
                        }));
                    }
                    break;
            }
        }
     
        #endregion


        private void frmMain_DoubleClick(object sender, EventArgs e)
        {
            loadData2UI();
            settime();
        }

        private void btnFPY_Click(object sender, EventArgs e)
        {
            
            Form f = new frmFPY();
            f.Show();
        }

        #region IO_Monitor

        private bool getStatus(string szDevice)
        {
            int[] istatus = new int[2];
            int iReturn = -1;
            iReturn = axActPLC.ReadDeviceBlock(szDevice, 2, out istatus[0]);
            if (iReturn == 0)
            {
                if (szDevice.ToUpper().StartsWith("X"))
                {
                    try
                    {
                        Int64 it32 = 0;
                        it32 = Convert.ToInt64(ConverString(istatus[0].ToString(), 10, 2));
                        Param.IN_Status_1 = string.Format("{0:0000000000000000}", it32);
                        it32 = Convert.ToInt64(ConverString(istatus[1].ToString(), 10, 2));
                        Param.IN_Status_2 = string.Format("{0:0000000000000000}", it32);
                        MessageBox.Show(Param.IN_Status_2.ToString());
                    }
                    catch (Exception ex)
                    {
                        SubFunction.updateMessage(lstStatusCommand, szDevice + ",X---" + ex.Message);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), szDevice + ",X---" + ex.Message);
                    }

                }
                if (szDevice.ToUpper().StartsWith("M"))
                {
                    try
                    {
                        Int64 it32 = 0;
                        it32 = Convert.ToInt64(ConverString(istatus[0].ToString(), 10, 2));
                        Param.M_Status_1 = string.Format("{0:0000000000000000}", it32);
                        it32 = Convert.ToInt64(ConverString(istatus[1].ToString(), 10, 2));
                        Param.M_Status_2 = string.Format("{0:0000000000000000}", it32);
                    }
                    catch (Exception ex)
                    {
                        SubFunction.updateMessage(lstStatusCommand, szDevice + ",M---" + ex.Message);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), szDevice + ",M---" + ex.Message);
                    }

                }

                if (szDevice.ToUpper().StartsWith("Y"))
                {
                    try
                    {
                        Param.OUT_Status_1 = string.Format("{0:0000000000000000}", Convert.ToInt32(ConverString(istatus[0].ToString(), 10, 2)));
                        Param.OUT_Status_2 = string.Format("{0:0000000000000000}", Convert.ToInt32(ConverString(istatus[1].ToString(), 10, 2)));
                    }
                    catch (Exception ex)
                    {
                        SubFunction.updateMessage(lstStatusCommand, szDevice + ",Y---" + ex.Message);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), szDevice + ",Y---" + ex.Message);
                    }
                }

                return true;
            }
            else
            {
                string sResult = string.Format("0x{0:x8}", iReturn);
                string sMessge = string.Empty;
                //  getErrorMessage(sResult,ref sMessge);
                //axActSupportMsg1.GetErrorMessage(iReturn, out sMessge);
                SubFunction.updateMessage(lstStatusCommand, "Read Device:" + szDevice + " fail,Message:" + sResult + "," + sMessge);
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Read Device:" + szDevice + " fail,Message:" + sResult + "," + sMessge);
                return false;
            }

        }

        private void getErrorMessage(string sResult, ref string smessage)
        {
            switch (sResult)
            {
                case "0x01010002":
                    smessage = "超时出错,建议:·对属性的超时值重新进行审核。·通过通信设置实用程序重新进行设置。·对可编程控制器CPU、模块的设置及电缆状态等系统重新进行审核。·进行Close 后，重新进行Open。·应结束程序后，重新启动计算机。";
                    break;
                case "0x01010005":
                    smessage = "报文出错,建议:·确认系统的噪声。·对属性的超时值重新进行审核。·通过通信设置实用程序重新进行设置。·对可编程控制器CPU、模块的设置及电缆状态等系统重新进行审核。·应进行Close 后，重新进行Open。·应结束程序后，重新启动计算机。";
                    break;
                case "0x01010010":
                    smessage = "PC No. 出错在指定站号中无法进行通";
                    break;
                case "0x01010011":
                    smessage = "模式出错是不支持的指令。";
                    break;
                default:
                    smessage = "";
                    break;
            }
        }

 


        /// <summary>
        /// 进制转换
        /// </summary>
        /// <param name="value">需要转换的值</param>
        /// <param name="fromBase">原进制</param>
        /// <param name="toBase">需要转换的进制</param>
        /// <returns>返回的結果</returns>
        public static string ConverString(string value, int fromBase, int toBase)
        {
            Int32 n = Convert.ToInt32(value, fromBase);
            return Convert.ToString(n, toBase);
        }

        #endregion 

        private void button1_Click(object sender, EventArgs e)
        {
           
            Form f = new frmIOMonitor();
            f.Show();
        }

        private void picTitle_Click(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.ActiveCaption;
            ErrCount = 0;
        }
        private void lblFICTStage_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("notepad.exe", Param.IniFilePath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        
            MySqlConnection Record_sql = getMySqlCon();
            String sqlSearch = "select * from " + Param.RecordIP_DataBase_TestInfo_Table;
            SubFunction.updateMessage(lstStatusCommand, "sqlDel = " + sqlSearch);
            MySqlCommand mySqlCommand = getSqlCommand(sqlSearch, Record_sql);
            Record_sql.Open();
            getResultset(mySqlCommand);//查询数据库
            Record_sql.Close();



            /*
            MySqlConnection Record_sql = getMySqlCon();
            String sqlDel = "delete from " + Record_DataBase_Table + " where id = 3";
            SubFunction.updateMessage(lstStatusCommand, "sqlDel = " + sqlDel);
            MySqlCommand mySqlCommand = getSqlCommand(sqlDel, Record_sql);
            Record_sql.Open();
            getDel(mySqlCommand);//删除数据库
            Record_sql.Close();
            */

            /*
            MySqlConnection Record_sql = getMySqlCon();
            String sqlInsert = sql;
            SubFunction.updateMessage(lstStatusCommand, "sqlDel = " + sqlInsert);
            MySqlCommand mySqlCommand = getSqlCommand(sqlInsert, Record_sql);
            Record_sql.Open();
            getInsert(mySqlCommand);//插入数据库
            Record_sql.Close();
            */

           
            /*
            MySqlConnection Record_sql = getMySqlCon();
            String sqlUpdate = "update " + Record_DataBase_Table +" set SN='aaaaaaaaaaaaaaaaa' where id= 3";
            SubFunction.updateMessage(lstStatusCommand, "sqlDel = " + sqlUpdate);
            MySqlCommand mySqlCommand = getSqlCommand(sqlUpdate, Record_sql);
            Record_sql.Open();
            getUpdate(mySqlCommand);//修改数据库
            Record_sql.Close();
            */





/*
            //   SELECT * FROM [user] WHERE u_name LIKE '%三%' AND u_name LIKE '%猫%' 

            MySqlConnection mysql = getMySqlCon();



            //查询sql
            String sqlSearch = "select * from snprocessinfo";
            //插入sql
            String sqlInsert = "insert into student values (12,'张三',25,'大专')";
            //修改sql
            String sqlUpdate = "update student set name='李四' where id= 3";
            //删除sql
            String sqlDel = "delete from student where id = 12";
            //打印SQL语句
            Console.WriteLine(sqlDel);
            //四种语句对象
            //MySqlCommand mySqlCommand = getSqlCommand(sqlSearch, mysql);
            //MySqlCommand mySqlCommand = getSqlCommand(sqlInsert, mysql);
            //MySqlCommand mySqlCommand = getSqlCommand(sqlUpdate, mysql);
            MySqlCommand mySqlCommand = getSqlCommand(sqlDel, mysql);
            mysql.Open();
            //getResultset(mySqlCommand);
            //getInsert(mySqlCommand);
            //getUpdate(mySqlCommand);
            getDel(mySqlCommand);
            //记得关闭
            mysql.Close();
            String readLine = Console.ReadLine();
*/
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("explorer.exe", Param.appFolder);
            //SubFunction.updateMessage(lstStatusCommand, "打开测试文件夹");
            //SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "打开测试文件夹");
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
           // getStatus("Y0");
            //getStatus("X0");
            //getStatus("M112");

            Clear_TestData();

/*
           // connStr = "server=" + Record_DataBase_IP + ";user id=" + Record_DataBase_Account + ";password=" + Record_DataBase_Password + ";persistsecurityinfo=True;database=" + Record_DataBase_DB;
            //string sql = "select * from " + DataBase_Table;
            //string sql = "select * from " + DataBase_Table + " WHERE " + Searchstr;
            //string sql = "select * from [user] WHERE " + DataBase_Table + " LIKE " + Searchstr;
            string sql = "select * from " + DataBase_Table + " WHERE " + address + " LIKE  '" + Searchstr + "'";
            MySqlConnection objConnection = new MySqlConnection(connStr);
            MySqlCommand objCommand = new MySqlCommand(sql, objConnection);
            MySqlDataReader objReader;
            try
            {
                objConnection.Open();
                if (boollog)
                {
                    updateMessage(this.listBox_HistoryLog, "成功连接上数据库");
                    savesysLog(sysLogFolder, "成功连接上数据库");
                }
            }
            catch (Exception ex)
            {
                updateMessage(this.listBox_HistoryLog, "连接数据库出错," + ex.Message);
                savesysLog(sysLogFolder, "连接数据库出错," + ex.Message);
                return false;
            }

            try
            {
                objReader = objCommand.ExecuteReader();
                if (objReader.HasRows)
                {
                    while (objReader.Read())
                    {
                        outdata = objReader[address].ToString();
                        if (boollog)
                        {
                            updateMessage(listBox_HistoryLog, "该MB为回流板 " + address + "=" + outdata);
                            savesysLog(sysLogFolder, "该MB为回流板 " + address + "=" + outdata);
                        }
                    }
                    objConnection.Close();
                }
                else
                {
                    updateMessage(this.listBox_HistoryLog, "该MB为首次进入FICT系统");
                    savesysLog(sysLogFolder, "该MB为首次进入FICT系统");
                    objConnection.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                updateMessage(this.listBox_HistoryLog, "从数据库读出错," + ex.Message);
                savesysLog(sysLogFolder, "从数据库读出错," + ex.Message);
                objConnection.Close();
                return false;
            }
            return true;

*/

/*
            int plcread = -1;
            readPLC("Y11", ref plcread);

            SubFunction.updateMessage(lstStatusCommand, "Y11 = " + plcread.ToString ());

            string NotWriteMysqlData = string.Empty;
            NotWriteMysqlData = DateTime.Now.ToString("yyyyMMddHHmmss") + "," + "R_PPID" + "," + "R_TestResult" + "," + "R_ReTestFlag" + "," + "R_SFCSFlag" + "," + "R_TestInfo" + "," + Param.Model + "," + Param.ModelFamily + "," + Param.MO + "," + Param.UPN + "," + icount_DUT_TestingTimeOut + "," + Param.FixtrueID + "," + Param.PCBLine + "," + "" + "\n";
            StreamWriter sw = File.AppendText(Param.MysqlTestDatatxt);
            sw.WriteLine(NotWriteMysqlData);
            sw.Flush();
            sw.Close();
            SubFunction.updateMessage(lstStatusCommand, "未上抛至记录数据库，现已写入暂存文档...");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "未上抛至记录数据库，现已写入暂存文档...");

            //Param.NG_Stop
            //TestLog_for_Record_DataBase("021---Bar_A-CN0PG0MHWSC006CH0JTHA00", "PASS", true, false, "PASS");
 * */
        }


        private void 配置档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
             SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "编辑配置文档...");
            System.Diagnostics.Process.Start("notepad.exe", Param.IniFilePath);
        }

        private void 加载配置档ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            SubFunction.loadConfigData(Param.IniFilePath);
            loadData2UI();
        }

        private void 测试文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Param.testLogfolder);
           
        }

        private void 系统文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Param.sysLogFolder);
            
        }

        private void 命令文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Param.comLogFolder);
           
        }

        private void 系统日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {

           
            //check comport
            if (Param.Scanner_Use)
                closeSerialPort(spScanner);
            //MB_A 共同的
            if (Param.DUT_A_Use)
                closeSerialPort(spDUT_A);
            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //双板需要判断MB_B
                if (Param.DUT_B_Use)
                    closeSerialPort(spDUT_B);
            }

            //mx 连接
            if (Param.PLC_Use)
                closePLC();

            //判断测试方式
            if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                timerScanDatabase.Stop();

            timerDetect_V_A.Stop();
            timerDetect_V_B.Stop();
            timerScanFICT.Stop();
            PressStopButtonUI();

            loadData2UI();

            Application.Exit();
        }

        private void 串口调试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnStart.Enabled == true )
            {
               
                Form f = new SerialPortDebug();
                f.ShowDialog();
            }
            else
            {
                
            }
        }
        /*
        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String Verinfo = string.Empty;
            Verinfo = "升级版本介绍" + "\n";
            Verinfo += "<1.0.7.0 >:" + "\n";
            Verinfo += "1.增加测试数据上抛至《记录数据库》" + "\n";
            Verinfo += "2.TestLog 中加入测试时间，MB Mode，MO，UPN等信息" + "\n";
            Verinfo += "3.对强抛MB板子进行计数和标记" + "\n";
            Verinfo += "4.测试电脑系统时间与记录数据库时间同步" + "\n"; 
            Verinfo += "5.加入部分菜单选项" + "\n";
            Verinfo += "<1.0.7.1 >:" + "\n";
            Verinfo += "1.修复超时时，直接退出不重测" + "\n";
            Verinfo += "<1.0.7.2 >:" + "\n";
            Verinfo += "1.修复串口编号易丢失情况，达到在测试过程中自修复" + "\n";
            Verinfo += "<1.0.7.0104 >:" + "\n";
            Verinfo += "1.加入暂存档，保存本机测试信息" + "\n";
            Verinfo += "<1.0.7.0109 >:" + "\n";
            Verinfo += "1.未成功上抛到记录数数据库的TestLog，暂存本地文档" + "\n";
            Verinfo += "2.修复PLC串口线掉落或移除后，不能自动打开" + "\n";
            Verinfo += "3.增加对记录数据库启用或关闭功能" + "\n";
            Verinfo += "4.增加部分关键设置防呆提醒窗口" + "\n";
            Verinfo += "<1.0.7.0115 >:" + "\n";
            Verinfo += "1.增加机台调试功能" + "\n";
            Verinfo += "2.修复串口掉落重新插上之后，另外两个串口编号混乱" + "\n";
            Verinfo += "3.增加本机台信息上抛至记录数据库，还不完善" + "\n";
            MessageBox.Show(Verinfo);
            /*
            Verinfo += "" + "\n";
            Verinfo += "" + "\n"; 
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            Verinfo += "" + "\n";
            
        }
        */
        private void 机台调试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (btnStart.Enabled == true)
            {
                
                Form f = new FICTDebug();
                f.ShowDialog();
            }
            else
            {
                
            }
        }

        private void 测试数据清零ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult MsgBoxResult;//设置对话框返回值
            MsgBoxResult = MessageBox.Show("你是否确定将所有测试数据清零？" + "\n" + "是点击“Yes”，否点击“NO”...", "小心操作！！！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (MsgBoxResult == DialogResult.Yes)
            {
                iTotalScan = 0;  //清除条码统计信息
                SubFunction.CleanTemp(Param.TestDataTempini);
                SubFunction.updateMessage(lstStatusCommand, "执行数据清零动作...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "执行数据清零动作...");
            }
            if (MsgBoxResult == DialogResult.No)
            {
                SubFunction.updateMessage(lstStatusCommand, "取消数据清零动作...");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "取消数据清零动作...");
            }
        }

        private void 设备管理器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("devmgmt.msc");
            
        }

        private void timerMonitor_Tick(object sender, EventArgs e)
        {
            timerMonitor.Stop();
            getStatus("Y0");
            getStatus("X0");
            getStatus("M112");
            //readPLC("D4", ref Param.D4);
            //readPLC("D7", ref Param.D7);
            //readPLC("D9", ref Param.D9);
            //readPLC("D11", ref Param.D11);
            //readPLC("D100", ref Param.D100);
            //readPLC("D101", ref Param.D101);
            timerMonitor.Start();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
           
            Form f = new MaintainRecord();
            //f.Show();
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure to quit?", "Quit Program", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }

            //check comport
            if (Param.Scanner_Use)
                closeSerialPort(spScanner);
            //MB_A 共同的
            if (Param.DUT_A_Use)
                closeSerialPort(spDUT_A);
            if (Param.MBType.ToUpper() == Param.mbType.Panel.ToString().ToUpper())
            {
                //双板需要判断MB_B
                if (Param.DUT_B_Use)
                    closeSerialPort(spDUT_B);
            }

            //mx 连接
            if (Param.PLC_Use)
                closePLC();

            //判断测试方式
            if (Param.TestingType.ToUpper() == Param.testingType.Auto.ToString().ToUpper())
                timerScanDatabase.Stop();

            timerDetect_V_A.Stop();
            timerDetect_V_B.Stop();
            timerScanFICT.Stop();
            PressStopButtonUI();

            loadData2UI();

        }

        private void 帮助ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            String Verinfo = string.Empty;
            Verinfo = "升级版本介绍" + "\n";
            Verinfo += "<1.0.7.0 >:" + "\n";
            Verinfo += "1.增加测试数据上抛至《记录数据库》" + "\n";
            Verinfo += "2.TestLog 中加入测试时间，MB Mode，MO，UPN等信息" + "\n";
            Verinfo += "3.对强抛MB板子进行计数和标记" + "\n";
            Verinfo += "4.测试电脑系统时间与记录数据库时间同步" + "\n";
            Verinfo += "5.加入部分菜单选项" + "\n";
            Verinfo += "<1.0.7.1 >:" + "\n";
            Verinfo += "1.修复超时时，直接退出不重测" + "\n";
            Verinfo += "<1.0.7.2 >:" + "\n";
            Verinfo += "1.修复串口编号易丢失情况，达到在测试过程中自修复" + "\n";
            Verinfo += "<1.0.7.0104 >:" + "\n";
            Verinfo += "1.加入暂存档，保存本机测试信息" + "\n";
            Verinfo += "<1.0.7.0109 >:" + "\n";
            Verinfo += "1.未成功上抛到记录数数据库的TestLog，暂存本地文档" + "\n";
            Verinfo += "2.修复PLC串口线掉落或移除后，不能自动打开" + "\n";
            Verinfo += "3.增加对记录数据库启用或关闭功能" + "\n";
            Verinfo += "4.增加部分关键设置防呆提醒窗口" + "\n";
            Verinfo += "<1.0.7.0115 >:" + "\n";
            Verinfo += "1.增加机台调试功能" + "\n";
            Verinfo += "2.修复串口掉落重新插上之后，另外两个串口编号混乱" + "\n";
            Verinfo += "3.增加本机台信息上抛至记录数据库，还不完善" + "\n";
            MessageBox.Show(Verinfo);
        }
  





    }
}
