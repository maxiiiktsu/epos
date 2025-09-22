using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pokl.system
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        Form1 form1 = new Form1();

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void button1_close_Click(object sender, EventArgs e)
        {
            this.Close();
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            form1.BackColor = Color.Red;
            

            
            
            

        }
    }
}
