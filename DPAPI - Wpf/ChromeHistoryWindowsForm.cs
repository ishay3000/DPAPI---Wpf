using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DPAPI___Wpf
{
    public partial class ChromeHistoryWindowsForm : Form
    {
        DataTable dt = null;
        public ChromeHistoryWindowsForm(DataTable chromeHistoryTable)
        {
            InitializeComponent();
            dt = chromeHistoryTable;
            gv.DataSource = dt;
            gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void ChromeHistoryWindowsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
