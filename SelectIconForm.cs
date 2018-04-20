using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class SelectIconForm : Form
    {
        public class IconItem
        {
            public string Path { get; private set; }
            public string Name { get; private set; }
            public IconItem(string path, string name)
            {
                Path = path;
                Name = name;
            }
            public override string ToString()
            {
                return Name;
            }
            public override bool Equals(object obj)
            {
                IconItem item = obj as IconItem;
                if (item == null)
                    return false;
                return Name.Equals(item.Name);
            }
            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }

        public SelectIconForm(string selected = null)
        {
            InitializeComponent();
            listBox.Items.Clear();

            HashSet<IconItem> defaultImageSet = new HashSet<IconItem>();
            foreach (var file in Directory.EnumerateFiles(NesMenuFolder.FolderImagesDirectory, "*.png", SearchOption.TopDirectoryOnly))
            {
                if (file.ToLower().EndsWith("_small.png"))
                    continue;
                defaultImageSet.Add(new IconItem(file, Path.GetFileNameWithoutExtension(file)));
            }

            HashSet<IconItem> imageSet = new HashSet<IconItem>();
            if (!string.IsNullOrEmpty(ConfigIni.Instance.FolderImagesSet))
            {
                string path = Path.Combine(NesMenuFolder.FolderImagesDirectory, ConfigIni.Instance.FolderImagesSet);
                if (Directory.Exists(path))
                {
                    foreach (var file in Directory.EnumerateFiles(path, "*.png", SearchOption.TopDirectoryOnly))
                    {
                        if (file.ToLower().EndsWith("_small.png"))
                            continue;
                        imageSet.Add(new IconItem(file, Path.GetFileNameWithoutExtension(file)));
                    }
                }
            }
            imageSet.UnionWith(defaultImageSet);

            listBox.Items.AddRange(imageSet.ToArray());
            listBox.Sorted = true;
            if (selected != null)
                for (int i = 0; i < listBox.Items.Count; i++)
                    if (listBox.Items[i].ToString() == selected)
                    {
                        listBox.SelectedIndex = i;
                        break;
                    }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedItems.Count > 0)
            {
                buttonOk.Enabled = true;
                pictureBoxArt.Image = Image.FromFile((listBox.SelectedItem as IconItem).Path);
                pictureBoxArt.SizeMode = (pictureBoxArt.Image.Width > (hakchi.IsSnes(ConfigIni.Instance.ConsoleType) ? 228 : 204) || pictureBoxArt.Image.Height > 204) ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
            }
            else
            {
                buttonOk.Enabled = false;
                pictureBoxArt.Image = null;
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
