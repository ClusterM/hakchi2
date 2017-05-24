using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.UI.Forms
{
    public partial class RomImporter : Form
    {
        bool _InfoOnly = false;
        public void SetInfoOnly()
        {
            _InfoOnly = true;
            importDataGridViewCheckBoxColumn.Visible = false;
            this.Text = "Rom informations";
            btnImport.Hide();
        }
        public RomImporter()
        {
            InitializeComponent();
            this.Shown += RomImporter_Shown;
        }

        private void RomImporter_Shown(object sender, EventArgs e)
        {
            
        }
        private RomType[] LoadRoms(string folder, bool skipZip)
        {
            string[] roms = System.IO.Directory.GetFiles(folder, "*.*", System.IO.SearchOption.AllDirectories);
            Dictionary<string, RomType> typesToImport = new Dictionary<string, RomType>();
            int zipcount = 0;
            List<string> compressed = new List<string>();
            foreach (String r in roms)
            {
                string ext = System.IO.Path.GetExtension(r).ToLower();
                if (!typesToImport.ContainsKey(ext))
                {
                    typesToImport[ext] = new RomType();
                    typesToImport[ext].extension = ext;
                    typesToImport[ext].import = true;
                }
                typesToImport[ext].romsPath.Add(r);
                if (ext == ".zip" || ext == ".7z" || ext == ".rar")
                {
                    zipcount++;
                    compressed.Add(r);
                }
            }
            if (!skipZip && !_InfoOnly)
            {
                /*Check if unzip required*/
                if (zipcount > 0)
                {
                    if (MessageBox.Show("It appears you are importing compressed files (" + zipcount.ToString() + "), uncompressing them will allow better emulator detection.\r\nDo you want to automatically extract them? \r\nClicking No will keep them as is.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (System.IO.Directory.Exists(".\\RomsTemp\\"))
                        {
                            System.IO.Directory.Delete(".\\RomsTemp\\",true);
                        }
                        System.IO.Directory.CreateDirectory(".\\RomsTemp\\");
                        Dictionary<string, string> fToDir = new Dictionary<string, string>();

                        foreach (string f in compressed)
                        {

                            string filename = System.IO.Path.GetFileName(f);
                            string tf = System.IO.Path.Combine(".\\RomsTemp\\", filename + "\\");
                            fToDir[f] = tf;                           
                        }
                        AsyncTask at = new AsyncTask(new Tooling.Tasks.ExtractFiles(fToDir));
                        at.ShowDialog();
                        RomType[] added = LoadRoms(".\\RomsTemp\\", true);
                        foreach(RomType rt in added)
                        {
                            if(!typesToImport.ContainsKey(rt.extension))
                            {
                                typesToImport[rt.extension] = rt;
                            }
                            else
                            {
                                typesToImport[rt.extension].romsPath.AddRange(rt.romsPath);
                            }
                        }
                        typesToImport.Remove(".zip");
                        typesToImport.Remove(".7z");
                        typesToImport.Remove(".rar");
                    }
                }
            }
            return typesToImport.Values.ToArray() ;
        }
        public void LoadRoms(string folder)
        {
            

            romTypeBindingSource.Clear();
            foreach (RomType rt in LoadRoms(folder,false))
            {
                romTypeBindingSource.Add(rt);
            }
            //    dataGridView1.DataSource = typesToImport.Values;
            //   dataGridView1.Rows.AddRange(typesToImport.Values.ToArray());
        }
        public class RomType
        {
            public string extension { get; set; }
            public bool import { get; set; }
            public List<string> romsPath = new List<string>();
            public string Size
            {
                get
                {
                    double total = 0;
                    foreach(string file in romsPath)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(file);
                        total = total + fi.Length;
                        
                    }
                    double d = (total / 1024.0 / 1024.0);
                    
                    return Math.Round(d,1).ToString() + " mB";
                }
            }
            public int RomCount { get
                {
                    return romsPath.Count;
                }
            }
        }

        private void romTypeBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(".\\RomsTemp\\"))
            {
                System.IO.Directory.Delete(".\\RomsTemp\\", true);
            }
            this.Close();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            List<string> toImport = new List<string>();
            foreach(RomType rt in romTypeBindingSource.List)
            {
                if(rt.import)
                {
                    toImport.AddRange(rt.romsPath);
                }
            }
            AsyncTask at = new AsyncTask(new Tooling.Tasks.ImportRoms(toImport));
            at.ShowDialog();
            if (System.IO.Directory.Exists(".\\RomsTemp\\"))
            {
                System.IO.Directory.Delete(".\\RomsTemp\\", true);
            }
            this.Close();
        }
    }
}
