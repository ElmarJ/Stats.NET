using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpssLib.Examples.DataReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog() { Filter = "SPSS File|*.sav" };
            dialog.ShowDialog();
            var stream = dialog.OpenFile();

            var datareader = new SpssLib.DataReader.SpssDataReader(stream);
            var dt = new DataTable();
            dt.Load(datareader);
            this.dataGridView1.DataSource = dt;
        }
    }
}
