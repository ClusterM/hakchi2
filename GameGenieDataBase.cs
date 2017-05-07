using com.clusterrr.Famicom;
using com.clusterrr.hakchi_gui.Properties;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace com.clusterrr.hakchi_gui
{
    class GameGenieCode
    {
        public delegate void ChangedEvent(GameGenieCode e);
        public event ChangedEvent Changed;

        private string FCode = "";
        private string FDescription = "";
        private string FOldCode;
        private string FOldDescription;

        public string Code
        {
            get { return FCode; }
            set
            {
                if (FCode != value)
                {
                    FOldCode = FCode;
                    FCode = value;
                    if ((FOldCode != "") && (Changed != null))
                        Changed(this);
                }
            }
        }
        public string Description
        {
            get { return FDescription; }
            set
            {
                if (FDescription != value)
                {
                    FOldDescription = FDescription;
                    FDescription = value;
                    if ((FOldDescription != "") && (Changed != null))
                        Changed(this);
                }
            }
        }
        public string OldCode { get { return FOldCode; } }
        public string OldDescription { get { return FOldDescription; } }

        public override string ToString()
        {
            return Description;
        }

        public GameGenieCode(string ACode, string ADescription)
        {
            Code = ACode;
            Description = ADescription;
        }
    }

    class GameGenieDataBase
    {
        private readonly string DataBasePath;
        private XmlDocument FXml = new XmlDocument();
        private XmlNode FGameNode = null;
        private List<GameGenieCode> FGameCodes = null;
        private string originalDatabasePath = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "data"), "GameGenieDB.xml");
        private string userDatabasePath = Path.Combine(Path.Combine(Program.BaseDirectoryExternal, ConfigIni.ConfigDir), "GameGenieDB.xml");
        private NesMiniApplication FGame = null;
        private bool FModified = false;

        private XmlNode GameNode
        {
            get
            {
                if (FGameNode == null)
                {
                    FGameNode = FXml.SelectSingleNode(string.Format("/database/game[@code='{0}']", FGame.Code));

                    if (FGameNode == null)
                    {
                        string lGamesDir = Path.Combine(Program.BaseDirectoryExternal, "games");
                        NesFile lGame = new NesFile(Path.Combine(Path.Combine(lGamesDir, FGame.Code), FGame.Code + ".nes"));
                        XmlAttribute lXmlAttribute;

                        FGameNode = FXml.CreateElement("game");
                        FXml.DocumentElement.AppendChild(FGameNode);

                        lXmlAttribute = FXml.CreateAttribute("code");
                        lXmlAttribute.Value = FGame.Code;
                        FGameNode.Attributes.Append(lXmlAttribute);

                        lXmlAttribute = FXml.CreateAttribute("name");
                        lXmlAttribute.Value = FGame.Name;
                        FGameNode.Attributes.Append(lXmlAttribute);

                        lXmlAttribute = FXml.CreateAttribute("crc");
                        lXmlAttribute.Value = lGame.CRC32.ToString("X");
                        FGameNode.Attributes.Append(lXmlAttribute);
                    }
                }
                return FGameNode;
            }
        }

        public List<GameGenieCode> GameCodes
        {
            get
            {
                if (FGameCodes == null)
                {
                    FGameCodes = new List<GameGenieCode>();
                    XmlNodeList lCodes = FXml.SelectNodes(string.Format("/database/game[@code='{0}']//gamegenie", FGame.Code));
                    foreach (XmlNode lCurNode in lCodes)
                    {
                        GameGenieCode lCurCode = new GameGenieCode(lCurNode.Attributes.GetNamedItem("code").Value, lCurNode.Attributes.GetNamedItem("description").Value);
                        FGameCodes.Add(lCurCode);
                    }
                }
                return FGameCodes;
            }
        }

        public bool Modified
        {
            get
            {
                return FModified;
            }

        }

        public GameGenieDataBase(NesMiniApplication AGame)
        {
            //DataBasePath = Path.Combine(Path.Combine(Program.BaseDirectoryInternal, "data"), "GameGenieDB.xml");
            FGame = AGame;
            //FDBName = DataBasePath;
            if (File.Exists(userDatabasePath))
                FXml.Load(userDatabasePath);
            else if (File.Exists(originalDatabasePath))
                FXml.Load(originalDatabasePath);
            else
                FXml.AppendChild(FXml.CreateElement("database"));
        }

        public void AddCode(GameGenieCode ACode)
        {
            FModified = true;

            XmlNode lCodeNode = FXml.CreateElement("gamegenie");
            GameNode.AppendChild(lCodeNode);

            XmlAttribute lAttribute;

            lAttribute = FXml.CreateAttribute("code");
            lAttribute.Value = ACode.Code.ToUpper().Trim();
            lCodeNode.Attributes.Append(lAttribute);

            lAttribute = FXml.CreateAttribute("description");
            lAttribute.Value = ACode.Description;
            lCodeNode.Attributes.Append(lAttribute);

            if (FGameCodes == null)
                FGameCodes = new List<GameGenieCode>();

            FGameCodes.Add(ACode);
        }

        public void ModifyCode(GameGenieCode ACode)
        {
            XmlNode lCurCode = GameNode.SelectSingleNode(string.Format("gamegenie[@code='{0}']", ACode.OldCode.ToUpper().Trim()));
            if (lCurCode != null)
            {
                lCurCode.Attributes.GetNamedItem("code").Value = ACode.Code.ToUpper().Trim();
                lCurCode.Attributes.GetNamedItem("description").Value = ACode.Description;
                FModified = true;
            }
        }

        public void DeleteCode(GameGenieCode ACode)
        {
            XmlNode lCurCode = GameNode.SelectSingleNode(string.Format("gamegenie[@code='{0}']", ACode.Code.ToUpper().Trim()));
            if (lCurCode != null)
                lCurCode.ParentNode.RemoveChild(lCurCode);
            FGameCodes.Remove(ACode);
            FModified = true;
        }

        public void ImportCodes(string AFileName, bool AQuiet = false)
        {
            if (File.Exists(AFileName))
            {
                XmlDocument lXml = new XmlDocument();
                XmlNodeList lCodes = null;
                XmlNode lCodeNode = null;
                XmlAttribute lAttribute = null;

                lXml.Load(AFileName);
                lCodes = lXml.SelectNodes("//genie/..");

                FModified = true;

                XmlNode lDeleteNode = GameNode.FirstChild;
                while (lDeleteNode != null)
                {
                    GameNode.RemoveChild(GameNode.FirstChild);
                    lDeleteNode = GameNode.FirstChild;
                }
                GameCodes.Clear();

                string lGameFileName = Path.Combine(Path.Combine(Path.Combine(Program.BaseDirectoryExternal, "games"), FGame.Code), FGame.Code + ".nes");
                foreach (XmlNode lCurCode in lCodes)
                {
                    NesFile lGame = new NesFile(lGameFileName);
                    try
                    {
                        lGame.PRG = GameGeniePatcher.Patch(lGame.PRG, lCurCode["genie"].InnerText);

                        lCodeNode = FXml.CreateElement("gamegenie");
                        GameNode.AppendChild(lCodeNode);

                        lAttribute = FXml.CreateAttribute("code");
                        lAttribute.Value = lCurCode["genie"].InnerText.ToUpper().Trim();
                        lCodeNode.Attributes.Append(lAttribute);

                        lAttribute = FXml.CreateAttribute("description");
                        lAttribute.Value = lCurCode["description"].InnerText;
                        lCodeNode.Attributes.Append(lAttribute);

                        GameCodes.Add(new GameGenieCode(lCurCode["genie"].InnerText.ToUpper().Trim(), lCurCode["description"].InnerText));
                    }
                    catch (GameGenieFormatException)
                    {
                        if (!AQuiet)
                            MessageBox.Show(string.Format(Resources.GameGenieFormatError, lCurCode["genie"].InnerText, FGame.Name), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (GameGenieNotFoundException)
                    {
                        if (!AQuiet)
                            MessageBox.Show(string.Format(Resources.GameGenieNotFound, lCurCode["genie"].InnerText, FGame.Name), Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void Save()
        {
            if (GameCodes.Count == 0)
                GameNode.ParentNode.RemoveChild(GameNode);
            Directory.CreateDirectory(Path.GetDirectoryName(userDatabasePath));
            FXml.Save(userDatabasePath);
        }
    }
}
