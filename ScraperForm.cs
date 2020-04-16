using com.clusterrr.hakchi_gui.data;
using com.clusterrr.hakchi_gui.Properties;
using SpineGen.DrawingBitmaps;
using SpineGen.JSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeamShinkansen.Scrapers.Interfaces;
using static com.clusterrr.hakchi_gui.MainForm;

namespace com.clusterrr.hakchi_gui
{
    public partial class ScraperForm : Form
    {
        private enum DataType { Name, Description, Publisher, Developer, ReleaseDate, Copyright, Genre, PlayerCount, FrontArt, SpineArt, SelectedScraper, SpineTemplate }
        private enum PageDirection { Previous, Next }
        private class ItemWrapper
        {
            public Result Result { get; set; }
            public NesApplication Game { get => Result.Game; }
            public Dictionary<IScraper, List<Task>> ScraperTasks { get; set; } = GetScraperTaskDictionary();
            public Dictionary<IScraper, IScraperResult> LastScraperResult { get; set; } = GetScraperResultDictionary();
            public string SearchTerm { get; set; } = null;
            public IScraper SelectedScraper { get; set; } = null;
            public Thread ScraperFetchThread { get; set; } = null;
            public Thread ScraperImageFetchThread { get; set; } = null;
            public Thread ScraperSpineFetchThread { get; set; } = null;
            public override string ToString() => Result.Game.Name;
        }
        public class Result
        {
            public bool ChangedName { get; set; } = false;
            public bool ChangedDescription { get; set; } = false;
            public bool ChangedPublisher { get; set; } = false;
            public bool ChangedDeveloper { get; set; } = false;
            public bool ChangedReleaseDate { get; set; } = false;
            public bool ChangedCopyright { get; set; } = false;
            public bool ChangedGenre { get; set; } = false;
            public bool ChangedPlayerCount { get; set; } = false;
            public bool ChangedFrontArt { get; set; } = false;
            public bool ChangedSpineArt { get; set; } = false;
            public bool ChangedClearLogo { get; set; } = false;
            public string Name { get; set; } = null;
            public string Description { get; set; } = null;
            public string Publisher { get; set; } = null;
            public string Developer { get; set; } = null;
            public string ReleaseDate { get; set; } = null;
            public string Copyright { get; set; } = null;
            public string Genre { get; set; } = null;
            public MaxPlayers? PlayerCount { get; set; } = null;
            public Image FrontArt { get; set; } = null;
            public Image SpineArt { get; set; } = null;
            public Image ClearLogo { get; set; } = null;
            public NesApplication Game { get; set; } = null;
            public override string ToString() => Game.Name;
        }

        public readonly List<NesApplication> Games = new List<NesApplication>();
        public Result[] Results { get; private set; } = new Result[] { };
        private ItemWrapper SelectedItem { get; set; }
        private IScraper SelectedScraper => comboBoxScrapers.SelectedItem as IScraper;
        private bool UpdateCheckboxes { get; set; } = true;
        private Dictionary<DataType, CheckBox> checkBoxes = new Dictionary<DataType, CheckBox>();
        public ScraperForm()
        {
            InitializeComponent();
            checkBoxes.Add(DataType.Name, checkBoxName);
            checkBoxes.Add(DataType.Publisher, checkBoxPublisher);
            checkBoxes.Add(DataType.Developer, checkBoxDeveloper);
            checkBoxes.Add(DataType.ReleaseDate, checkBoxReleaseDate);
            checkBoxes.Add(DataType.Copyright, checkBoxCopyright);
            checkBoxes.Add(DataType.Genre, checkBoxGenre);
            checkBoxes.Add(DataType.PlayerCount, checkBoxPlayerCount);
            checkBoxes.Add(DataType.Description, checkBoxDescription);
            checkBoxes.Add(DataType.SpineArt, checkBoxSpine);
            checkBoxes.Add(DataType.FrontArt, checkBoxFront);
        }

        private List<Thread> Threads = new List<Thread>();

