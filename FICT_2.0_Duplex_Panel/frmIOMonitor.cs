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
    public partial class frmIOMonitor : Form
    {
        public frmIOMonitor()
        {
            InitializeComponent();
        }

        #region 参数
        string exeTitle = "IO 状态监控...";
        #endregion




        #region checkIOStatus

        private void checkIOStatus(Char status, Button button)
        {
            if (status == '0')
            {
                button.BackColor = Color.Red;
            }
            else if (status == '1')
            {
                button.BackColor = Color.Green;
            }
            else
            {
                button.BackColor = Color.Gray;
            }
        }


        private void checkIN()
        {
            int i = -1;
            if (Param.IN_Status_1.Length == 16)
            {
                foreach (Char s in Param.IN_Status_1)
                {
                    i++;
                    switch (i)
                    {
                        case 0:
                            checkIOStatus(s, btnX17);
                            break;
                        case 1:
                            checkIOStatus(s, btnX16);
                            break;
                        case 2:
                            checkIOStatus(s, btnX15);
                            break;
                        case 3:
                            checkIOStatus(s, btnX14);
                            break;
                        case 4:
                            checkIOStatus(s, btnX13);
                            break;
                        case 5:
                            checkIOStatus(s, btnX12);
                            break;
                        case 6:
                            checkIOStatus(s, btnX11);
                            break;
                        case 7:
                            checkIOStatus(s, btnX10);
                            break;
                        case 8:
                            checkIOStatus(s, btnX7);
                            break;
                        case 9:
                            checkIOStatus(s, btnX6);
                            break;
                        case 10:
                            checkIOStatus(s, btnX5);
                            break;
                        case 11:
                            checkIOStatus(s, btnX4);
                            break;
                        case 12:
                            checkIOStatus(s, btnX3);
                            break;
                        case 13:
                            checkIOStatus(s, btnX2);
                            break;
                        case 14:
                            checkIOStatus(s, btnX1);
                            break;
                        case 15:
                            checkIOStatus(s, btnX0);
                            break;
                        default:
                            break;
                    }
                }
            }

            i = -1;
            if (Param.IN_Status_2.Length == 16)
            {
                foreach (Char s in Param.IN_Status_1)
                {
                    i++;
                    switch (i)
                    {
                        case 0:
                            //checkIOStatus  (s, btnX17);
                            break;
                        case 1:
                            //checkIOStatus(s, btnX16);
                            break;
                        case 2:
                            //checkIOStatus(s, btnX15);
                            break;
                        case 3:
                            //checkIOStatus(s, btnX14);
                            break;
                        case 4:
                            //checkIOStatus(s, btnX13);
                            break;
                        case 5:
                            //checkIOStatus(s, btnX12);
                            break;
                        case 6:
                            //checkIOStatus(s, btnX11);
                            break;
                        case 7:
                            //checkIOStatus(s, btnX10);
                            break;
                        case 8:
                            checkIOStatus(s, btnX27);
                            break;
                        case 9:
                            checkIOStatus(s, btnX26);
                            break;
                        case 10:
                            checkIOStatus(s, btnX25);
                            break;
                        case 11:
                            checkIOStatus(s, btnX24);
                            break;
                        case 12:
                            checkIOStatus(s, btnX23);
                            break;
                        case 13:
                            checkIOStatus(s, btnX22);
                            break;
                        case 14:
                            checkIOStatus(s, btnX21);
                            break;
                        case 15:
                            checkIOStatus(s, btnX20);
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        private void checkOUT()
        {
            int i = -1;
            if (Param.OUT_Status_1.Length == 16)
            {
                foreach (Char s in Param.OUT_Status_1)
                {
                    i++;
                    switch (i)
                    {
                        case 0:
                            checkIOStatus(s, btnY17);
                            break;
                        case 1:
                            checkIOStatus(s, btnY16);
                            break;
                        case 2:
                            checkIOStatus(s, btnY15);
                            break;
                        case 3:
                            checkIOStatus(s, btnY14);
                            break;
                        case 4:
                            checkIOStatus(s, btnY13);
                            break;
                        case 5:
                            checkIOStatus(s, btnY12);
                            break;
                        case 6:
                            checkIOStatus(s, btnY11);
                            break;
                        case 7:
                            checkIOStatus(s, btnY10);
                            break;
                        case 8:
                            checkIOStatus(s, btnY7);
                            break;
                        case 9:
                            checkIOStatus(s, btnY6);
                            break;
                        case 10:
                            checkIOStatus(s, btnY5);
                            break;
                        case 11:
                            checkIOStatus(s, btnY4);
                            break;
                        case 12:
                            checkIOStatus(s, btnY3);
                            break;
                        case 13:
                            checkIOStatus(s, btnY2);
                            break;
                        case 14:
                            checkIOStatus(s, btnY1);
                            break;
                        case 15:
                            checkIOStatus(s, btnY0);
                            break;
                        default:
                            break;
                    }
                }
            }
            i = -1;
            if (Param.OUT_Status_2.Length == 16)
            {
                foreach (Char s in Param.IN_Status_1)
                {
                    i++;
                    switch (i)
                    {
                        case 0:
                            //checkIOStatus  (s, btnY17);
                            break;
                        case 1:
                            //checkIOStatus(s, btnY16);
                            break;
                        case 2:
                            //checkIOStatus(s, btnY15);
                            break;
                        case 3:
                            //checkIOStatus(s, btnY14);
                            break;
                        case 4:
                            //checkIOStatus(s, btnY13);
                            break;
                        case 5:
                            //checkIOStatus(s, btnY12);
                            break;
                        case 6:
                            //checkIOStatus(s, btnY11);
                            break;
                        case 7:
                            //checkIOStatus(s, btnY10);
                            break;
                        case 8:
                            checkIOStatus(s, btnY27);
                            break;
                        case 9:
                            checkIOStatus(s, btnY26);
                            break;
                        case 10:
                            checkIOStatus(s, btnY25);
                            break;
                        case 11:
                            checkIOStatus(s, btnY24);
                            break;
                        case 12:
                            checkIOStatus(s, btnY23);
                            break;
                        case 13:
                            checkIOStatus(s, btnY22);
                            break;
                        case 14:
                            checkIOStatus(s, btnY21);
                            break;
                        case 15:
                            checkIOStatus(s, btnY20);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void checkM()
        {
            int i = -1;
            if (Param.M_Status_1.Length == 16)
            {
                foreach (Char s in Param.IN_Status_1)
                {
                    i++;
                    switch (i)
                    {
                        case 0:
                            //checkIOStatus  (s, btnM127);
                            break;
                        case 1:
                            // checkIOStatus(s, btnM126);
                            break;
                        case 2:
                            //checkIOStatus(s, btnM125);
                            break;
                        case 3:
                            //checkIOStatus(s, btnM124);
                            break;
                        case 4:
                            checkIOStatus(s, btnM123);
                            break;
                        case 5:
                            checkIOStatus(s, btnM122);
                            break;
                        case 6:
                            checkIOStatus(s, btnM121);
                            break;
                        case 7:
                            checkIOStatus(s, btnM120);
                            break;
                        case 8:
                            //checkIOStatus(s, btnM119);
                            break;
                        case 9:
                            //checkIOStatus(s, btnM118);
                            break;
                        case 10:
                            //checkIOStatus(s, btnM117);
                            break;
                        case 11:
                            //checkIOStatus(s, btnM116);
                            break;
                        case 12:
                            // checkIOStatus(s, btnM115);
                            break;
                        case 13:
                            //checkIOStatus(s, btnM114);
                            break;
                        case 14:
                            //checkIOStatus(s, btnM113);
                            break;
                        case 15:
                            // checkIOStatus(s, btnM112);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void checkD()
        {
            txtD4.Text = Param.D4.ToString();
            txtD7.Text = Param.D7.ToString();
            txtD9.Text = Param.D9.ToString();
            txtD11.Text = Param.D11.ToString();
            txtD100.Text = Param.D100.ToString();
            txtD101.Text = Param.D101.ToString();
            try
            {
                if (Param.D4 != -1)
                    txtD4S.Text = SubFunction.Chr(Param.D4);
                if (Param.D7 != -1)
                    txtD11S.Text = SubFunction.Chr(Param.D7);
                if (Param.D9 != -1)
                    txtD9S.Text = SubFunction.Chr(Param.D9);
                if (Param.D11 != -1)
                    txtD11S.Text = SubFunction.Chr(Param.D11);

            }
            catch (Exception)
            {


            }

        }
        #endregion



        private void frmIOMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerMoniter.Stop();
        }


        private void frmIOMonitor_Load(object sender, EventArgs e)
        {
            this.Text = exeTitle;
        }

        private void timerMoniter_Tick(object sender, EventArgs e)
        {
            timerMoniter.Stop();
            checkIN();
            checkOUT();
            checkM();
            checkD();
            timerMoniter.Start();
        }





    }
}
