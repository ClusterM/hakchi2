using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeamShinkansen.Scrapers.TheGamesDB.ApiModels;

namespace TeamShinkansen.Scrapers.TheGamesDB
{
    public class API
    {
        /// <summary>
        /// The API key to use
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// If true, requested API URLs will be sent to Trace.WriteLine, these URLs will not contain the API key.
        /// </summary>
        public static bool TraceURLs { get; set; } = false;

        internal async Task<responseType> ApiRequest<responseType>(string route, Dictionary<string, string> parameters = null)
        {
            using (var wc = new WebClient())
            {
                var escapedParameters = new List<string>();

                escapedParameters.Add($"apikey={WebUtility.UrlEncode(Key)}");

                foreach (var item in parameters ?? new Dictionary<string, string>())
                {
                    escapedParameters.Add($"{WebUtility.UrlEncode(item.Key)}={WebUtility.UrlEncode(item.Value)}");
                }

                return await ApiRequest<responseType>(string.Format("https://api.thegamesdb.net/v1{0}?{1}", route, string.Join("&", escapedParameters.ToArray())));
            }
        }

        public static async Task<responseType> ApiRequest<responseType>(string url)
        {
            using (var wc = new WebClient())
            {
                if (TraceURLs)
                    Trace.WriteLine($"Fetching: {Regex.Replace(url, "apikey=[^&]+", "")}");

                var json = await wc.DownloadStringTaskAsync(url);

                if (typeof(responseType) == typeof(string))
                {
                    return (responseType)(object)json;
                }

                try
                {
                    var deserialized = JsonConvert.DeserializeObject<responseType>(json);
                    return deserialized;
                }
                catch (Exception ex)
                {
                    return default(responseType);
                }
            }
        }

        /// <summary>
        /// Fetch game(s) by id
        /// </summary>
        /// <param name="gameId">(Required) - supports , delimited list</param>
        /// <param name="fields">(Optional) - valid , delimited options: players, publishers, genres, overview, last_updated, rating, platform, coop, youtube, os, processor, ram, hdd, video, sound, alternates</param>
        /// <param name="include">(Optional) - valid , delimited options: boxart, platform</param>
        /// <returns></returns>
        public async Task<GamesResponse> GetGamesByGameID(string gameId, string fields = "players,publishers,genres,overview,last_updated,rating,platform,coop,youtube,os,processor,ram,hdd,video,sound,alternates", string include = "boxart,platform") => await ApiRequest<GamesResponse>("/Games/ByGameID", new Dictionary<string, string>()
        {
            ["id"] = gameId,
            ["fields"] = fields,
            ["include"] = include
        });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">(Required) - Search term</param>
        /// <param name="fields">(Optional) - valid , delimited options: players, publishers, genres, overview, last_updated, rating, platform, coop, youtube, os, processor, ram, hdd, video, sound, alternates</param>
        /// <param name="filter">(Optional) - platform id can be obtain from the platforms api below, supports , delimited list</param>
        /// <param name="include">(Optional) - valid , delimited options: boxart, platform</param>
        /// <returns></returns>
        public async Task<GamesResponse> GetGamesByGameName(string name, string fields = "players,publishers,genres,overview,last_updated,rating,platform,coop,youtube,os,processor,ram,hdd,video,sound,alternates", string filter = "", string include = "boxart,platform") => await ApiRequest<GamesResponse>("/Games/ByGameName", new Dictionary<string, string>()
        {
            ["name"] = name,
            ["fields"] = fields,
            ["filter[platform]"] = filter,
            ["include"] = include
        });
      
        /// <summary>
        /// Fetch game(s) by platform id
        /// </summary>
        /// <param name="platformId">(Required) - platform id can be obtain from the platforms api below, supports , delimited list</param>
        /// <param name="fields">(Optional) - valid , delimited options: players, publishers, genres, overview, last_updated, rating, platform, coop, youtube, os, processor, ram, hdd, video, sound, alternates</param>
        /// <param name="include">(Optional) - valid , delimited options: boxart, platform</param>
        /// <returns></returns>
        public async Task<GamesResponse> GetGamesByPlatformID(string platformId, string fields = "players,publishers,genres,overview,last_updated,rating,platform,coop,youtube,os,processor,ram,hdd,video,sound,alternates", string include = "boxart,platform") => await ApiRequest<GamesResponse>("/Games/ByPlatformID", new Dictionary<string, string>()
        {
            ["id"] = platformId,
            ["fields"] = fields,
            ["include"] = include
        });

        //-------------------------------------------------------------------
        /// <summary>
        /// Fetch game(s) images by game(s) id
        /// </summary>
        /// <param name="gamesId">(Required) - game(s) id can be obtain from the above games api, supports , delimited list</param>
        /// <param name="filter">(Optional) - valid , delimited options: fanart, banner, boxart, screenshot, clearlogo</param>
        /// <returns></returns>
        public async Task<GamesImageResponse> GetGamesImages(string gamesId, string filter = "") => await ApiRequest<GamesImageResponse>("/Games/Images", new Dictionary<string, string>()
        {
            ["games_id"] = gamesId,
            ["filter[type]"] = filter
        });
     
        /// <summary>
        /// Fetch game(s) images by game(s) id
        /// </summary>
        /// <param name="gamesId">(Required) - game(s) id can be obtain from the above games api, supports , delimited list</param>
        /// <param name="filter">(Optional) - valid , delimited options: fanart, banner, boxart, screenshot, clearlogo</param>
        /// <returns></returns>
        public async Task<GamesImageResponse> GetGamesImages(string[] gamesId, string filter = "") => await GetGamesImages(string.Join(",", gamesId), filter);
     