        private static Dictionary<IScraper, List<Task>> GetScraperTaskDictionary()
        {
            var dictionary = new Dictionary<IScraper, List<Task>>();

            foreach (var scraper in Program.Scrapers)
            {
                dictionary.Add(scraper, new List<Task>());
            }

            return dictionary;
        }
        private static Dictionary<IScraper, IScraperResult> GetScraperResultDictionary()
        {
            var dictionary = new Dictionary<IScraper, IScraperResult>();

            foreach (var scraper in Program.Scrapers)
            {
                dictionary.Add(scraper, null);
            }

            return dictionary;
        }

        private void ScraperForm_Load(object sender, EventArgs e)
        {
            textBoxDescription.Size = new Size(1, 1);
            PopulateGenres(comboBoxGenre);
            PopulateMaxPlayers(maxPlayersComboBox);

            if (Program.Scrapers.Count == 0)
            {
                throw new InvalidOperationException("No scrapers are available.");
            }

            foreach (var scraper in Program.Scrapers.OrderBy(s => s.ProviderName))
            {

                comboBoxScrapers.Items.Add(scraper);
            }

            comboBoxScrapers.SelectedIndex = 0;
            comboBoxScrapers.Visible = Program.Scrapers.Count > 1;

            listViewGames.Items.Clear();
            if (Games.Count == 0)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            
            foreach (var game in Games.OrderBy(g => g.Name))
            {
                listViewGames.Items.Add(new ListViewItem(game.Name)
                {
                    Tag = new ItemWrapper()
                    {
                        Result = new Result()
                        {
                            Game = game
                        }
                    }
                });
            }

            listViewGames.Items[0].Selected = true;
            listViewGames.Visible = Games.Count != 1;

            foreach (var template in Program.SpineTemplates.Values)
            {
                comboBoxSpineTemplates.Items.Add(template);
            }
        }

        private void listViewGames_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            buttonScraperPrevious.Enabled =
                buttonScraperNext.Enabled = 
                buttonSearch.Enabled =
                textBoxSearchTerm.Enabled = false;
            
