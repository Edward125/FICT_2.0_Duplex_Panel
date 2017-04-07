namespace FICT_2._0_Duplex_Panel
{
    partial class SerialPortDebug
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
            this.CB_SerialPort_A = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CB_SerialPort_B = new System.Windows.Forms.ComboBox();
            this.Btn_Refresh = new System.Windows.Forms.Button();
            this.Btn_Test = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextBox_A = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.richTextBox_B = new System.Windows.Forms.RichTextBox();
            this.serialPort_A = new System.IO.Ports.SerialPort(this.components);
            this.serialPort_B = new System.IO.Ports.SerialPort(this.components);
            this.Btn_Stop = new System.Windows.Forms.Button();
            this.TestOut = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CB_SerialPort_A
            // 
            this.CB_SerialPort_A.FormattingEnabled = true;
            this.CB_SerialPort_A.Location = new System.Drawing.Point(63, 12);
            this.CB_SerialPort_A.Name = "CB_SerialPort_A";
            this.CB_SerialPort_A.Size = new System.Drawing.Size(66, 20);
            this.CB_SerialPort_A.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "串口A";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "串口B";
            // 
            // CB_SerialPort_B
            // 
            this.CB_SerialPort_B.FormattingEnabled = true;
            this.CB_SerialPort_B.Location = new System.Drawing.Point(63, 51);
            this.CB_SerialPort_B.Name = "CB_SerialPort_B";
            this.CB_SerialPort_B.Size = new System.Drawing.Size(66, 20);
            this.CB_SerialPort_B.TabIndex = 3;
            // 
            // Btn_Refresh
            // 
            this.Btn_Refresh.Location = new System.Drawing.Point(165, 50);
            this.Btn_Refresh.Name = "Btn_Refresh";
            this.Btn_Refresh.Size = new System.Drawing.Size(52, 42);
            this.Btn_Refresh.TabIndex = 4;
            this.Btn_Refresh.Text = "刷新";
            this.Btn_Refresh.UseVisualStyleBackColor = true;
            this.Btn_Refresh.Click += new System.EventHandler(this.Btn_Refresh_Click);
            // 
            // Btn_Test
            // 
            this.Btn_Test.Location = new System.Drawing.Point(247, 51);
            this.Btn_Test.Name = "Btn_Test";
            this.Btn_Test.Size = new System.Drawing.Size(52, 41);
            this.Btn_Test.TabIndex = 5;
            this.Btn_Test.Text = "测试";
            this.Btn_Test.UseVisualStyleBackColor = true;
            this.Btn_Test.Click += new System.EventHandler(this.Btn_Test_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextBox_A);
            this.groupBox1.Location = new System.Drawing.Point(20, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(358, 134);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "串口A显示";
            // 
            // richTextBox_A
            // 
            this.richTextBox_A.Location = new System.Drawing.Point(7, 21);
            this.richTextBox_A.Name = "richTextBox_A";
            this.richTextBox_A.Size = new System.Drawing.Size(345, 107);
            this.richTextBox_A.TabIndex = 0;
            this.richTextBox_A.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox_B);
            this.groupBox2.Location = new System.Drawing.Point(20, 236);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(358, 135);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "串口B显示";
            // 
            // richTextBox_B
            // 
            this.richTextBox_B.Location = new System.Drawing.Point(7, 19);
            this.richTextBox_B.Name = "richTextBox_B";
            this.richTextBox_B.Size = new System.Drawing.Size(345, 108);
            this.richTextBox_B.TabIndex = 1;
            this.richTextBox_B.Text = "";
            // 
            // serialPort_A
            // 
            this.serialPort_A.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_A_DataReceived);
            // 
            // serialPort_B
            // 
            this.serialPort_B.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_B_DataReceived);
            // 
            // Btn_Stop
            // 
            this.Btn_Stop.Location = new System.Drawing.Point(326, 51);
            this.Btn_Stop.Name = "Btn_Stop";
            this.Btn_Stop.Size = new System.Drawing.Size(52, 41);
            this.Btn_Stop.TabIndex = 10;
            this.Btn_Stop.Text = "停止";
            this.Btn_Stop.UseVisualStyleBackColor = true;
            this.Btn_Stop.Click += new System.EventHandler(this.Btn_Stop_Click);
            // 
            // TestOut
            // 
            this.TestOut.AutoSize = true;
            this.TestOut.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TestOut.Location = new System.Drawing.Point(202, 9);
            this.TestOut.Name = "TestOut";
            this.TestOut.Size = new System.Drawing.Size(79, 33);
            this.TestOut.TabIndex = 11;
            this.TestOut.Text = "pass";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // SerialPortDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(392, 385);
            this.Controls.Add(this.TestOut);
            this.Controls.Add(this.Btn_Stop);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Btn_Test);
            this.Controls.Add(this.Btn_Refresh);
            this.Controls.Add(this.CB_SerialPort_B);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CB_SerialPort_A);
            this.Name = "SerialPortDebug";
            this.Text = "串口调试";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialPortDebug_FormClosing);
            this.Load += new System.EventHandler(this.SerialPortDebug_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CB_SerialPort_A;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CB_SerialPort_B;
        private System.Windows.Forms.Button Btn_Refresh;
        private System.Windows.Forms.Button Btn_Test;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.IO.Ports.SerialPort serialPort_A;
        private System.IO.Ports.SerialPort serialPort_B;
        private System.Windows.Forms.Button Btn_Stop;
        private System.Windows.Forms.RichTextBox richTextBox_A;
        private System.Windows.Forms.RichTextBox richTextBox_B;
        private System.Windows.Forms.Label TestOut;
        private System.Windows.Forms.Timer timer1;
    }
}