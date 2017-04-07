using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FICT_2._0_Duplex_Panel
{
    class Param
    {
        #region 枚舉定義

        /// <summary>
        /// switch status,OPEN or CLOSE
        /// </summary>
        public enum swithStatus
        {
            OPEN,
            CLOSE
        }

        /// <summary>
        /// 条码類型,A or B,仅拼板
        /// </summary>
        public enum barcodeType
        {
            A,
            B
        }

        /// <summary>
        /// MB類型，单板和拼板
        /// </summary>
        public enum mbType
        {
            Single,
            Panel

        }

        /// <summary>
        /// PCB 线别,AS2~AS9
        /// </summary>
        public enum pcbLine
        {
            AP2,
            AP3,
            AP4,
            AP5,
            AP6,
            AP7,
            AP8,
            AP9
        }

        /// <summary>
        /// fict的站別
        /// </summary>
        public enum fictStage
        {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H
        }

        /// <summary>
        /// 测试類型，自动测试 or 手动测试
        /// </summary>
        public enum testingType
        {
            Auto,
            Manual
        }

        /// <summary>
        /// log類型
        /// </summary>
        public enum logType
        {
            SYSLOG,//系統log,全部log
            COMLOG,//only command
            ANALOG
        }

        /// <summary>
        /// IP类型,V4,V6
        /// </summary>
        public enum IPType
        {
            IPV4,
            iPV6
        }

        /// <summary>
        /// sfcs 站别
        /// </summary>
        public enum sfc_stage
        {
            TD,
        }

        /// <summary>
        /// 测试結果
        /// </summary>
        public enum testResult
        {
            PASS,
            FAIL
        }

        /// <summary>
        /// MB的状态，等待测试，测试PASS，测试FAIL
        /// </summary>
        public enum mbStatus
        {
            WAIT,
            TEST,
            PASS,
            FAIL
        }
        /// <summary>
        /// 机台的状态，待板, 进板,测试,出板,离线,故障
        /// </summary>
        public enum StationStatus
        {
            待板,
            进板,
            测试,
            出板,
            离线,
            故障
        }
        /// <summary>
        /// database status,OK 正常流程测试,NG,直接跳过FICT跳过,收到数据立即回传
        /// </summary>
        public enum dataBaseStatus
        {
            OK,
            NG
        }

        /// <summary>
        /// 检测哪塊MB的电压，A or B
        /// </summary>
        public enum MBVType
        {
            A,
            B
        }
        /// <summary>
        /// 判断MB的FICT状态，TRUE可操作FICT,FALSE不可操作FICT，仅用于双连板
        /// </summary>
        public enum MBFICTStatus
        {
            TRUE,
            FALSE
        }
        /// <summary>
        /// AD 模块的型号，不同型号，电压分辨率不一样
        /// </summary>
        public enum AD_Module
        {
            FX2N_4AD,
            FX3U_4AD
        }
        /// <summary>
        /// 单板测试，机台控制面选择
        /// </summary>
        public enum singleselect //单板测试，MB_A：表示选择机台MB_A面控制，Mb_B:表示机台MB_B面控制
        {
            Station_A,
            Station_B
        }

        #endregion

        #region 參數定義

        //folder
        public static string appFolder =  @"C:\FICT_2.0_Duplex_Panel";
        public static string sysLogFolder = appFolder + @"\" + logType.SYSLOG.ToString();
        public static string comLogFolder = appFolder + @"\" + logType.COMLOG.ToString();
        public static string anaLogFolder = appFolder + @"\" + logType.ANALOG.ToString();
        public static string testLogfolder = appFolder + @"\Test_Log";

        //public static string IniFilePath = @appFolder + @"\" + "FICT_2.0_Duplex_Panel.ini"; //配置档的地址

        public static string TestDataFolder = @"C:\Windows\System32\2048\SysFICTClient";//暂存测试数据文件夹
        public static string TestDataTempini = @TestDataFolder + @"\" + "873323.ini";//暂存测试数据档地址
        public static string MysqlTestDatatxt = @TestDataFolder + @"\" + "NotWriteMysqlData.txt";//暂存未写入数据库的测试信息

        //SysConfig
        public static string IniFilePath = @appFolder + @"\" + "FICT_2.0_Duplex_Panel.ini"; //配置档的地址
        public static string SysVersion = string.Empty; //系統版本=当前程式版本
        public static string FICTStage = string.Empty; //FICT的站別
        public static string FixtrueID = string.Empty; //fixture id
        public static string PCBLine = string.Empty; //当前线别
        public static string TestingType = string.Empty;//测试方式
        public static string MBType = string.Empty;//当前MB的類型，单板or双板
        public static string RobotModule = string.Empty; //robot模組
        public static string BarcodeType = string.Empty;//刷条码的類型，刷MB_A or MB_B
        public static int MaxErrorCount = -1; // 最大连续NG次數
        public static int MaxRetestCount = -1; //最大重测次數
        public static bool LeftInsert = false; //左侧插
        public static bool RightInsert = false; //右侧插
        public static bool RTC_Use = false; //MB是否装有RTC电池
        public static int  DetectDelay = 15; // 延迟5秒后
        public static int  ShutDown = 6; //开机断电次數
        public static bool LeftInsertRe = false; //左侧插是否重複插一次,false:不插;true:插
        public static bool RightInserRe = false;//右侧插是否重複插一次,false:不插;true：插
        public static bool UseCommand = false ;//使用PLC內部命令判断电压，false，使用寄存器內部的值判断电压
        public static int InsertDelay = 1; //侧插延时插入的时間(s)
        
        public static bool ST_Flag = false;//其中1pcs PASS，另外1pcs 是否by pass
        public static bool NG_Stop = true;//默认连续超过3 pcs NG，测试机台暂停运行
        public static string AD_Module_Type = AD_Module.FX2N_4AD.ToString();

        public static bool MB_A_Re = true; //定义MB_A当检测不到电压时,否是断电再通电一次.true:通,false:不通
        public static bool MB_B_Re = true; //定义MB_B当检测不到电压时,否是断电再通电一次.true:通,false:不通
        //SFCS_Set
        public static bool CheckRouter = true;//是否检查站別
        public static bool Web_Use = true;//是否使用Web Service
        public static string Web_Site = string.Empty;
        public static bool OKBBBStatus = true; //测试OK 是否上拋BBB
        public static bool NGBBBStatus = true; //测试NG 是否上拋BBB
        public static string SFC_Stage = string.Empty; //SFCS 站別
        public static string OPID = string.Empty; //工号
        public static bool Arms_Use = true;//测试是否上拋Arms
        public static string Arms_Version = string.Empty;//上拋Arms的版本号
        public static string Arms_Path = string.Empty;//上拋Arms的网络路径
        public static string Net_Server = string.Empty;//上拋Arms的网盘ID地址(可含文件夾)
        public static string Net_ID = string.Empty;//登陆网盘的账号
        public static string Net_Password = string.Empty;//登陆网盘的密码

        //DB_set
        //CenterIP_DataBase
        public static bool Center_DataBase_Use = true; //是否使用中控数据库
        public static string CenterIP_DataBase_IP = string.Empty;//数据库的IP
        public static string CenterIP_DataBase_DB = string.Empty;//数据库的数据库名
        public static string CenterIP_DataBase_Table = string.Empty;//数据库的表名
        public static string CenterIP_DataBase_Account = string.Empty;//数据的账户
        public static string CenterIP_DataBase_Password = string.Empty;//数据库的密码
        //RecordIP_Database
        public static bool Record_DataBase_Use = false; //是否使用记录数据库
        public static string RecordIP_DataBase_IP = string.Empty;//数据库的IP
        public static string RecordIP_DataBase_DB = string.Empty;//数据库的数据库名
        public static string RecordIP_DataBase_TestInfo_Table = string.Empty;//数据库的MB测试信息表名
        public static string RecordIP_DataBase_Station_Table = string.Empty;//数据库的机台站别信息表名
        public static string RecordIP_DataBase_Account = string.Empty;//数据的账户
        public static string RecordIP_DataBase_Password = string.Empty;//数据库的密码 

        

        //ComPort_Set
        public static bool Scanner_Use = false; //是否使用条码槍
        public static string Scanner = string.Empty;//条码槍的串口号
        public static bool DUT_A_Use = true; //是否使用DUT_A
        public static string DUT_A = string.Empty; //DUT_A的串口号
        public static bool DUT_B_Use = true; //是否使用DUT_B
        public static string DUT_B = string.Empty; //DUT_B的串口号
        public static bool PLC_Use = true; //是否使用FICT的PLC
        public static string PLC = string.Empty; //PLC的串口号
        //TimeOut Set
        public static int PowerONTimeOut = -1; //开机超时时間
        public static int TestOKTimeOut = -1; //测试OK超时时間
        //
        public static string Center_DB_ConnStr = string.Empty;//中控数据库连接字符串
        public static string Record_DB_ConnStr = string.Empty;//记录数据库连接字符串
        //
        public static string bar_A = string.Empty; //双连板时A板条码，单板时MB的条码
        public static string bar_B = string.Empty; //双连板时B班的条码


        public static bool Test_MB_A = false; //A板测试标记；true，测试A板，false,不测试A板    //add 20170103
        public static bool Test_MB_B = false; //B板测试标记；true，测试A板，false,不测试B板    //add 20170103
        
        public static bool Test_Log = true; //TestLog记录标志位
     
        public static string SingleSelectTest = "Station_A"; //单板测试，Station_A：表示选择机台MB_A面控制，Station_B:表示机台MB_B面控制

        //FPY

        public static int iPass_A = 0;// MB_A Pass
        public static int iFail_A = 0;// MB_A Fail 
        public static int iFail_Pass_A = 0;// MB_A 强抛 

        public static int iPass_B = 0; //MB_B Fail
        public static int iFail_B = 0; //MB_B Fail
        public static int iFail_Pass_B = 0;// MB_A 强抛 

        public static int iPass = 0;// A and B OK
        public static int iFail = 0;// A or B Fail
        public static int iFail_Pass = 0;// A or B Fail 强抛 


        //PLC状态监控
        public static string IN_Status_1 = "-1"; //X0-X7,X10-X17
        public static string IN_Status_2 = "-1";//X20-X27,X30-X37  
        public static string OUT_Status_1 = "-1";
        public static string OUT_Status_2 = "-1";
        public static string M_Status_1 = "-1";
        public static string M_Status_2 = "-1";
        public static int[] i_IN_Status = new int[2];
        public static int[] i_OUT_Status = new int[2];
        public static int D4 = -1;
        public static int D7 = -1;
        public static int D9 = -1;
        public static int D11 = -1;
        public static int D100 = -1;
        public static int D101 = -1;

        public static  bool Upload_SFCS_type = false;    //false:只要两块PASS才算Pass，模一一块Fail，一块PASS，两块算FAil； true：AB板单独上抛

        //add by channing Wang
        //FPY Calculate
        public static int i = 0;  //记录计数
        public static string RecordTime = string.Empty;
        //1.按订单统计
        public static string MO_Last = "NULL";
        public static int Mo_Total = 0;
        public static int Mo_Pass = 0;
        public static int Mo_FAIL = 0;

        //2.按班别统计
        public static string Day_Start_str = DateTime.Now.ToString("yyyy-MM-dd") + " 08:00:00";
        public static string Day_End_str = DateTime.Now.ToString("yyyy-MM-dd") + " 20:00:00";
        public static DateTime starttime = Convert.ToDateTime(Day_Start_str);
        public static DateTime endtime = Convert.ToDateTime(Day_End_str);
        public static int Day_Total = 0;
        public static int Day_Pass = 0;
        public static int Day_FAIL = 0;

        public static int Night_Total = 0;
        public static int Night_Pass = 0;
        public static int Night_FAIL = 0;

        //3.按MB名称统计
        //单板 & 双板大板
        public static string Model_Last = "NULL";
        public static int Model_Total = 0;
        public static int Model_Pass = 0;
        public static int Model_FAIL = 0;

        //4.按时间段统计
        public static string Hour_Last = "Hour_Start";
        public static int Hour_Total = 0;
        public static int Hour_Pass = 0;
        public static int Hour_FAIL = 0;

        //5.回流板统计
        public static int ReTest_Total_Day = 0;
        public static int ReTest_Total_Night = 0;

        //6.按料号统计

        public static string UPN_Last = "NULL";
        public static int UPN_Total = 0;
        public static int UPN_Pass = 0;
        public static int UPN_FAIL = 0;

        public static string  Model = string.Empty;//MB名称
        public static string  ModelFamily = string.Empty;//MB单号
        public static string  MO = string.Empty;//MB工单
        public static string UPN = string.Empty;//MB料号
        public static string MBNumber = string.Empty;//MB版号

        public static string sql = string.Empty;
        public static string ReturnSNStage = string.Empty;

        public static int Wait_Time_Count = 0;//进MB时，计时
        public static int Wait_MB_TimeOut = -1;//进MB时，超时设定
        public static int In_Time_Count=0;//进MB时，计时
        public static int In_Time_TimeOut= -1;//进MB时，超时设定
        public static int Test_Time_Count = 0;//测试时，计时
        public static int Test_Time_TimeOut= -1;//测试时，超时设定
        public static int Out_Time_Count = 0;//退MB时，计时
        public static int Out_Time_TimeOut= -1;//退MB时，超时设定
        public static int Test_PeriodCycle_Time_Count = 0;//单板测试周期，计时
        public static int Test_PeriodCycle_TimeOut= -1;//单板测试周期，超时设定


        //FPY Calculate
        public static string Day_Night_Charge_Time = string.Empty;//换班时间，默认08：00：00
       // public static int i = 0;  //记录计数
       // public static string RecordTime = string.Empty;

        #endregion


    }
}
