using System;

namespace TeamShinkansen.Scrapers.Interfaces
{
    public interface IScraperData
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        string[] Publishers { get; }
        string[] Developers { get; }
        string Platform { get; }
        DateTime ReleaseDate { get; }
        string Copyright { get; }
        IScraperGenre[] Genres { get; }
        int PlayerCount { get; }
        IScraperImage[] Images { get; }
    }
}