            tableLayoutPanel2.Enabled = listViewGames.SelectedItems.Count > 0;
            listViewScraperResults.Items.Clear();
            if (listViewGames.SelectedItems.Count == 0)
            {
                SelectedItem = null;
                textBoxName.Text =
                    textBoxPublisher.Text =
                    textBoxDeveloper.Text =
                    maskedTextBoxReleaseDate.Text =
                    textBoxCopyright.Text =
                    textBoxDescription.Text =
                    textBoxSearchTerm.Text = "";
                maxPlayersComboBox.SelectedIndex = 0;
                comboBoxGenre.SelectedIndex = 0;
                pictureBoxM2Front.Image?.Dispose();
                pictureBoxM2Front.Image = null;
                pictureBoxM2Spine.Image?.Dispose();
                pictureBoxM2Spine.Image = null;
                foreach (var box in checkBoxes.Values)
                {
                    box.Checked = false;
                }
            }
            else
            {
                var SelectedItem = listViewGames.SelectedItems[0].Tag as ItemWrapper;
                this.SelectedItem = SelectedItem;

                UpdateCheckboxes = false;

                checkBoxName.Checked = SelectedItem.Result.ChangedName;
                checkBoxPublisher.Checked = SelectedItem.Result.ChangedPublisher;
                checkBoxDeveloper.Checked = SelectedItem.Result.ChangedDeveloper;
                checkBoxReleaseDate.Checked = SelectedItem.Result.ChangedReleaseDate;
                checkBoxCopyright.Checked = SelectedItem.Result.ChangedCopyright;
                checkBoxGenre.Checked = SelectedItem.Result.ChangedGenre;
                checkBoxPlayerCount.Checked = SelectedItem.Result.ChangedPlayerCount;
                checkBoxDescription.Checked = SelectedItem.Result.ChangedDescription;
                checkBoxSpine.Checked = SelectedItem.Result.ChangedSpineArt;
                checkBoxFront.Checked = SelectedItem.Result.ChangedFrontArt;

                checkBoxSpine.Enabled = SelectedItem.Result.SpineArt != null;
                checkBoxFront.Enabled = SelectedItem.Result.FrontArt != null;

                textBoxName.Text = SelectedItem.Result.Name ?? SelectedItem.Game.Name;
                textBoxPublisher.Text = SelectedItem.Result.Publisher ?? SelectedItem.Game.Desktop.Publisher;
                textBoxDeveloper.Text = SelectedItem.Result.Developer;
                maskedTextBoxReleaseDate.Text = SelectedItem.Result.ReleaseDate ?? SelectedItem.Game.Desktop.ReleaseDate;
                textBoxCopyright.Text = SelectedItem.Result.Copyright ?? SelectedItem.Game.Desktop.Copyright;
                textBoxDescription.Text = SelectedItem.Result.Description ?? SelectedItem.Game.Desktop.Description;

                var hash = SelectedItem.Game?.Metadata?.OriginalCrc32 ?? 0;

                if (data.GamesDB.HashLookup.ContainsKey(hash) && data.GamesDB.HashLookup[hash].Length > 0)
                {
                    SelectedItem.SearchTerm = $"ID: {string.Join(", ", data.GamesDB.HashLookup[hash])}";
                }
                
                textBoxSearchTerm.Text = SelectedItem.SearchTerm ?? SelectedItem.Game.Name;

                maxPlayersComboBox.SelectedIndex = (int)(SelectedItem.Result.PlayerCount ?? GetPlayersFromDesktop(SelectedItem.Game.Desktop));

                comboBoxSpineTemplates.Enabled = SelectedItem.Result.ClearLogo != null;
                var disposableFront = SelectedItem.Result.Game.M2Front;
                var disposableSpine = SelectedItem.Result.Game.M2Spine;

                pictureBoxM2Front.Image?.Dispose();
                if (SelectedItem.Result.FrontArt != null)
                {
                    pictureBoxM2Front.Image = new Bitmap(SelectedItem.Result.FrontArt);
                    disposableFront?.Dispose();
                }
                else if (disposableFront != null)
                {
                    pictureBoxM2Front.Image = disposableFront;
                }


                pictureBoxM2Spine.Image?.Dispose();
                if (SelectedItem.Result.SpineArt != null)
                {
                    pictureBoxM2Spine.Image = new Bitmap(SelectedItem.Result.SpineArt);
                    disposableSpine?.Dispose();
                }
                else if (disposableSpine != null)
                {
                    pictureBoxM2Spine.Image = disposableSpine;
                }

                var genre = comboBoxGenre.Items.Cast<NameValuePair<string>>().
                    Where(g => g.Value == (SelectedItem.Result.Genre ?? SelectedItem.Game.Desktop.Genre));

                if (genre.Count() > 0)
                {
                    comboBoxGenre.SelectedItem = genre.First();
                }

                new Thread(() =>
                {
                    Threads.Add(Thread.CurrentThread);
                    var item = SelectedItem;
                    IScraper scraper = null;
                    this.Invoke(new Action(() =>
                    {
                        scraper = SelectedScraper;
                        listViewScraperResults.BeginUpdate();
                        listViewScraperResults.Items.Clear();
                        listViewScraperResults.Enabled = false;
                        listViewScraperResults.Items.Add(Resources.Loading);
                        listViewScraperResults.EndUpdate();
                    }));

                    item.SelectedScraper = scraper;

                    IScraperResult results = item.LastScraperResult[scraper];

                    if (results == null)
                    {

                        Task<IScraperResult> resultsTask = null;

                        var matches = Regex.Matches(item.SearchTerm ?? item.Game.Name, "^ID:(?:,?\\s*(\\d+))+$");
                        var ids = Regex.Matches(item.SearchTerm ?? item.Game.Name, "(\\d+)").Cast<Match>().Select(m => int.Parse(m.Groups[1].Value)).ToArray();

                        if (scraper is TeamShinkansen.Scrapers.TheGamesDB.Scraper && matches.Count > 0 && ids.Length > 0)
                        {
                            resultsTask = ((TeamShinkansen.Scrapers.TheGamesDB.Scraper)scraper).GetInfoByID(ids);
                        }
                        else
                        {
                            resultsTask = scraper.GetInfoByName(item.SearchTerm ?? item.Game.Name);
                        }

                        item.ScraperTasks[scraper].Add(resultsTask);
                        try
                        {
                            resultsTask.Wait();
                        } 
                        catch (ThreadAbortException ex)
                        {
                            Threads.Remove(Thread.CurrentThread);
                            item.ScraperTasks[scraper].Remove(resultsTask);
                            return;
                        }
                        catch (Exception ex)
                        {
                            var error = ex.InnerException?.Message ?? ex.Message;
                            if (error != null)
                            {
                                Tasks.MessageForm.Show(Resources.Error, error, Resources.sign_error);
                            }
                            
                            Threads.Remove(Thread.CurrentThread);
                            item.ScraperTasks[scraper].Remove(resultsTask);
                            return;
                        }
                        item.ScraperTasks[scraper].Remove(resultsTask);
                        results = resultsTask.Result;
                        item.LastScraperResult[scraper] = results;
                    }
                    
                    if (item == SelectedItem && item.SelectedScraper == scraper)
                    {
                        PopulateScraperResults(results, item);
                    }
                    Threads.Remove(Thread.CurrentThread);
                }).Start();

                UpdateCheckboxes = true;
            }
        }

