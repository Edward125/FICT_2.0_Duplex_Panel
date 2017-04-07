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
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace FICT_2._0_Duplex_Panel
{
    public partial class SerialPortDebug : Form
    {
        public SerialPortDebug()
        {
            InitializeComponent();
        }

        //MB 串口參数
        string serialPort_A_Str = string.Empty; //DUT A 串口收到的数据
        string serialPort_A_Last_Str = string.Empty; //DUT A 串口上次收到的数据
        string serialPort_B_Str = string.Empty; //DUT B 串口收到的数据
        string serialPort_B_Last_Str = string.Empty; //DUT B 串口收到的数据

        private void SerialPortDebug_Load(object sender, EventArgs e)
        {
            TestOut.Text = "WAIT";
            getSerialPort(CB_SerialPort_A);
            getSerialPort(CB_SerialPort_B);
            CB_SerialPort_B.Items.ToString();
            //MessageBox.Show(CB_SerialPort_B.Items.ToString());

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
                    //SubFunction.updateMessage(lstStatusCommand, "Open SerialPort=" + portname + " success.");//Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Open SerialPort=" + portname + " success.\r\n");//Message:" + e.Message + "\r\n");
                }
                catch (Exception e)
                {
                    MessageBox.Show("Can't open SerialPort=" + portname + ",Message:" + e.Message);
                    //SubFunction.updateMessage(lstStatusCommand, "Can't open SerialPort=" + portname + ",Message:" + e.Message);
                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "Can't open SerialPort=" + portname + ",Message:" + e.Message + "\r\n");
                    Stop_Button();
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
                    //SubFunction.updateMessage(lstStatusCommand, "Can't close SerialPort=" + sp.PortName.ToString() + ",Message:" + e.Message);
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
                //MessageBox.Show("发送成功");
            }
            catch (Exception e)
            {
                //SubFunction.updateMessage(lstStatusCommand, "Send " + spport.PortName + " " + strdata + "fail");
                //SubFunction.updateMessage(lstStatusCommand, e.Message);
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

                                if (Btn_Test.Enabled)
                                {
                                    getSerialPort(CB_SerialPort_A);
                                    //MessageBox.Show(CB_SerialPort_A.Text);
                                    getSerialPort(CB_SerialPort_B);
                                    //getSerialPort(comboDUTPort_B);
                                    //getSerialPort(comboFICTPort);
                                }
                                else
                                {
                                    //sp = new System.Media.SoundPlayer(global::SpotTestTester.Properties.Resources.ng);
                                    //sp.Play();
                                    richTextBox_A.AppendText("串口调试： "+"串口编号" + portName + "已经离线"+ "\n");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试： " + "串口编号" + portName + "已经离线");
                                    //SubFunction.updateMessage(lstStatusCommand, "偵测到串口丟失，請重新设置后點擊开始，若无法啟动，點擊Restart再點擊Start。");
                                    //closePort(spPLC);
                                    //closePort(spSN);
                                    //pressStopButton();
                                  
                                }


                            }
                            catch (Exception ex)
                            {
                                //sp = new System.Media.SoundPlayer(global::SpotTestTester.Properties.Resources.ng);
                                //sp.Play();
                                richTextBox_A.AppendText("串口调试： " +"Error," + ex.Message+ "\n");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试： " + "Error," + ex.Message);
                            }
                            Console.WriteLine("Port '" + portName + "' leaved.");
                        }

                        break;
                    case DBT_DEVICEARRIVAL:             // USB插入获取对应串口名称
                        DEV_BROADCAST_HDR dbhdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                            getSerialPort(CB_SerialPort_A);
                            getSerialPort(CB_SerialPort_B);

                            richTextBox_A.AppendText("串口调试： 串口已检测到《" + portName+ "》..."  + "\n");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试： 串口已检测到《" + portName + "》...");
                            Console.WriteLine("Port '" + portName + "' arrived.");
                        }
                        break;
                }
            }
            base.WndProc(ref m);

        }
        #endregion


        private void CheckPort(ComboBox PortName ,Message m)
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

                                if (Btn_Test.Enabled)
                                {
                                    getSerialPort(PortName);
                                }
                                else
                                {
                                    //sp = new System.Media.SoundPlayer(global::SpotTestTester.Properties.Resources.ng);
                                    //sp.Play();
                                    richTextBox_A.AppendText("串口调试： " + "串口编号" + portName + "已经离线" + "\n");
                                    SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试： " + "串口编号" + portName + "已经离线");
                                    //SubFunction.updateMessage(lstStatusCommand, "偵测到串口丟失，請重新设置后點擊开始，若无法啟动，點擊Restart再點擊Start。");
                                    //closePort(spPLC);
                                    //closePort(spSN);
                                    //pressStopButton();

                                }


                            }
                            catch (Exception ex)
                            {
                                //sp = new System.Media.SoundPlayer(global::SpotTestTester.Properties.Resources.ng);
                                //sp.Play();
                                richTextBox_A.AppendText("串口调试： " + "Error," + ex.Message + "\n");
                                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试： " + "Error," + ex.Message);
                            }
                            Console.WriteLine("Port '" + portName + "' leaved.");
                        }

                        break;
                    case DBT_DEVICEARRIVAL:             // USB插入获取对应串口名称
                        DEV_BROADCAST_HDR dbhdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));
                        if (dbhdr.dbch_devicetype == DBT_DEVTYP_PORT)
                        {
                          
                                getSerialPort(CB_SerialPort_A);
                                getSerialPort(CB_SerialPort_B);

                            //getSerialPort(CB_SerialPort_A);
                            //getSerialPort(CB_SerialPort_B);


                            //MessageBox.Show(CB_SerialPort_A.Text);
                            //getSerialPort(comboDUTPort_B);
                            //getSerialPort(comboFICTPort);
                            richTextBox_A.AppendText("串口调试： 串口已检测到《" + portName + "》..." + "\n");
                            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试： 串口已检测到《" + portName + "》...");
                            Console.WriteLine("Port '" + portName + "' arrived.");
                        }
                        break;
                }
            }
            base.WndProc(ref m);
        }



        private void Btn_Refresh_Click(object sender, EventArgs e)
        {
            //加载串口
            getSerialPort(CB_SerialPort_A);
            getSerialPort(CB_SerialPort_B);
        }

        private void Stop_Button()
        {
            this.CB_SerialPort_A.Enabled = true;
            this.CB_SerialPort_B.Enabled = true;
            this.Btn_Stop.Enabled = false;
            this.Btn_Test.Enabled = true;
            this.Btn_Refresh.Enabled = true;
            richTextBox_A.AppendText("串口调试：准备关闭串口A..." + "\n");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：准备关闭串口A....");
            closeSerialPort(serialPort_A);
            richTextBox_B.AppendText("串口调试：准备关闭串口B..." + "\n");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：准备关闭串口B....");
            closeSerialPort(serialPort_B);
        }


        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Stop_Button();
        }

        private void Btn_Test_Click(object sender, EventArgs e)
        {

            TestOut.Text =string.Empty ;
            Delay(50);
            richTextBox_A.Clear();
            richTextBox_B.Clear();
            this.CB_SerialPort_A.Enabled = false;
            this.CB_SerialPort_B.Enabled = false;
            this.Btn_Stop.Enabled = true;
            this.Btn_Test.Enabled = false;
            this.Btn_Refresh.Enabled = false;

            richTextBox_A.AppendText("串口调试：准备打开串口A..."+"\n");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：准备打开串口A....");
            if (!string.IsNullOrEmpty(CB_SerialPort_A.Text))
            {
                if (!openSerialPort(serialPort_A, CB_SerialPort_A.Text))
                    return;
            }
            richTextBox_B.AppendText("串口调试：准备打开串口B..." + "\n");

            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：准备打开串口B....");
            if (!string.IsNullOrEmpty(CB_SerialPort_B.Text))
            {
                if (!openSerialPort(serialPort_B, CB_SerialPort_B.Text))
                    return;
            }

            sendData(serialPort_A, "AAA");
            richTextBox_A.AppendText("串口调试：向B串口发送<AAA>字符串..." + "\n");
            sendData(serialPort_B, "BBB");
            richTextBox_B.AppendText("串口调试：向A串口发送<BBB>字符串..." + "\n");

            timer1.Start();
        }
        /// <summary>
        /// 延时子程序
        /// </summary>
        /// <param name="interval">延时的时間，单位毫秒</param>
        private void Delay(double interval)
        {
            DateTime time = DateTime.Now;
            double span = interval * 10000;
            while (DateTime.Now.Ticks - time.Ticks < span)
            {
                Application.DoEvents();
            }

        }
        private void serialPort_A_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort_A.BytesToRead == 0)
                return;
            Delay(50);
            string temp_A = string.Empty;
            serialPort_A_Str = serialPort_A.ReadExisting().Trim();
            temp_A = serialPort_A_Str;
            string spString = serialPort_A_Str;
            serialPort_A.DiscardInBuffer();//清空buffer
            this.Invoke((EventHandler)(delegate
            {
                richTextBox_A.AppendText("串口调试：串口A已接收到:" + serialPort_A_Str + "\n");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：串口A已接收到:" + serialPort_A_Str);
            }));



            if (TestOut.Text == "PASS")
            {
                serialPort_A_Str = string.Empty;
            }
        }

        private void serialPort_B_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort_B.BytesToRead == 0)
                return;
            Delay(50);
            string temp_B = string.Empty;
            serialPort_B_Str = serialPort_B.ReadExisting().Trim();
            temp_B = serialPort_B_Str;
            string spString = serialPort_B_Str;
            serialPort_B.DiscardInBuffer();//清空buffer
            this.Invoke((EventHandler)(delegate
            {
                richTextBox_B.AppendText("串口调试：串口B已接收到:" + serialPort_B_Str + "\n");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：串口B已接收到:" + serialPort_B_Str);
            }));


            if (TestOut.Text == "PASS")
            {
                serialPort_B_Str = string.Empty;
            }
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //MessageBox.Show(serialPort_B_Str);

            if ((serialPort_B_Str == "AAA") && (serialPort_A_Str == "BBB"))
            {
                timer1.Stop();
                TestOut.Text = "PASS";
                TestOut.BackColor = Color.Green;
                richTextBox_A.AppendText("串口测试OK，可以直接拔掉..." + "\n");
                richTextBox_B.AppendText("串口测试OK，可以直接拔掉..." + "\n");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口测试OK，可以直接拔掉" + "\n");
                Stop_Button();
                serialPort_B_Str = string.Empty;
                serialPort_A_Str = string.Empty;
            }
            else
            {
                timer1.Stop();
                TestOut.Text = "FAIL";
                TestOut.BackColor = Color.Red;
                richTextBox_A.AppendText("串口测试NG，请检测串口回路..." + "\n");
                richTextBox_B.AppendText("串口测试NG，请检测串口回路..." + "\n");
                SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口测试NG，请检测串口回路..." + "\n");
                Stop_Button();
            }
        }


        private void SerialPortDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "机台调试:点击窗口右上角 <关闭> 按钮...");
            richTextBox_A.AppendText("串口调试：准备关闭串口A..." + "\n");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：准备关闭串口A....");
            closeSerialPort(serialPort_A);
            richTextBox_B.AppendText("串口调试：准备关闭串口B..." + "\n");
            SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "串口调试：准备关闭串口B....");
            closeSerialPort(serialPort_B);
        }

    }
}
