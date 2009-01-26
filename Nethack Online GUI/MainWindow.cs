using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Threading;

namespace Nethack_Online_GUI
{
    public partial class MainWindow : Form
    {
        NethackController nhControl;
        Thread socketThread;
        static Mutex myMutex;

        public MainWindow(NethackController nhControl)
        {
            InitializeComponent();

            this.nhControl = nhControl;

            myMutex = new Mutex();
        }

        override protected void OnPaint(PaintEventArgs e)
        {
            TerminalCell[,] termCells = nhControl.getTermCells();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About myAbout = new About();
            myAbout.Visible = true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            nhControl.Connect("username", "password", "nethack.alt.org", 23);

            if (socketThread != null)
                throw new Exception("socketThread already exists");

            socketThread = new Thread(PollTelnetServer);

            socketThread.Start();
        }

        // Game Panel
        private void gamePanel_Paint(object sender, PaintEventArgs e)
        {
            nhControl.Paint(e.Graphics);
        }

        private void PollTelnetServer()
        {
            Monitor.TryEnter(this);
            if (nhControl.DataAvailable)
            {
                myMutex.GetAccessControl();
                nhControl.PollTelnetServer();
                myMutex.ReleaseMutex();
            }
            Monitor.Exit(this);
        }
    }
}