namespace FICT_2._0_Duplex_Panel
{
    partial class FICTDebug
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FICTDebug));
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFresh = new System.Windows.Forms.Button();
            this.btnCLOSE = new System.Windows.Forms.Button();
            this.comboPLC = new System.Windows.Forms.ComboBox();
            this.btnOPEN = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOR = new System.Windows.Forms.Button();
            this.btnOL = new System.Windows.Forms.Button();
            this.btnDO = new System.Windows.Forms.Button();
            this.btnOU = new System.Windows.Forms.Button();
            this.btnRC = new System.Windows.Forms.Button();
            this.btnLC = new System.Windows.Forms.Button();
            this.btnUP = new System.Windows.Forms.Button();
            this.btnIN = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.btnXA = new System.Windows.Forms.Button();
            this.btnTA = new System.Windows.Forms.Button();
            this.btnAaU = new System.Windows.Forms.Button();
            this.btnS1 = new System.Windows.Forms.Button();
            this.btnaaL = new System.Windows.Forms.Button();
            this.btnO1 = new System.Windows.Forms.Button();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.btnXB = new System.Windows.Forms.Button();
            this.btnTB = new System.Windows.Forms.Button();
            this.btnABU = new System.Windows.Forms.Button();
            this.btnS2 = new System.Windows.Forms.Button();
            this.btnabL = new System.Windows.Forms.Button();
            this.btnO2 = new System.Windows.Forms.Button();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.lstCommandList = new System.Windows.Forms.ListBox();
            this.serialPort_PLC = new System.IO.Ports.SerialPort(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.TB_MB_B_Voltage = new System.Windows.Forms.TextBox();
            this.TB_MB_A_Voltage = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.axActPLC = new AxActProgTypeLib.AxActProgType();
            this.axActProgType_Debug = new AxActProgTypeLib.AxActProgType();
            this.timerScanFICT = new System.Windows.Forms.Timer(this.components);
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axActPLC)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axActProgType_Debug)).BeginInit();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.btnFresh);
            this.GroupBox2.Controls.Add(this.btnCLOSE);
            this.GroupBox2.Controls.Add(this.comboPLC);
            this.GroupBox2.Controls.Add(this.btnOPEN);
            this.GroupBox2.Location = new System.Drawing.Point(6, 270);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(139, 99);
            this.GroupBox2.TabIndex = 52;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "PLC串口操作";
            // 
            // btnFresh
            // 
            this.btnFresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFresh.Location = new System.Drawing.Point(80, 21);
            this.btnFresh.Name = "btnFresh";
            this.btnFresh.Size = new System.Drawing.Size(51, 29);
            this.btnFresh.TabIndex = 47;
            this.btnFresh.Text = "刷新";
            this.btnFresh.UseVisualStyleBackColor = true;
            this.btnFresh.Click += new System.EventHandler(this.btnFresh_Click);
            // 
            // btnCLOSE
            // 
            this.btnCLOSE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCLOSE.Location = new System.Drawing.Point(80, 59);
            this.btnCLOSE.Name = "btnCLOSE";
            this.btnCLOSE.Size = new System.Drawing.Size(51, 29);
            this.btnCLOSE.TabIndex = 50;
            this.btnCLOSE.Text = "关闭";
            this.btnCLOSE.UseVisualStyleBackColor = true;
            this.btnCLOSE.Click += new System.EventHandler(this.btnCLOSE_Click);
            // 
            // comboPLC
            // 
            this.comboPLC.FormattingEnabled = true;
            this.comboPLC.Location = new System.Drawing.Point(12, 23);
            this.comboPLC.Name = "comboPLC";
            this.comboPLC.Size = new System.Drawing.Size(52, 20);
            this.comboPLC.TabIndex = 46;
            // 
            // btnOPEN
            // 
            this.btnOPEN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOPEN.Location = new System.Drawing.Point(12, 59);
            this.btnOPEN.Name = "btnOPEN";
            this.btnOPEN.Size = new System.Drawing.Size(51, 29);
            this.btnOPEN.TabIndex = 49;
            this.btnOPEN.Text = "开始";
            this.btnOPEN.UseVisualStyleBackColor = true;
            this.btnOPEN.Click += new System.EventHandler(this.btnOPEN_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.btnOR);
            this.GroupBox1.Controls.Add(this.btnOL);
            this.GroupBox1.Controls.Add(this.btnDO);
            this.GroupBox1.Controls.Add(this.btnOU);
            this.GroupBox1.Controls.Add(this.btnRC);
            this.GroupBox1.Controls.Add(this.btnLC);
            this.GroupBox1.Controls.Add(this.btnUP);
            this.GroupBox1.Controls.Add(this.btnIN);
            this.GroupBox1.Location = new System.Drawing.Point(242, 271);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(394, 99);
            this.GroupBox1.TabIndex = 53;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "机台动作命令";
            // 
            // btnOR
            // 
            this.btnOR.Location = new System.Drawing.Point(297, 58);
            this.btnOR.Name = "btnOR";
            this.btnOR.Size = new System.Drawing.Size(91, 32);
            this.btnOR.TabIndex = 7;
            this.btnOR.Text = "退右侧插(OR)";
            this.btnOR.UseVisualStyleBackColor = true;
            this.btnOR.Click += new System.EventHandler(this.btnOR_Click);
            // 
            // btnOL
            // 
            this.btnOL.Location = new System.Drawing.Point(202, 59);
            this.btnOL.Name = "btnOL";
            this.btnOL.Size = new System.Drawing.Size(90, 32);
            this.btnOL.TabIndex = 6;
            this.btnOL.Text = "退左侧插(OL)";
            this.btnOL.UseVisualStyleBackColor = true;
            this.btnOL.Click += new System.EventHandler(this.btnOL_Click);
            // 
            // btnDO
            // 
            this.btnDO.Location = new System.Drawing.Point(102, 58);
            this.btnDO.Name = "btnDO";
            this.btnDO.Size = new System.Drawing.Size(91, 32);
            this.btnDO.TabIndex = 5;
            this.btnDO.Text = "下降(DO)";
            this.btnDO.UseVisualStyleBackColor = true;
            this.btnDO.Click += new System.EventHandler(this.btnDO_Click);
            // 
            // btnOU
            // 
            this.btnOU.Location = new System.Drawing.Point(5, 59);
            this.btnOU.Name = "btnOU";
            this.btnOU.Size = new System.Drawing.Size(91, 32);
            this.btnOU.TabIndex = 4;
            this.btnOU.Text = "退板(OU)";
            this.btnOU.UseVisualStyleBackColor = true;
            this.btnOU.Click += new System.EventHandler(this.btnOU_Click);
            // 
            // btnRC
            // 
            this.btnRC.Location = new System.Drawing.Point(297, 20);
            this.btnRC.Name = "btnRC";
            this.btnRC.Size = new System.Drawing.Size(91, 32);
            this.btnRC.TabIndex = 3;
            this.btnRC.Text = "插右侧插(RC)";
            this.btnRC.UseVisualStyleBackColor = true;
            this.btnRC.Click += new System.EventHandler(this.btnRC_Click);
            // 
            // btnLC
            // 
            this.btnLC.Location = new System.Drawing.Point(202, 21);
            this.btnLC.Name = "btnLC";
            this.btnLC.Size = new System.Drawing.Size(89, 32);
            this.btnLC.TabIndex = 2;
            this.btnLC.Text = "插左侧插(LC)";
            this.btnLC.UseVisualStyleBackColor = true;
            this.btnLC.Click += new System.EventHandler(this.btnLC_Click);
            // 
            // btnUP
            // 
            this.btnUP.Location = new System.Drawing.Point(102, 21);
            this.btnUP.Name = "btnUP";
            this.btnUP.Size = new System.Drawing.Size(91, 32);
            this.btnUP.TabIndex = 1;
            this.btnUP.Text = "上升(UP)";
            this.btnUP.UseVisualStyleBackColor = true;
            this.btnUP.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnIN
            // 
            this.btnIN.Location = new System.Drawing.Point(5, 21);
            this.btnIN.Name = "btnIN";
            this.btnIN.Size = new System.Drawing.Size(91, 32);
            this.btnIN.TabIndex = 0;
            this.btnIN.Text = "进板(IN)";
            this.btnIN.UseVisualStyleBackColor = true;
            this.btnIN.Click += new System.EventHandler(this.btnIN_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.btnXA);
            this.GroupBox3.Controls.Add(this.btnTA);
            this.GroupBox3.Controls.Add(this.btnAaU);
            this.GroupBox3.Controls.Add(this.btnS1);
            this.GroupBox3.Controls.Add(this.btnaaL);
            this.GroupBox3.Controls.Add(this.btnO1);
            this.GroupBox3.Location = new System.Drawing.Point(6, 376);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(310, 100);
            this.GroupBox3.TabIndex = 54;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "单板(MB_A)测试命令";
            // 
            // btnXA
            // 
            this.btnXA.Location = new System.Drawing.Point(206, 59);
            this.btnXA.Name = "btnXA";
            this.btnXA.Size = new System.Drawing.Size(92, 32);
            this.btnXA.TabIndex = 13;
            this.btnXA.Text = "查询电压(XA)";
            this.btnXA.UseVisualStyleBackColor = true;
            this.btnXA.Click += new System.EventHandler(this.btnXA_Click);
            // 
            // btnTA
            // 
            this.btnTA.Location = new System.Drawing.Point(206, 21);
            this.btnTA.Name = "btnTA";
            this.btnTA.Size = new System.Drawing.Size(92, 32);
            this.btnTA.TabIndex = 12;
            this.btnTA.Text = "检测电压(TA)";
            this.btnTA.UseVisualStyleBackColor = true;
            this.btnTA.Click += new System.EventHandler(this.btnTA_Click);
            // 
            // btnAaU
            // 
            this.btnAaU.Location = new System.Drawing.Point(12, 20);
            this.btnAaU.Name = "btnAaU";
            this.btnAaU.Size = new System.Drawing.Size(91, 32);
            this.btnAaU.TabIndex = 8;
            this.btnAaU.Text = "通入19V(AA)";
            this.btnAaU.UseVisualStyleBackColor = true;
            this.btnAaU.Click += new System.EventHandler(this.btnAaU_Click);
            // 
            // btnS1
            // 
            this.btnS1.Location = new System.Drawing.Point(109, 59);
            this.btnS1.Name = "btnS1";
            this.btnS1.Size = new System.Drawing.Size(91, 32);
            this.btnS1.TabIndex = 11;
            this.btnS1.Text = "强制关机(S1)";
            this.btnS1.UseVisualStyleBackColor = true;
            this.btnS1.Click += new System.EventHandler(this.btnS1_Click);
            // 
            // btnaaL
            // 
            this.btnaaL.Location = new System.Drawing.Point(12, 59);
            this.btnaaL.Name = "btnaaL";
            this.btnaaL.Size = new System.Drawing.Size(91, 32);
            this.btnaaL.TabIndex = 9;
            this.btnaaL.Text = "断开19V(aa)";
            this.btnaaL.UseVisualStyleBackColor = true;
            this.btnaaL.Click += new System.EventHandler(this.btnaaL_Click);
            // 
            // btnO1
            // 
            this.btnO1.Location = new System.Drawing.Point(109, 21);
            this.btnO1.Name = "btnO1";
            this.btnO1.Size = new System.Drawing.Size(91, 32);
            this.btnO1.TabIndex = 10;
            this.btnO1.Text = "开机按钮(O1)";
            this.btnO1.UseVisualStyleBackColor = true;
            this.btnO1.Click += new System.EventHandler(this.btnO1_Click);
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.btnXB);
            this.GroupBox4.Controls.Add(this.btnTB);
            this.GroupBox4.Controls.Add(this.btnABU);
            this.GroupBox4.Controls.Add(this.btnS2);
            this.GroupBox4.Controls.Add(this.btnabL);
            this.GroupBox4.Controls.Add(this.btnO2);
            this.GroupBox4.Location = new System.Drawing.Point(326, 376);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(310, 100);
            this.GroupBox4.TabIndex = 55;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "单板(MB_B)测试命令";
            // 
            // btnXB
            // 
            this.btnXB.Location = new System.Drawing.Point(209, 56);
            this.btnXB.Name = "btnXB";
            this.btnXB.Size = new System.Drawing.Size(89, 32);
            this.btnXB.TabIndex = 13;
            this.btnXB.Text = "查询电压(XB)";
            this.btnXB.UseVisualStyleBackColor = true;
            this.btnXB.Click += new System.EventHandler(this.btnXB_Click);
            // 
            // btnTB
            // 
            this.btnTB.Location = new System.Drawing.Point(209, 18);
            this.btnTB.Name = "btnTB";
            this.btnTB.Size = new System.Drawing.Size(89, 32);
            this.btnTB.TabIndex = 12;
            this.btnTB.Text = "检测电压(TB)";
            this.btnTB.UseVisualStyleBackColor = true;
            this.btnTB.Click += new System.EventHandler(this.btnTB_Click);
            // 
            // btnABU
            // 
            this.btnABU.Location = new System.Drawing.Point(12, 18);
            this.btnABU.Name = "btnABU";
            this.btnABU.Size = new System.Drawing.Size(91, 32);
            this.btnABU.TabIndex = 8;
            this.btnABU.Text = "通入19V(AB)";
            this.btnABU.UseVisualStyleBackColor = true;
            this.btnABU.Click += new System.EventHandler(this.btnABU_Click);
            // 
            // btnS2
            // 
            this.btnS2.Location = new System.Drawing.Point(109, 58);
            this.btnS2.Name = "btnS2";
            this.btnS2.Size = new System.Drawing.Size(91, 32);
            this.btnS2.TabIndex = 11;
            this.btnS2.Text = "强制关机(S2)";
            this.btnS2.UseVisualStyleBackColor = true;
            this.btnS2.Click += new System.EventHandler(this.btnS2_Click);
            // 
            // btnabL
            // 
            this.btnabL.Location = new System.Drawing.Point(12, 58);
            this.btnabL.Name = "btnabL";
            this.btnabL.Size = new System.Drawing.Size(91, 32);
            this.btnabL.TabIndex = 9;
            this.btnabL.Text = "断开19V(ab)";
            this.btnabL.UseVisualStyleBackColor = true;
            this.btnabL.Click += new System.EventHandler(this.btnabL_Click);
            // 
            // btnO2
            // 
            this.btnO2.Location = new System.Drawing.Point(109, 18);
            this.btnO2.Name = "btnO2";
            this.btnO2.Size = new System.Drawing.Size(91, 32);
            this.btnO2.TabIndex = 10;
            this.btnO2.Text = "开机按钮(O2)";
            this.btnO2.UseVisualStyleBackColor = true;
            this.btnO2.Click += new System.EventHandler(this.btnO2_Click);
            // 
            // GroupBox5
            // 
            this.GroupBox5.Controls.Add(this.lstCommandList);
            this.GroupBox5.Location = new System.Drawing.Point(0, 12);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(636, 252);
            this.GroupBox5.TabIndex = 56;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "显示窗口";
            // 
            // lstCommandList
            // 
            this.lstCommandList.FormattingEnabled = true;
            this.lstCommandList.HorizontalScrollbar = true;
            this.lstCommandList.ItemHeight = 12;
            this.lstCommandList.Items.AddRange(new object[] {
            "PC->PLC:S1"});
            this.lstCommandList.Location = new System.Drawing.Point(6, 21);
            this.lstCommandList.Name = "lstCommandList";
            this.lstCommandList.ScrollAlwaysVisible = true;
            this.lstCommandList.Size = new System.Drawing.Size(637, 232);
            this.lstCommandList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 58;
            this.label1.Text = "MB_A电压(v)";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.TB_MB_B_Voltage);
            this.groupBox6.Controls.Add(this.TB_MB_A_Voltage);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Location = new System.Drawing.Point(151, 271);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(85, 99);
            this.groupBox6.TabIndex = 53;
            this.groupBox6.TabStop = false;
            // 
            // TB_MB_B_Voltage
            // 
            this.TB_MB_B_Voltage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.TB_MB_B_Voltage.Location = new System.Drawing.Point(6, 73);
            this.TB_MB_B_Voltage.Name = "TB_MB_B_Voltage";
            this.TB_MB_B_Voltage.ReadOnly = true;
            this.TB_MB_B_Voltage.Size = new System.Drawing.Size(68, 21);
            this.TB_MB_B_Voltage.TabIndex = 61;
            // 
            // TB_MB_A_Voltage
            // 
            this.TB_MB_A_Voltage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.TB_MB_A_Voltage.Location = new System.Drawing.Point(6, 33);
            this.TB_MB_A_Voltage.Name = "TB_MB_A_Voltage";
            this.TB_MB_A_Voltage.ReadOnly = true;
            this.TB_MB_A_Voltage.Size = new System.Drawing.Size(68, 21);
            this.TB_MB_A_Voltage.TabIndex = 60;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 59;
            this.label2.Text = "MB_B电压(v)";
            // 
            // axActPLC
            // 
            this.axActPLC.Enabled = true;
            this.axActPLC.Location = new System.Drawing.Point(639, 12);
            this.axActPLC.Name = "axActPLC";
            this.axActPLC.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axActPLC.OcxState")));
            this.axActPLC.Size = new System.Drawing.Size(32, 32);
            this.axActPLC.TabIndex = 57;
            // 
            // axActProgType_Debug
            // 
            this.axActProgType_Debug.Enabled = true;
            this.axActProgType_Debug.Location = new System.Drawing.Point(622, 12);
            this.axActProgType_Debug.Name = "axActProgType_Debug";
            this.axActProgType_Debug.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axActProgType_Debug.OcxState")));
            this.axActProgType_Debug.Size = new System.Drawing.Size(32, 32);
            this.axActProgType_Debug.TabIndex = 58;
            // 
            // timerScanFICT
            // 
            this.timerScanFICT.Tick += new System.EventHandler(this.timerScanFICT_Tick);
            // 
            // FICTDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(655, 483);
            this.Controls.Add(this.axActProgType_Debug);
            this.Controls.Add(this.GroupBox4);
            this.Controls.Add(this.axActPLC);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.GroupBox5);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.GroupBox2);
            this.Name = "FICTDebug";
            this.Text = "机台调试";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FICTDebug_FormClosing);
            this.Load += new System.EventHandler(this.FICTDebug_Load);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axActPLC)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axActProgType_Debug)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.Button btnFresh;
        internal System.Windows.Forms.Button btnCLOSE;
        internal System.Windows.Forms.ComboBox comboPLC;
        internal System.Windows.Forms.Button btnOPEN;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Button btnOR;
        internal System.Windows.Forms.Button btnOL;
        internal System.Windows.Forms.Button btnDO;
        internal System.Windows.Forms.Button btnOU;
        internal System.Windows.Forms.Button btnRC;
        internal System.Windows.Forms.Button btnLC;
        internal System.Windows.Forms.Button btnUP;
        internal System.Windows.Forms.Button btnIN;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.Button btnXA;
        internal System.Windows.Forms.Button btnTA;
        internal System.Windows.Forms.Button btnAaU;
        internal System.Windows.Forms.Button btnS1;
        internal System.Windows.Forms.Button btnaaL;
        internal System.Windows.Forms.Button btnO1;
        internal System.Windows.Forms.GroupBox GroupBox4;
        internal System.Windows.Forms.Button btnXB;
        internal System.Windows.Forms.Button btnTB;
        internal System.Windows.Forms.Button btnABU;
        internal System.Windows.Forms.Button btnS2;
        internal System.Windows.Forms.Button btnabL;
        internal System.Windows.Forms.Button btnO2;
        internal System.Windows.Forms.GroupBox GroupBox5;
        internal System.Windows.Forms.ListBox lstCommandList;
        private System.IO.Ports.SerialPort serialPort_PLC;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox TB_MB_B_Voltage;
        private System.Windows.Forms.TextBox TB_MB_A_Voltage;
        private System.Windows.Forms.Label label2;
        private AxActProgTypeLib.AxActProgType axActPLC;
        private AxActProgTypeLib.AxActProgType axActProgType_Debug;
        private System.Windows.Forms.Timer timerScanFICT;
    }
}