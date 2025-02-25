using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsForms4900;


namespace WindowsForms4900
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            

        }
        
        //--------------------------------------------------------
        //--------------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.ItemSize = new System.Drawing.Size(0, 1);//Hide Tab_control from user

            //label5.Text = "lmao";
            String tempstring = "\n\tSolar Data read from .txt;;;\n\n\t"
                + File.ReadAllText("solar.txt");//reads from project-folder -> bin -> Within debug folder
            label5.Text = tempstring;

        }

        //--------------------------------------------------------
        //--------------------------------------------------------

        private void button3_Click(object sender, EventArgs e)
        {
        }


        //--------------------------------------------------------
        //--------------------------------------------------------

        private void button4_Click(object sender, EventArgs e)
        {//HOME Button
            tabControl1.SelectTab(0);
        }

        //--------------------------------------------------------
        //--------------------------------------------------------

        private void button3_Click_1(object sender, EventArgs e)
        {//STANDARD (AES) Button
            tabControl1.SelectTab(1);
        }

        //----------

        private void button1_Click(object sender, EventArgs e)
        {//ENCRYPTION AES Button

        }

        //----------

        private void button2_Click(object sender, EventArgs e)
        {//DECRYPTION AES Button

        }

        //--------------------------------------------------------
        //--------------------------------------------------------

        private void button5_Click(object sender, EventArgs e)
        {//Encryption Type 2 Button
            tabControl1.SelectTab(2);
        }

        //--------------------------------------------------------
        //--------------------------------------------------------

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        //--------------------------------------------------------
        //--------------------------------------------------------
    }
}