        private void PopulateScraperResults(IScraperResult results, ItemWrapper item)
        {
            this.Invoke(new Action(() =>
            {
                if (SelectedItem != item)
                    return;

                listViewScraperResults.Tag = results;
                listViewScraperResults.BeginUpdate();
                listViewScraperResults.Items.Clear();

                buttonScraperPrevious.Enabled = results.HasPreviousPage;
                buttonScraperNext.Enabled = results.HasNextPage;

                foreach (var result in results.Items)
                {
                    listViewScraperResults.Items.Add(new ListViewItem($"{result.Name} ({result.Platform})")
                    {
                        Tag = result
                    });
                }
                listViewScraperResults.Enabled = 
                    buttonSearch.Enabled =
                    textBoxSearchTerm.Enabled = true;
                listViewScraperResults.EndUpdate();
            }));
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Results = listViewGames.Items.Cast<ListViewItem>().
                    Select(item => item.Tag).
                    Where(tag => tag != null && tag is ItemWrapper).
                    Cast<ItemWrapper>().
                    Select(result => result.Result).ToArray();
        }

        private void DataChanged(object sender, EventArgs e)
        {
            if (sender != null && sender is Control)
            {
                var control = sender as Control;

                if ((DataType)control.Tag != DataType.SelectedScraper && SelectedItem == null || !UpdateCheckboxes)
                    return;

                if (control.Tag != null && control.Tag is DataType)
                {
                    if (checkBoxes.ContainsKey((DataType)control.Tag))
                    {
                        checkBoxes[(DataType)control.Tag].Checked = true;
                    }
                    
                    switch ((DataType)control.Tag)
                    {
                        case DataType.Name:
                            listViewGames.SelectedItems[0].Text = textBoxName.Text;
                            SelectedItem.Result.Name = textBoxName.Text;
                            break;

                        case DataType.Publisher:
                            SelectedItem.Result.Publisher = textBoxPublisher.Text;
                            break;

                        case DataType.Developer:
                            SelectedItem.Result.Developer = textBoxDeveloper.Text;
                            break;

                        case DataType.ReleaseDate:
                            SelectedItem.Result.ReleaseDate = maskedTextBoxReleaseDate.Text;
                            break;

                        case DataType.Copyright:
                            SelectedItem.Result.Copyright = textBoxCopyright.Text;
                            break;

                        case DataType.Genre:
                            SelectedItem.Result.Genre = (comboBoxGenre.SelectedItem as NameValuePair<string>?)?.Value ?? "";
                            break;

                        case DataType.PlayerCount:
                            SelectedItem.Result.PlayerCount = (MaxPlayers)maxPlayersComboBox.SelectedIndex;
                            break;

                        case DataType.Description:
                            SelectedItem.Result.Description = textBoxDescription.Text;
                            break;

                        case DataType.SpineArt: break;
                        case DataType.FrontArt: break;

                        case DataType.SelectedScraper:
                            linkLabelPoweredBy.Visible = SelectedScraper.ProviderUrl != null && isUrl(SelectedScraper.ProviderUrl);
                            linkLabelPoweredBy.Tag = SelectedScraper.ProviderUrl;
                            linkLabelPoweredBy.Text = string.Format(Resources.PoweredBy0, SelectedScraper.ProviderName);
                            break;

                        case DataType.SpineTemplate:
                            GenerateSpine();
                            break;

                        default: break;
                    }
                }
            }
        }

