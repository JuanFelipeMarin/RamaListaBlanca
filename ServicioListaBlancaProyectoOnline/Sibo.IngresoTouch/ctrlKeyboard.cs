using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sibo.WhiteList.IngresoTouch
{
    public partial class ctrlKeyboard : UserControl
    {
        string quantity = string.Empty, strQuantity = string.Empty;
        public bool swKeyboard = false;

        public delegate void ctrlOkHandler(string strValue);
        public event ctrlOkHandler ctrlOk;

        public ctrlKeyboard()
        {
            InitializeComponent();
        }

        public string Quantity
        {
            get
            {
                return strQuantity;
            }

            set
            {
                if (value.Length <= 14)
                {
                    strQuantity = value;
                    lblQuantity.Text = value;
                }
            }
        }

        private void cmdOne_Click(object sender, EventArgs e)
        {
            Efect(cmdOne);
            Quantity += "1";
        }

        private void Efect(PictureBox cmd)
        {
            cmd.BorderStyle = BorderStyle.Fixed3D;
            System.Threading.Thread.Sleep(100);
            cmd.BorderStyle = BorderStyle.None;
            swKeyboard = false;
        }

        private void cmdTwo_Click(object sender, EventArgs e)
        {
            Efect(cmdTwo);
            Quantity += "2";
        }

        private void cmdThree_Click(object sender, EventArgs e)
        {
            Efect(cmdThree);
            Quantity += "3";
        }

        private void cmdFour_Click(object sender, EventArgs e)
        {
            Efect(cmdFour);
            Quantity += "4";
        }

        private void cmdFive_Click(object sender, EventArgs e)
        {
            Efect(cmdFive);
            Quantity += "5";
        }

        private void cmdSix_Click(object sender, EventArgs e)
        {
            Efect(cmdSix);
            Quantity += "6";
        }

        private void cmdSeven_Click(object sender, EventArgs e)
        {
            Efect(cmdSeven);
            Quantity += "7";
        }

        private void cmdEight_Click(object sender, EventArgs e)
        {
            Efect(cmdEight);
            Quantity += "8";
        }

        private void cmdNine_Click(object sender, EventArgs e)
        {
            Efect(cmdNine);
            Quantity += "9";
        }

        private void cmdBack_Click(object sender, EventArgs e)
        {
            Efect(cmdBack);

            if (Quantity.Length > 0)
            {
                Quantity = Quantity.Remove(Quantity.Length - 1, 1);
            }
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            Efect(cmdOk);

            if (Quantity.Length > 0)
            {
                ctrlOk(Quantity);
                Quantity = string.Empty;
                swKeyboard = false;
            }
        }

        private void cmdZero_Click(object sender, EventArgs e)
        {
            Efect(cmdZero);
            Quantity += "0";
        }
    }
}
