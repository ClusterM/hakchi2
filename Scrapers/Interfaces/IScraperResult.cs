using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamShinkansen.Scrapers.Interfaces
{
    public interface IScraperResult
    {
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        IEnumerable<IScraperData> Items { get; }
        Task<IScraperResult> GetNextPage();
        Task<IScraperResult> GetPreviousPage();
    }
}
