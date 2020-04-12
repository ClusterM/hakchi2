using System.Threading.Tasks;

namespace TeamShinkansen.Scrapers.Interfaces
{
    public interface IScraper
    {
        // General scraper info
        string ProviderName { get; }
        string ProviderUrl { get; }
        string ApiKey { get; set; }

        // Booleans describing if certain data is provided
        bool ProvidesDescription { get; }
        bool ProvidesReleaseDate { get; }
        bool ProvidesCopyright { get; }
        bool ProvidesGenre { get; }
        bool ProvidesPlayerCount { get; }

        // Bools describing if a provider supports searching by hash
        bool CanUseCRC { get; }
        bool CanUseMD5 { get; }
        bool CanUseSHA1 { get; }

        // Methods to find information
        Task<IScraperResult> GetInfoByName(string gameName);
        Task<IScraperResult> GetInfoByCRC(byte[] crc);
        Task<IScraperResult> GetInfoByMD5(byte[] md5);
        Task<IScraperResult> GetInfoBySHA1(byte[] sha1);
    }
}
