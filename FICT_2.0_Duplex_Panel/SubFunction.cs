using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Edward;



namespace FICT_2._0_Duplex_Panel
{
    class SubFunction
    {
        #region 配置档的操作
        /// <summary>
        /// 创建INI配置档，并初始化值
        /// </summary>
        /// <param name="inifilepath">ini配置档地址</param>
        public static void creatInI(string inifilepath)
        {
            FileStream fs = File.Create(@inifilepath);
            fs.Close();
            try
            {
                IniFile.IniWriteValue("SysConfig", "SysVersion", Application.ProductVersion, @inifilepath);
                IniFile.IniWriteValue("SysConfig", "FICTStage", Param.fictStage.A.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "FixtureID", string.Empty, @inifilepath);
                IniFile.IniWriteValue("SysConfig", "PCBLine", Param.pcbLine.AP2.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "TestingType", Param.testingType.Auto.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "MBType", Param.mbType.Panel.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "RobotModule", "1", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "BarcodeType", Param.barcodeType.A.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "MaxErrorCount", "3", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "MaxRetestCount", "2", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "LeftInsert", "1", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "RightInsert", "1", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "RTC_Use", "0", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "DetectDelay", Param.DetectDelay.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "ShutDown", Param.ShutDown.ToString(), @inifilepath);
                IniFile.IniWriteValue("SysConfig", "LeftInsertRe", "0", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "RightInsertRe", "0", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "UseCommand", "0", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "InsertDelay", "1", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "NG_Stop", "1", @inifilepath); //add by channing 20161101
                IniFile.IniWriteValue("SysConfig", "ST_Flag", "0", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "AD_Module_Type", Param.AD_Module_Type, @inifilepath);
                IniFile.IniWriteValue("SysConfig", "MA_A_Re", "1", @inifilepath);
                IniFile.IniWriteValue("SysConfig", "MA__Re", "1", @inifilepath);
                //SFCS_Set
                IniFile.IniWriteValue("SFCS_Set", "CheckRouter", "0", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Web_Use", "1", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Web_Site", "http://172.0.1.172/Tester.WebService/WebService.asmx", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "OKBBBStatus", "1", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "NGBBBStatus", "1", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "SFC_Stage", "TD", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "OPID", string.Empty, @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Arms_Use", "1", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Arms_Version", "1.00.002", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Arms_Path", @"M:\SFCS\PCBARMS\", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Net_Server", @"\\172.0.1.161\netware\message\", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Net_ID", "Admin", @inifilepath);
                IniFile.IniWriteValue("SFCS_Set", "Net_Password", "ndk800", @inifilepath);

                IniFile.IniWriteValue("DB_Set", "Center_DataBase_Use", "1", @inifilepath);//是否启用中控数据库
                IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_IP", "127.0.0.1", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_DB", "test", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_Table", "fict", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_Account", "root", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "CenterIP_DataBase_Password", "123456", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "Record_DataBase_Use", "0", @inifilepath);//是否启用记录数据库
                IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_IP", "172.0.1.90", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_DB", "fictsninfo", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_TestInfo_Table", "testinfo", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_Station_Table", "stationinfo", @inifilepath);          
                IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_Account", "root", @inifilepath);
                IniFile.IniWriteValue("DB_Set", "RecordIP_DataBase_Password", "123456", @inifilepath);

                IniFile.IniWriteValue("ComPort_Set", "Scanner_Use", "0", @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "Scanner", string.Empty, @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "DUT_A_Use", "1", @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "DUT_A", string.Empty, @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "DUT_B_Use", "1", @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "DUT_B", string.Empty, @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "PLC_Use", "1", @inifilepath);
                IniFile.IniWriteValue("ComPort_Set", "PLC", String.Empty, @inifilepath);

