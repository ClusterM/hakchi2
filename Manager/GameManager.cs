using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using com.clusterrr.hakchi_gui.Properties;
namespace com.clusterrr.hakchi_gui.Manager
{
    public class GameManager
    {
        static NesDefaultGame[] defaultNesGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-NAAAE",  Name = "Super Mario Bros.", Size = 571031 },
            new NesDefaultGame { Code = "CLV-P-NAACE",  Name = "Super Mario Bros. 3", Size = 1163285 },
            new NesDefaultGame { Code = "CLV-P-NAADE",  Name = "Super Mario Bros. 2",Size = 1510337 },
            new NesDefaultGame { Code = "CLV-P-NAAEE",  Name = "Donkey Kong", Size = 556016 },
            new NesDefaultGame { Code = "CLV-P-NAAFE",  Name = "Donkey Kong Jr." , Size = 558176 },
            new NesDefaultGame { Code = "CLV-P-NAAHE",  Name = "Excitebike", Size = 573231 },
            new NesDefaultGame { Code = "CLV-P-NAANE",  Name = "The Legend of Zelda", Size = 663910 },
            new NesDefaultGame { Code = "CLV-P-NAAPE",  Name = "Kirby's Adventure", Size = 1321661 },
            new NesDefaultGame { Code = "CLV-P-NAAQE",  Name = "Metroid", Size = 662601 },
            new NesDefaultGame { Code = "CLV-P-NAARE",  Name = "Balloon Fight", Size = 556131 },
            new NesDefaultGame { Code = "CLV-P-NAASE",  Name = "Zelda II - The Adventure of Link", Size = 1024158 },
            new NesDefaultGame { Code = "CLV-P-NAATE",  Name = "Punch-Out!! Featuring Mr. Dream", Size = 1038128 },
            new NesDefaultGame { Code = "CLV-P-NAAUE",  Name = "Ice Climber", Size = 553436 },
            new NesDefaultGame { Code = "CLV-P-NAAVE",  Name = "Kid Icarus", Size = 670710 },
            new NesDefaultGame { Code = "CLV-P-NAAWE",  Name = "Mario Bros.", Size = 1018973 },
            new NesDefaultGame { Code = "CLV-P-NAAXE",  Name = "Dr. MARIO", Size = 1089427 },
            new NesDefaultGame { Code = "CLV-P-NAAZE",  Name = "StarTropics", Size = 1299361 },
            new NesDefaultGame { Code = "CLV-P-NABBE",  Name = "MEGA MAN™ 2", Size = 569868 },
            new NesDefaultGame { Code = "CLV-P-NABCE",  Name = "GHOSTS'N GOBLINS™", Size = 440971 },
            new NesDefaultGame { Code = "CLV-P-NABJE",  Name = "FINAL FANTASY®", Size = 552556 },
            new NesDefaultGame { Code = "CLV-P-NABKE",  Name = "BUBBLE BOBBLE" , Size = 474232 },
            new NesDefaultGame { Code = "CLV-P-NABME",  Name = "PAC-MAN", Size = 325888 },
            new NesDefaultGame { Code = "CLV-P-NABNE",  Name = "Galaga", Size =  347079 },
            new NesDefaultGame { Code = "CLV-P-NABQE",  Name = "Castlevania", Size = 434240 },
            new NesDefaultGame { Code = "CLV-P-NABRE",  Name = "GRADIUS", Size = 370790 },
            new NesDefaultGame { Code = "CLV-P-NABVE",  Name = "Super C", Size = 565974 },
            new NesDefaultGame { Code = "CLV-P-NABXE",  Name = "Castlevania II Simon's Quest", Size = 569759 },
            new NesDefaultGame { Code = "CLV-P-NACBE",  Name = "NINJA GAIDEN", Size =573536 },
            new NesDefaultGame { Code = "CLV-P-NACDE",  Name = "TECMO BOWL", Size =568276 },
            new NesDefaultGame { Code = "CLV-P-NACHE",  Name = "DOUBLE DRAGON II: The Revenge", Size = 578900 }
        };
        NesDefaultGame[] defaultFamicomGames = new NesDefaultGame[] {
            new NesDefaultGame { Code = "CLV-P-HAAAJ",  Name = "スーパーマリオブラザーズ", Size = 596775 },
            new NesDefaultGame { Code = "CLV-P-HAACJ",  Name = "スーパーマリオブラザーズ３", Size = 1411534 },
            new NesDefaultGame { Code = "CLV-P-HAADJ",  Name = "スーパーマリオＵＳＡ", Size = 1501542 },
            new NesDefaultGame { Code = "CLV-P-HAAEJ",  Name = "ドンキーコング" , Size = 568006 },
            new NesDefaultGame { Code = "CLV-P-HAAHJ",  Name = "エキサイトバイク" , Size = 597513 },
            new NesDefaultGame { Code = "CLV-P-HAAMJ",  Name = "マリオオープンゴルフ" , Size = 798179 },
            new NesDefaultGame { Code = "CLV-P-HAANJ",  Name = "ゼルダの伝説", Size = 677971  },
            new NesDefaultGame { Code = "CLV-P-HAAPJ",  Name = "星のカービィ　夢の泉の物語" , Size = 1331436 },
            new NesDefaultGame { Code = "CLV-P-HAAQJ",  Name = "メトロイド" , Size = 666895 },
            new NesDefaultGame { Code = "CLV-P-HAARJ",  Name = "バルーンファイト" , Size = 569750 },
            new NesDefaultGame { Code = "CLV-P-HAASJ",  Name = "リンクの冒険" , Size = 666452 },
            new NesDefaultGame { Code = "CLV-P-HAAUJ",  Name = "アイスクライマー" , Size = 812372    },
            new NesDefaultGame { Code = "CLV-P-HAAWJ",  Name = "マリオブラザーズ" , Size = 1038275 },
            new NesDefaultGame { Code = "CLV-P-HAAXJ",  Name = "ドクターマリオ" , Size = 1083234   },
            new NesDefaultGame { Code = "CLV-P-HABBJ",  Name = "ロックマン®2 Dr.ワイリーの謎" , Size = 592425  },
            new NesDefaultGame { Code = "CLV-P-HABCJ",  Name = "魔界村®", Size = 456166    },
            new NesDefaultGame { Code = "CLV-P-HABLJ",  Name = "ファイナルファンタジー®III" , Size = 830898 },
            new NesDefaultGame { Code = "CLV-P-HABMJ",  Name = "パックマン" , Size = 341593 },
            new NesDefaultGame { Code = "CLV-P-HABNJ",  Name = "ギャラガ", Size =  345552 },
            new NesDefaultGame { Code = "CLV-P-HABQJ",  Name = "悪魔城ドラキュラ" , Size = 428522 },
            new NesDefaultGame { Code = "CLV-P-HABRJ",  Name = "グラディウス", Size = 393055 },
            new NesDefaultGame { Code = "CLV-P-HABVJ",  Name = "スーパー魂斗羅" , Size = 569537 },
            new NesDefaultGame { Code = "CLV-P-HACAJ",  Name = "イー・アル・カンフー", Size = 336353 },
            new NesDefaultGame { Code = "CLV-P-HACBJ",  Name = "忍者龍剣伝" , Size = 578623 },
            new NesDefaultGame { Code = "CLV-P-HACCJ",  Name = "ソロモンの鍵" , Size = 387084 },
            new NesDefaultGame { Code = "CLV-P-HACEJ",  Name = "つっぱり大相撲", Size = 392595 },
            new NesDefaultGame { Code = "CLV-P-HACHJ",  Name = "ダブルドラゴンⅡ The Revenge", Size = 579757 },
            new NesDefaultGame { Code = "CLV-P-HACJJ",  Name = "ダウンタウン熱血物語" , Size = 588367 },
            new NesDefaultGame { Code = "CLV-P-HACLJ",  Name = "ダウンタウン熱血行進曲 それゆけ大運動会", Size = 587083 },
            new NesDefaultGame { Code = "CLV-P-HACPJ",  Name = "アトランチスの謎", Size = 376213 }
        };
        public static GameManager GetInstance()
        {
            if(instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }
        private static GameManager instance;
        private Dictionary<AppTypeCollection.AppInfo, List<NesMiniApplication>> systemClassifiedGames = new Dictionary<AppTypeCollection.AppInfo, List<NesMiniApplication>>();
        private List<NesMiniApplication> gameLibrary = new List<NesMiniApplication>();
        private GameManager()
        {
            LoadLibrary();
        }
        public IOrderedEnumerable<NesMiniApplication> getAllGames()
        {
            return gameLibrary.OrderBy(o => o.Name);
        }
        public void SaveChanges()
        {
            var selected = new List<string>();
            foreach (var game in gameLibrary)
            {
                try
                {
                    if(game.Selected)
                    {
                        selected.Add(game.Code);
                    }
                    // Maybe type was changed? Need to reload games
                    game.Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show( ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            /*Rebuild selected config*/
            ConfigIni.SelectedGames = string.Join(";", selected.ToArray());
            ConfigIni.Save();
            
            /*Why? If a game type has change its going to be reloaded...*/
          //  LoadLibrary();
        }
        public void ReloadDefault()
        {
            string[] selectedGames = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<NesDefaultGame> toremove = new List<NesDefaultGame>();
            foreach (var game in gameLibrary)
            {
                if (game.GetType() == typeof(NesDefaultGame))
                {
                    toremove.Add((NesDefaultGame)game);
                }
            }
            foreach (var tr in toremove)
            {
                gameLibrary.Remove(tr);
            }
            AppTypeCollection.AppInfo inf = AppTypeCollection.GetAppByClass(typeof(NesDefaultGame));
            if (systemClassifiedGames.ContainsKey(inf))
            {
                systemClassifiedGames[inf].Clear();
            }

            foreach (var game in new List<NesDefaultGame>(ConfigIni.ConsoleType == 0 ? defaultNesGames : defaultFamicomGames).OrderBy(o => o.Name))
            {
                game.Selected = selectedGames.Contains(game.Code);

                gameLibrary.Add(game);

                AppTypeCollection.AppInfo inf2 = AppTypeCollection.GetAppByClass(game.GetType());
                if (!systemClassifiedGames.ContainsKey(inf2))
                {
                    systemClassifiedGames[inf2] = new List<NesMiniApplication>();
                }
                systemClassifiedGames[inf2].Add(game);
            }
        }
        private void LoadLibrary()
        {

            gameLibrary.Clear();
            ReloadDefault();
            string[] selectedGames = ConfigIni.SelectedGames.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (!System.IO.Directory.Exists(NesMiniApplication.GamesDirectory))
            {
                Directory.CreateDirectory(NesMiniApplication.GamesDirectory);
            }
            var gameDirs = Directory.GetDirectories(NesMiniApplication.GamesDirectory);
         
            foreach (var gameDir in gameDirs)
            {
                try
                {
                    // Removing empty directories without errors
                    try
                    {
                        var game = NesMiniApplication.FromDirectory(gameDir);
                        game.Selected = selectedGames.Contains(game.Code);
                        
                        gameLibrary.Add(game);

                        AppTypeCollection.AppInfo inf = AppTypeCollection.GetAppByClass(game.GetType());
                        if(!systemClassifiedGames.ContainsKey(inf))
                        {
                            systemClassifiedGames[inf] = new List<NesMiniApplication>();
                        }
                        systemClassifiedGames[inf].Add(game);
                    }
                    catch (FileNotFoundException ex) // Remove bad directories if any
                    {
                        Debug.WriteLine(ex.Message + ex.StackTrace);
                        Directory.Delete(gameDir, true);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    MessageBox.Show(ex.Message, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
            }
        }
    }
}
