using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var txt = textBox1.Text;
        }
        private void openHolder()
        {
            folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openHolder();
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
        }
    }
}
