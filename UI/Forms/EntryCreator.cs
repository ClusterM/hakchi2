using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.UI
{
    public partial class EntryCreator : Form
    {
        private bool EditMode = false;
        private Manager.BookManager.Book _theBook;
        public EntryCreator(Manager.BookManager.Book theBook)
        {
            InitializeComponent();
            _theBook = theBook;
            foreach(Manager.BookManager.Page p in _theBook.Pages)
            {
                comboBox1.Items.Add(p);
            }
            foreach(Manager.CoverManager.Cover c in Manager.CoverManager.getInstance().GetLibrary())
            {
                cmbCover.Items.Add(c);
            }
            foreach(Manager.EmulatorManager.Emulator emu in Manager.EmulatorManager.getInstance().getEmulatorList())
            {
                cmbEmulator.Items.Add(emu);
            }
        }
        public void EditEntry(Manager.BookManager.Entry _entr)
        {
            EditMode = true;
            entr = _entr;
            if(entr.IsLink)
            {
                tabControl1.SelectedTab = tpLink;
                comboBox1.SelectedItem = _theBook.GetPageById(_entr.PageId);
            }
            else
            {
                tabControl1.SelectedTab = tpGame;
                cmbEmulator.SelectedItem = _entr.Emulator;
                cmbRoms.SelectedItem = _entr.Rom;
            }
            textBox1.Text = _entr.Label;
            cmbCover.SelectedItem = _entr.Cover;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Manager.CoverManager.Cover c = Manager.CoverManager.getInstance().GetCoverByName(((Manager.BookManager.Page)comboBox1.SelectedItem).FriendlyName);
            if(c!=null)
            {
                cmbCover.SelectedItem = c;
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            if (cmbCover.SelectedItem != null)
            {


                pictureBox1.Image = Manager.BitmapManager.getInstance().GetBitmap(((Manager.CoverManager.Cover)cmbCover.SelectedItem).LocalPath);
            }
        }
        public Manager.BookManager.Entry entr;
        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim() != "")
            {
                Manager.BookManager.Entry le = null;
                if (EditMode)
                {
                    le = entr;
                }
                else
                {
                    le=   new Manager.BookManager.Entry();
                }
                le.Cover = (Manager.CoverManager.Cover)cmbCover.SelectedItem;
                le.Label = textBox1.Text;
                if (tabControl1.SelectedTab == tpLink)
                {
                    le.PageId = ((Manager.BookManager.Page)comboBox1.SelectedItem).Id;
                    le.IsLink = true;
                }
                else
                {
                    le.Emulator = (Manager.EmulatorManager.Emulator)cmbEmulator.SelectedItem;
                    if (cmbRoms.SelectedItem != null)
                    {
                        le.Rom = (Manager.RomManager.Rom)cmbRoms.SelectedItem;
                    }
                }
                entr = le;
                DialogResult = DialogResult.OK;
                this.Close();

            }
            else

            {
                MessageBox.Show("You need to enter a name");
            }
        }

        private void cmbEmulator_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbRoms.Items.Clear();
            cmbRoms.SelectedItem = null;
            if (cmbEmulator.SelectedItem != null)
            {
                Manager.EmulatorManager.Emulator emu = (Manager.EmulatorManager.Emulator)cmbEmulator.SelectedItem;
             
                
                    cmbRoms.Enabled = emu.NeedRomParameter;
                    foreach (Manager.RomManager.Rom r in emu.GetAllCompatibleRoms())
                    {
                        cmbRoms.Items.Add(r);
                    }
                
            }
            else
            {
                
            }
        }

        private void cmbRoms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbRoms.SelectedItem == null)
            {
                textBox1.Text = "";
            }
            else
            {
                Manager.RomManager.Rom r = (Manager.RomManager.Rom)cmbRoms.SelectedItem;
                textBox1.Text = r.DetectedName;


                Manager.CoverManager.Cover c = Manager.CoverManager.getInstance().GetCoverByName(r.DetectedName);
                if (c != null)
                {
                    cmbCover.SelectedItem = c;
                }
                else
                {
                    Manager.EmulatorManager.Emulator emu = (Manager.EmulatorManager.Emulator)cmbEmulator.SelectedItem;
                   c = Manager.CoverManager.getInstance().AddCover(".\\images\\" + emu.DefaultImage);
                    cmbCover.SelectedItem = c;
         
                }

            }
        }
    }
}
