using System;
using TeamShinkansen.Scrapers.Interfaces;

namespace TeamShinkansen.Scrapers.TheGamesDB
{
    public class ScraperData : IScraperData
    {
        public string ID { get; internal set; }
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string[] Publishers { get; internal set; }
        public string[] Developers { get; internal set; }
        public string Platform { get; internal set; }
        public DateTime ReleaseDate { get; internal set; }
        public string Copyright { get; internal set; }
        public IScraperGenre[] Genres { get; internal set; }
        public int PlayerCount { get; internal set; }
        public IScraperImage[] Images { get; internal set; }
    }
}
