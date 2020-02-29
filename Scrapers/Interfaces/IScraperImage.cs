using TeamShinkansen.Scrapers.Enums;

namespace TeamShinkansen.Scrapers.Interfaces
{
    public interface IScraperImage
    {
        ArtType Type { get; }
        string Url { get; }
        string Description { get; }
    }
}
