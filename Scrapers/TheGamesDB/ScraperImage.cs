using TeamShinkansen.Scrapers.Enums;
using TeamShinkansen.Scrapers.Interfaces;

namespace TeamShinkansen.Scrapers.TheGamesDB
{
    public class ScraperImage : IScraperImage
    {
        public ArtType Type { get; internal set; }

        public string Url { get; internal set; }

        public string Description { get; internal set; }
    }
}
