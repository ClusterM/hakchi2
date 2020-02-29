using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamShinkansen.Scrapers.Interfaces;
using TeamShinkansen.Scrapers.TheGamesDB.ApiModels;

namespace TeamShinkansen.Scrapers.TheGamesDB
{
    public class ScraperResult : IScraperResult
    {
        internal Scraper scraper { get; set; }
        public IEnumerable<IScraperData> Items { get; internal set; } = new ScraperData[] { };

        public int RemainingMonthlyAllowance { get; internal set; } = 0;
        public int ExtraAllowance { get; internal set; } = 0;

        internal string PreviousPageURL = null;
        internal string NextPageURL = null;
        public bool HasPreviousPage { get => PreviousPageURL != null; }
        public bool HasNextPage { get => NextPageURL != null; }

        public async Task<IScraperResult> GetNextPage()
        {
            if (NextPageURL == null)
            {
                throw new Exception("Page unavailable");
            }
            var response = await API.ApiRequest<GamesResponse>(NextPageURL);
            return await scraper.ApiToResult(response);
        }
        public async Task<IScraperResult> GetPreviousPage() 
        {
            if (PreviousPageURL == null)
            {
                throw new Exception("Page unavailable");
            }
            var response = await API.ApiRequest<GamesResponse>(PreviousPageURL);
            return await scraper.ApiToResult(response);
        }
    }
}