        private void CheckChanged(object sender, EventArgs e)
        {
            if (SelectedItem == null)
                return;

            if (sender != null && sender is Control)
            {
                var control = sender as CheckBox;

                if (control.Tag != null && control.Tag is DataType)
                {
                    switch ((DataType)control.Tag)
                    {
                        case DataType.Name:
                            SelectedItem.Result.ChangedName = control.Checked;
                            break;

                        case DataType.Publisher:
                            SelectedItem.Result.ChangedPublisher = control.Checked;
                            break;

                        case DataType.Developer:
                            SelectedItem.Result.ChangedDeveloper = control.Checked;
                            break;

                        case DataType.ReleaseDate:
                            SelectedItem.Result.ChangedReleaseDate = control.Checked;
                            break;

                        case DataType.Copyright:
                            SelectedItem.Result.ChangedCopyright = control.Checked;
                            break;

                        case DataType.Genre:
                            SelectedItem.Result.ChangedGenre = control.Checked;
                            break;

                        case DataType.PlayerCount:
                            SelectedItem.Result.ChangedPlayerCount = control.Checked;
                            break;

                        case DataType.Description:
                            SelectedItem.Result.ChangedDescription = control.Checked;
                            break;

                        case DataType.SpineArt:
                            SelectedItem.Result.ChangedSpineArt = control.Checked;
                            break;

                        case DataType.FrontArt: 
                            SelectedItem.Result.ChangedFrontArt = control.Checked;
                            break;

                        default: break;
                    }
                }
            }
        }

