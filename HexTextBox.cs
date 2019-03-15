using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    class HexTextBox: TextBox
    {
        public HexTextBox()
        {
            this.TextChanged += HexTextBox_TextChanged;
        }

        private void HexTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.Text = Regex.Replace(this.Text, "[^0-9A-Fa-f]", "");
        }
    }

    class Int8HexTextBox : HexTextBox
    {
        public sbyte SignedValue
        {
            get
            {
                return sbyte.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X2");
            }
        }

        public byte UnsignedValue
        {
            get
            {
                return byte.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X2");
            }
        }

        public Int8HexTextBox() : base()
        {
            this.MaxLength = 2;
        }
    }

    class Int16HexTextBox : HexTextBox
    {
        public Int16 SignedValue
        {
            get
            {
                return Int16.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X4");
            }
        }

        public UInt16 UnsignedValue
        {
            get
            {
                return UInt16.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X4");
            }
        }

        public Int16HexTextBox() : base()
        {
            this.MaxLength = 4;
        }
    }

    class Int32HexTextBox : HexTextBox
    {
        public Int32 SignedValue
        {
            get
            {
                return Int32.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X8");
            }
        }

        public UInt32 UnsignedValue
        {
            get
            {
                return UInt32.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X8");
            }
        }

        public Int32HexTextBox() : base()
        {
            this.MaxLength = 8;
        }
    }

    class Int64HexTextBox : HexTextBox
    {
        public Int64 SignedValue
        {
            get
            {
                return Int64.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X16");
            }
        }

        public UInt64 UnsignedValue
        {
            get
            {
                return UInt64.Parse(this.Text, System.Globalization.NumberStyles.HexNumber);
            }
            set
            {
                this.Text = value.ToString("X16");
            }
        }

        public Int64HexTextBox() : base()
        {
            this.MaxLength = 16;
        }
    }
}
