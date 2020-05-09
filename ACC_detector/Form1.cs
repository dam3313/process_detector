using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// A Ajouter
using System.Diagnostics;

namespace ACC_detector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("ACC").Length > 0)
            {
                textBoxACC.Text = "ACC ON ";
            }
            else
            {
                textBoxACC.Text = "ACC OFF ";
            }

            if (Process.GetProcesses().Where(x => x.ProcessName.ToLower().StartsWith("cheat")).ToList().LongCount() > 0)
            {
                textBoxACC.Text += "Cheat Engine ON ";
            }
            else
            {
                textBoxACC.Text += "Cheat Engine OFF ";
            }

        }
    }
}
