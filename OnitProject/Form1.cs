using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnitProject
{
    public partial class Form1 : Form
    {
        Controller _controller;
        public Form1()
        {
            InitializeComponent();
            _controller = new Controller();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String path = Utility.openFile("sqlite");
            _controller.readData(path);
        }
    }
}
