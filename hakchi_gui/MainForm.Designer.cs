namespace com.clusterrr.hakchi_gui
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                StaticRef = null;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMoreGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autodetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.asIsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCustomAppToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.presetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.addPresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deletePresetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.exportGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.synchronizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.resetOriginalGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flashCustomKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.flashUbootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sDModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.membootOriginalKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.membootCustomKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.membootRecoveryKernelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.dumpOriginalKernellegacyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem24 = new System.Windows.Forms.ToolStripSeparator();
            this.dumpTheWholeNANDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolFlashTheWholeNANDStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpNANDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flashNANDBPartitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpNANDCPartitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flashNANDCPartitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatNANDCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.factoryResetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.installModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallModulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateModulesReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modRepoStartSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.modRepoEndSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.manageModRepositoriesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.originalGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionAtTheTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionAtTheBottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionSortedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionHiddenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGamesWithoutBoxArtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.segaUiThemeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unitedStatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.europeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.japanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sFROMToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableSFROMToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usePCMPatchWhenAvailableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertSNESROMSToSFROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separateGamesStorageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressBoxArtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.centerBoxArtThumbnailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableHakchi2PopupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableInformationScrapeOnImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem25 = new System.Windows.Forms.ToolStripSeparator();
            this.developerToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devForceSshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadTotmpforTestingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem23 = new System.Windows.Forms.ToolStripSeparator();
            this.forceNetworkMembootsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forceClovershellMembootsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadLatestHakchiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separateGamesForMultibootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysCopyOriginalGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useLinkedSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripSeparator();
            this.cloverconHackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetUsingCombinationOfButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectButtonCombinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enableAutofireToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useXYOnClassicControllerAsAutofireABToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upABStartOnSecondControllerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kachikachiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.canoeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.retroarchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.epilepsyProtectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSettingsToNESMiniNowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.takeScreenshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDmesgOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importGamesFromMiniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.openFTPInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTelnetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.bootImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeBootImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disableBootImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetDefaultBootImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchRunningFirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formatSDCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripSeparator();
            this.prepareArtDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bluetoothToolStripMenuItem = new com.clusterrr.hakchi_gui.Wireless.Bluetooth.BluetoothMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gitHubPageWithActualReleasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.joinOurDiscordServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rRockinTheClassicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fAQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem22 = new System.Windows.Forms.ToolStripSeparator();
            this.technicalInformationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messageOfTheDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonAddGames = new System.Windows.Forms.Button();
            this.openFileDialogNes = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.explorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addPrefixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePrefixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.scrapeSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanForNewBoxArtForSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadBoxArtForSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedGamesBoxArtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripSeparator();
            this.archiveSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compressSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decompressSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSelectedGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripSeparator();
            this.sFROMToolToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editROMHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.resetROMHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repairGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripSeparator();
            this.selectEmulationCoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.buttonStart = new System.Windows.Forms.Button();
            this.timerCalculateGames = new System.Windows.Forms.Timer(this.components);
            this.timerConnectionCheck = new System.Windows.Forms.Timer(this.components);
            this.saveDumpFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openDumpFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.listViewGames = new System.Windows.Forms.ListView();
            this.gameName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timerShowSelected = new System.Windows.Forms.Timer(this.components);
            this.buttonExport = new System.Windows.Forms.Button();
            this.labelID = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.labelPublisher = new System.Windows.Forms.Label();
            this.textBoxPublisher = new System.Windows.Forms.TextBox();
            this.labelCommandLine = new System.Windows.Forms.Label();
            this.textBoxArguments = new System.Windows.Forms.TextBox();
            this.pictureBoxArt = new System.Windows.Forms.PictureBox();
            this.buttonBrowseImage = new System.Windows.Forms.Button();
            this.buttonGoogle = new System.Windows.Forms.Button();
            this.labelMaxPlayers = new System.Windows.Forms.Label();
            this.labelGameGenie = new System.Windows.Forms.Label();
            this.textBoxGameGenie = new System.Windows.Forms.TextBox();
            this.labelReleaseDate = new System.Windows.Forms.Label();
            this.maskedTextBoxReleaseDate = new System.Windows.Forms.MaskedTextBox();
            this.buttonShowGameGenieDatabase = new System.Windows.Forms.Button();
            this.checkBoxCompressed = new System.Windows.Forms.CheckBox();
            this.labelSize = new System.Windows.Forms.Label();
            this.buttonDefaultCover = new System.Windows.Forms.Button();
            this.pictureBoxThumbnail = new System.Windows.Forms.PictureBox();
            this.labelSortName = new System.Windows.Forms.Label();
            this.textBoxSortName = new System.Windows.Forms.TextBox();
            this.labelSaveCount = new System.Windows.Forms.Label();
            this.numericUpDownSaveCount = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanelGameInfo = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.maxPlayersComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanelGameID = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.tableLayoutPanelGameGenie = new System.Windows.Forms.TableLayoutPanel();
            this.labelCompress = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelGenre = new System.Windows.Forms.Label();
            this.comboBoxGenre = new System.Windows.Forms.ComboBox();
            this.labelCountry = new System.Windows.Forms.Label();
            this.comboBoxCountry = new System.Windows.Forms.ComboBox();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.textBoxCopyright = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelArtButtons = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSpine = new System.Windows.Forms.Button();
            this.pictureBoxM2Spine = new System.Windows.Forms.PictureBox();
            this.pictureBoxM2Front = new System.Windows.Forms.PictureBox();
            this.structureButton = new System.Windows.Forms.Button();
            this.foldersContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.disablePagefoldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.automaticToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pagesOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldersOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldersSplitByFirstLetterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldersSplitByFirstLetterOriginalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.maximumGamesPerFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backFolderButtonPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightmostToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderImagesSetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem20 = new System.Windows.Forms.ToolStripSeparator();
            this.syncStructureForAllGamesCollectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gamesConsoleComboBox = new System.Windows.Forms.ComboBox();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxButtons = new System.Windows.Forms.GroupBox();
            this.groupBoxCurrentGamesCollection = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxArtSega = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBoxArtNintendo = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBoxGameInfo = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelStatusBar = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripStatusConnectionIcon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelStatusBarInner = new System.Windows.Forms.TableLayoutPanel();
            this.toolStripStatusLabelShell = new System.Windows.Forms.Label();
            this.toolStripStatusLabelSelected = new System.Windows.Forms.Label();
            this.toolStripStatusLabelSize = new System.Windows.Forms.Label();
            this.toolStripProgressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.menuStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveCount)).BeginInit();
            this.tableLayoutPanelGameInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanelGameID.SuspendLayout();
            this.tableLayoutPanelGameGenie.SuspendLayout();
            this.tableLayoutPanelArtButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Spine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Front)).BeginInit();
            this.foldersContextMenuStrip.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxButtons.SuspendLayout();
            this.groupBoxCurrentGamesCollection.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBoxArtSega.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBoxArtNintendo.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBoxGameInfo.SuspendLayout();
            this.tableLayoutPanelStatusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolStripStatusConnectionIcon)).BeginInit();
            this.tableLayoutPanelStatusBarInner.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.kernelToolStripMenuItem,
            this.modulesToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.bluetoothToolStripMenuItem,
            this.helpToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.MenuActivate += new System.EventHandler(this.MenuStrip_MenuActivate);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMoreGamesToolStripMenuItem,
            this.addCustomAppToolStripMenuItem,
            this.presetsToolStripMenuItem,
            this.toolStripMenuItem13,
            this.exportGamesToolStripMenuItem,
            this.synchronizeToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.reloadGamesToolStripMenuItem,
            this.toolStripMenuItem12,
            this.resetOriginalGamesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // addMoreGamesToolStripMenuItem
            // 
            this.addMoreGamesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autodetectToolStripMenuItem,
            this.asIsToolStripMenuItem});
            this.addMoreGamesToolStripMenuItem.Name = "addMoreGamesToolStripMenuItem";
            resources.ApplyResources(this.addMoreGamesToolStripMenuItem, "addMoreGamesToolStripMenuItem");
            // 
            // autodetectToolStripMenuItem
            // 
            this.autodetectToolStripMenuItem.Name = "autodetectToolStripMenuItem";
            resources.ApplyResources(this.autodetectToolStripMenuItem, "autodetectToolStripMenuItem");
            this.autodetectToolStripMenuItem.Click += new System.EventHandler(this.buttonAddGames_Click);
            // 
            // asIsToolStripMenuItem
            // 
            this.asIsToolStripMenuItem.Name = "asIsToolStripMenuItem";
            resources.ApplyResources(this.asIsToolStripMenuItem, "asIsToolStripMenuItem");
            this.asIsToolStripMenuItem.Click += new System.EventHandler(this.asIsToolStripMenuItem_Click);
            // 
            // addCustomAppToolStripMenuItem
            // 
            this.addCustomAppToolStripMenuItem.Name = "addCustomAppToolStripMenuItem";
            resources.ApplyResources(this.addCustomAppToolStripMenuItem, "addCustomAppToolStripMenuItem");
            this.addCustomAppToolStripMenuItem.Click += new System.EventHandler(this.addCustomAppToolStripMenuItem_Click);
            // 
            // presetsToolStripMenuItem
            // 
            this.presetsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.addPresetToolStripMenuItem,
            this.deletePresetToolStripMenuItem});
            this.presetsToolStripMenuItem.Name = "presetsToolStripMenuItem";
            resources.ApplyResources(this.presetsToolStripMenuItem, "presetsToolStripMenuItem");
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // addPresetToolStripMenuItem
            // 
            this.addPresetToolStripMenuItem.Name = "addPresetToolStripMenuItem";
            resources.ApplyResources(this.addPresetToolStripMenuItem, "addPresetToolStripMenuItem");
            this.addPresetToolStripMenuItem.Click += new System.EventHandler(this.AddPreset);
            // 
            // deletePresetToolStripMenuItem
            // 
            this.deletePresetToolStripMenuItem.Name = "deletePresetToolStripMenuItem";
            resources.ApplyResources(this.deletePresetToolStripMenuItem, "deletePresetToolStripMenuItem");
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            resources.ApplyResources(this.toolStripMenuItem13, "toolStripMenuItem13");
            // 
            // exportGamesToolStripMenuItem
            // 
            this.exportGamesToolStripMenuItem.Name = "exportGamesToolStripMenuItem";
            resources.ApplyResources(this.exportGamesToolStripMenuItem, "exportGamesToolStripMenuItem");
            this.exportGamesToolStripMenuItem.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // synchronizeToolStripMenuItem
            // 
            this.synchronizeToolStripMenuItem.Name = "synchronizeToolStripMenuItem";
            resources.ApplyResources(this.synchronizeToolStripMenuItem, "synchronizeToolStripMenuItem");
            this.synchronizeToolStripMenuItem.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            resources.ApplyResources(this.searchToolStripMenuItem, "searchToolStripMenuItem");
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // reloadGamesToolStripMenuItem
            // 
            this.reloadGamesToolStripMenuItem.Name = "reloadGamesToolStripMenuItem";
            resources.ApplyResources(this.reloadGamesToolStripMenuItem, "reloadGamesToolStripMenuItem");
            this.reloadGamesToolStripMenuItem.Click += new System.EventHandler(this.reloadGamesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            resources.ApplyResources(this.toolStripMenuItem12, "toolStripMenuItem12");
            // 
            // resetOriginalGamesToolStripMenuItem
            // 
            this.resetOriginalGamesToolStripMenuItem.Name = "resetOriginalGamesToolStripMenuItem";
            resources.ApplyResources(this.resetOriginalGamesToolStripMenuItem, "resetOriginalGamesToolStripMenuItem");
            this.resetOriginalGamesToolStripMenuItem.Click += new System.EventHandler(this.resetOriginalGamesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // kernelToolStripMenuItem
            // 
            this.kernelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.flashCustomKernelToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.uninstallToolStripMenuItem,
            this.toolStripMenuItem11,
            this.flashUbootToolStripMenuItem,
            this.advancedToolStripMenuItem});
            this.kernelToolStripMenuItem.Name = "kernelToolStripMenuItem";
            resources.ApplyResources(this.kernelToolStripMenuItem, "kernelToolStripMenuItem");
            // 
            // flashCustomKernelToolStripMenuItem
            // 
            this.flashCustomKernelToolStripMenuItem.Name = "flashCustomKernelToolStripMenuItem";
            resources.ApplyResources(this.flashCustomKernelToolStripMenuItem, "flashCustomKernelToolStripMenuItem");
            this.flashCustomKernelToolStripMenuItem.Click += new System.EventHandler(this.flashCustomKernelToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            resources.ApplyResources(this.resetToolStripMenuItem, "resetToolStripMenuItem");
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // uninstallToolStripMenuItem
            // 
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            resources.ApplyResources(this.uninstallToolStripMenuItem, "uninstallToolStripMenuItem");
            this.uninstallToolStripMenuItem.Text = global::com.clusterrr.hakchi_gui.Properties.Resources.Uninstall;
            this.uninstallToolStripMenuItem.Click += new System.EventHandler(this.uninstallToolStripMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            resources.ApplyResources(this.toolStripMenuItem11, "toolStripMenuItem11");
            // 
            // flashUbootToolStripMenuItem
            // 
            this.flashUbootToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.normalModeToolStripMenuItem,
            this.sDModeToolStripMenuItem});
            this.flashUbootToolStripMenuItem.Name = "flashUbootToolStripMenuItem";
            resources.ApplyResources(this.flashUbootToolStripMenuItem, "flashUbootToolStripMenuItem");
            // 
            // normalModeToolStripMenuItem
            // 
            this.normalModeToolStripMenuItem.Name = "normalModeToolStripMenuItem";
            resources.ApplyResources(this.normalModeToolStripMenuItem, "normalModeToolStripMenuItem");
            this.normalModeToolStripMenuItem.Tag = com.clusterrr.hakchi_gui.Tasks.MembootTasks.MembootTaskType.FlashNormalUboot;
            this.normalModeToolStripMenuItem.Click += new System.EventHandler(this.flashUbootToolStripMenuItem_Click);
            // 
            // sDModeToolStripMenuItem
            // 
            this.sDModeToolStripMenuItem.Name = "sDModeToolStripMenuItem";
            resources.ApplyResources(this.sDModeToolStripMenuItem, "sDModeToolStripMenuItem");
            this.sDModeToolStripMenuItem.Tag = com.clusterrr.hakchi_gui.Tasks.MembootTasks.MembootTaskType.FlashSDUboot;
            this.sDModeToolStripMenuItem.Click += new System.EventHandler(this.flashUbootToolStripMenuItem_Click);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.membootOriginalKernelToolStripMenuItem,
            this.membootCustomKernelToolStripMenuItem,
            this.membootRecoveryKernelToolStripMenuItem,
            this.toolStripMenuItem10,
            this.dumpOriginalKernellegacyToolStripMenuItem,
            this.toolStripMenuItem24,
            this.dumpTheWholeNANDToolStripMenuItem,
            this.toolFlashTheWholeNANDStripMenuItem,
            this.dumpNANDBToolStripMenuItem,
            this.flashNANDBPartitionToolStripMenuItem,
            this.dumpNANDCPartitionToolStripMenuItem,
            this.flashNANDCPartitionToolStripMenuItem,
            this.formatNANDCToolStripMenuItem,
            this.toolStripSeparator1,
            this.factoryResetToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            resources.ApplyResources(this.advancedToolStripMenuItem, "advancedToolStripMenuItem");
            // 
            // membootOriginalKernelToolStripMenuItem
            // 
            this.membootOriginalKernelToolStripMenuItem.Name = "membootOriginalKernelToolStripMenuItem";
            resources.ApplyResources(this.membootOriginalKernelToolStripMenuItem, "membootOriginalKernelToolStripMenuItem");
            this.membootOriginalKernelToolStripMenuItem.Click += new System.EventHandler(this.membootOriginalKernelToolStripMenuItem_Click);
            // 
            // membootCustomKernelToolStripMenuItem
            // 
            this.membootCustomKernelToolStripMenuItem.Name = "membootCustomKernelToolStripMenuItem";
            resources.ApplyResources(this.membootCustomKernelToolStripMenuItem, "membootCustomKernelToolStripMenuItem");
            this.membootCustomKernelToolStripMenuItem.Click += new System.EventHandler(this.membootCustomKernelToolStripMenuItem_Click);
            // 
            // membootRecoveryKernelToolStripMenuItem
            // 
            this.membootRecoveryKernelToolStripMenuItem.Name = "membootRecoveryKernelToolStripMenuItem";
            resources.ApplyResources(this.membootRecoveryKernelToolStripMenuItem, "membootRecoveryKernelToolStripMenuItem");
            this.membootRecoveryKernelToolStripMenuItem.Click += new System.EventHandler(this.membootRecoveryKernelToolStripMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
            // 
            // dumpOriginalKernellegacyToolStripMenuItem
            // 
            this.dumpOriginalKernellegacyToolStripMenuItem.Name = "dumpOriginalKernellegacyToolStripMenuItem";
            resources.ApplyResources(this.dumpOriginalKernellegacyToolStripMenuItem, "dumpOriginalKernellegacyToolStripMenuItem");
            this.dumpOriginalKernellegacyToolStripMenuItem.Click += new System.EventHandler(this.dumpOriginalKernellegacyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem24
            // 
            this.toolStripMenuItem24.Name = "toolStripMenuItem24";
            resources.ApplyResources(this.toolStripMenuItem24, "toolStripMenuItem24");
            // 
            // dumpTheWholeNANDToolStripMenuItem
            // 
            this.dumpTheWholeNANDToolStripMenuItem.Name = "dumpTheWholeNANDToolStripMenuItem";
            resources.ApplyResources(this.dumpTheWholeNANDToolStripMenuItem, "dumpTheWholeNANDToolStripMenuItem");
            this.dumpTheWholeNANDToolStripMenuItem.Click += new System.EventHandler(this.dumpTheWholeNANDToolStripMenuItem_Click);
            // 
            // toolFlashTheWholeNANDStripMenuItem
            // 
            this.toolFlashTheWholeNANDStripMenuItem.Name = "toolFlashTheWholeNANDStripMenuItem";
            resources.ApplyResources(this.toolFlashTheWholeNANDStripMenuItem, "toolFlashTheWholeNANDStripMenuItem");
            this.toolFlashTheWholeNANDStripMenuItem.Click += new System.EventHandler(this.toolFlashTheWholeNANDStripMenuItem_Click);
            // 
            // dumpNANDBToolStripMenuItem
            // 
            this.dumpNANDBToolStripMenuItem.Name = "dumpNANDBToolStripMenuItem";
            resources.ApplyResources(this.dumpNANDBToolStripMenuItem, "dumpNANDBToolStripMenuItem");
            this.dumpNANDBToolStripMenuItem.Click += new System.EventHandler(this.dumpNANDBToolStripMenuItem_Click);
            // 
            // flashNANDBPartitionToolStripMenuItem
            // 
            this.flashNANDBPartitionToolStripMenuItem.Name = "flashNANDBPartitionToolStripMenuItem";
            resources.ApplyResources(this.flashNANDBPartitionToolStripMenuItem, "flashNANDBPartitionToolStripMenuItem");
            this.flashNANDBPartitionToolStripMenuItem.Click += new System.EventHandler(this.flashNANDBPartitionToolStripMenuItem_Click);
            // 
            // dumpNANDCPartitionToolStripMenuItem
            // 
            this.dumpNANDCPartitionToolStripMenuItem.Name = "dumpNANDCPartitionToolStripMenuItem";
            resources.ApplyResources(this.dumpNANDCPartitionToolStripMenuItem, "dumpNANDCPartitionToolStripMenuItem");
            this.dumpNANDCPartitionToolStripMenuItem.Click += new System.EventHandler(this.dumpNANDCPartitionToolStripMenuItem_Click);
            // 
            // flashNANDCPartitionToolStripMenuItem
            // 
            this.flashNANDCPartitionToolStripMenuItem.Name = "flashNANDCPartitionToolStripMenuItem";
            resources.ApplyResources(this.flashNANDCPartitionToolStripMenuItem, "flashNANDCPartitionToolStripMenuItem");
            this.flashNANDCPartitionToolStripMenuItem.Click += new System.EventHandler(this.flashNANDCPartitionToolStripMenuItem_Click);
            // 
            // formatNANDCToolStripMenuItem
            // 
            this.formatNANDCToolStripMenuItem.Name = "formatNANDCToolStripMenuItem";
            resources.ApplyResources(this.formatNANDCToolStripMenuItem, "formatNANDCToolStripMenuItem");
            this.formatNANDCToolStripMenuItem.Click += new System.EventHandler(this.formatNANDCToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // factoryResetToolStripMenuItem
            // 
            resources.ApplyResources(this.factoryResetToolStripMenuItem, "factoryResetToolStripMenuItem");
            this.factoryResetToolStripMenuItem.Name = "factoryResetToolStripMenuItem";
            this.factoryResetToolStripMenuItem.Text = global::com.clusterrr.hakchi_gui.Properties.Resources.FactoryReset;
            this.factoryResetToolStripMenuItem.Click += new System.EventHandler(this.factoryResetToolStripMenuItem_Click);
            // 
            // modulesToolStripMenuItem
            // 
            this.modulesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installModulesToolStripMenuItem,
            this.uninstallModulesToolStripMenuItem,
            this.generateModulesReportToolStripMenuItem,
            this.modRepoStartSeparator,
            this.modRepoEndSeparator,
            this.manageModRepositoriesToolStripMenuItem});
            this.modulesToolStripMenuItem.Name = "modulesToolStripMenuItem";
            resources.ApplyResources(this.modulesToolStripMenuItem, "modulesToolStripMenuItem");
            // 
            // installModulesToolStripMenuItem
            // 
            this.installModulesToolStripMenuItem.Name = "installModulesToolStripMenuItem";
            resources.ApplyResources(this.installModulesToolStripMenuItem, "installModulesToolStripMenuItem");
            this.installModulesToolStripMenuItem.Click += new System.EventHandler(this.installModulesToolStripMenuItem_Click);
            // 
            // uninstallModulesToolStripMenuItem
            // 
            this.uninstallModulesToolStripMenuItem.Name = "uninstallModulesToolStripMenuItem";
            resources.ApplyResources(this.uninstallModulesToolStripMenuItem, "uninstallModulesToolStripMenuItem");
            this.uninstallModulesToolStripMenuItem.Click += new System.EventHandler(this.uninstallModulesToolStripMenuItem_Click);
            // 
            // generateModulesReportToolStripMenuItem
            // 
            this.generateModulesReportToolStripMenuItem.Name = "generateModulesReportToolStripMenuItem";
            resources.ApplyResources(this.generateModulesReportToolStripMenuItem, "generateModulesReportToolStripMenuItem");
            this.generateModulesReportToolStripMenuItem.Click += new System.EventHandler(this.generateModulesReportToolStripMenuItem_Click);
            // 
            // modRepoStartSeparator
            // 
            this.modRepoStartSeparator.Name = "modRepoStartSeparator";
            resources.ApplyResources(this.modRepoStartSeparator, "modRepoStartSeparator");
            // 
            // modRepoEndSeparator
            // 
            this.modRepoEndSeparator.Name = "modRepoEndSeparator";
            resources.ApplyResources(this.modRepoEndSeparator, "modRepoEndSeparator");
            // 
            // manageModRepositoriesToolStripMenuItem
            // 
            this.manageModRepositoriesToolStripMenuItem.Name = "manageModRepositoriesToolStripMenuItem";
            resources.ApplyResources(this.manageModRepositoriesToolStripMenuItem, "manageModRepositoriesToolStripMenuItem");
            this.manageModRepositoriesToolStripMenuItem.Click += new System.EventHandler(this.manageModRepositoriesToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.originalGamesToolStripMenuItem,
            this.sortByToolStripMenuItem,
            this.showGamesWithoutBoxArtToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // originalGamesToolStripMenuItem
            // 
            this.originalGamesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.positionAtTheTopToolStripMenuItem,
            this.positionAtTheBottomToolStripMenuItem,
            this.positionSortedToolStripMenuItem,
            this.positionHiddenToolStripMenuItem});
            this.originalGamesToolStripMenuItem.Name = "originalGamesToolStripMenuItem";
            resources.ApplyResources(this.originalGamesToolStripMenuItem, "originalGamesToolStripMenuItem");
            // 
            // positionAtTheTopToolStripMenuItem
            // 
            this.positionAtTheTopToolStripMenuItem.Name = "positionAtTheTopToolStripMenuItem";
            resources.ApplyResources(this.positionAtTheTopToolStripMenuItem, "positionAtTheTopToolStripMenuItem");
            this.positionAtTheTopToolStripMenuItem.Tag = "0";
            this.positionAtTheTopToolStripMenuItem.Click += new System.EventHandler(this.originalGamesPositionToolStripMenuItem_Click);
            // 
            // positionAtTheBottomToolStripMenuItem
            // 
            this.positionAtTheBottomToolStripMenuItem.Name = "positionAtTheBottomToolStripMenuItem";
            resources.ApplyResources(this.positionAtTheBottomToolStripMenuItem, "positionAtTheBottomToolStripMenuItem");
            this.positionAtTheBottomToolStripMenuItem.Tag = "1";
            this.positionAtTheBottomToolStripMenuItem.Click += new System.EventHandler(this.originalGamesPositionToolStripMenuItem_Click);
            // 
            // positionSortedToolStripMenuItem
            // 
            this.positionSortedToolStripMenuItem.Name = "positionSortedToolStripMenuItem";
            resources.ApplyResources(this.positionSortedToolStripMenuItem, "positionSortedToolStripMenuItem");
            this.positionSortedToolStripMenuItem.Tag = "2";
            this.positionSortedToolStripMenuItem.Click += new System.EventHandler(this.originalGamesPositionToolStripMenuItem_Click);
            // 
            // positionHiddenToolStripMenuItem
            // 
            this.positionHiddenToolStripMenuItem.Name = "positionHiddenToolStripMenuItem";
            resources.ApplyResources(this.positionHiddenToolStripMenuItem, "positionHiddenToolStripMenuItem");
            this.positionHiddenToolStripMenuItem.Tag = "3";
            this.positionHiddenToolStripMenuItem.Click += new System.EventHandler(this.originalGamesPositionToolStripMenuItem_Click);
            // 
            // sortByToolStripMenuItem
            // 
            this.sortByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nameToolStripMenuItem,
            this.coreToolStripMenuItem,
            this.systemToolStripMenuItem});
            this.sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
            resources.ApplyResources(this.sortByToolStripMenuItem, "sortByToolStripMenuItem");
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            resources.ApplyResources(this.nameToolStripMenuItem, "nameToolStripMenuItem");
            this.nameToolStripMenuItem.Tag = "0";
            this.nameToolStripMenuItem.Click += new System.EventHandler(this.sortByToolStripMenuItem_Click);
            // 
            // coreToolStripMenuItem
            // 
            this.coreToolStripMenuItem.Name = "coreToolStripMenuItem";
            resources.ApplyResources(this.coreToolStripMenuItem, "coreToolStripMenuItem");
            this.coreToolStripMenuItem.Tag = "1";
            this.coreToolStripMenuItem.Click += new System.EventHandler(this.sortByToolStripMenuItem_Click);
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            resources.ApplyResources(this.systemToolStripMenuItem, "systemToolStripMenuItem");
            this.systemToolStripMenuItem.Tag = "2";
            this.systemToolStripMenuItem.Click += new System.EventHandler(this.sortByToolStripMenuItem_Click);
            // 
            // showGamesWithoutBoxArtToolStripMenuItem
            // 
            this.showGamesWithoutBoxArtToolStripMenuItem.Checked = true;
            this.showGamesWithoutBoxArtToolStripMenuItem.CheckOnClick = true;
            this.showGamesWithoutBoxArtToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGamesWithoutBoxArtToolStripMenuItem.Name = "showGamesWithoutBoxArtToolStripMenuItem";
            resources.ApplyResources(this.showGamesWithoutBoxArtToolStripMenuItem, "showGamesWithoutBoxArtToolStripMenuItem");
            this.showGamesWithoutBoxArtToolStripMenuItem.Click += new System.EventHandler(this.showGamesWithoutBoxArtToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageToolStripMenuItem,
            this.segaUiThemeToolStripMenuItem,
            this.sFROMToolToolStripMenuItem,
            this.convertSNESROMSToSFROMToolStripMenuItem,
            this.separateGamesStorageToolStripMenuItem,
            this.compressGamesToolStripMenuItem,
            this.compressBoxArtToolStripMenuItem,
            this.centerBoxArtThumbnailToolStripMenuItem,
            this.disableHakchi2PopupsToolStripMenuItem,
            this.enableInformationScrapeOnImportToolStripMenuItem,
            this.toolStripMenuItem25,
            this.developerToolsToolStripMenuItem,
            this.separateGamesForMultibootToolStripMenuItem,
            this.alwaysCopyOriginalGamesToolStripMenuItem,
            this.useLinkedSyncToolStripMenuItem,
            this.toolStripMenuItem16,
            this.cloverconHackToolStripMenuItem,
            this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem,
            this.epilepsyProtectionToolStripMenuItem,
            this.toolStripMenuItem5,
            this.saveSettingsToNESMiniNowToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            // 
            // languageToolStripMenuItem
            // 
            resources.ApplyResources(this.languageToolStripMenuItem, "languageToolStripMenuItem");
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            // 
            // segaUiThemeToolStripMenuItem
            // 
            this.segaUiThemeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoToolStripMenuItem,
            this.unitedStatesToolStripMenuItem,
            this.europeToolStripMenuItem,
            this.japanToolStripMenuItem});
            this.segaUiThemeToolStripMenuItem.Name = "segaUiThemeToolStripMenuItem";
            resources.ApplyResources(this.segaUiThemeToolStripMenuItem, "segaUiThemeToolStripMenuItem");
            // 
            // autoToolStripMenuItem
            // 
            this.autoToolStripMenuItem.Name = "autoToolStripMenuItem";
            resources.ApplyResources(this.autoToolStripMenuItem, "autoToolStripMenuItem");
            this.autoToolStripMenuItem.Tag = "auto";
            this.autoToolStripMenuItem.Click += new System.EventHandler(this.changeM2Theme);
            // 
            // unitedStatesToolStripMenuItem
            // 
            this.unitedStatesToolStripMenuItem.Name = "unitedStatesToolStripMenuItem";
            resources.ApplyResources(this.unitedStatesToolStripMenuItem, "unitedStatesToolStripMenuItem");
            this.unitedStatesToolStripMenuItem.Tag = "us";
            this.unitedStatesToolStripMenuItem.Click += new System.EventHandler(this.changeM2Theme);
            // 
            // europeToolStripMenuItem
            // 
            this.europeToolStripMenuItem.Name = "europeToolStripMenuItem";
            resources.ApplyResources(this.europeToolStripMenuItem, "europeToolStripMenuItem");
            this.europeToolStripMenuItem.Tag = "eu";
            this.europeToolStripMenuItem.Click += new System.EventHandler(this.changeM2Theme);
            // 
            // japanToolStripMenuItem
            // 
            this.japanToolStripMenuItem.Name = "japanToolStripMenuItem";
            resources.ApplyResources(this.japanToolStripMenuItem, "japanToolStripMenuItem");
            this.japanToolStripMenuItem.Tag = "jp";
            this.japanToolStripMenuItem.Click += new System.EventHandler(this.changeM2Theme);
            // 
            // sFROMToolToolStripMenuItem
            // 
            this.sFROMToolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableSFROMToolToolStripMenuItem,
            this.usePCMPatchWhenAvailableToolStripMenuItem});
            this.sFROMToolToolStripMenuItem.Name = "sFROMToolToolStripMenuItem";
            resources.ApplyResources(this.sFROMToolToolStripMenuItem, "sFROMToolToolStripMenuItem");
            // 
            // enableSFROMToolToolStripMenuItem
            // 
            this.enableSFROMToolToolStripMenuItem.CheckOnClick = true;
            this.enableSFROMToolToolStripMenuItem.Name = "enableSFROMToolToolStripMenuItem";
            resources.ApplyResources(this.enableSFROMToolToolStripMenuItem, "enableSFROMToolToolStripMenuItem");
            this.enableSFROMToolToolStripMenuItem.Click += new System.EventHandler(this.enableSFROMToolToolStripMenuItem_Click);
            // 
            // usePCMPatchWhenAvailableToolStripMenuItem
            // 
            this.usePCMPatchWhenAvailableToolStripMenuItem.CheckOnClick = true;
            this.usePCMPatchWhenAvailableToolStripMenuItem.Name = "usePCMPatchWhenAvailableToolStripMenuItem";
            resources.ApplyResources(this.usePCMPatchWhenAvailableToolStripMenuItem, "usePCMPatchWhenAvailableToolStripMenuItem");
            this.usePCMPatchWhenAvailableToolStripMenuItem.Click += new System.EventHandler(this.usePCMPatchWhenAvailableToolStripMenuItem_Click);
            // 
            // convertSNESROMSToSFROMToolStripMenuItem
            // 
            this.convertSNESROMSToSFROMToolStripMenuItem.Checked = true;
            this.convertSNESROMSToSFROMToolStripMenuItem.CheckOnClick = true;
            this.convertSNESROMSToSFROMToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.convertSNESROMSToSFROMToolStripMenuItem.Name = "convertSNESROMSToSFROMToolStripMenuItem";
            resources.ApplyResources(this.convertSNESROMSToSFROMToolStripMenuItem, "convertSNESROMSToSFROMToolStripMenuItem");
            this.convertSNESROMSToSFROMToolStripMenuItem.Click += new System.EventHandler(this.convertSNESROMSToSFROMToolStripMenuItem_Click);
            // 
            // separateGamesStorageToolStripMenuItem
            // 
            this.separateGamesStorageToolStripMenuItem.Checked = true;
            this.separateGamesStorageToolStripMenuItem.CheckOnClick = true;
            this.separateGamesStorageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.separateGamesStorageToolStripMenuItem.Name = "separateGamesStorageToolStripMenuItem";
            resources.ApplyResources(this.separateGamesStorageToolStripMenuItem, "separateGamesStorageToolStripMenuItem");
            this.separateGamesStorageToolStripMenuItem.Click += new System.EventHandler(this.separateGamesStorageToolStripMenuItem_Click);
            // 
            // compressGamesToolStripMenuItem
            // 
            this.compressGamesToolStripMenuItem.Checked = true;
            this.compressGamesToolStripMenuItem.CheckOnClick = true;
            this.compressGamesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compressGamesToolStripMenuItem.Name = "compressGamesToolStripMenuItem";
            resources.ApplyResources(this.compressGamesToolStripMenuItem, "compressGamesToolStripMenuItem");
            this.compressGamesToolStripMenuItem.Click += new System.EventHandler(this.compressGamesToolStripMenuItem_Click);
            // 
            // compressBoxArtToolStripMenuItem
            // 
            this.compressBoxArtToolStripMenuItem.Checked = true;
            this.compressBoxArtToolStripMenuItem.CheckOnClick = true;
            this.compressBoxArtToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.compressBoxArtToolStripMenuItem.Name = "compressBoxArtToolStripMenuItem";
            resources.ApplyResources(this.compressBoxArtToolStripMenuItem, "compressBoxArtToolStripMenuItem");
            this.compressBoxArtToolStripMenuItem.Click += new System.EventHandler(this.compressBoxArtToolStripMenuItem_Click);
            // 
            // centerBoxArtThumbnailToolStripMenuItem
            // 
            this.centerBoxArtThumbnailToolStripMenuItem.Checked = true;
            this.centerBoxArtThumbnailToolStripMenuItem.CheckOnClick = true;
            this.centerBoxArtThumbnailToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.centerBoxArtThumbnailToolStripMenuItem.Name = "centerBoxArtThumbnailToolStripMenuItem";
            resources.ApplyResources(this.centerBoxArtThumbnailToolStripMenuItem, "centerBoxArtThumbnailToolStripMenuItem");
            this.centerBoxArtThumbnailToolStripMenuItem.Click += new System.EventHandler(this.centerBoxArtThumbnailToolStripMenuItem_Click);
            // 
            // disableHakchi2PopupsToolStripMenuItem
            // 
            this.disableHakchi2PopupsToolStripMenuItem.Checked = true;
            this.disableHakchi2PopupsToolStripMenuItem.CheckOnClick = true;
            this.disableHakchi2PopupsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.disableHakchi2PopupsToolStripMenuItem.Name = "disableHakchi2PopupsToolStripMenuItem";
            resources.ApplyResources(this.disableHakchi2PopupsToolStripMenuItem, "disableHakchi2PopupsToolStripMenuItem");
            this.disableHakchi2PopupsToolStripMenuItem.Click += new System.EventHandler(this.disableHakchi2PopupsToolStripMenuItem_Click);
            // 
            // enableInformationScrapeOnImportToolStripMenuItem
            // 
            this.enableInformationScrapeOnImportToolStripMenuItem.Checked = true;
            this.enableInformationScrapeOnImportToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enableInformationScrapeOnImportToolStripMenuItem.Name = "enableInformationScrapeOnImportToolStripMenuItem";
            resources.ApplyResources(this.enableInformationScrapeOnImportToolStripMenuItem, "enableInformationScrapeOnImportToolStripMenuItem");
            this.enableInformationScrapeOnImportToolStripMenuItem.Click += new System.EventHandler(this.enableInformationScrapeOnImportToolStripMenuItem_Click);
            // 
            // toolStripMenuItem25
            // 
            this.toolStripMenuItem25.Name = "toolStripMenuItem25";
            resources.ApplyResources(this.toolStripMenuItem25, "toolStripMenuItem25");
            // 
            // developerToolsToolStripMenuItem
            // 
            this.developerToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.devForceSshToolStripMenuItem,
            this.uploadTotmpforTestingToolStripMenuItem,
            this.toolStripMenuItem23,
            this.forceNetworkMembootsToolStripMenuItem,
            this.forceClovershellMembootsToolStripMenuItem,
            this.downloadLatestHakchiToolStripMenuItem});
            this.developerToolsToolStripMenuItem.Name = "developerToolsToolStripMenuItem";
            resources.ApplyResources(this.developerToolsToolStripMenuItem, "developerToolsToolStripMenuItem");
            // 
            // devForceSshToolStripMenuItem
            // 
            this.devForceSshToolStripMenuItem.CheckOnClick = true;
            this.devForceSshToolStripMenuItem.Name = "devForceSshToolStripMenuItem";
            resources.ApplyResources(this.devForceSshToolStripMenuItem, "devForceSshToolStripMenuItem");
            this.devForceSshToolStripMenuItem.Click += new System.EventHandler(this.devForceSshToolStripMenuItem_Click);
            // 
            // uploadTotmpforTestingToolStripMenuItem
            // 
            this.uploadTotmpforTestingToolStripMenuItem.CheckOnClick = true;
            this.uploadTotmpforTestingToolStripMenuItem.Name = "uploadTotmpforTestingToolStripMenuItem";
            resources.ApplyResources(this.uploadTotmpforTestingToolStripMenuItem, "uploadTotmpforTestingToolStripMenuItem");
            this.uploadTotmpforTestingToolStripMenuItem.Click += new System.EventHandler(this.uploadTotmpforTestingToolStripMenuItem_Click);
            // 
            // toolStripMenuItem23
            // 
            this.toolStripMenuItem23.Name = "toolStripMenuItem23";
            resources.ApplyResources(this.toolStripMenuItem23, "toolStripMenuItem23");
            // 
            // forceNetworkMembootsToolStripMenuItem
            // 
            this.forceNetworkMembootsToolStripMenuItem.CheckOnClick = true;
            this.forceNetworkMembootsToolStripMenuItem.Name = "forceNetworkMembootsToolStripMenuItem";
            resources.ApplyResources(this.forceNetworkMembootsToolStripMenuItem, "forceNetworkMembootsToolStripMenuItem");
            this.forceNetworkMembootsToolStripMenuItem.Click += new System.EventHandler(this.forceNetworkMembootsToolStripMenuItem_Click);
            // 
            // forceClovershellMembootsToolStripMenuItem
            // 
            this.forceClovershellMembootsToolStripMenuItem.CheckOnClick = true;
            this.forceClovershellMembootsToolStripMenuItem.Name = "forceClovershellMembootsToolStripMenuItem";
            resources.ApplyResources(this.forceClovershellMembootsToolStripMenuItem, "forceClovershellMembootsToolStripMenuItem");
            this.forceClovershellMembootsToolStripMenuItem.Click += new System.EventHandler(this.forceClovershellMembootsToolStripMenuItem_Click);
            // 
            // downloadLatestHakchiToolStripMenuItem
            // 
            this.downloadLatestHakchiToolStripMenuItem.Name = "downloadLatestHakchiToolStripMenuItem";
            resources.ApplyResources(this.downloadLatestHakchiToolStripMenuItem, "downloadLatestHakchiToolStripMenuItem");
            this.downloadLatestHakchiToolStripMenuItem.Click += new System.EventHandler(this.DownloadLatestHakchiToolStripMenuItem_Click);
            // 
            // separateGamesForMultibootToolStripMenuItem
            // 
            this.separateGamesForMultibootToolStripMenuItem.Checked = true;
            this.separateGamesForMultibootToolStripMenuItem.CheckOnClick = true;
            this.separateGamesForMultibootToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.separateGamesForMultibootToolStripMenuItem.Name = "separateGamesForMultibootToolStripMenuItem";
            resources.ApplyResources(this.separateGamesForMultibootToolStripMenuItem, "separateGamesForMultibootToolStripMenuItem");
            this.separateGamesForMultibootToolStripMenuItem.Click += new System.EventHandler(this.separateGamesForMultibootToolStripMenuItem_Click);
            // 
            // alwaysCopyOriginalGamesToolStripMenuItem
            // 
            this.alwaysCopyOriginalGamesToolStripMenuItem.Checked = true;
            this.alwaysCopyOriginalGamesToolStripMenuItem.CheckOnClick = true;
            this.alwaysCopyOriginalGamesToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.alwaysCopyOriginalGamesToolStripMenuItem.Name = "alwaysCopyOriginalGamesToolStripMenuItem";
            resources.ApplyResources(this.alwaysCopyOriginalGamesToolStripMenuItem, "alwaysCopyOriginalGamesToolStripMenuItem");
            this.alwaysCopyOriginalGamesToolStripMenuItem.Click += new System.EventHandler(this.alwaysCopyOriginalGamesToolStripMenuItem_Click);
            // 
            // useLinkedSyncToolStripMenuItem
            // 
            this.useLinkedSyncToolStripMenuItem.Checked = true;
            this.useLinkedSyncToolStripMenuItem.CheckOnClick = true;
            this.useLinkedSyncToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useLinkedSyncToolStripMenuItem.Name = "useLinkedSyncToolStripMenuItem";
            resources.ApplyResources(this.useLinkedSyncToolStripMenuItem, "useLinkedSyncToolStripMenuItem");
            this.useLinkedSyncToolStripMenuItem.Click += new System.EventHandler(this.useLinkedSyncToolStripMenuItem_Click);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            resources.ApplyResources(this.toolStripMenuItem16, "toolStripMenuItem16");
            // 
            // cloverconHackToolStripMenuItem
            // 
            this.cloverconHackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetUsingCombinationOfButtonsToolStripMenuItem,
            this.selectButtonCombinationToolStripMenuItem,
            this.enableAutofireToolStripMenuItem,
            this.useXYOnClassicControllerAsAutofireABToolStripMenuItem,
            this.upABStartOnSecondControllerToolStripMenuItem});
            this.cloverconHackToolStripMenuItem.Name = "cloverconHackToolStripMenuItem";
            resources.ApplyResources(this.cloverconHackToolStripMenuItem, "cloverconHackToolStripMenuItem");
            // 
            // resetUsingCombinationOfButtonsToolStripMenuItem
            // 
            this.resetUsingCombinationOfButtonsToolStripMenuItem.Checked = true;
            this.resetUsingCombinationOfButtonsToolStripMenuItem.CheckOnClick = true;
            this.resetUsingCombinationOfButtonsToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetUsingCombinationOfButtonsToolStripMenuItem.Name = "resetUsingCombinationOfButtonsToolStripMenuItem";
            resources.ApplyResources(this.resetUsingCombinationOfButtonsToolStripMenuItem, "resetUsingCombinationOfButtonsToolStripMenuItem");
            this.resetUsingCombinationOfButtonsToolStripMenuItem.Click += new System.EventHandler(this.cloverconHackToolStripMenuItem_Click);
            // 
            // selectButtonCombinationToolStripMenuItem
            // 
            this.selectButtonCombinationToolStripMenuItem.Name = "selectButtonCombinationToolStripMenuItem";
            resources.ApplyResources(this.selectButtonCombinationToolStripMenuItem, "selectButtonCombinationToolStripMenuItem");
            this.selectButtonCombinationToolStripMenuItem.Click += new System.EventHandler(this.selectButtonCombinationToolStripMenuItem_Click);
            // 
            // enableAutofireToolStripMenuItem
            // 
            this.enableAutofireToolStripMenuItem.CheckOnClick = true;
            this.enableAutofireToolStripMenuItem.Name = "enableAutofireToolStripMenuItem";
            resources.ApplyResources(this.enableAutofireToolStripMenuItem, "enableAutofireToolStripMenuItem");
            this.enableAutofireToolStripMenuItem.Click += new System.EventHandler(this.enableAutofireToolStripMenuItem_Click);
            // 
            // useXYOnClassicControllerAsAutofireABToolStripMenuItem
            // 
            this.useXYOnClassicControllerAsAutofireABToolStripMenuItem.CheckOnClick = true;
            this.useXYOnClassicControllerAsAutofireABToolStripMenuItem.Name = "useXYOnClassicControllerAsAutofireABToolStripMenuItem";
            resources.ApplyResources(this.useXYOnClassicControllerAsAutofireABToolStripMenuItem, "useXYOnClassicControllerAsAutofireABToolStripMenuItem");
            this.useXYOnClassicControllerAsAutofireABToolStripMenuItem.Click += new System.EventHandler(this.useXYOnClassicControllerAsAutofireABToolStripMenuItem_Click);
            // 
            // upABStartOnSecondControllerToolStripMenuItem
            // 
            this.upABStartOnSecondControllerToolStripMenuItem.CheckOnClick = true;
            this.upABStartOnSecondControllerToolStripMenuItem.Name = "upABStartOnSecondControllerToolStripMenuItem";
            resources.ApplyResources(this.upABStartOnSecondControllerToolStripMenuItem, "upABStartOnSecondControllerToolStripMenuItem");
            this.upABStartOnSecondControllerToolStripMenuItem.Click += new System.EventHandler(this.upABStartOnSecondControllerToolStripMenuItem_Click);
            // 
            // globalCommandLineArgumentsexpertsOnluToolStripMenuItem
            // 
            this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kachikachiToolStripMenuItem,
            this.canoeToolStripMenuItem,
            this.retroarchToolStripMenuItem});
            this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem.Name = "globalCommandLineArgumentsexpertsOnluToolStripMenuItem";
            resources.ApplyResources(this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem, "globalCommandLineArgumentsexpertsOnluToolStripMenuItem");
            // 
            // kachikachiToolStripMenuItem
            // 
            this.kachikachiToolStripMenuItem.Name = "kachikachiToolStripMenuItem";
            resources.ApplyResources(this.kachikachiToolStripMenuItem, "kachikachiToolStripMenuItem");
            this.kachikachiToolStripMenuItem.Tag = "0";
            this.kachikachiToolStripMenuItem.Click += new System.EventHandler(this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem_Click);
            // 
            // canoeToolStripMenuItem
            // 
            this.canoeToolStripMenuItem.Name = "canoeToolStripMenuItem";
            resources.ApplyResources(this.canoeToolStripMenuItem, "canoeToolStripMenuItem");
            this.canoeToolStripMenuItem.Tag = "1";
            this.canoeToolStripMenuItem.Click += new System.EventHandler(this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem_Click);
            // 
            // retroarchToolStripMenuItem
            // 
            this.retroarchToolStripMenuItem.Name = "retroarchToolStripMenuItem";
            resources.ApplyResources(this.retroarchToolStripMenuItem, "retroarchToolStripMenuItem");
            this.retroarchToolStripMenuItem.Tag = "2";
            this.retroarchToolStripMenuItem.Click += new System.EventHandler(this.globalCommandLineArgumentsexpertsOnluToolStripMenuItem_Click);
            // 
            // epilepsyProtectionToolStripMenuItem
            // 
            this.epilepsyProtectionToolStripMenuItem.Checked = true;
            this.epilepsyProtectionToolStripMenuItem.CheckOnClick = true;
            this.epilepsyProtectionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.epilepsyProtectionToolStripMenuItem.Name = "epilepsyProtectionToolStripMenuItem";
            resources.ApplyResources(this.epilepsyProtectionToolStripMenuItem, "epilepsyProtectionToolStripMenuItem");
            this.epilepsyProtectionToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemArmet_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            // 
            // saveSettingsToNESMiniNowToolStripMenuItem
            // 
            resources.ApplyResources(this.saveSettingsToNESMiniNowToolStripMenuItem, "saveSettingsToNESMiniNowToolStripMenuItem");
            this.saveSettingsToNESMiniNowToolStripMenuItem.Name = "saveSettingsToNESMiniNowToolStripMenuItem";
            this.saveSettingsToNESMiniNowToolStripMenuItem.Click += new System.EventHandler(this.saveSettingsToNESMiniNowToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveStateManagerToolStripMenuItem,
            this.importGamesFromMiniToolStripMenuItem,
            this.takeScreenshotToolStripMenuItem,
            this.saveDmesgOutputToolStripMenuItem,
            this.toolStripMenuItem6,
            this.openFTPInExplorerToolStripMenuItem,
            this.openTelnetToolStripMenuItem,
            this.toolStripMenuItem8,
            this.bootImageToolStripMenuItem,
            this.rebootToolStripMenuItem,
            this.switchRunningFirmwareToolStripMenuItem,
            this.formatSDCardToolStripMenuItem,
            this.toolStripMenuItem18,
            this.prepareArtDirectoryToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // saveStateManagerToolStripMenuItem
            // 
            this.saveStateManagerToolStripMenuItem.Name = "saveStateManagerToolStripMenuItem";
            resources.ApplyResources(this.saveStateManagerToolStripMenuItem, "saveStateManagerToolStripMenuItem");
            this.saveStateManagerToolStripMenuItem.Click += new System.EventHandler(this.saveStateManagerToolStripMenuItem_Click);
            // 
            // takeScreenshotToolStripMenuItem
            // 
            this.takeScreenshotToolStripMenuItem.Name = "takeScreenshotToolStripMenuItem";
            resources.ApplyResources(this.takeScreenshotToolStripMenuItem, "takeScreenshotToolStripMenuItem");
            this.takeScreenshotToolStripMenuItem.Click += new System.EventHandler(this.takeScreenshotToolStripMenuItem_Click);
            // 
            // saveDmesgOutputToolStripMenuItem
            // 
            this.saveDmesgOutputToolStripMenuItem.Name = "saveDmesgOutputToolStripMenuItem";
            resources.ApplyResources(this.saveDmesgOutputToolStripMenuItem, "saveDmesgOutputToolStripMenuItem");
            this.saveDmesgOutputToolStripMenuItem.Click += new System.EventHandler(this.saveDmesgOutputToolStripMenuItem_Click);
            // 
            // importGamesFromMiniToolStripMenuItem
            // 
            this.importGamesFromMiniToolStripMenuItem.Name = "importGamesFromMiniToolStripMenuItem";
            resources.ApplyResources(this.importGamesFromMiniToolStripMenuItem, "importGamesFromMiniToolStripMenuItem");
            this.importGamesFromMiniToolStripMenuItem.Click += new System.EventHandler(this.importGamesFromMiniToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            // 
            // openFTPInExplorerToolStripMenuItem
            // 
            resources.ApplyResources(this.openFTPInExplorerToolStripMenuItem, "openFTPInExplorerToolStripMenuItem");
            this.openFTPInExplorerToolStripMenuItem.Name = "openFTPInExplorerToolStripMenuItem";
            this.openFTPInExplorerToolStripMenuItem.Click += new System.EventHandler(this.openFTPInExplorerToolStripMenuItem_Click);
            // 
            // openTelnetToolStripMenuItem
            // 
            resources.ApplyResources(this.openTelnetToolStripMenuItem, "openTelnetToolStripMenuItem");
            this.openTelnetToolStripMenuItem.Name = "openTelnetToolStripMenuItem";
            this.openTelnetToolStripMenuItem.Click += new System.EventHandler(this.openTelnetToolStripMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
            // 
            // bootImageToolStripMenuItem
            // 
            this.bootImageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeBootImageToolStripMenuItem,
            this.disableBootImageToolStripMenuItem,
            this.resetDefaultBootImageToolStripMenuItem});
            this.bootImageToolStripMenuItem.Name = "bootImageToolStripMenuItem";
            resources.ApplyResources(this.bootImageToolStripMenuItem, "bootImageToolStripMenuItem");
            // 
            // changeBootImageToolStripMenuItem
            // 
            this.changeBootImageToolStripMenuItem.Name = "changeBootImageToolStripMenuItem";
            resources.ApplyResources(this.changeBootImageToolStripMenuItem, "changeBootImageToolStripMenuItem");
            this.changeBootImageToolStripMenuItem.Click += new System.EventHandler(this.changeBootImageToolStripMenuItem_Click);
            // 
            // disableBootImageToolStripMenuItem
            // 
            this.disableBootImageToolStripMenuItem.Name = "disableBootImageToolStripMenuItem";
            resources.ApplyResources(this.disableBootImageToolStripMenuItem, "disableBootImageToolStripMenuItem");
            this.disableBootImageToolStripMenuItem.Click += new System.EventHandler(this.disableBootImageToolStripMenuItem_Click);
            // 
            // resetDefaultBootImageToolStripMenuItem
            // 
            this.resetDefaultBootImageToolStripMenuItem.Name = "resetDefaultBootImageToolStripMenuItem";
            resources.ApplyResources(this.resetDefaultBootImageToolStripMenuItem, "resetDefaultBootImageToolStripMenuItem");
            this.resetDefaultBootImageToolStripMenuItem.Click += new System.EventHandler(this.resetDefaultBootImageToolStripMenuItem_Click);
            // 
            // rebootToolStripMenuItem
            // 
            this.rebootToolStripMenuItem.Name = "rebootToolStripMenuItem";
            resources.ApplyResources(this.rebootToolStripMenuItem, "rebootToolStripMenuItem");
            this.rebootToolStripMenuItem.Click += new System.EventHandler(this.rebootToolStripMenuItem_Click);
            // 
            // switchRunningFirmwareToolStripMenuItem
            // 
            this.switchRunningFirmwareToolStripMenuItem.Name = "switchRunningFirmwareToolStripMenuItem";
            resources.ApplyResources(this.switchRunningFirmwareToolStripMenuItem, "switchRunningFirmwareToolStripMenuItem");
            this.switchRunningFirmwareToolStripMenuItem.Click += new System.EventHandler(this.switchRunningFirmwareToolStripMenuItem_Click);
            // 
            // formatSDCardToolStripMenuItem
            // 
            resources.ApplyResources(this.formatSDCardToolStripMenuItem, "formatSDCardToolStripMenuItem");
            this.formatSDCardToolStripMenuItem.Name = "formatSDCardToolStripMenuItem";
            this.formatSDCardToolStripMenuItem.Click += new System.EventHandler(this.formatSDCardToolStripMenuItem_Click);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            resources.ApplyResources(this.toolStripMenuItem18, "toolStripMenuItem18");
            // 
            // prepareArtDirectoryToolStripMenuItem
            // 
            this.prepareArtDirectoryToolStripMenuItem.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.folder_sm;
            this.prepareArtDirectoryToolStripMenuItem.Name = "prepareArtDirectoryToolStripMenuItem";
            resources.ApplyResources(this.prepareArtDirectoryToolStripMenuItem, "prepareArtDirectoryToolStripMenuItem");
            this.prepareArtDirectoryToolStripMenuItem.Click += new System.EventHandler(this.prepareArtDirectoryToolStripMenuItem_Click);
            // 
            // bluetoothToolStripMenuItem
            // 
            this.bluetoothToolStripMenuItem.Name = "bluetoothToolStripMenuItem";
            resources.ApplyResources(this.bluetoothToolStripMenuItem, "bluetoothToolStripMenuItem");
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.gitHubPageWithActualReleasesToolStripMenuItem,
            this.joinOurDiscordServerToolStripMenuItem,
            this.rRockinTheClassicsToolStripMenuItem,
            this.donateToolStripMenuItem,
            this.fAQToolStripMenuItem,
            this.toolStripMenuItem22,
            this.technicalInformationToolStripMenuItem,
            this.messageOfTheDayToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            // 
            // gitHubPageWithActualReleasesToolStripMenuItem
            // 
            this.gitHubPageWithActualReleasesToolStripMenuItem.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.github;
            this.gitHubPageWithActualReleasesToolStripMenuItem.Name = "gitHubPageWithActualReleasesToolStripMenuItem";
            resources.ApplyResources(this.gitHubPageWithActualReleasesToolStripMenuItem, "gitHubPageWithActualReleasesToolStripMenuItem");
            this.gitHubPageWithActualReleasesToolStripMenuItem.Tag = "https://github.com/TeamShinkansen/hakchi2/releases";
            this.gitHubPageWithActualReleasesToolStripMenuItem.Click += new System.EventHandler(this.openWebsiteLink);
            // 
            // joinOurDiscordServerToolStripMenuItem
            // 
            this.joinOurDiscordServerToolStripMenuItem.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.discord;
            this.joinOurDiscordServerToolStripMenuItem.Name = "joinOurDiscordServerToolStripMenuItem";
            resources.ApplyResources(this.joinOurDiscordServerToolStripMenuItem, "joinOurDiscordServerToolStripMenuItem");
            this.joinOurDiscordServerToolStripMenuItem.Tag = "https://discord.gg/C9EDFyg";
            this.joinOurDiscordServerToolStripMenuItem.Click += new System.EventHandler(this.openWebsiteLink);
            // 
            // rRockinTheClassicsToolStripMenuItem
            // 
            this.rRockinTheClassicsToolStripMenuItem.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.reddit;
            this.rRockinTheClassicsToolStripMenuItem.Name = "rRockinTheClassicsToolStripMenuItem";
            resources.ApplyResources(this.rRockinTheClassicsToolStripMenuItem, "rRockinTheClassicsToolStripMenuItem");
            this.rRockinTheClassicsToolStripMenuItem.Tag = "https://www.reddit.com/r/RockinTheClassics/";
            this.rRockinTheClassicsToolStripMenuItem.Click += new System.EventHandler(this.openWebsiteLink);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.paypal;
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            resources.ApplyResources(this.donateToolStripMenuItem, "donateToolStripMenuItem");
            this.donateToolStripMenuItem.Tag = "https://www.paypal.me/clusterm";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.openWebsiteLink);
            // 
            // fAQToolStripMenuItem
            // 
            this.fAQToolStripMenuItem.Name = "fAQToolStripMenuItem";
            resources.ApplyResources(this.fAQToolStripMenuItem, "fAQToolStripMenuItem");
            this.fAQToolStripMenuItem.Tag = "https://github.com/TeamShinkansen/hakchi2/wiki/FAQ";
            this.fAQToolStripMenuItem.Click += new System.EventHandler(this.openWebsiteLink);
            // 
            // toolStripMenuItem22
            // 
            this.toolStripMenuItem22.Name = "toolStripMenuItem22";
            resources.ApplyResources(this.toolStripMenuItem22, "toolStripMenuItem22");
            // 
            // technicalInformationToolStripMenuItem
            // 
            this.technicalInformationToolStripMenuItem.Name = "technicalInformationToolStripMenuItem";
            resources.ApplyResources(this.technicalInformationToolStripMenuItem, "technicalInformationToolStripMenuItem");
            this.technicalInformationToolStripMenuItem.Click += new System.EventHandler(this.technicalInformationToolStripMenuItem_Click);
            // 
            // messageOfTheDayToolStripMenuItem
            // 
            this.messageOfTheDayToolStripMenuItem.Name = "messageOfTheDayToolStripMenuItem";
            resources.ApplyResources(this.messageOfTheDayToolStripMenuItem, "messageOfTheDayToolStripMenuItem");
            this.messageOfTheDayToolStripMenuItem.Click += new System.EventHandler(this.messageOfTheDayToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // buttonAddGames
            // 
            resources.ApplyResources(this.buttonAddGames, "buttonAddGames");
            this.buttonAddGames.Name = "buttonAddGames";
            this.buttonAddGames.UseVisualStyleBackColor = true;
            this.buttonAddGames.Click += new System.EventHandler(this.buttonAddGames_Click);
            // 
            // openFileDialogNes
            // 
            this.openFileDialogNes.DefaultExt = "nes";
            this.openFileDialogNes.Multiselect = true;
            resources.ApplyResources(this.openFileDialogNes, "openFileDialogNes");
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.explorerToolStripMenuItem,
            this.toolStripSeparator2,
            this.addPrefixToolStripMenuItem,
            this.removePrefixToolStripMenuItem,
            this.toolStripMenuItem14,
            this.scrapeSelectedGamesToolStripMenuItem,
            this.scanForNewBoxArtForSelectedGamesToolStripMenuItem,
            this.downloadBoxArtForSelectedGamesToolStripMenuItem,
            this.deleteSelectedGamesBoxArtToolStripMenuItem,
            this.toolStripMenuItem15,
            this.archiveSelectedGamesToolStripMenuItem,
            this.compressSelectedGamesToolStripMenuItem,
            this.decompressSelectedGamesToolStripMenuItem,
            this.deleteSelectedGamesToolStripMenuItem,
            this.toolStripMenuItem17,
            this.sFROMToolToolStripMenuItem1,
            this.repairGamesToolStripMenuItem,
            this.toolStripMenuItem19,
            this.selectEmulationCoreToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // explorerToolStripMenuItem
            // 
            resources.ApplyResources(this.explorerToolStripMenuItem, "explorerToolStripMenuItem");
            this.explorerToolStripMenuItem.Name = "explorerToolStripMenuItem";
            this.explorerToolStripMenuItem.Click += new System.EventHandler(this.explorerToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // addPrefixToolStripMenuItem
            // 
            resources.ApplyResources(this.addPrefixToolStripMenuItem, "addPrefixToolStripMenuItem");
            this.addPrefixToolStripMenuItem.Name = "addPrefixToolStripMenuItem";
            this.addPrefixToolStripMenuItem.Click += new System.EventHandler(this.addPrefixToolStripMenuItem_Click);
            // 
            // removePrefixToolStripMenuItem
            // 
            resources.ApplyResources(this.removePrefixToolStripMenuItem, "removePrefixToolStripMenuItem");
            this.removePrefixToolStripMenuItem.Name = "removePrefixToolStripMenuItem";
            this.removePrefixToolStripMenuItem.Click += new System.EventHandler(this.removePrefixToolStripMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            resources.ApplyResources(this.toolStripMenuItem14, "toolStripMenuItem14");
            // 
            // scrapeSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.scrapeSelectedGamesToolStripMenuItem, "scrapeSelectedGamesToolStripMenuItem");
            this.scrapeSelectedGamesToolStripMenuItem.Name = "scrapeSelectedGamesToolStripMenuItem";
            this.scrapeSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.scrapeSelectedGamesToolStripMenuItem_Click);
            // 
            // scanForNewBoxArtForSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.scanForNewBoxArtForSelectedGamesToolStripMenuItem, "scanForNewBoxArtForSelectedGamesToolStripMenuItem");
            this.scanForNewBoxArtForSelectedGamesToolStripMenuItem.Name = "scanForNewBoxArtForSelectedGamesToolStripMenuItem";
            this.scanForNewBoxArtForSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.scanForNewBoxArtForSelectedGamesToolStripMenuItem_Click);
            // 
            // downloadBoxArtForSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.downloadBoxArtForSelectedGamesToolStripMenuItem, "downloadBoxArtForSelectedGamesToolStripMenuItem");
            this.downloadBoxArtForSelectedGamesToolStripMenuItem.Name = "downloadBoxArtForSelectedGamesToolStripMenuItem";
            this.downloadBoxArtForSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.downloadBoxArtForSelectedGamesToolStripMenuItem_Click);
            // 
            // deleteSelectedGamesBoxArtToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteSelectedGamesBoxArtToolStripMenuItem, "deleteSelectedGamesBoxArtToolStripMenuItem");
            this.deleteSelectedGamesBoxArtToolStripMenuItem.Name = "deleteSelectedGamesBoxArtToolStripMenuItem";
            this.deleteSelectedGamesBoxArtToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedGamesBoxArtToolStripMenuItem_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            resources.ApplyResources(this.toolStripMenuItem15, "toolStripMenuItem15");
            // 
            // archiveSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.archiveSelectedGamesToolStripMenuItem, "archiveSelectedGamesToolStripMenuItem");
            this.archiveSelectedGamesToolStripMenuItem.Name = "archiveSelectedGamesToolStripMenuItem";
            this.archiveSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.archiveSelectedGamesToolStripMenuItem_Click);
            // 
            // compressSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.compressSelectedGamesToolStripMenuItem, "compressSelectedGamesToolStripMenuItem");
            this.compressSelectedGamesToolStripMenuItem.Name = "compressSelectedGamesToolStripMenuItem";
            this.compressSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.compressSelectedGamesToolStripMenuItem_Click);
            // 
            // decompressSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.decompressSelectedGamesToolStripMenuItem, "decompressSelectedGamesToolStripMenuItem");
            this.decompressSelectedGamesToolStripMenuItem.Name = "decompressSelectedGamesToolStripMenuItem";
            this.decompressSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.decompressSelectedGamesToolStripMenuItem_Click);
            // 
            // deleteSelectedGamesToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteSelectedGamesToolStripMenuItem, "deleteSelectedGamesToolStripMenuItem");
            this.deleteSelectedGamesToolStripMenuItem.Name = "deleteSelectedGamesToolStripMenuItem";
            this.deleteSelectedGamesToolStripMenuItem.Click += new System.EventHandler(this.deleteSelectedGamesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            resources.ApplyResources(this.toolStripMenuItem17, "toolStripMenuItem17");
            // 
            // sFROMToolToolStripMenuItem1
            // 
            this.sFROMToolToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editROMHeaderToolStripMenuItem,
            this.toolStripMenuItem9,
            this.resetROMHeaderToolStripMenuItem});
            this.sFROMToolToolStripMenuItem1.Name = "sFROMToolToolStripMenuItem1";
            resources.ApplyResources(this.sFROMToolToolStripMenuItem1, "sFROMToolToolStripMenuItem1");
            // 
            // editROMHeaderToolStripMenuItem
            // 
            this.editROMHeaderToolStripMenuItem.Name = "editROMHeaderToolStripMenuItem";
            resources.ApplyResources(this.editROMHeaderToolStripMenuItem, "editROMHeaderToolStripMenuItem");
            this.editROMHeaderToolStripMenuItem.Click += new System.EventHandler(this.editROMHeaderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
            // 
            // resetROMHeaderToolStripMenuItem
            // 
            this.resetROMHeaderToolStripMenuItem.Name = "resetROMHeaderToolStripMenuItem";
            resources.ApplyResources(this.resetROMHeaderToolStripMenuItem, "resetROMHeaderToolStripMenuItem");
            this.resetROMHeaderToolStripMenuItem.Click += new System.EventHandler(this.resetROMHeaderToolStripMenuItem_Click);
            // 
            // repairGamesToolStripMenuItem
            // 
            this.repairGamesToolStripMenuItem.Name = "repairGamesToolStripMenuItem";
            resources.ApplyResources(this.repairGamesToolStripMenuItem, "repairGamesToolStripMenuItem");
            this.repairGamesToolStripMenuItem.Click += new System.EventHandler(this.repairGamesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            resources.ApplyResources(this.toolStripMenuItem19, "toolStripMenuItem19");
            // 
            // selectEmulationCoreToolStripMenuItem
            // 
            this.selectEmulationCoreToolStripMenuItem.Name = "selectEmulationCoreToolStripMenuItem";
            resources.ApplyResources(this.selectEmulationCoreToolStripMenuItem, "selectEmulationCoreToolStripMenuItem");
            this.selectEmulationCoreToolStripMenuItem.Click += new System.EventHandler(this.selectEmulationCoreToolStripMenuItem_Click);
            // 
            // openFileDialogImage
            // 
            resources.ApplyResources(this.openFileDialogImage, "openFileDialogImage");
            // 
            // buttonStart
            // 
            resources.ApplyResources(this.buttonStart, "buttonStart");
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // timerCalculateGames
            // 
            this.timerCalculateGames.Interval = 500;
            this.timerCalculateGames.Tick += new System.EventHandler(this.timerCalculateGames_Tick);
            // 
            // timerConnectionCheck
            // 
            this.timerConnectionCheck.Interval = 500;
            this.timerConnectionCheck.Tick += new System.EventHandler(this.timerConnectionCheck_Tick);
            // 
            // saveDumpFileDialog
            // 
            this.saveDumpFileDialog.DefaultExt = "bin";
            this.saveDumpFileDialog.FileName = "nand.bin";
            resources.ApplyResources(this.saveDumpFileDialog, "saveDumpFileDialog");
            // 
            // openDumpFileDialog
            // 
            this.openDumpFileDialog.FileName = "...";
            resources.ApplyResources(this.openDumpFileDialog, "openDumpFileDialog");
            // 
            // listViewGames
            // 
            this.listViewGames.CheckBoxes = true;
            this.listViewGames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.gameName});
            this.tableLayoutPanel3.SetColumnSpan(this.listViewGames, 2);
            resources.ApplyResources(this.listViewGames, "listViewGames");
            this.listViewGames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewGames.HideSelection = false;
            this.listViewGames.Name = "listViewGames";
            this.listViewGames.UseCompatibleStateImageBehavior = false;
            this.listViewGames.View = System.Windows.Forms.View.Details;
            this.listViewGames.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewGames_ItemCheck);
            this.listViewGames.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewGames_ItemSelectionChanged);
            this.listViewGames.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewGames_KeyDown);
            this.listViewGames.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewGames_MouseDown);
            this.listViewGames.Resize += new System.EventHandler(this.listViewGames_Resize);
            // 
            // gameName
            // 
            resources.ApplyResources(this.gameName, "gameName");
            // 
            // timerShowSelected
            // 
            this.timerShowSelected.Interval = 50;
            this.timerShowSelected.Tick += new System.EventHandler(this.timerShowSelected_Tick);
            // 
            // buttonExport
            // 
            resources.ApplyResources(this.buttonExport, "buttonExport");
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // labelID
            // 
            resources.ApplyResources(this.labelID, "labelID");
            this.labelID.Name = "labelID";
            // 
            // textBoxName
            // 
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.textBoxName, 2);
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // labelPublisher
            // 
            resources.ApplyResources(this.labelPublisher, "labelPublisher");
            this.labelPublisher.Name = "labelPublisher";
            // 
            // textBoxPublisher
            // 
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.textBoxPublisher, 2);
            resources.ApplyResources(this.textBoxPublisher, "textBoxPublisher");
            this.textBoxPublisher.Name = "textBoxPublisher";
            this.textBoxPublisher.TextChanged += new System.EventHandler(this.textBoxPublisher_TextChanged);
            // 
            // labelCommandLine
            // 
            resources.ApplyResources(this.labelCommandLine, "labelCommandLine");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.labelCommandLine, 2);
            this.labelCommandLine.Name = "labelCommandLine";
            // 
            // textBoxArguments
            // 
            this.textBoxArguments.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.textBoxArguments, 2);
            resources.ApplyResources(this.textBoxArguments, "textBoxArguments");
            this.textBoxArguments.Name = "textBoxArguments";
            this.textBoxArguments.TextChanged += new System.EventHandler(this.textBoxArguments_TextChanged);
            // 
            // pictureBoxArt
            // 
            this.pictureBoxArt.AllowDrop = true;
            this.pictureBoxArt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBoxArt, "pictureBoxArt");
            this.pictureBoxArt.Name = "pictureBoxArt";
            this.pictureBoxArt.TabStop = false;
            this.pictureBoxArt.Tag = com.clusterrr.hakchi_gui.NesMenuElementBase.GameImageType.CloverFront;
            this.pictureBoxArt.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragDrop);
            this.pictureBoxArt.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragEnter);
            this.pictureBoxArt.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxArt_MouseClick);
            // 
            // buttonBrowseImage
            // 
            resources.ApplyResources(this.buttonBrowseImage, "buttonBrowseImage");
            this.buttonBrowseImage.Name = "buttonBrowseImage";
            this.buttonBrowseImage.Tag = com.clusterrr.hakchi_gui.NesMenuElementBase.GameImageType.AllFront;
            this.buttonBrowseImage.UseVisualStyleBackColor = true;
            this.buttonBrowseImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxArt_MouseClick);
            // 
            // buttonGoogle
            // 
            resources.ApplyResources(this.buttonGoogle, "buttonGoogle");
            this.buttonGoogle.Name = "buttonGoogle";
            this.buttonGoogle.UseVisualStyleBackColor = true;
            this.buttonGoogle.Click += new System.EventHandler(this.buttonGoogle_Click);
            // 
            // labelMaxPlayers
            // 
            resources.ApplyResources(this.labelMaxPlayers, "labelMaxPlayers");
            this.labelMaxPlayers.Name = "labelMaxPlayers";
            // 
            // labelGameGenie
            // 
            resources.ApplyResources(this.labelGameGenie, "labelGameGenie");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.labelGameGenie, 2);
            this.labelGameGenie.Name = "labelGameGenie";
            // 
            // textBoxGameGenie
            // 
            this.textBoxGameGenie.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.textBoxGameGenie, "textBoxGameGenie");
            this.textBoxGameGenie.Name = "textBoxGameGenie";
            this.textBoxGameGenie.TextChanged += new System.EventHandler(this.textBoxGameGenie_TextChanged);
            // 
            // labelReleaseDate
            // 
            resources.ApplyResources(this.labelReleaseDate, "labelReleaseDate");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.labelReleaseDate, 2);
            this.labelReleaseDate.Name = "labelReleaseDate";
            // 
            // maskedTextBoxReleaseDate
            // 
            this.maskedTextBoxReleaseDate.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.maskedTextBoxReleaseDate, "maskedTextBoxReleaseDate");
            this.maskedTextBoxReleaseDate.Name = "maskedTextBoxReleaseDate";
            this.maskedTextBoxReleaseDate.TextChanged += new System.EventHandler(this.maskedTextBoxReleaseDate_TextChanged);
            // 
            // buttonShowGameGenieDatabase
            // 
            resources.ApplyResources(this.buttonShowGameGenieDatabase, "buttonShowGameGenieDatabase");
            this.buttonShowGameGenieDatabase.Name = "buttonShowGameGenieDatabase";
            this.buttonShowGameGenieDatabase.UseVisualStyleBackColor = true;
            this.buttonShowGameGenieDatabase.Click += new System.EventHandler(this.buttonShowGameGenieDatabase_Click);
            // 
            // checkBoxCompressed
            // 
            resources.ApplyResources(this.checkBoxCompressed, "checkBoxCompressed");
            this.checkBoxCompressed.Name = "checkBoxCompressed";
            this.checkBoxCompressed.UseVisualStyleBackColor = true;
            this.checkBoxCompressed.Click += new System.EventHandler(this.checkBoxCompressed_CheckedChanged);
            // 
            // labelSize
            // 
            resources.ApplyResources(this.labelSize, "labelSize");
            this.labelSize.Name = "labelSize";
            // 
            // buttonDefaultCover
            // 
            resources.ApplyResources(this.buttonDefaultCover, "buttonDefaultCover");
            this.buttonDefaultCover.Name = "buttonDefaultCover";
            this.buttonDefaultCover.UseVisualStyleBackColor = true;
            this.buttonDefaultCover.Click += new System.EventHandler(this.buttonDefaultCover_Click);
            // 
            // pictureBoxThumbnail
            // 
            this.pictureBoxThumbnail.AllowDrop = true;
            this.pictureBoxThumbnail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBoxThumbnail, "pictureBoxThumbnail");
            this.pictureBoxThumbnail.Name = "pictureBoxThumbnail";
            this.pictureBoxThumbnail.TabStop = false;
            this.pictureBoxThumbnail.Tag = com.clusterrr.hakchi_gui.NesMenuElementBase.GameImageType.CloverThumbnail;
            this.pictureBoxThumbnail.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragDrop);
            this.pictureBoxThumbnail.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragEnter);
            this.pictureBoxThumbnail.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxArt_MouseClick);
            // 
            // labelSortName
            // 
            resources.ApplyResources(this.labelSortName, "labelSortName");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.labelSortName, 2);
            this.labelSortName.Name = "labelSortName";
            // 
            // textBoxSortName
            // 
            this.textBoxSortName.BackColor = System.Drawing.SystemColors.Window;
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.textBoxSortName, 2);
            resources.ApplyResources(this.textBoxSortName, "textBoxSortName");
            this.textBoxSortName.Name = "textBoxSortName";
            this.textBoxSortName.TextChanged += new System.EventHandler(this.textBoxSortName_TextChanged);
            this.textBoxSortName.Leave += new System.EventHandler(this.textBoxSortName_Leave);
            // 
            // labelSaveCount
            // 
            resources.ApplyResources(this.labelSaveCount, "labelSaveCount");
            this.labelSaveCount.Name = "labelSaveCount";
            // 
            // numericUpDownSaveCount
            // 
            this.numericUpDownSaveCount.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.numericUpDownSaveCount, "numericUpDownSaveCount");
            this.numericUpDownSaveCount.Name = "numericUpDownSaveCount";
            this.numericUpDownSaveCount.ValueChanged += new System.EventHandler(this.numericUpDownSaveCount_ValueChanged);
            // 
            // tableLayoutPanelGameInfo
            // 
            resources.ApplyResources(this.tableLayoutPanelGameInfo, "tableLayoutPanelGameInfo");
            this.tableLayoutPanelGameInfo.Controls.Add(this.label10, 1, 1);
            this.tableLayoutPanelGameInfo.Controls.Add(this.panel1, 0, 22);
            this.tableLayoutPanelGameInfo.Controls.Add(this.textBoxArguments, 0, 20);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelCommandLine, 0, 19);
            this.tableLayoutPanelGameInfo.Controls.Add(this.numericUpDownSaveCount, 0, 16);
            this.tableLayoutPanelGameInfo.Controls.Add(this.maxPlayersComboBox, 0, 14);
            this.tableLayoutPanelGameInfo.Controls.Add(this.tableLayoutPanelGameID, 0, 0);
            this.tableLayoutPanelGameInfo.Controls.Add(this.tableLayoutPanelGameGenie, 0, 18);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelMaxPlayers, 0, 13);
            this.tableLayoutPanelGameInfo.Controls.Add(this.textBoxPublisher, 0, 8);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelSaveCount, 0, 15);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelGameGenie, 0, 17);
            this.tableLayoutPanelGameInfo.Controls.Add(this.textBoxSortName, 0, 6);
            this.tableLayoutPanelGameInfo.Controls.Add(this.checkBoxCompressed, 0, 2);
            this.tableLayoutPanelGameInfo.Controls.Add(this.textBoxName, 0, 4);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelCompress, 0, 1);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelPublisher, 0, 7);
            this.tableLayoutPanelGameInfo.Controls.Add(this.maskedTextBoxReleaseDate, 0, 12);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelSortName, 0, 5);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelDescription, 0, 21);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelReleaseDate, 0, 11);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelName, 0, 3);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelGenre, 1, 15);
            this.tableLayoutPanelGameInfo.Controls.Add(this.comboBoxGenre, 1, 16);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelCountry, 1, 13);
            this.tableLayoutPanelGameInfo.Controls.Add(this.comboBoxCountry, 1, 14);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelSize, 1, 2);
            this.tableLayoutPanelGameInfo.Controls.Add(this.labelCopyright, 0, 9);
            this.tableLayoutPanelGameInfo.Controls.Add(this.textBoxCopyright, 0, 10);
            this.tableLayoutPanelGameInfo.Name = "tableLayoutPanelGameInfo";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.textBoxDescription);
            this.panel1.Name = "panel1";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.TextChanged += new System.EventHandler(this.textBoxDescription_TextChanged);
            // 
            // maxPlayersComboBox
            // 
            resources.ApplyResources(this.maxPlayersComboBox, "maxPlayersComboBox");
            this.maxPlayersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maxPlayersComboBox.FormattingEnabled = true;
            this.maxPlayersComboBox.Name = "maxPlayersComboBox";
            this.maxPlayersComboBox.SelectedIndexChanged += new System.EventHandler(this.maxPlayersComboBox_SelectedIndexChanged);
            // 
            // tableLayoutPanelGameID
            // 
            resources.ApplyResources(this.tableLayoutPanelGameID, "tableLayoutPanelGameID");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.tableLayoutPanelGameID, 2);
            this.tableLayoutPanelGameID.Controls.Add(this.labelID, 1, 0);
            this.tableLayoutPanelGameID.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanelGameID.Name = "tableLayoutPanelGameID";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // tableLayoutPanelGameGenie
            // 
            resources.ApplyResources(this.tableLayoutPanelGameGenie, "tableLayoutPanelGameGenie");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.tableLayoutPanelGameGenie, 2);
            this.tableLayoutPanelGameGenie.Controls.Add(this.buttonShowGameGenieDatabase, 1, 0);
            this.tableLayoutPanelGameGenie.Controls.Add(this.textBoxGameGenie, 0, 0);
            this.tableLayoutPanelGameGenie.Name = "tableLayoutPanelGameGenie";
            // 
            // labelCompress
            // 
            resources.ApplyResources(this.labelCompress, "labelCompress");
            this.labelCompress.Name = "labelCompress";
            // 
            // labelDescription
            // 
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.labelDescription, 2);
            this.labelDescription.Name = "labelDescription";
            // 
            // labelName
            // 
            resources.ApplyResources(this.labelName, "labelName");
            this.labelName.Name = "labelName";
            // 
            // labelGenre
            // 
            resources.ApplyResources(this.labelGenre, "labelGenre");
            this.labelGenre.Name = "labelGenre";
            // 
            // comboBoxGenre
            // 
            resources.ApplyResources(this.comboBoxGenre, "comboBoxGenre");
            this.comboBoxGenre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGenre.FormattingEnabled = true;
            this.comboBoxGenre.Name = "comboBoxGenre";
            this.comboBoxGenre.SelectedValueChanged += new System.EventHandler(this.comboBoxGenre_SelectedValueChanged);
            // 
            // labelCountry
            // 
            resources.ApplyResources(this.labelCountry, "labelCountry");
            this.labelCountry.Name = "labelCountry";
            // 
            // comboBoxCountry
            // 
            resources.ApplyResources(this.comboBoxCountry, "comboBoxCountry");
            this.comboBoxCountry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCountry.FormattingEnabled = true;
            this.comboBoxCountry.Name = "comboBoxCountry";
            this.comboBoxCountry.SelectedValueChanged += new System.EventHandler(this.comboBoxCountry_SelectedValueChanged);
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.labelCopyright, 2);
            this.labelCopyright.Name = "labelCopyright";
            // 
            // textBoxCopyright
            // 
            this.tableLayoutPanelGameInfo.SetColumnSpan(this.textBoxCopyright, 2);
            resources.ApplyResources(this.textBoxCopyright, "textBoxCopyright");
            this.textBoxCopyright.Name = "textBoxCopyright";
            this.textBoxCopyright.TextChanged += new System.EventHandler(this.textBoxCopyright_TextChanged);
            // 
            // tableLayoutPanelArtButtons
            // 
            resources.ApplyResources(this.tableLayoutPanelArtButtons, "tableLayoutPanelArtButtons");
            this.tableLayoutPanelArtButtons.Controls.Add(this.buttonGoogle, 0, 1);
            this.tableLayoutPanelArtButtons.Controls.Add(this.buttonBrowseImage, 0, 0);
            this.tableLayoutPanelArtButtons.Controls.Add(this.buttonDefaultCover, 0, 0);
            this.tableLayoutPanelArtButtons.Controls.Add(this.buttonSpine, 1, 1);
            this.tableLayoutPanelArtButtons.Name = "tableLayoutPanelArtButtons";
            // 
            // buttonSpine
            // 
            resources.ApplyResources(this.buttonSpine, "buttonSpine");
            this.buttonSpine.Name = "buttonSpine";
            this.buttonSpine.UseVisualStyleBackColor = true;
            this.buttonSpine.Click += new System.EventHandler(this.buttonSpine_Click);
            // 
            // pictureBoxM2Spine
            // 
            this.pictureBoxM2Spine.AllowDrop = true;
            this.pictureBoxM2Spine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBoxM2Spine, "pictureBoxM2Spine");
            this.pictureBoxM2Spine.Name = "pictureBoxM2Spine";
            this.pictureBoxM2Spine.TabStop = false;
            this.pictureBoxM2Spine.Tag = com.clusterrr.hakchi_gui.NesMenuElementBase.GameImageType.MdSpine;
            this.pictureBoxM2Spine.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragDrop);
            this.pictureBoxM2Spine.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragEnter);
            this.pictureBoxM2Spine.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxArt_MouseClick);
            // 
            // pictureBoxM2Front
            // 
            this.pictureBoxM2Front.AllowDrop = true;
            this.pictureBoxM2Front.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pictureBoxM2Front, "pictureBoxM2Front");
            this.pictureBoxM2Front.Name = "pictureBoxM2Front";
            this.pictureBoxM2Front.TabStop = false;
            this.pictureBoxM2Front.Tag = com.clusterrr.hakchi_gui.NesMenuElementBase.GameImageType.MdFront;
            this.pictureBoxM2Front.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragDrop);
            this.pictureBoxM2Front.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBoxArt_DragEnter);
            this.pictureBoxM2Front.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxArt_MouseClick);
            // 
            // structureButton
            // 
            resources.ApplyResources(this.structureButton, "structureButton");
            this.structureButton.Name = "structureButton";
            this.structureButton.UseVisualStyleBackColor = true;
            this.structureButton.Click += new System.EventHandler(this.structureButton_Click);
            // 
            // foldersContextMenuStrip
            // 
            this.foldersContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.disablePagefoldersToolStripMenuItem,
            this.customToolStripMenuItem,
            this.toolStripMenuItem3,
            this.automaticToolStripMenuItem,
            this.automaticOriginalToolStripMenuItem,
            this.pagesToolStripMenuItem,
            this.pagesOriginalToolStripMenuItem,
            this.foldersToolStripMenuItem,
            this.foldersOriginalToolStripMenuItem,
            this.foldersSplitByFirstLetterToolStripMenuItem,
            this.foldersSplitByFirstLetterOriginalToolStripMenuItem,
            this.toolStripMenuItem4,
            this.maximumGamesPerFolderToolStripMenuItem,
            this.backFolderButtonPositionToolStripMenuItem,
            this.folderImagesSetToolStripMenuItem,
            this.toolStripMenuItem20,
            this.syncStructureForAllGamesCollectionsToolStripMenuItem});
            this.foldersContextMenuStrip.Name = "foldersContextMenuStrip";
            resources.ApplyResources(this.foldersContextMenuStrip, "foldersContextMenuStrip");
            // 
            // disablePagefoldersToolStripMenuItem
            // 
            this.disablePagefoldersToolStripMenuItem.Name = "disablePagefoldersToolStripMenuItem";
            resources.ApplyResources(this.disablePagefoldersToolStripMenuItem, "disablePagefoldersToolStripMenuItem");
            this.disablePagefoldersToolStripMenuItem.Tag = "0";
            this.disablePagefoldersToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            resources.ApplyResources(this.customToolStripMenuItem, "customToolStripMenuItem");
            this.customToolStripMenuItem.Tag = "99";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // automaticToolStripMenuItem
            // 
            this.automaticToolStripMenuItem.Checked = true;
            this.automaticToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.automaticToolStripMenuItem.Name = "automaticToolStripMenuItem";
            resources.ApplyResources(this.automaticToolStripMenuItem, "automaticToolStripMenuItem");
            this.automaticToolStripMenuItem.Tag = "2";
            this.automaticToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // automaticOriginalToolStripMenuItem
            // 
            this.automaticOriginalToolStripMenuItem.Name = "automaticOriginalToolStripMenuItem";
            resources.ApplyResources(this.automaticOriginalToolStripMenuItem, "automaticOriginalToolStripMenuItem");
            this.automaticOriginalToolStripMenuItem.Tag = "3";
            this.automaticOriginalToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // pagesToolStripMenuItem
            // 
            resources.ApplyResources(this.pagesToolStripMenuItem, "pagesToolStripMenuItem");
            this.pagesToolStripMenuItem.Name = "pagesToolStripMenuItem";
            this.pagesToolStripMenuItem.Tag = "4";
            this.pagesToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // pagesOriginalToolStripMenuItem
            // 
            resources.ApplyResources(this.pagesOriginalToolStripMenuItem, "pagesOriginalToolStripMenuItem");
            this.pagesOriginalToolStripMenuItem.Name = "pagesOriginalToolStripMenuItem";
            this.pagesOriginalToolStripMenuItem.Tag = "5";
            this.pagesOriginalToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // foldersToolStripMenuItem
            // 
            this.foldersToolStripMenuItem.Name = "foldersToolStripMenuItem";
            resources.ApplyResources(this.foldersToolStripMenuItem, "foldersToolStripMenuItem");
            this.foldersToolStripMenuItem.Tag = "6";
            this.foldersToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // foldersOriginalToolStripMenuItem
            // 
            this.foldersOriginalToolStripMenuItem.Name = "foldersOriginalToolStripMenuItem";
            resources.ApplyResources(this.foldersOriginalToolStripMenuItem, "foldersOriginalToolStripMenuItem");
            this.foldersOriginalToolStripMenuItem.Tag = "7";
            this.foldersOriginalToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // foldersSplitByFirstLetterToolStripMenuItem
            // 
            this.foldersSplitByFirstLetterToolStripMenuItem.Name = "foldersSplitByFirstLetterToolStripMenuItem";
            resources.ApplyResources(this.foldersSplitByFirstLetterToolStripMenuItem, "foldersSplitByFirstLetterToolStripMenuItem");
            this.foldersSplitByFirstLetterToolStripMenuItem.Tag = "8";
            this.foldersSplitByFirstLetterToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // foldersSplitByFirstLetterOriginalToolStripMenuItem
            // 
            this.foldersSplitByFirstLetterOriginalToolStripMenuItem.Name = "foldersSplitByFirstLetterOriginalToolStripMenuItem";
            resources.ApplyResources(this.foldersSplitByFirstLetterOriginalToolStripMenuItem, "foldersSplitByFirstLetterOriginalToolStripMenuItem");
            this.foldersSplitByFirstLetterOriginalToolStripMenuItem.Tag = "9";
            this.foldersSplitByFirstLetterOriginalToolStripMenuItem.Click += new System.EventHandler(this.pagesModefoldersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // maximumGamesPerFolderToolStripMenuItem
            // 
            this.maximumGamesPerFolderToolStripMenuItem.Name = "maximumGamesPerFolderToolStripMenuItem";
            resources.ApplyResources(this.maximumGamesPerFolderToolStripMenuItem, "maximumGamesPerFolderToolStripMenuItem");
            // 
            // backFolderButtonPositionToolStripMenuItem
            // 
            this.backFolderButtonPositionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftmostToolStripMenuItem,
            this.rightmostToolStripMenuItem});
            this.backFolderButtonPositionToolStripMenuItem.Name = "backFolderButtonPositionToolStripMenuItem";
            resources.ApplyResources(this.backFolderButtonPositionToolStripMenuItem, "backFolderButtonPositionToolStripMenuItem");
            // 
            // leftmostToolStripMenuItem
            // 
            this.leftmostToolStripMenuItem.Name = "leftmostToolStripMenuItem";
            resources.ApplyResources(this.leftmostToolStripMenuItem, "leftmostToolStripMenuItem");
            this.leftmostToolStripMenuItem.Click += new System.EventHandler(this.leftmostToolStripMenuItem_Click);
            // 
            // rightmostToolStripMenuItem
            // 
            this.rightmostToolStripMenuItem.Name = "rightmostToolStripMenuItem";
            resources.ApplyResources(this.rightmostToolStripMenuItem, "rightmostToolStripMenuItem");
            this.rightmostToolStripMenuItem.Click += new System.EventHandler(this.rightmostToolStripMenuItem_Click);
            // 
            // folderImagesSetToolStripMenuItem
            // 
            this.folderImagesSetToolStripMenuItem.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.folder_sm;
            this.folderImagesSetToolStripMenuItem.Name = "folderImagesSetToolStripMenuItem";
            resources.ApplyResources(this.folderImagesSetToolStripMenuItem, "folderImagesSetToolStripMenuItem");
            // 
            // toolStripMenuItem20
            // 
            this.toolStripMenuItem20.Name = "toolStripMenuItem20";
            resources.ApplyResources(this.toolStripMenuItem20, "toolStripMenuItem20");
            // 
            // syncStructureForAllGamesCollectionsToolStripMenuItem
            // 
            resources.ApplyResources(this.syncStructureForAllGamesCollectionsToolStripMenuItem, "syncStructureForAllGamesCollectionsToolStripMenuItem");
            this.syncStructureForAllGamesCollectionsToolStripMenuItem.Name = "syncStructureForAllGamesCollectionsToolStripMenuItem";
            this.syncStructureForAllGamesCollectionsToolStripMenuItem.Click += new System.EventHandler(this.syncStructureForAllGamesCollectionsToolStripMenuItem_Click);
            // 
            // gamesConsoleComboBox
            // 
            this.gamesConsoleComboBox.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.gamesConsoleComboBox, "gamesConsoleComboBox");
            this.gamesConsoleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gamesConsoleComboBox.FormattingEnabled = true;
            this.gamesConsoleComboBox.Name = "gamesConsoleComboBox";
            this.gamesConsoleComboBox.SelectedIndexChanged += new System.EventHandler(this.gamesConsoleComboBox_SelectedIndexChanged);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // tableLayoutPanelMain
            // 
            resources.ApplyResources(this.tableLayoutPanelMain, "tableLayoutPanelMain");
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxButtons, 2, 2);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxCurrentGamesCollection, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.buttonStart, 2, 3);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxArtSega, 2, 1);
            this.tableLayoutPanelMain.Controls.Add(this.buttonExport, 1, 3);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxArtNintendo, 2, 0);
            this.tableLayoutPanelMain.Controls.Add(this.buttonAddGames, 0, 3);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxGameInfo, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelStatusBar, 0, 4);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            // 
            // groupBoxButtons
            // 
            this.groupBoxButtons.Controls.Add(this.tableLayoutPanelArtButtons);
            resources.ApplyResources(this.groupBoxButtons, "groupBoxButtons");
            this.groupBoxButtons.Name = "groupBoxButtons";
            this.groupBoxButtons.TabStop = false;
            // 
            // groupBoxCurrentGamesCollection
            // 
            this.groupBoxCurrentGamesCollection.Controls.Add(this.tableLayoutPanel3);
            resources.ApplyResources(this.groupBoxCurrentGamesCollection, "groupBoxCurrentGamesCollection");
            this.groupBoxCurrentGamesCollection.Name = "groupBoxCurrentGamesCollection";
            this.tableLayoutPanelMain.SetRowSpan(this.groupBoxCurrentGamesCollection, 3);
            this.groupBoxCurrentGamesCollection.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.structureButton, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.gamesConsoleComboBox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.listViewGames, 0, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // groupBoxArtSega
            // 
            this.groupBoxArtSega.Controls.Add(this.panel3);
            resources.ApplyResources(this.groupBoxArtSega, "groupBoxArtSega");
            this.groupBoxArtSega.Name = "groupBoxArtSega";
            this.groupBoxArtSega.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pictureBoxM2Spine);
            this.panel3.Controls.Add(this.pictureBoxM2Front);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // groupBoxArtNintendo
            // 
            this.groupBoxArtNintendo.Controls.Add(this.panel2);
            resources.ApplyResources(this.groupBoxArtNintendo, "groupBoxArtNintendo");
            this.groupBoxArtNintendo.Name = "groupBoxArtNintendo";
            this.groupBoxArtNintendo.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBoxArt);
            this.panel2.Controls.Add(this.pictureBoxThumbnail);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // groupBoxGameInfo
            // 
            this.groupBoxGameInfo.Controls.Add(this.tableLayoutPanelGameInfo);
            resources.ApplyResources(this.groupBoxGameInfo, "groupBoxGameInfo");
            this.groupBoxGameInfo.Name = "groupBoxGameInfo";
            this.tableLayoutPanelMain.SetRowSpan(this.groupBoxGameInfo, 3);
            this.groupBoxGameInfo.TabStop = false;
            // 
            // tableLayoutPanelStatusBar
            // 
            resources.ApplyResources(this.tableLayoutPanelStatusBar, "tableLayoutPanelStatusBar");
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelStatusBar, 3);
            this.tableLayoutPanelStatusBar.Controls.Add(this.toolStripStatusConnectionIcon, 0, 0);
            this.tableLayoutPanelStatusBar.Controls.Add(this.tableLayoutPanelStatusBarInner, 1, 0);
            this.tableLayoutPanelStatusBar.Name = "tableLayoutPanelStatusBar";
            this.tableLayoutPanelStatusBar.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // toolStripStatusConnectionIcon
            // 
            resources.ApplyResources(this.toolStripStatusConnectionIcon, "toolStripStatusConnectionIcon");
            this.toolStripStatusConnectionIcon.Image = global::com.clusterrr.hakchi_gui.Properties.Resources.red;
            this.toolStripStatusConnectionIcon.Name = "toolStripStatusConnectionIcon";
            this.toolStripStatusConnectionIcon.TabStop = false;
            // 
            // tableLayoutPanelStatusBarInner
            // 
            resources.ApplyResources(this.tableLayoutPanelStatusBarInner, "tableLayoutPanelStatusBarInner");
            this.tableLayoutPanelStatusBarInner.Controls.Add(this.toolStripStatusLabelShell, 0, 0);
            this.tableLayoutPanelStatusBarInner.Controls.Add(this.toolStripStatusLabelSelected, 1, 0);
            this.tableLayoutPanelStatusBarInner.Controls.Add(this.toolStripStatusLabelSize, 2, 0);
            this.tableLayoutPanelStatusBarInner.Controls.Add(this.toolStripProgressBar, 3, 0);
            this.tableLayoutPanelStatusBarInner.Name = "tableLayoutPanelStatusBarInner";
            // 
            // toolStripStatusLabelShell
            // 
            resources.ApplyResources(this.toolStripStatusLabelShell, "toolStripStatusLabelShell");
            this.toolStripStatusLabelShell.ForeColor = System.Drawing.SystemColors.GrayText;
            this.toolStripStatusLabelShell.Name = "toolStripStatusLabelShell";
            this.toolStripStatusLabelShell.Paint += new System.Windows.Forms.PaintEventHandler(this.labelBorder_Paint);
            // 
            // toolStripStatusLabelSelected
            // 
            resources.ApplyResources(this.toolStripStatusLabelSelected, "toolStripStatusLabelSelected");
            this.toolStripStatusLabelSelected.Name = "toolStripStatusLabelSelected";
            this.toolStripStatusLabelSelected.Paint += new System.Windows.Forms.PaintEventHandler(this.labelBorder_Paint);
            // 
            // toolStripStatusLabelSize
            // 
            resources.ApplyResources(this.toolStripStatusLabelSize, "toolStripStatusLabelSize");
            this.toolStripStatusLabelSize.Name = "toolStripStatusLabelSize";
            this.toolStripStatusLabelSize.Paint += new System.Windows.Forms.PaintEventHandler(this.labelBorder_Paint);
            // 
            // toolStripProgressBar
            // 
            resources.ApplyResources(this.toolStripProgressBar, "toolStripProgressBar");
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label1.Name = "label1";
            this.label1.Paint += new System.Windows.Forms.PaintEventHandler(this.labelBorder_Paint);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.Paint += new System.Windows.Forms.PaintEventHandler(this.labelBorder_Paint);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.Paint += new System.Windows.Forms.PaintEventHandler(this.labelBorder_Paint);
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.menuStrip);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = global::com.clusterrr.hakchi_gui.Properties.Resources.icon;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.dragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.dragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveCount)).EndInit();
            this.tableLayoutPanelGameInfo.ResumeLayout(false);
            this.tableLayoutPanelGameInfo.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanelGameID.ResumeLayout(false);
            this.tableLayoutPanelGameID.PerformLayout();
            this.tableLayoutPanelGameGenie.ResumeLayout(false);
            this.tableLayoutPanelGameGenie.PerformLayout();
            this.tableLayoutPanelArtButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Spine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxM2Front)).EndInit();
            this.foldersContextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.groupBoxButtons.ResumeLayout(false);
            this.groupBoxCurrentGamesCollection.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBoxArtSega.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBoxArtNintendo.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBoxGameInfo.ResumeLayout(false);
            this.tableLayoutPanelStatusBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toolStripStatusConnectionIcon)).EndInit();
            this.tableLayoutPanelStatusBarInner.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMoreGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button buttonAddGames;
        private System.Windows.Forms.OpenFileDialog openFileDialogNes;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ToolStripMenuItem kernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashCustomKernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.Timer timerCalculateGames;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fAQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gitHubPageWithActualReleasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem presetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addPresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deletePresetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem epilepsyProtectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cloverconHackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetUsingCombinationOfButtonsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectButtonCombinationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableAutofireToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem globalCommandLineArgumentsexpertsOnluToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upABStartOnSecondControllerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem installModulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallModulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem synchronizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useXYOnClassicControllerAsAutofireABToolStripMenuItem;
        private System.Windows.Forms.Timer timerConnectionCheck;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToNESMiniNowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFTPInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTelnetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem takeScreenshotToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveDumpFileDialog;
        private System.Windows.Forms.OpenFileDialog openDumpFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem compressGamesToolStripMenuItem;
        internal System.Windows.Forms.ListView listViewGames;
        private System.Windows.Forms.ColumnHeader gameName;
        private System.Windows.Forms.ToolStripMenuItem compressSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decompressSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBoxArtForSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compressBoxArtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSelectedGamesBoxArtToolStripMenuItem;
        private System.Windows.Forms.Timer timerShowSelected;
        private System.Windows.Forms.ToolStripMenuItem reloadGamesToolStripMenuItem;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem exportGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem explorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem scanForNewBoxArtForSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem disableHakchi2PopupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem16;
        private System.Windows.Forms.ToolStripMenuItem resetOriginalGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem originalGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionAtTheTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionAtTheBottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionSortedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem separateGamesForMultibootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sFROMToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem enableSFROMToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashUbootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sDModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem membootOriginalKernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem membootRecoveryKernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem dumpTheWholeNANDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolFlashTheWholeNANDStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpNANDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpNANDCPartitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashNANDCPartitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usePCMPatchWhenAvailableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sFROMToolToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem editROMHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem resetROMHeaderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem17;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelPublisher;
        private System.Windows.Forms.TextBox textBoxPublisher;
        private System.Windows.Forms.Label labelCommandLine;
        private System.Windows.Forms.TextBox textBoxArguments;
        private System.Windows.Forms.PictureBox pictureBoxArt;
        private System.Windows.Forms.Button buttonBrowseImage;
        private System.Windows.Forms.Button buttonGoogle;
        private System.Windows.Forms.Label labelMaxPlayers;
        private System.Windows.Forms.Label labelGameGenie;
        private System.Windows.Forms.TextBox textBoxGameGenie;
        private System.Windows.Forms.Label labelReleaseDate;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxReleaseDate;
        private System.Windows.Forms.Button buttonShowGameGenieDatabase;
        private System.Windows.Forms.CheckBox checkBoxCompressed;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.Button buttonDefaultCover;
        private System.Windows.Forms.PictureBox pictureBoxThumbnail;
        private System.Windows.Forms.Label labelSortName;
        private System.Windows.Forms.TextBox textBoxSortName;
        private System.Windows.Forms.Label labelSaveCount;
        private System.Windows.Forms.NumericUpDown numericUpDownSaveCount;
        private System.Windows.Forms.ToolStripMenuItem centerBoxArtThumbnailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionHiddenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectEmulationCoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCustomAppToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useLinkedSyncToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bootImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeBootImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disableBootImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetDefaultBootImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem18;
        private System.Windows.Forms.ToolStripMenuItem prepareArtDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatNANDCToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.Button structureButton;
        private System.Windows.Forms.ContextMenuStrip foldersContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem disablePagefoldersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem automaticToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automaticOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pagesOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldersOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldersSplitByFirstLetterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldersSplitByFirstLetterOriginalToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem maximumGamesPerFolderToolStripMenuItem;
        private System.Windows.Forms.ComboBox gamesConsoleComboBox;
        private System.Windows.Forms.ToolStripMenuItem kachikachiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem canoeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem retroarchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flashNANDBPartitionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem coreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGamesWithoutBoxArtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem developerToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem devForceSshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem19;
        private System.Windows.Forms.ToolStripMenuItem repairGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem membootCustomKernelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uploadTotmpforTestingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem folderImagesSetToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem factoryResetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backFolderButtonPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightmostToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem20;
        private System.Windows.Forms.ToolStripMenuItem syncStructureForAllGamesCollectionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator modRepoStartSeparator;
        private System.Windows.Forms.ToolStripMenuItem messageOfTheDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem22;
        private System.Windows.Forms.ToolStripMenuItem forceClovershellMembootsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem23;
        private System.Windows.Forms.ToolStripMenuItem technicalInformationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpOriginalKernellegacyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem24;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.ToolStripMenuItem forceNetworkMembootsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysCopyOriginalGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem25;
        private System.Windows.Forms.ToolStripMenuItem convertSNESROMSToSFROMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem separateGamesStorageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autodetectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asIsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem switchRunningFirmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem generateModulesReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem archiveSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator modRepoEndSeparator;
        private System.Windows.Forms.ToolStripMenuItem manageModRepositoriesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveDmesgOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem joinOurDiscordServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rRockinTheClassicsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem formatSDCardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadLatestHakchiToolStripMenuItem;
        private com.clusterrr.hakchi_gui.Wireless.Bluetooth.BluetoothMenuItem bluetoothToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelGameGenie;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label labelCompress;
        private System.Windows.Forms.ComboBox maxPlayersComboBox;
        private System.Windows.Forms.PictureBox pictureBoxM2Spine;
        private System.Windows.Forms.PictureBox pictureBoxM2Front;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelGameInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelGameID;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelArtButtons;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Button buttonSpine;
        private System.Windows.Forms.Label labelGenre;
        private System.Windows.Forms.ComboBox comboBoxGenre;
        private System.Windows.Forms.Label labelCountry;
        private System.Windows.Forms.ComboBox comboBoxCountry;
        private System.Windows.Forms.ToolStripMenuItem segaUiThemeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unitedStatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem europeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem japanToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxCopyright;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.ToolStripMenuItem scrapeSelectedGamesToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBoxArtSega;
        private System.Windows.Forms.GroupBox groupBoxArtNintendo;
        private System.Windows.Forms.GroupBox groupBoxGameInfo;
        private System.Windows.Forms.GroupBox groupBoxCurrentGamesCollection;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStatusBar;
        private System.Windows.Forms.PictureBox toolStripStatusConnectionIcon;
        private System.Windows.Forms.Label toolStripStatusLabelShell;
        private System.Windows.Forms.Label toolStripStatusLabelSelected;
        private System.Windows.Forms.Label toolStripStatusLabelSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar toolStripProgressBar;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelStatusBarInner;
        private System.Windows.Forms.GroupBox groupBoxButtons;
        private System.Windows.Forms.ToolStripMenuItem addPrefixToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePrefixToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem enableInformationScrapeOnImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importGamesFromMiniToolStripMenuItem;
    }
}