                IniFile.IniWriteValue("TimeOut_Set", "Wait_MB_TimeOut", "300", @inifilepath);  //待板超时
                IniFile.IniWriteValue("TimeOut_Set", "In_Time_TimeOut", "30", @inifilepath);  //进板超时
                IniFile.IniWriteValue("TimeOut_Set", "PowerONTimeOut", "120", @inifilepath);  //开机超时
                IniFile.IniWriteValue("TimeOut_Set", "TestOKTimeOut", "180", @inifilepath);  //测试超时
                IniFile.IniWriteValue("TimeOut_Set", "Out_Time_TimeOut", "30", @inifilepath);  //退出超时
                IniFile.IniWriteValue("TimeOut_Set", "Test_PeriodCycle_TimeOut", "3000", @inifilepath); //单板测试周期超时
                saveLog(Param.logType.SYSLOG.ToString(), "Create .INI file.");
            }
            catch (Exception ex)
            {
                saveLog(Param.logType.SYSLOG.ToString(), "配置档写入出错，，请检查配置档，error：" + ex.Message);
                MessageBox.Show("配置档写入出错，，请检查配置档，error：" + ex.Message);
            }
        }

        /// <summary>
        /// 从配置档中读取数据
        /// </summary>
        /// <param name="inifilepath"></param>
        public static void loadConfigData(string inifilepath)
        {
            try
            {
                //sysconfig
                Param.SysVersion = IniFile.IniReadValue("SysConfig", "SysVersion", @inifilepath);

                //配置档升级

                Int32 ini_Version = Convert.ToInt32(Param.SysVersion.ToString().Replace(".", ""));  //将版本号转换成整数
                Int32 Soft_Version = Convert.ToInt32(Application.ProductVersion.ToString().Replace(".", ""));
                if (ini_Version > Soft_Version)
                {
                    saveLog(Param.logType.SYSLOG.ToString(), "配置档版本高于软件版本...");
                    MessageBox.Show("配置档版本高于软件版本：");
                }
                if (ini_Version > Soft_Version)
                {
                    saveLog(Param.logType.SYSLOG.ToString(), "配置档版本与软件版本匹配...");
                }
                if (ini_Version < Soft_Version)
                {
                    saveLog(Param.logType.SYSLOG.ToString(), "配置档版本低于软件版本...");
                    DialogResult MsgBoxResult;//设置对话框返回值
                    string MEG = string.Empty;
                    MEG += "配置档版本：" + Param.SysVersion.ToString() + "\n";
                    MEG += "软件版本：" + Application.ProductVersion.ToString() + "\n";
                    MEG += "因版本不匹配，你是否确定要更新配置档版本？" + "\n";
                    MEG += "\n";
                    MEG += "注意：" + "\n" + "1.不更新,仅变更配置档版本号，可能引发参数调用错误" + "\n";
                    MEG += "2.更新后,可能有必要修改部分参数！！！" + "\n";
              
                    MEG += "\n";
                    MEG += "是点击“Yes”，否点击“NO”" + "\n";
                    MsgBoxResult = MessageBox.Show(MEG, "小心操作！！！", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                    if (MsgBoxResult == DialogResult.Yes)
                    {
                        File.Delete(@inifilepath);//删除配置档
                        if (!File.Exists(Param.IniFilePath))
                            SubFunction.creatInI(Param.IniFilePath);
                        SubFunction.loadConfigData(Param.IniFilePath);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "完成配置档更新...");
                    }
                    if (MsgBoxResult == DialogResult.No)
                    {
                        IniFile.IniWriteValue("SysConfig", "SysVersion", Application.ProductVersion, @inifilepath);
                        SubFunction.saveLog(Param.logType.SYSLOG.ToString(), "取消配置档更新，仅更新配置档版本号...");
                        //Application.Exit();
                    }
                }

                Param.FICTStage = IniFile.IniReadValue("SysConfig", "FICTStage", @inifilepath);
                Param.FixtrueID = IniFile.IniReadValue("SysConfig", "FixtureID", @inifilepath);
                Param.PCBLine = IniFile.IniReadValue("SysConfig", "PCBLine", @inifilepath);
                Param.TestingType = IniFile.IniReadValue("SysConfig", "TestingType", @inifilepath);
                Param.MBType = IniFile.IniReadValue("SysConfig", "MBType", @inifilepath);
                Param.RobotModule = IniFile.IniReadValue("SysConfig", "RobotModule", @inifilepath);
                Param.BarcodeType = IniFile.IniReadValue("SysConfig", "BarcodeType", @inifilepath);
                Param.MaxErrorCount = Convert.ToInt16(IniFile.IniReadValue("SysConfig", "MaxErrorCount", @inifilepath));
                Param.MaxRetestCount = Convert.ToInt16(IniFile.IniReadValue("SysConfig", "MaxRetestCount", @inifilepath));
                changeString2Bool(IniFile.IniReadValue("SysConfig", "LeftInsert", @inifilepath), ref Param.LeftInsert);
                changeString2Bool(IniFile.IniReadValue("SysConfig", "RightInsert", @inifilepath), ref Param.RightInsert);
                changeString2Bool(IniFile.IniReadValue("SysConfig", "RTC_Use", @inifilepath), ref Param.RTC_Use);
                //MessageBox.Show(IniFile.IniReadValue("SysConfig", "DetectDelay", @inifilepath));
                Param.DetectDelay = Convert.ToInt16(IniFile.IniReadValue("SysConfig", "DetectDelay", @inifilepath));
                Param.ShutDown = Convert.ToInt16(IniFile.IniReadValue("SysConfig", "ShutDown", @inifilepath));
                changeString2Bool(IniFile.IniReadValue("SysConfig", "LeftInsertRe", @inifilepath), ref Param.LeftInsertRe);
                changeString2Bool(IniFile.IniReadValue("SysConfig", "RightInsertRe", @inifilepath), ref Param.RightInserRe);
                changeString2Bool(IniFile.IniReadValue("SysConfig", "UseCommand", @inifilepath), ref Param.UseCommand);
                //IniFile.IniWriteValue("SysConfig", "InsertDelay", "5", @inifilepath);
                //IniFile.IniWriteValue("SysConfig", "ST_Flag", "0", @inifilepath);
                if (!string.IsNullOrEmpty(IniFile.IniReadValue("SysConfig", "InsertDelay", @inifilepath)))
                    Param.InsertDelay = Convert.ToInt16(IniFile.IniReadValue("SysConfig", "InsertDelay", @inifilepath));

                if (string.IsNullOrEmpty(IniFile.IniReadValue("SysConfig", "NG_Stop", @inifilepath)))
                    changeString2Bool(IniFile.IniReadValue("SysConfig", "NG_Stop", @inifilepath), ref Param.NG_Stop);

                if (string.IsNullOrEmpty(IniFile.IniReadValue("SysConfig", "ST_Flag", @inifilepath)))
                    changeString2Bool(IniFile.IniReadValue("SysConfig", "ST_Flag", @inifilepath), ref Param.ST_Flag);

                if (!string.IsNullOrEmpty(IniFile.IniReadValue("SysConfig", "AD_Module_Type", @inifilepath).Trim()))
                {
                    Param.AD_Module_Type = IniFile.IniReadValue("SysConfig", "AD_Module_Type", @inifilepath).ToUpper();
                }

                if (!string.IsNullOrEmpty(IniFile.IniReadValue("SysConfig", "MB_A_Re", @inifilepath)))
                    changeString2Bool(IniFile.IniReadValue("SysConfig", "MB_A_Re", @inifilepath), ref Param.MB_A_Re);
                if (!string.IsNullOrEmpty(IniFile.IniReadValue("SysConfig", "MB_B_Re", @inifilepath)))
                    changeString2Bool(IniFile.IniReadValue("SysConfig", "MB_B_Re", @inifilepath), ref Param.MB_B_Re);

                //sfcs_set
                changeString2Bool(IniFile.IniReadValue("SFCS_Set", "CheckRouter", @inifilepath), ref Param.CheckRouter);
                changeString2Bool(IniFile.IniReadValue("SFCS_Set", "Web_Use", @inifilepath), ref Param.Web_Use);
                Param.Web_Site = IniFile.IniReadValue("SFCS_Set", "Web_Site", @inifilepath);
                changeString2Bool(IniFile.IniReadValue("SFCS_Set", "OKBBBStatus", @inifilepath), ref Param.OKBBBStatus);
                changeString2Bool(IniFile.IniReadValue("SFCS_Set", "NGBBBStatus", @inifilepath), ref Param.NGBBBStatus);
                Param.SFC_Stage = IniFile.IniReadValue("SFCS_Set", "SFC_Stage", @inifilepath);
                Param.OPID = IniFile.IniReadValue("SFCS_Set", "OPID", @inifilepath);
                changeString2Bool(IniFile.IniReadValue("SFCS_Set", "Arms_Use", @inifilepath), ref Param.Arms_Use);
                Param.Arms_Version = IniFile.IniReadValue("SFCS_Set", "Arms_Version", @inifilepath);
                Param.Arms_Path = IniFile.IniReadValue("SFCS_Set", "Arms_Path", @inifilepath);
                Param.Net_Server = IniFile.IniReadValue("SFCS_Set", "Net_Server", @inifilepath);
                Param.Net_ID = IniFile.IniReadValue("SFCS_Set", "Net_ID", @inifilepath);
                Param.Net_Password = IniFile.IniReadValue("SFCS_Set", "Net_Password", @inifilepath);
                //db_set

                changeString2Bool(IniFile.IniReadValue("DB_Set", "Center_DataBase_Use", @inifilepath), ref Param.Center_DataBase_Use);
                Param.CenterIP_DataBase_IP = IniFile.IniReadValue("DB_Set", "CenterIP_DataBase_IP", @inifilepath);
                Param.CenterIP_DataBase_DB = IniFile.IniReadValue("DB_Set", "CenterIP_DataBase_DB", @inifilepath);
                Param.CenterIP_DataBase_Table = IniFile.IniReadValue("DB_Set", "CenterIP_DataBase_Table", @inifilepath);
                Param.CenterIP_DataBase_Account = IniFile.IniReadValue("DB_Set", "CenterIP_DataBase_Account", @inifilepath);
                Param.CenterIP_DataBase_Password = IniFile.IniReadValue("DB_Set", "CenterIP_DataBase_Password", @inifilepath);

                changeString2Bool(IniFile.IniReadValue("DB_Set", "Record_DataBase_Use", @inifilepath), ref Param.Record_DataBase_Use);
                Param.RecordIP_DataBase_IP = IniFile.IniReadValue("DB_Set", "RecordIP_DataBase_IP", @inifilepath);
                Param.RecordIP_DataBase_DB = IniFile.IniReadValue("DB_Set", "RecordIP_DataBase_DB", @inifilepath);
                Param.RecordIP_DataBase_TestInfo_Table = IniFile.IniReadValue("DB_Set", "RecordIP_DataBase_TestInfo_Table", @inifilepath);
                Param.RecordIP_DataBase_Station_Table = IniFile.IniReadValue("DB_Set", "RecordIP_DataBase_Station_Table", @inifilepath);
                Param.RecordIP_DataBase_Account = IniFile.IniReadValue("DB_Set", "RecordIP_DataBase_Account", @inifilepath);
                Param.RecordIP_DataBase_Password = IniFile.IniReadValue("DB_Set", "RecordIP_DataBase_Password", @inifilepath);

                //comport
                changeString2Bool(IniFile.IniReadValue("ComPort_Set", "Scanner_Use", @inifilepath), ref Param.Scanner_Use);
                Param.Scanner = IniFile.IniReadValue("ComPort_Set", "Scanner", @inifilepath);
                changeString2Bool(IniFile.IniReadValue("ComPort_Set", "DUT_A_Use", @inifilepath), ref Param.DUT_A_Use);
                Param.DUT_A = IniFile.IniReadValue("ComPort_Set", "DUT_A", @inifilepath);
                changeString2Bool(IniFile.IniReadValue("ComPort_Set", "DUT_B_Use", @inifilepath), ref Param.DUT_B_Use);
                Param.DUT_B = IniFile.IniReadValue("ComPort_Set", "DUT_B", @inifilepath);
                changeString2Bool(IniFile.IniReadValue("ComPort_Set", "PLC_Use", @inifilepath), ref Param.PLC_Use);
                Param.PLC = IniFile.IniReadValue("ComPort_Set", "PLC", @inifilepath);
                
                //timeout
                Param.Wait_MB_TimeOut = Convert.ToInt16(IniFile.IniReadValue("TimeOut_Set", "Wait_MB_TimeOut", @inifilepath));
                Param.In_Time_TimeOut = Convert.ToInt16(IniFile.IniReadValue("TimeOut_Set", "In_Time_TimeOut", @inifilepath));
                Param.PowerONTimeOut = Convert.ToInt16(IniFile.IniReadValue("TimeOut_Set", "PowerONTimeOut", @inifilepath));
                Param.TestOKTimeOut = Convert.ToInt16(IniFile.IniReadValue("TimeOut_Set", "TestOKTimeOut", @inifilepath));
                Param.Out_Time_TimeOut = Convert.ToInt16(IniFile.IniReadValue("TimeOut_Set", "Out_Time_TimeOut", @inifilepath));
                Param.Test_PeriodCycle_TimeOut = Convert.ToInt16(IniFile.IniReadValue("TimeOut_Set", "Test_PeriodCycle_TimeOut", @inifilepath));

                //连接字符串
                Param.Center_DB_ConnStr = "server=" + Param.CenterIP_DataBase_IP + ";user id=" + Param.CenterIP_DataBase_Account + ";password=" + Param.CenterIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.CenterIP_DataBase_DB;
                Param.Record_DB_ConnStr = "server=" + Param.RecordIP_DataBase_IP + ";user id=" + Param.RecordIP_DataBase_Account + ";password=" + Param.RecordIP_DataBase_Password + ";persistsecurityinfo=True;database=" + Param.RecordIP_DataBase_DB;
            }
            catch (Exception ex)
            {
                saveLog(Param.logType.SYSLOG.ToString(), "配置档读取出错，请检查配置档，error：" + ex.Message);
                MessageBox.Show("配置档读取出错，，请检查配置档，error：" + ex.Message);
            }



        }


        /// <summary>
        /// 将字符串的1,0转换成bool，1=true,0=fales
        /// </summary>
        /// <param name="invalue">输入的值</param>
        /// <param name="outvalue">输出的值</param>
        public static void changeString2Bool(string invalue, ref bool outvalue)
        {
            if (invalue.Trim() == "1")
                outvalue = true;
            if (invalue.Trim() == "0")
                outvalue = false;
        }


        /// <summary>
        /// 创建临时统计数据储存文件
        /// </summary>
        /// <param name="Tempfilepath">文件路径</param>
        public static void CreatTemp(string Tempfilepath)
        {
            FileStream fs = File.Create(@Tempfilepath);
            fs.Close();
            try
            {
                IniFile.IniWriteValue("MB_A", "iPass_A", "0", @Tempfilepath);
                IniFile.IniWriteValue("MB_A", "iFail_A", "0", @Tempfilepath);
                IniFile.IniWriteValue("MB_A", "iFail_Pass_A", "0", @Tempfilepath);

                IniFile.IniWriteValue("MB_B", "iPass_B", "0", @Tempfilepath);
                IniFile.IniWriteValue("MB_B", "iFail_B", "0", @Tempfilepath);
                IniFile.IniWriteValue("MB_B", "iFail_Pass_B", "0", @Tempfilepath);

                IniFile.IniWriteValue("MB_A_B", "iPass", "0", @Tempfilepath);
                IniFile.IniWriteValue("MB_A_B", "iFail", "0", @Tempfilepath);
                IniFile.IniWriteValue("MB_A_B", "iFail_Pass", "0", @Tempfilepath);
            }
            catch (Exception ex)
            {
                saveLog(Param.logType.SYSLOG.ToString(), "创建临时统计数据出错...请检查，error：" + ex.Message);
                saveLog(Param.logType.SYSLOG.ToString(), "路径：" + @Tempfilepath);
                MessageBox.Show("创建临时统计数据出错...请检查，error：" + ex.Message);
            }
               
        }

        public static void loadTemp(string Tempfilepath)
        {
            try
            {
                //TestInfo
                Param.iPass_A = Convert.ToInt16(IniFile.IniReadValue("MB_A", "iPass_A", @Tempfilepath));
                Param.iFail_A = Convert.ToInt16(IniFile.IniReadValue("MB_A", "iFail_A", @Tempfilepath));
                Param.iFail_Pass_A = Convert.ToInt16(IniFile.IniReadValue("MB_A", "iFail_Pass_A", @Tempfilepath));

                Param.iPass_B = Convert.ToInt16(IniFile.IniReadValue("MB_B", "iPass_B", @Tempfilepath));
                Param.iFail_B = Convert.ToInt16(IniFile.IniReadValue("MB_B", "iFail_B", @Tempfilepath));
                Param.iFail_Pass_B = Convert.ToInt16(IniFile.IniReadValue("MB_B", "iFail_Pass_B", @Tempfilepath));

                Param.iPass = Convert.ToInt16(IniFile.IniReadValue("MB_A_B", "iPass", @Tempfilepath));
                Param.iFail = Convert.ToInt16(IniFile.IniReadValue("MB_A_B", "iFail", @Tempfilepath));
                Param.iFail_Pass = Convert.ToInt16(IniFile.IniReadValue("MB_A_B", "iFail_Pass", @Tempfilepath));
            }
            catch (Exception ex)
            {
                saveLog(Param.logType.SYSLOG.ToString(), "加载临时统计数据出错...请检查，error：" + ex.Message);
                saveLog(Param.logType.SYSLOG.ToString(), "路径：" + @Tempfilepath);
                MessageBox.Show("加载临时统计数据出错...请检查，error：" + ex.Message);
            }
        }

        public static void WriteTemp(string Tempfilepath)
        {
            FileStream fs = File.Create(@Tempfilepath);
            fs.Close();
            try
            {
                IniFile.IniWriteValue("MB_A", "iPass_A", Param.iPass_A.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A", "iFail_A", Param.iFail_A.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A", "iFail_Pass_A", Param.iFail_Pass_A.ToString(), @Tempfilepath);

                IniFile.IniWriteValue("MB_B", "iPass_B", Param.iPass_B.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_B", "iFail_B", Param.iFail_B.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_B", "iFail_Pass_B", Param.iFail_Pass_B.ToString(), @Tempfilepath);

                IniFile.IniWriteValue("MB_A_B", "iPass", Param.iPass.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A_B", "iFail", Param.iFail.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A_B", "iFail_Pass", Param.iFail_Pass.ToString(), @Tempfilepath);
            }
            catch (Exception ex)
            {
                saveLog(Param.logType.SYSLOG.ToString(), "创建临时统计数据出错...请检查，error：" + ex.Message);
                saveLog(Param.logType.SYSLOG.ToString(), "路径：" + @Tempfilepath);
                MessageBox.Show("创建临时统计数据出错...请检查，error：" + ex.Message);
            }
               
        }

        public static void CleanTemp(string Tempfilepath)
        {

            Param.iPass_A = 0;
            Param.iFail_A=0;
            Param.iFail_Pass_A = 0;

            Param.iPass_B = 0;
            Param.iFail_B = 0;
            Param.iFail_Pass_B = 0;

            Param.iPass = 0;
            Param.iFail = 0;
            Param.iFail_Pass = 0;

            FileStream fs = File.Create(@Tempfilepath);
            fs.Close();

            try
            {
                IniFile.IniWriteValue("MB_A", "iPass_A", Param.iPass_A.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A", "iFail_A", Param.iFail_A.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A", "iFail_Pass_A", Param.iFail_Pass_A.ToString(), @Tempfilepath);

                IniFile.IniWriteValue("MB_B", "iPass_B", Param.iPass_B.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_B", "iFail_B", Param.iFail_B.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_B", "iFail_Pass_B", Param.iFail_Pass_B.ToString(), @Tempfilepath);

                IniFile.IniWriteValue("MB_A_B", "iPass", Param.iPass.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A_B", "iFail", Param.iFail.ToString(), @Tempfilepath);
                IniFile.IniWriteValue("MB_A_B", "iFail_Pass", Param.iFail_Pass.ToString(), @Tempfilepath);
            }
            catch (Exception ex)
            {
                saveLog(Param.logType.SYSLOG.ToString(), "创建临时统计数据出错...请检查，error：" + ex.Message);
                saveLog(Param.logType.SYSLOG.ToString(), "路径：" + @Tempfilepath);
                MessageBox.Show("创建临时统计数据出错...请检查，error：" + ex.Message);
            }

        }


        #endregion

        #region 更新計算良率
        /// <summary>
        /// 更新計算良率
        /// </summary>
        /// <param name="txttotal">显示测试条码數的box</param>
        /// <param name="txtpass">显示测试pass的条码數</param>
        /// <param name="txtfail">显示测试fail的条码數</param>
        /// <param name="txtfpy">显示fpy</param>
        /// <param name="ipass">pass數量</param>
        /// <param name="ifail">fail數量</param>
        //public static void updateFPY(TextBox txttotal,
        //                             TextBox txtpass,
        //                             TextBox txtfail,
        //                             TextBox txtfpy,
        //                             int ipass,
        //                             int ifail)
        //{
        //    int itotal = ipass + ifail;
        //    txttotal.Text = ipass.ToString();
        //    txtpass.Text = ipass.ToString();
        //    txtfail.Text = ifail.ToString();

        //    if ((ipass + ifail) == 0)
        //        txtfpy.Text = "0%";
        //    else
        //    {
        //        Int32 temp = Convert.ToInt32(((ipass / (ipass + ifail)) * 10000));
        //        int dfpy = temp / 100;
        //        txtfpy.Text = dfpy.ToString() + "%";
        //    }
        //}


        /// <summary>
        /// 更新計算良率
        /// </summary>
        /// <param name="txttotal">显示测试条码數的box</param>
        /// <param name="txtpass">显示测试pass的条码數</param>
        /// <param name="txtfail">显示测试fail的条码數</param>
        /// <param name="txtfpy">显示fpy</param>
        /// <param name="ipass">pass數量</param>
        /// <param name="ifail">fail數量</param>
        public static void updateFPY(TextBox txttotal,
                                     TextBox txtpass,
                                     TextBox txtfail,
                                     TextBox txtfpy,
                                     int ipass,
                                     int ifail)
        {
            int itotal = ipass + ifail;
            txttotal.Text = itotal.ToString ();
            txtpass.Text = ipass.ToString();
            txtfail.Text = ifail.ToString();

            if ((ipass + ifail) == 0)
                txtfpy.Text = "0%";
            else
            {
               // Int32 temp = Convert.ToInt32(((ipass / (ipass + ifail)) * 10000));
                Int32 temp = Convert.ToInt32(( (double)ipass / (double)itotal) * 10000);
                int dfpy = temp / 100;
                txtfpy.Text = dfpy.ToString() + "%";
            }


        }

        /// <summary>
        /// 更新計算良率
        /// </summary>
        /// <param name="txttotal">显示测试条码數的box</param>
        /// <param name="txtpass">显示测试pass的条码數</param>
        /// <param name="txtfail">显示测试fail的条码數</param>
        /// <param name="txtfpy">显示fpy</param>
        /// <param name="ipass">pass數量</param>
        /// <param name="ifail">fail數量</param>
        public static void updateFPY(TextBox txttotal,
                                     TextBox txtpass,
                                     TextBox txtfail,
                                     TextBox txtfpy,
                                     TextBox txtfp,
                                     TextBox txtfpt_fpy,
                                     int ipass,
                                     int ifail,
                                     int ifpt
                                    )
        {
            int itotal = ipass + ifail;
            txttotal.Text = itotal.ToString();
            txtpass.Text = ipass.ToString();
            txtfail.Text = ifail.ToString();
            txtfp.Text = ifpt.ToString();

            if ((ipass + ifail) == 0)
            {
                txtfpy.Text = "0%";
                txtfpt_fpy.Text = "0%";
            }
            else
            {
                // Int32 temp = Convert.ToInt32(((ipass / (ipass + ifail)) * 10000));
                Int32 temp = Convert.ToInt32(((double)ipass / (double)itotal) * 10000);
                int dfpy = temp / 100;
                txtfpy.Text = dfpy.ToString() + "%";

                Int32 tempfp = Convert.ToInt32(((double)ifpt / (double)itotal) * 10000);
                int fpfpy = tempfp / 100;
                txtfpt_fpy.Text = fpfpy.ToString() + "%";
            }
        }
        #endregion

        #region 更新信息
        /// <summary>
        /// 更新信息到listbox中
        /// </summary>
        /// <param name="listbox">listbox name</param>
        /// <param name="message">message</param>
        public static void updateMessage(ListBox  listbox, string message)
        {
            if (listbox.Items.Count > 1000)
                listbox.Items.RemoveAt(0);
 
            string item = string.Empty;
            item = DateTime.Now.ToString("HH:mm:ss") + " " + @message;
            listbox.Items.Add(item);
            if (listbox.Items.Count > 1)
            {
                listbox.TopIndex = listbox.Items.Count - 1;
                listbox.SetSelected(listbox.Items.Count - 1, true);
            }
        }
        #endregion

        #region 保存log
        /// <summary>
        /// 保存log
        /// </summary>
        /// <param name="logtype">log類型</param>
        /// <param name="logcontents">log內容</param>
        public static void saveLog(string logtype, string logcontents)
        {
            //根据logtype获取对应的文件路徑以及文件名
            string logpath = string.Empty;
            if (logtype.ToUpper() == Param.logType.COMLOG.ToString())
                logpath = Param.comLogFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (logtype.ToUpper() == Param.logType.SYSLOG.ToString())
                logpath = Param.sysLogFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            if (logtype.ToUpper() == Param.logType.ANALOG.ToString())
                logpath = Param.anaLogFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            //判斷文件是否存在，不存在就创建文件，存在就写入文件
            if (!File.Exists(@logpath))
            {
                FileStream fs = File.Create(@logpath);
                fs.Close();
            }
            else
            {
                try
                {
                    File.AppendAllText(@logpath, DateTime.Now.ToString("yyyyMMddHHmmss") + " " + @logcontents + "\r\n");
                }
                catch (Exception)
                {
                    //wait

                }
            }

        }

        /// <summary>
        /// 保存log
        /// </summary>
        /// <param name="usn">条码</param>
        /// <param name="testresult">测试結果，PASS,FAIL</param>
        /// <param name="bretest">重複测试的标记，重複测试=true，沒重複测试=false</param>
        /// <param name="bsfcsresult">上拋SFCS的結果,成功=true，失敗=false</param>
        /// <param name="testitem">测试項目</param>
        public static void saveLog(string usn, string testresult, bool bretest, bool bsfcsresult, string testitem)
        {
            //Dim logFormat = "TestTime,PPID,TestResult,ReTestFlag,SFCSFlag,TestInfo"
            //checkfolder
            string testfolder = @Param.appFolder + @"\Test_Log";
            if (!Directory.Exists(testfolder))
                Directory.CreateDirectory(testfolder);
            string testlogpath = @testfolder + @"\" + @DateTime.Now.ToString("yyyyMMdd") + @".log";
            if (!File.Exists(testlogpath))
            {
                FileStream fs = File.Create(testlogpath);
                fs.Close();
            }
            string logcontents = DateTime.Now.ToString("yyyyMMddHHmmss") + "," + usn + "," + testresult + "," +
                bretest.ToString() + "," + bsfcsresult.ToString() + "," + testitem + "\r\n";
            File.AppendAllText(testlogpath, @logcontents);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="usn">条码</param>
        /// <param name="testresult">测试結果，PASS,FAIL</param>
        /// <param name="bretest">重複测试的标记，重複测试=true，沒重複测试=false</param>
        /// <param name="bsfcsresult">上拋SFCS的結果,成功=true，失敗=false</param>
        /// <param name="testitem">测试項目</param>
        /// <param name="T_Model">测试机种名字</param>
        /// <param name="T_ModelFamily"></param>
        /// <param name="T_MO">工单</param>
        /// <param name="T_UPN">料号</param>
        /// <param name="T_TestTime">测试时间</param>
        /// <param name="T_TestStation">站别</param>
        public static void SaveTestLog(string usn, string testresult, bool bretest, bool bsfcsresult, string testitem, string T_Model, string T_ModelFamily, string T_MO, string T_UPN, string T_TestTime, string T_TestStation)
        {

            //Dim logFormat = "TestTime,PPID,TestResult,ReTestFlag,SFCSFlag,TestInfo"
            //checkfolder
            string testfolder = @Param.appFolder + @"\Test_Log";
            if (!Directory.Exists(testfolder))
                Directory.CreateDirectory(testfolder);
            string testlogpath = @testfolder + @"\" + @DateTime.Now.ToString("yyyyMMdd") + @".log";
            if (!File.Exists(testlogpath))
            {
                FileStream fs = File.Create(testlogpath);
                fs.Close();
                string First = "RecordTime,PPID,TestResult,ReTestFlag,SFCSFlag,TestInfo,Model,ModelFamily,MO,UPN,TestTime,TestStation" + "\r\n";   //add by channing Wang
                File.AppendAllText(testlogpath, @First);
            }
            string logcontents = DateTime.Now.ToString("yyyyMMddHHmmss") + "," + usn + "," + testresult + "," +
                bretest.ToString() + "," + bsfcsresult.ToString() + "," + testitem + "," + T_Model + "," + T_ModelFamily + "," + T_MO + "," + T_UPN + "," + T_TestTime + "," + T_TestStation + "\r\n";
            File.AppendAllText(testlogpath, @logcontents);
        }


        #endregion

        #region 检测文件夹

        /// <summary>
        /// 插件文件夾，如果不存在，就创建文件夾
        /// </summary>
        public static void checkFolder()
        {
            if (!Directory.Exists(@Param.appFolder))
                Directory.CreateDirectory(Param.appFolder);
            if (!Directory.Exists(@Param.sysLogFolder))
                Directory.CreateDirectory(Param.sysLogFolder);
            if (!Directory.Exists(@Param.comLogFolder))
                Directory.CreateDirectory(@Param.comLogFolder);
            if (!Directory.Exists(@Param.TestDataFolder))   //临时数据储存文件夹
                Directory.CreateDirectory(@Param.TestDataFolder);
            if (!Directory.Exists(@Param.anaLogFolder))   //存放分析数据文件夹
                Directory.CreateDirectory(@Param.anaLogFolder);

        }

        #endregion

        #region 获取IP

        /// <summary>
        /// 获取IP地址,本机IP地址hostname=dns.gethostname(),返回一个IP list
        /// </summary>
        /// <param name="hostname">hostname</param>
        /// <returns>返回一个字符串类型的ip list</returns>
        public static List<string> getIP(string hostname)
        {
            List<string> iplist = new List<string>();
            System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostname);//会返回所有地址，包括IPv4和IPv6   
            foreach (IPAddress ip in addressList)
            {
                iplist.Add(ip.ToString());
            }
            return iplist;
        }

        /// <summary>
        /// 获取IP地址,本机IP地址hostname=dns.gethostname(),返回一个IP list
        /// </summary>
        /// <param name="hostname">hostname</param>
        /// <param name="iptype">ip地址的类型，IPV4,IPV6</param>
        /// <returns>返回一个字符串类型的ip list</returns>
        public static List<string> getIP(string hostname, string iptype)
        {
            List<string> iplist = new List<string>();
            IPAddress[] addressList = Dns.GetHostAddresses(hostname);
            foreach (IPAddress ip in addressList)
            {
                if (iptype.ToUpper() == Param.IPType.IPV4.ToString())
                {
                    if (ip.ToString().Contains("."))
                        iplist.Add(ip.ToString());
                }
                if (iptype.ToUpper() == Param.IPType.iPV6.ToString())
                {
                    if (!ip.ToString().Contains("."))
                        iplist.Add(ip.ToString());
                }
            }
            return iplist;
        }

        #endregion

        #region Asc Chr convert
        /// <summary>
        /// convert the asc code to character
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                //  throw new Exception("ASCII Code is not valid.");
                return null;
            }

        }

        /// <summary>
        /// convert the character to Asc code
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int Asc(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }
        }
        #endregion
    }
}
