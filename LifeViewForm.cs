using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class LifeViewForm : Form
    {
        Thread screenShotThread;

        public LifeViewForm()
        {
            InitializeComponent();
            screenShotThread = new Thread(screenShotLoop);
            screenShotThread.Start();
        }

        void screenShotLoop()
        {
            while (true)
            {
                try
                {
                    var screenshot = WorkerForm.TakeScreenshot(false);
                    Invoke(new Action(delegate ()
                    {
                        pictureBox.Image = screenshot;
                    }));
                }
                catch (ThreadAbortException)
                {
                    screenShotThread = null;
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message + ex.StackTrace);
                    Thread.Sleep(1000);
                }
            }
        }

        private void LifeViewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (screenShotThread != null)
                screenShotThread.Abort();
        }
    }
}
