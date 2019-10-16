using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Dynamic_Spam
{
    public partial class DSMain : Form
    {

        #region Global Hotkeys
        //Declarations
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        private IntPtr thisWindow;
        public enum fsModifiers
        {
            Alt = 0x0001,
            Control = 0x0002,
            Shift = 0x0004,
            Window = 0x0008,
        }
        //Gather
        private void DSMain_Load(object sender, EventArgs e)
        {
            thisWindow = FindWindow(null, "Dynamic-Spam");
            RegisterHotKey(thisWindow, 1, (uint)0, (uint)Keys.F7);
            RegisterHotKey(thisWindow, 2, (uint)0, (uint)Keys.F8);
        }
        private void DSMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey(thisWindow, 1);
            UnregisterHotKey(thisWindow, 2);
        }
        //Action
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                if ((int)m.WParam == 1)
                {
                    Start_Button.PerformClick();
                }
                if ((int)m.WParam == 2)
                {
                    Stop_Button.PerformClick();
                }
            }
            base.WndProc(ref m);
        }
        #endregion

        public DSMain()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public enum ClickEvent
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        Random RandomDelay = new Random();
        double RandomPercentage = 0;
        int Delay = 0;
        int DelayOriginal = 0;
        Point ClickLocation = new Point(0, 0);
        Point ClickReturnLocation = new Point(0, 0);

        private void Start_Button_Click(object sender, EventArgs e)
        {
            //Startup
            Selection_TabControl.Enabled = false;
            Random_GroupBox.Enabled = false;
            Process_GroupBox.Enabled = false;
            Delay_GroupBox.Enabled = false;
            Start_Button.Enabled = false;
            Stop_Button.Enabled = true;


            //Gather
            RandomPercentage = ((double)Random_TrackBar.Value * 0.01);
            try
            {
                Delay = Int32.Parse(Delay_TextBox.Text);
            }
            catch
            {
                MessageBox.Show("Input a integer in the Delay.");
                Stop_Button.PerformClick();
                return;
            }
            DelayOriginal = Delay;

            //Act
            if (Selection_TabControl.SelectedIndex == 0)
            {
                if (ClickLocation_CheckBox.Checked)
                {
                    try
                    {
                        ClickLocation.X = Int32.Parse(ClickLocationX_TextBox.Text);
                        ClickLocation.Y = Int32.Parse(ClickLocationY_TextBox.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Input integers in the Location boxes.");
                        Stop_Button.PerformClick();
                        return;
                    }
                }
                Click_Timer.Enabled = true;
            }
            if (Selection_TabControl.SelectedIndex == 1)
            {
                Type_Timer.Enabled = true;
            }
            if (Selection_TabControl.SelectedIndex == 2)
            {
                Script_Timer.Enabled = true;
            }

        }

        private void Stop_Button_Click(object sender, EventArgs e)
        {
            //Shutdown
            Selection_TabControl.Enabled = true;
            Random_GroupBox.Enabled = true;
            Process_GroupBox.Enabled = true;
            Delay_GroupBox.Enabled = true;
            Start_Button.Enabled = true;
            Stop_Button.Enabled = false;
            Click_Timer.Enabled = false;
            Type_Timer.Enabled = false;
            Script_Timer.Enabled = false;

            Delay_TextBox.Text = DelayOriginal.ToString();
        }

        private void Click_Timer_Tick(object sender, EventArgs e)
        {

            Delay = DelayOriginal;
            if (DelaySecond_RadioButton.Checked)
            {
                Delay *= 1000;
            }
            if (Random_TrackBar.Value != 0)
            {
                Delay = (Delay + RandomDelay.Next(0, ((Int32)((Double)Delay * RandomPercentage))));
                if (DelaySecond_RadioButton.Checked)
                {
                    Delay_TextBox.Text = (Delay / 1000).ToString();
                }
                if (!DelaySecond_RadioButton.Checked)
                {
                    Delay_TextBox.Text = Delay.ToString();
                }
            }
            Click_Timer.Interval = Delay;
            if (ClickLeft_RadioButton.Checked)
            {
                if (ClickLocation_CheckBox.Checked)
                {
                    ClickReturnLocation.X = Cursor.Position.X;
                    ClickReturnLocation.Y = Cursor.Position.Y;
                    SetCursorPos(ClickLocation.X, ClickLocation.Y);
                    mouse_event((int)ClickEvent.LEFTDOWN | (int)ClickEvent.LEFTUP, 0, 0, 0, 0);
                    SetCursorPos(ClickLocation.X, ClickLocation.Y);
                }
                else
                {
                    mouse_event((int)ClickEvent.LEFTDOWN | (int)ClickEvent.LEFTUP, 0, 0, 0, 0);
                }
            }
            else if (ClickRight_RadioButton.Checked)
            {
                if (ClickLocation_CheckBox.Checked)
                {

                }
                else
                {
                    mouse_event((int)ClickEvent.RIGHTDOWN | (int)ClickEvent.RIGHTUP, 0, 0, 0, 0);
                }
            }
            else
            {
                if (ClickLocation_CheckBox.Checked)
                {

                }
                else
                {
                   
                }
            }
        }

        #region Design Functionality
        private void Process_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Process_ComboBox.Enabled = Process_CheckBox.Checked;
        }

        private void ClickLocation_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ClickLocationX_TextBox.Enabled = ClickLocation_CheckBox.Checked;
            ClickLocationY_TextBox.Enabled = ClickLocation_CheckBox.Checked;
        }

        private void TypeKey_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            TypeKey_TextBox.Enabled = TypeKey_RadioButton.Checked;
        }

        private void TypeText_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            TypeText_RichTextBox.Enabled = TypeText_RadioButton.Checked;
        }

        private void TypeKey_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TypeKey_TextBox.Text = e.KeyCode.ToString();
        }

        private void Random_TrackBar_Scroll(object sender, EventArgs e)
        {
            Random_GroupBox.Text = ("Random " + Random_TrackBar.Value.ToString() + "%");
        }

        private void Script1_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script1_ComboBox.Enabled = Script1_CheckBox.Checked;
            Script1Var1_TextBox.Enabled = Script1_CheckBox.Checked;
            Script1Var2_TextBox.Enabled = Script1_CheckBox.Checked;
        }

        private void Script2_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script2_ComboBox.Enabled = Script2_CheckBox.Checked;
            Script2Var1_TextBox.Enabled = Script2_CheckBox.Checked;
            Script2Var2_TextBox.Enabled = Script2_CheckBox.Checked;
        }

        private void Script3_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script3_ComboBox.Enabled = Script3_CheckBox.Checked;
            Script3Var1_TextBox.Enabled = Script3_CheckBox.Checked;
            Script3Var2_TextBox.Enabled = Script3_CheckBox.Checked;
        }

        private void Script4_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script4_ComboBox.Enabled = Script4_CheckBox.Checked;
            Script4Var1_TextBox.Enabled = Script4_CheckBox.Checked;
            Script4Var2_TextBox.Enabled = Script4_CheckBox.Checked;
        }

        private void Script5_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script5_ComboBox.Enabled = Script5_CheckBox.Checked;
            Script5Var1_TextBox.Enabled = Script5_CheckBox.Checked;
            Script5Var2_TextBox.Enabled = Script5_CheckBox.Checked;
        }

        private void Script6_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script6_ComboBox.Enabled = Script6_CheckBox.Checked;
            Script6Var1_TextBox.Enabled = Script6_CheckBox.Checked;
            Script6Var2_TextBox.Enabled = Script6_CheckBox.Checked;
        }

        private void Script7_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Script7_ComboBox.Enabled = Script7_CheckBox.Checked;
            Script7Var1_TextBox.Enabled = Script7_CheckBox.Checked;
            Script7Var2_TextBox.Enabled = Script7_CheckBox.Checked;
        }

        private void Delay_TextBox_Click(object sender, EventArgs e)
        {
            Delay_TextBox.Text = "";
        }
        private void ClickDraw_Timer_Tick(object sender, EventArgs e)
        {
            if (!ClickLocation_CheckBox.Checked)
            {
                ClickLocationX_TextBox.Text = Cursor.Position.X.ToString();
                ClickLocationY_TextBox.Text = Cursor.Position.Y.ToString();
            }
        }
        #endregion
    }
}