        /// <summary>
        /// Fetch game(s) images by game(s) id
        /// </summary>
        /// <param name="gamesId">(Required) - game(s) id can be obtain from the above games api, supports , delimited list</param>
        /// <param name="filter">(Optional) - valid , delimited options: fanart, banner, boxart, screenshot, clearlogo</param>
        /// <returns></returns>
        public async Task<GamesImageResponse> GetGamesImages(string[] gamesId, string[] filter) => await GetGamesImages(string.Join(",", gamesId), string.Join(",", filter));
    
        /// <summary>
        /// Fetch game(s) images by game(s) id
        /// </summary>
        /// <param name="gamesId">(Required) - game(s) id can be obtain from the above games api, supports , delimited list</param>
        /// <param name="filter">(Optional) - valid , delimited options: fanart, banner, boxart, screenshot, clearlogo</param>
        /// <returns></returns>
        public async Task<GamesImageResponse> GetGamesImages(string gamesId, string[] filter) => await GetGamesImages(gamesId, string.Join(",", filter));
    
        /// <summary>
        /// Fetch platforms list
        /// </summary>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatforms(string fields = "icon,console,controller,developer,manufacturer,media,cpu,memory,graphics,sound,maxcontrollers,display,overview,youtube") => await ApiRequest<PlatformDataResponse>("/Platforms", new Dictionary<string, string>()
        {
            ["fields"] = fields
        });
     
        /// <summary>
        /// Fetch platforms list
        /// </summary>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatforms(string[] fields) => await GetPlatforms(string.Join(",", fields));
    
        /// <summary>
        /// Fetch platforms list by id
        /// </summary>
        /// <param name="id">(Required) - supports , delimited list</param>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatformsByPlatformID(string id, string fields = "icon,console,controller,developer,manufacturer,media,cpu,memory,graphics,sound,maxcontrollers,display,overview,youtube") => await ApiRequest<PlatformDataResponse>("/Platforms/ByPlatformID", new Dictionary<string, string>()
        {
            ["id"] = id,
            ["fields"] = fields
        });
     
        /// <summary>
        /// Fetch platforms list by id
        /// </summary>
        /// <param name="id">(Required) - supports , delimited list</param>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatformsByPlatformID(int[] id, string fields = "icon,console,controller,developer,manufacturer,media,cpu,memory,graphics,sound,maxcontrollers,display,overview,youtube") => await GetPlatformsByPlatformID(string.Join(",", id.Cast<string>()), fields);
       
        /// <summary>
        /// Fetch platforms list by id
        /// </summary>
        /// <param name="id">(Required) - supports , delimited list</param>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatformsByPlatformID(int[] id, string[] fields) => await GetPlatformsByPlatformID(string.Join(",", id.Cast<string>()), string.Join(",", fields));
       
        /// <summary>
        /// Fetch platforms list by id
        /// </summary>
        /// <param name="id">(Required) - supports , delimited list</param>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatformsByPlatformID(string id, string[] fields) => await GetPlatformsByPlatformID(id, string.Join(",", fields));
    
        /// <summary>
        /// Fetch platforms by name
        /// </summary>
        /// <param name="name">(Required)</param>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatformsByPlatformName(string name, string fields = "icon,console,controller,developer,manufacturer,media,cpu,memory,graphics,sound,maxcontrollers,display,overview,youtube") => await ApiRequest<PlatformDataResponse>("/Platforms/ByPlatformName", new Dictionary<string, string>()
        {
            ["name"] = name,
            ["fields"] = fields
        });
     
        /// <summary>
        /// Fetch platforms by name
        /// </summary>
        /// <param name="name">(Required)</param>
        /// <param name="fields">(Optional) - valid , delimited options: icon, console, controller, developer, manufacturer, media, cpu, memory, graphics, sound, maxcontrollers, display, overview, youtube</param>
        /// <returns></returns>
        public async Task<PlatformDataResponse> GetPlatformsByPlatformName(string name, string[] fields) => await GetPlatformsByPlatformName(name, string.Join(",", fields));
     
        /// <summary>
        /// Fetch platform(s) images by platform(s) id
        /// </summary>
        /// <param name="platformsId">(Required) - platform(s) id can be obtain from the above platforms api, supports , delimited list</param>
        /// <param name="filter">(Optional) - valid , delimited options: fanart, banner, boxart</param>
        /// <returns></returns>
        public async Task<PlatformsImagesResponse> GetPlatformsImages(string platformsId, string filter = "") => await ApiRequest<PlatformsImagesResponse>("/Platforms/Images", new Dictionary<string, string>()
        {
            ["platforms_id"] = platformsId,
            ["filter[type]"] = filter
        });
     
        /// <summary>
        /// Fetch platform(s) images by platform(s) id
        /// </summary>
        /// <param name="platformsId">(Required) - platform(s) id can be obtain from the above platforms api, supports , delimited list</param>
        /// <param name="filter">(Optional) - valid , delimited options: fanart, banner, boxart</param>
        /// <returns></returns>
        public async Task<PlatformsImagesResponse> GetPlatformsImages(int[] platformsId, string filter = "") => await (GetPlatformsImages(string.Join(",", platformsId.Cast<string>())));
      
        /// <summary>
        /// Fetch Genres list
        /// </summary>
        /// <returns></returns>
        public async Task<GenreResponse> GetGenres() => await ApiRequest<GenreResponse>("/Genres", null);

        /// <summary>
        /// Fetch Developers list
        /// </summary>
        /// <returns></returns>
        public async Task<DeveloperResponse> GetDevelopers() => await ApiRequest<DeveloperResponse>("/Developers", null);

        /// <summary>
        /// Fetch Publishers list
        /// </summary>
        /// <returns></returns>
        public async Task<PublishersResponse> GetPublishers() => await ApiRequest<PublishersResponse>("/Publishers", null);
    }
}
