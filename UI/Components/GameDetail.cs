using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui.UI.Components
{
    public partial class GameDetail : UserControl
    {
        public GameDetail()
        {
            InitializeComponent();

            // Little tweak for easy translation
            var tbl = textBoxName.Left;
            textBoxName.Left = labelName.Left + labelName.Width;
            textBoxName.Width -= textBoxName.Left - tbl;
            maskedTextBoxReleaseDate.Left = label1.Left + label1.Width + 3;
            tbl = textBoxPublisher.Left;
            textBoxPublisher.Left = label2.Left + label2.Width;
            textBoxPublisher.Width -= textBoxPublisher.Left - tbl;
        }
        private NesMiniApplication currentApp;
        public void SetGame(NesMiniApplication app)
        {
            currentApp = app;
            ReloadInfo();
        }
        private void ReloadInfo()
        {
            if (currentApp == null)
            {

                groupBoxOptions.Visible = true;
                groupBoxOptions.Enabled = false;
                labelID.Text = "ID: ";
                textBoxName.Text = "";
                radioButtonOne.Checked = true;
                radioButtonTwo.Checked = false;
                radioButtonTwoSim.Checked = false;
                maskedTextBoxReleaseDate.Text = "";
                textBoxPublisher.Text = "";
                textBoxArguments.Text = "";
                pictureBoxArt.Image = null;
            }
            else
            {
              

                groupBoxOptions.Visible = true;
                labelID.Text = "ID: " + currentApp.Code;
                textBoxName.Text = currentApp.Name;
                if (currentApp.Simultaneous && currentApp.Players == 2)
                    radioButtonTwoSim.Checked = true;
                else if (currentApp.Players == 2)
                    radioButtonTwo.Checked = true;
                else
                    radioButtonOne.Checked = true;
                maskedTextBoxReleaseDate.Text = currentApp.ReleaseDate;
                textBoxPublisher.Text = currentApp.Publisher;
                if (currentApp is NesGame)
                    textBoxArguments.Text = (currentApp as NesGame).Args;
                else if (currentApp is FdsGame)
                    textBoxArguments.Text = (currentApp as FdsGame).Args;
                else
                    textBoxArguments.Text = currentApp.Command;
                if (System.IO.File.Exists(currentApp.IconPath))
                    pictureBoxArt.Image = NesMiniApplication.LoadBitmap(currentApp.IconPath);
                else
                    pictureBoxArt.Image = null;
                buttonShowGameGenieDatabase.Enabled = textBoxGameGenie.Enabled = currentApp is NesGame;
                textBoxGameGenie.Text = (currentApp is NesGame) ? (currentApp as NesGame).GameGenie : "";
                groupBoxOptions.Enabled = true;
            }
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
          
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            game.Name = textBoxName.Text;
        }

        private void radioButtonOne_CheckedChanged(object sender, EventArgs e)
        {
          
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            game.Players = (byte)(radioButtonOne.Checked ? 1 : 2);
            game.Simultaneous = radioButtonTwoSim.Checked;
        }

        private void textBoxPublisher_TextChanged(object sender, EventArgs e)
        {
           
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            game.Publisher = textBoxPublisher.Text.ToUpper();
        }

        private void textBoxArguments_TextChanged(object sender, EventArgs e)
        {
            
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            if (game is NesGame)
                (game as NesGame).Args = textBoxArguments.Text;
            else if (game is FdsGame)
                (game as FdsGame).Args = textBoxArguments.Text;
            else
                game.Command = textBoxArguments.Text;
        }

        private void maskedTextBoxReleaseDate_TextChanged(object sender, EventArgs e)
        {
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            game.ReleaseDate = maskedTextBoxReleaseDate.Text;
        }

        private void textBoxGameGenie_TextChanged(object sender, EventArgs e)
        {
            if (currentApp == null || !(currentApp is NesGame)) return;
            var game = (currentApp as NesGame);
            game.GameGenie = textBoxGameGenie.Text;
        }

        private void buttonShowGameGenieDatabase_Click(object sender, EventArgs e)
        {
            if (!(currentApp is NesGame)) return;
            NesGame nesGame = currentApp as NesGame;
            GameGenieCodeForm lFrm = new GameGenieCodeForm(nesGame);
            if (lFrm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBoxGameGenie.Text = nesGame.GameGenie;
        }

        public void SetImageForSelectedGame(string fileName)
        {
        
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            game.Image = NesMiniApplication.LoadBitmap(fileName);
            SetGame(game);
           
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            openFileDialogImage.Filter = Properties.Resources.Images + " (*.bmp;*.png;*.jpg;*.jpeg;*.gif)|*.bmp;*.png;*.jpg;*.jpeg;*.gif|" + Properties.Resources.AllFiles + "|*.*";
            if (openFileDialogImage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetImageForSelectedGame(openFileDialogImage.FileName);
            }
        }

        private void buttonGoogle_Click(object sender, EventArgs e)
        {
            if (currentApp == null || !(currentApp is NesMiniApplication)) return;
            var game = (currentApp as NesMiniApplication);
            var googler = new ImageGooglerForm(game);
            if (googler.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                game.Image = googler.Result;
                SetGame(game);
            }
        }
    }
}
