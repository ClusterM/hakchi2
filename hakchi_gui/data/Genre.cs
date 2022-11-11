using System.Collections.Generic;
using System.Linq;

namespace com.clusterrr.hakchi_gui.data
{
    public class Genre
    {
        public string FolderImageId { get; set; }
        public string LocalizedName => Properties.Resources.ResourceManager.GetString(LocalizedNameKey);

        private string _EnglishName = null;
        public string EnglishName
        {
            get => _EnglishName ?? DesktopName;
            set => _EnglishName = value;
        }
        public string DesktopName { get; set; }
        public string LocalizedNameKey { get; set; }
        public int[] GamesDbId { get; set; } = new int[] { };

        public static IReadOnlyList<Genre> GenreList = new List<Genre>()
        {
            new Genre(){
                DesktopName = "Action",
                LocalizedNameKey = "GenreAction",
                FolderImageId = "genre_action",
                GamesDbId = new int[] { 1 }
            },
            new Genre(){
                DesktopName = "ActionShooting",
                EnglishName = "Action / Shooting",
                LocalizedNameKey = "GenreActionShooting",
                FolderImageId = "genre_shooting",
                GamesDbId = new int[] { 8 }
            },
            new Genre(){
                DesktopName = "Adventure",
                LocalizedNameKey = "GenreAdventure",
                FolderImageId = "genre_adventure",
                GamesDbId = new int[] { 2 }
            },
            new Genre(){
                DesktopName = "Educational",
                LocalizedNameKey = "GenreEducational",
                FolderImageId = "genre_educational"
            },
            new Genre(){
                DesktopName = "Fighting",
                LocalizedNameKey = "GenreFighting",
                FolderImageId = "genre_fighting",
                GamesDbId = new int[] { 10 }
            },
            new Genre(){
                DesktopName = "Puzzle",
                LocalizedNameKey = "GenrePuzzle",
                FolderImageId = "genre_puzzle",
                GamesDbId = new int[] { 5 }
            },
            new Genre(){
                DesktopName = "Racing",
                LocalizedNameKey = "GenreRacing",
                FolderImageId = "genre_racing",
                GamesDbId = new int[] { 7 }
            },
            new Genre(){
                DesktopName = "RacingSports",
                EnglishName = "Racing / Sports",
                LocalizedNameKey = "GenreRacingSports",
                FolderImageId = "genre_racing_sports"
            },
            new Genre(){
                DesktopName = "RPG",
                LocalizedNameKey = "GenreRPG",
                FolderImageId = "genre_rpg",
                GamesDbId = new int[] { 4, 14 }
            },
            new Genre(){
                DesktopName = "Shoot-'em-Up",
                LocalizedNameKey = "GenreShootEmUp",
                FolderImageId = "genre_shootemup",
                GamesDbId = new int[] { 8 }
            },
            new Genre(){
                DesktopName = "Simulation",
                LocalizedNameKey = "GenreSimulation",
                FolderImageId = "genre_simulation",
                GamesDbId = new int[] { 3, 9, 13, 19 }
            },
            new Genre(){
                DesktopName = "Sports",
                LocalizedNameKey = "GenreSports",
                FolderImageId = "genre_sports",
                GamesDbId = new int[] { 11 }
            },
            new Genre(){
                DesktopName = "Table",
                LocalizedNameKey = "GenreTable",
                FolderImageId = "genre_table"
            }
        };
        private static Dictionary<string, Genre> _GenreDictionary = null;
        public static IReadOnlyDictionary<string, Genre> GenreDictionary
        {
            get
            {
                if (_GenreDictionary == null)
                {
                    _GenreDictionary = new Dictionary<string, Genre>();

                    foreach (var genre in GenreList.OrderBy(e => e.DesktopName))
                    {
                        _GenreDictionary[genre.DesktopName] = genre;
                    }
                }

                return _GenreDictionary;
            }
        }
    }
}