        private void listViewScraperResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewScraperResults.SelectedItems.Count > 0)
            {
                var result = listViewScraperResults.SelectedItems[0].Tag as IScraperData;

                if (result != null)
                {
                    if (result.Name != null)
                        textBoxName.Text = result.Name;

                    if (result.Publishers != null && result.Publishers.Length > 0)
                        textBoxPublisher.Text = String.Join(", ", result.Publishers);

                    if (result.Developers != null && result.Developers.Length > 0)
                        textBoxDeveloper.Text = String.Join(", ", result.Developers);

                    if (result.ReleaseDate != null)
                        maskedTextBoxReleaseDate.Text = result.ReleaseDate.ToString("yyyy-MM-dd");

                    if (result.Copyright != null)
                        textBoxCopyright.Text = result.Copyright;

                    if (result.Genres != null && result.Genres.Length > 0)
                    {
                        foreach (var genre in result.Genres)
                        {
                            var match = Genre.GenreList.Where(g => g.GamesDbId.Contains(genre.ID)).Select(g => g);

                            if (match.Count() > 0)
                            {
                                var first = match.First();
                                var selectedGenre = comboBoxGenre.Items.Cast<NameValuePair<string>>().Where(g => g.Value == first.DesktopName);

                                if (selectedGenre.Count() > 0)
                                {
                                    comboBoxGenre.SelectedItem = selectedGenre.First();
                                    break;
                                }
                            }
                        }
                    }

                    if (result.PlayerCount != null)
                    {
                        switch (result.PlayerCount)
                        {
                            case 0:
                            case 1:
                                maxPlayersComboBox.SelectedIndex = (int)MaxPlayers.OnePlayer;
                                break;

                            case 2:
                                maxPlayersComboBox.SelectedIndex = (int)MaxPlayers.TwoPlayer;
                                break;

                            case 3:
                                maxPlayersComboBox.SelectedIndex = (int)MaxPlayers.ThreePlayer;
                                break;

                            case 4:
                                maxPlayersComboBox.SelectedIndex = (int)MaxPlayers.FourPlayer;
                                break;

                            default:
                                maxPlayersComboBox.SelectedIndex = (int)MaxPlayers.FivePlayer;
                                break;
                        }
                    }

                    if (result.Description != null)
                    {
                        textBoxDescription.Text = result.Description;
                    }

                    SelectedItem.ScraperImageFetchThread?.Abort();
                    SelectedItem.ScraperImageFetchThread = new Thread(() =>
                    {
                        Threads.Add(Thread.CurrentThread);
                        var item = SelectedItem;
                        var innerResult = result;
                        try
                        {
                            var frontImages = innerResult.Images.Where(i => i.Type == TeamShinkansen.Scrapers.Enums.ArtType.Front);

                            if (frontImages.Count() > 0)
                            {
                                var frontUrl = frontImages.First().Url;

                                Invoke(new Action(() =>
                                {
                                    pictureBoxM2Front.Image = new Bitmap(Resources.LoadingFront);
                                }));

                                using (var wc = new HakchiWebClient())
                                {
                                    var imageData = wc.DownloadData(frontUrl);
                                    using (var ms = new MemoryStream(imageData)) 
                                    {
                                        ms.Seek(0, SeekOrigin.Begin);
                                        var bitmap = new Bitmap(ms);
                                        item.Result.FrontArt = bitmap;
                                        item.Result.ChangedFrontArt = true;
                                        if (SelectedItem == item)
                                        {
                                            Invoke(new Action(() =>
                                            {
                                                checkBoxFront.Enabled = true;
                                                checkBoxFront.Checked = true;
                                                ms.Seek(0, SeekOrigin.Begin);
                                                pictureBoxM2Front.Image = new Bitmap(ms);
                                            }));
                                        }
                                    }
                                };
                            }

                        }
                        catch (ThreadAbortException ex) { }
                        catch (WebException ex) { }
                        finally
                        {
                            item.ScraperImageFetchThread = null;
                        }
                        Threads.Remove(Thread.CurrentThread);
                    });
                    SelectedItem.ScraperImageFetchThread.Start();

                    SelectedItem.ScraperSpineFetchThread?.Abort();
                    SelectedItem.ScraperSpineFetchThread = new Thread(() =>
                    {
                        Threads.Add(Thread.CurrentThread);
                        var item = SelectedItem;
                        var innerResult = result;
                        try
                        {
                            if (innerResult is TeamShinkansen.Scrapers.TheGamesDB.ScraperData)
                            {
                                var tgdbResult = innerResult as TeamShinkansen.Scrapers.TheGamesDB.ScraperData;
                                using (var wc = new HakchiWebClient())
                                {
                                    try
                                    {
                                        var imageData = wc.DownloadData($"https://cdn.thegamesdb.net/images/original/clearlogo/{tgdbResult.ID}.png");

                                        using (var ms = new MemoryStream(imageData))
                                        {
                                            ms.Seek(0, SeekOrigin.Begin);
                                            item.Result.ClearLogo = new Bitmap(ms);
                                            if (item == SelectedItem)
                                            {
                                                Invoke(new Action(() =>
                                                {
                                                    comboBoxSpineTemplates.Enabled = true;
                                                    if (comboBoxSpineTemplates.SelectedItem == null)
                                                    {
                                                        comboBoxSpineTemplates.SelectedIndex = 0;
                                                    }
                                                    GenerateSpine();
                                                }));
                                            }
                                        }
                                    }
                                    catch (WebException ex) { }
                                }
                            }
                        }
                        catch (ThreadAbortException ex) { }
                        finally
                        {
                            item.ScraperSpineFetchThread = null;
                        }
                        Threads.Remove(Thread.CurrentThread);
                    });
                    SelectedItem.ScraperSpineFetchThread.Start();
                }
            }
        }

        private void GenerateSpine()
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.Result.ClearLogo != null && comboBoxSpineTemplates.SelectedItem != null && comboBoxSpineTemplates.SelectedItem is SpineTemplate<Bitmap>)
            {
                var template = comboBoxSpineTemplates.SelectedItem as SpineTemplate<Bitmap>;
                using (var clearLogo = new Bitmap(SelectedItem.Result.ClearLogo))
                using (var spineBitmap = new SystemDrawingBitmap(clearLogo as Bitmap))
                using (var rendered = template.Process(spineBitmap))
                {
                    checkBoxSpine.Enabled = true;
                    checkBoxSpine.Checked = true;
                    pictureBoxM2Spine.Image = new Bitmap(rendered.Bitmap);
                    SelectedItem.Result.SpineArt = new Bitmap(rendered.Bitmap);
                }
            }
        }

        private void SwitchPage(object sender, EventArgs e)
        {
            if (SelectedItem == null)
                return;

            if (sender != null && sender is Control)
            {
                var control = sender as Control;

                if (control.Tag != null && control.Tag is PageDirection)
                {
                    var direction = (PageDirection)control.Tag;
                    var item = SelectedItem;
                    if (item.SelectedScraper != null)
                    {
                        var scraper = item.SelectedScraper;

                        if (item.LastScraperResult[scraper] != null)
                        {
                            new Thread(() =>
                            {
                                Threads.Add(Thread.CurrentThread);
                                IScraperResult results = null;
                                Task<IScraperResult> resultsTask = null;

                                switch (direction)
                                {
                                    case PageDirection.Previous:
                                        if (item.LastScraperResult[scraper].HasPreviousPage)
                                        {
                                            resultsTask = item.LastScraperResult[scraper].GetPreviousPage();
                                        }
                                        else
                                        {
                                            return;
                                        }
                                        break;

                                    case PageDirection.Next:
                                        if (item.LastScraperResult[scraper].HasNextPage)
                                        {
                                            resultsTask = item.LastScraperResult[scraper].GetNextPage();
                                        }
                                        else
                                        {
                                            return;
                                        }
                                        break;

                                    default: return;
                                }

                                Invoke(new Action(() =>
                                {
                                    listViewScraperResults.BeginUpdate();
                                    listViewScraperResults.Items.Clear();
                                    listViewScraperResults.Enabled = false;
                                    listViewScraperResults.Items.Add(Resources.Loading);
                                    listViewScraperResults.EndUpdate();
                                }));

                                item.ScraperTasks[scraper].Add(resultsTask);
                                try
                                {
                                    resultsTask.Wait();
                                }
                                catch (ThreadAbortException ex)
                                {
                                    Threads.Remove(Thread.CurrentThread);
                                    item.ScraperTasks[scraper].Remove(resultsTask);
                                    return;
                                }
                                catch (Exception ex)
                                {
                                    var error = ex.InnerException?.Message ?? ex.Message;
                                    if (error != null)
                                    {
                                        Tasks.MessageForm.Show(Resources.Error, error, Resources.sign_error);
                                    }
                                    Threads.Remove(Thread.CurrentThread);
                                    item.ScraperTasks[scraper].Remove(resultsTask);
                                    return;
                                }
                                item.ScraperTasks[scraper].Remove(resultsTask);
                                results = resultsTask.Result;

                                if (item.LastScraperResult[scraper] == listViewScraperResults.Tag)
                                {
                                    // Make sure the user hasn't switch games and that this thread isn't just an orphan
                                    PopulateScraperResults(results, item);
                                }

                                item.LastScraperResult[scraper] = results;

                                Threads.Remove(Thread.CurrentThread);
                            }).Start();
                        }
                    }
                    
                }
            }
        }

        private void pictureBoxM2Spine_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null || !checkBoxSpine.Enabled)
                return;

            checkBoxSpine.Checked = !checkBoxSpine.Checked;
        }

        private void pictureBoxM2Front_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null || !checkBoxFront.Enabled)
                return;

            checkBoxFront.Checked = !checkBoxFront.Checked;
        }

        private void ScraperForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var thread in Threads.ToArray())
            {
                if (thread != null && thread.IsAlive)
                    thread.Abort();
            }
        }

        private void ScraperForm_Shown(object sender, EventArgs e)
        {
            if (StartPosition == FormStartPosition.CenterParent && Owner != null && Owner is Form)
            {
                var newX = Owner.Location.X + ((Owner.Width / 2) - (Width / 2));
                var newY = Owner.Location.Y + ((Owner.Height / 2) - (Height / 2));
                Location = new Point(newX, newY);
            }
        }
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            if (SelectedItem == null)
                return;

            var item = SelectedItem;

            if (item.SearchTerm == textBoxSearchTerm.Text)
                return;

            item.SearchTerm = textBoxSearchTerm.Text;

            if (item.SelectedScraper != null)
            {
                var scraper = item.SelectedScraper;

                buttonScraperPrevious.Enabled =
                    buttonScraperNext.Enabled = 
                    buttonSearch.Enabled =
                    textBoxSearchTerm.Enabled = false;

                new Thread(() =>
                {
                    Threads.Add(Thread.CurrentThread);
                    IScraperResult results = null;
                    Task<IScraperResult> resultsTask = null;
                    var matches = Regex.Matches(item.SearchTerm, "^ID:(?:,?\\s*(\\d+))+$");
                    var ids = Regex.Matches(item.SearchTerm, "(\\d+)").Cast<Match>().Select(m => int.Parse(m.Groups[1].Value)).ToArray();
                    
                    if (matches.Count > 0 && ids.Length > 0 && scraper is TeamShinkansen.Scrapers.TheGamesDB.Scraper)
                    {
                        resultsTask = ((TeamShinkansen.Scrapers.TheGamesDB.Scraper)scraper).GetInfoByID(ids);
                    }
                    else
                    {
                        resultsTask = scraper.GetInfoByName(item.SearchTerm);
                    }
                    
                    Invoke(new Action(() =>
                    {
                        listViewScraperResults.BeginUpdate();
                        listViewScraperResults.Items.Clear();
                        listViewScraperResults.Enabled = false;
                        listViewScraperResults.Items.Add(Resources.Loading);
                        listViewScraperResults.EndUpdate();
                    }));

                    item.ScraperTasks[scraper].Add(resultsTask);
                    try
                    {
                        resultsTask.Wait();
                    }
                    catch (ThreadAbortException ex)
                    {
                        Threads.Remove(Thread.CurrentThread);
                        item.ScraperTasks[scraper].Remove(resultsTask);
                        return;
                    }
                    catch (Exception ex)
                    {
                        var error = ex.InnerException?.Message ?? ex.Message;
                        if (error != null)
                        {
                            Tasks.MessageForm.Show(Resources.Error, error, Resources.sign_error);
                        }
                        Threads.Remove(Thread.CurrentThread);
                        item.ScraperTasks[scraper].Remove(resultsTask);
                        return;
                    }
                    item.ScraperTasks[scraper].Remove(resultsTask);
                    results = resultsTask.Result;

                    if (item == SelectedItem)
                    {
                        // Make sure the user hasn't switched games and that this thread isn't just an orphan
                        PopulateScraperResults(results, item);
                    }

                    item.LastScraperResult[scraper] = results;

                    Threads.Remove(Thread.CurrentThread);
                }).Start();
            }
        }

        private void textBoxSearchTerm_KeyDown(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 && listViewScraperResults.Enabled)
            {
                buttonSearch_Click(sender, e);
            }
        }

        private bool isUrl(string input)
        {
            return input != null && input.Length > 0 && (input.ToLower().StartsWith("http://") || input.ToLower().StartsWith("https://"));
        }

        private void linkLabelPoweredBy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabelPoweredBy.Tag != null && linkLabelPoweredBy.Tag is string && isUrl((string)linkLabelPoweredBy.Tag))
            {
                (new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        UseShellExecute = true,
                        FileName = (string)linkLabelPoweredBy.Tag
                    }
                }).Start();
            }
        }
    }
}
