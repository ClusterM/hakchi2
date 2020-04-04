using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeamShinkansen.Scrapers.Interfaces;
using TeamShinkansen.Scrapers.TheGamesDB.ApiModels;

namespace TeamShinkansen.Scrapers.TheGamesDB
{
    public enum ArtSize { Original, Small, Thumb, CroppedCenterThumb, Medium, Large }
    public class Scraper : IScraper
    {
        public API TgdbApi = new API();
        public string ProviderName => "TheGamesDB";
        public string ApiKey { get => TgdbApi.Key; set => TgdbApi.Key = value; }
        public bool ProvidesDescription => true;
        public bool ProvidesReleaseDate => true;
        public bool ProvidesCopyright => false;
        public bool ProvidesGenre => true;
        public bool ProvidesPlayerCount => true;
        public bool CanUseCRC => false;
        public bool CanUseMD5 => false;
        public bool CanUseSHA1 => false;
        private string _CachePath = null;
        public string CachePath {
            get
            {
                return _CachePath;
            }
            set
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }

                _CachePath = value;
            } 
        }
        public ArtSize ArtSize { get; set; } = ArtSize.Original;


        private bool LoadedGenreCache = false;
        private bool LoadedPublisherCache = false;
        private bool LoadedPlatformCache = false;
        private bool LoadedDeveloperCache = false;

        private Dictionary<string, ScraperGenre> genres = new Dictionary<string, ScraperGenre>();
        private Dictionary<string, string> publishers = new Dictionary<string, string>();
        private Dictionary<string, string> developers = new Dictionary<string, string>();
        private Dictionary<string, Platform> platforms = new Dictionary<string, Platform>();

        internal enum DictionaryTypes { Genre, Publisher, Platform }

        internal async Task<ScraperGenre> GetGenreValue(string key)
        {
            if (genres.ContainsKey(key))
            {
                return genres[key];
            }

            string jsonPath = null;

            if (CachePath != null)
            {
                jsonPath = Path.Combine(CachePath, "genres.json");
            }

            if (CachePath != null && LoadedGenreCache == false && File.Exists(jsonPath))
            {
                var genresCache = JsonConvert.DeserializeObject<Dictionary<string, ScraperGenre>>(File.ReadAllText(jsonPath));
                LoadedGenreCache = true;

                foreach (var genre in genresCache)
                {
                    if (!genres.ContainsKey(genre.Key))
                    {
                        genres.Add(genre.Key, genre.Value);
                    }
                }

                if (genres.ContainsKey(key))
                {
                    return genres[key];
                }
            }

            var response = await TgdbApi.GetGenres();

            foreach (var genre in response.Data.Genres.Values)
            {
                if (!genres.ContainsKey(genre.ID.ToString()))
                {
                    genres.Add(genre.ID.ToString(), new ScraperGenre()
                    {
                        ID = genre.ID,
                        Name = genre.Name
                    });
                }
            }

            if (jsonPath != null)
            {
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(genres));
            }

            if (genres.ContainsKey(key))
            {
                return genres[key];
            }

            return null;
        }
        internal async Task<string> GetPublisherValue(string key)
        {
            if (publishers.ContainsKey(key))
            {
                return publishers[key];
            }

            string jsonPath = null;

            if (CachePath != null)
            {
                jsonPath = Path.Combine(CachePath, "publishers.json");
            }

            if (jsonPath != null && LoadedPublisherCache == false && File.Exists(jsonPath))
            {
                var publishersCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonPath));
                LoadedPublisherCache = true;

                foreach (var publisher in publishersCache)
                {
                    if (!publishers.ContainsKey(publisher.Key))
                    {
                        publishers.Add(publisher.Key, publisher.Value);
                    }
                }

                if (publishers.ContainsKey(key))
                {
                    return publishers[key];
                }
            }

            var response = await TgdbApi.GetPublishers();

            foreach (var publisher in response.Data.Publishers.Values)
            {
                if (!publishers.ContainsKey(publisher.ID.ToString()))
                {
                    publishers.Add(publisher.ID.ToString(), publisher.Name);
                }
            }

            if (jsonPath != null)
            {
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(publishers));
            }

            if (publishers.ContainsKey(key))
            {
                return publishers[key];
            }

            return null;
        }
        internal async Task<Platform> GetPlatformValue(string key)
        {
            if (platforms.ContainsKey(key))
            {
                return platforms[key];
            }

            string jsonPath = null;

            if (CachePath != null)
            {
                jsonPath = Path.Combine(CachePath, "platforms.json");
            }

            if (jsonPath != null && LoadedPlatformCache == false && File.Exists(jsonPath))
            {
                var platformsCache = JsonConvert.DeserializeObject<Dictionary<string, Platform>>(File.ReadAllText(jsonPath));
                LoadedPlatformCache = true;

                foreach (var platform in platformsCache)
                {
                    if (!platforms.ContainsKey(platform.Key))
                    {
                        platforms.Add(platform.Key, platform.Value);
                    }
                }

                if (platforms.ContainsKey(key))
                {
                    return platforms[key];
                }
            }

            var response = await TgdbApi.GetPlatforms();

            foreach (var platform in response.Data.Platforms.Values)
            {
                if (!platforms.ContainsKey(platform.ID.ToString()))
                {
                    platforms.Add(platform.ID.ToString(), platform);
                }
            }

            if (jsonPath != null)
            {
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(platforms));
            }

            if (platforms.ContainsKey(key))
            {
                return platforms[key];
            }

            return null;
        }
        internal async Task<string> GetDeveloperValue(string key)
        {
            if (developers.ContainsKey(key))
            {
                return developers[key];
            }

            string jsonPath = null;

            if (CachePath != null)
            {
                jsonPath = Path.Combine(CachePath, "developers.json");
            }

            if (jsonPath != null && LoadedDeveloperCache == false && File.Exists(jsonPath))
            {
                var developersCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonPath));
                LoadedDeveloperCache = true;

                foreach (var developer in developersCache)
                {
                    if (!developers.ContainsKey(developer.Key))
                    {
                        developers.Add(developer.Key, developer.Value);
                    }
                }

                if (developers.ContainsKey(key))
                {
                    return developers[key];
                }
            }

            var response = await TgdbApi.GetDevelopers();

            foreach (var developer in response.Data.Developers.Values)
            {
                if (!developers.ContainsKey(developer.ID.ToString()))
                {
                    developers.Add(developer.ID.ToString(), developer.Name);
                }
            }

            if (jsonPath != null)
            {
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(developers));
            }

            if (developers.ContainsKey(key))
            {
                return developers[key];
            }

            return null;
        }

        internal async Task<ScraperResult> ApiToResult(GamesResponse response)
        {

            var result = new ScraperResult()
            {
                scraper = this,
                PreviousPageURL = response.Pages.Previous,
                NextPageURL = response.Pages.Next,
                RemainingMonthlyAllowance = response.RemainingMonthlyAllowance,
                ExtraAllowance = response.ExtraAllowance
            };

            var items = new List<ScraperData>();

            foreach (var item in response.Data.Games)
            {
                var data = new ScraperData()
                {
                    Name = item.Title,
                    Description = item.Overview,
                    PlayerCount = item.Players ?? 1,
                    ReleaseDate = DateTime.ParseExact(item.ReleaseDate ?? "1970-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    ID = (item.ID ?? -1).ToString()
                };

                var Genres = new List<ScraperGenre>();
                var Publishers = new List<string>();
                var Developers = new List<string>();
                var Images = new List<ScraperImage>();

                if (item.Genres != null)
                {
                    foreach (var key in item.Genres)
                    {
                        var genre = await GetGenreValue(key.ToString());
                        if (genre != null)
                        {
                            Genres.Add(genre);
                        }
                    }
                }

                if (item.Publishers != null)
                {
                    foreach (var key in item.Publishers)
                    {
                        var publisher = await GetPublisherValue(key.ToString());
                        if (publisher != null)
                        {
                            Publishers.Add(publisher);
                        }
                    }
                }

                if (item.Developers != null)
                {
                    foreach (var key in item.Developers)
                    {
                        var developer = await GetDeveloperValue(key.ToString());
                        if (developer != null)
                        {
                            Developers.Add(developer);
                        }
                    }
                }

                var platformValue = item.Platform == null ? new Platform()
                {
                    Name = ""
                } : await GetPlatformValue(item.Platform.ToString());

                BoxartImage[] boxart;
                if (response.Includes.Boxart.Data.TryGetValue(item.ID.ToString(), out boxart))
                {
                    foreach (var image in boxart)
                    {
                        var output = new ScraperImage();
                        switch (ArtSize)
                        {
                            case ArtSize.Thumb:
                                output.Url = $"{response.Includes.Boxart.BaseURL.Thumbnail}{image.Filename}";
                                break;

                            case ArtSize.CroppedCenterThumb:
                                output.Url = $"{response.Includes.Boxart.BaseURL.CroppedCenterThumbnail}{image.Filename}";
                                break;

                            case ArtSize.Small:
                                output.Url = $"{response.Includes.Boxart.BaseURL.Small}{image.Filename}";
                                break;

                            case ArtSize.Medium:
                                output.Url = $"{response.Includes.Boxart.BaseURL.Medium}{image.Filename}";
                                break;

                            case ArtSize.Large:
                                output.Url = $"{response.Includes.Boxart.BaseURL.Large}{image.Filename}";
                                break;

                            default:
                                output.Url = $"{response.Includes.Boxart.BaseURL.Original}{image.Filename}";
                                break;
                        }

                        switch (image.Type)
                        {
                            case "boxart":
                                switch (image.Side)
                                {
                                    case "front":
                                        output.Type = Enums.ArtType.Front;
                                        break;

                                    case "back":
                                        output.Type = Enums.ArtType.Back;
                                        break;

                                    case "spine":
                                        output.Type = Enums.ArtType.Spine;
                                        break;

                                    default:
                                        output.Type = Enums.ArtType.Other;
                                        break;
                                }
                                break;

                            case "fanart":
                                output.Type = Enums.ArtType.FanArt;
                                break;

                            case "clearlogo":
                                output.Type = Enums.ArtType.ClearLogo;
                                break;

                            case "banner":
                                output.Type = Enums.ArtType.Banner;
                                break;

                            case "screenshot":
                                output.Type = Enums.ArtType.Screenshot;
                                break;

                            default:
                                output.Type = Enums.ArtType.Other;
                                break;
                        }
                        Images.Add(output);
                    }
                }

                data.Genres = Genres.Cast<IScraperGenre>().ToArray();
                data.Publishers = Publishers.ToArray();
                data.Developers = Developers.ToArray();
                data.Platform = platformValue?.Name;
                data.Images = Images.ToArray();

                if (data.ReleaseDate != null)
                {
                    data.Copyright = $"© {data.ReleaseDate.Year} {String.Join(", ", data.Developers)}";
                }
                else
                {
                    data.Copyright = $"© {String.Join(", ", data.Developers)}";
                }

                items.Add(data);

            }
            result.Items = items.ToArray();

            return result;
        }

        public async Task<IScraperResult> GetInfoById(params int[] id)
        {
            var response = await TgdbApi.GetGamesByGameID(String.Join(",", id));
            return await ApiToResult(response);
        }
        public async Task<IScraperResult> GetInfoByName(string gameName)
        {
            var response = await TgdbApi.GetGamesByGameName(gameName);
            return await ApiToResult(response);
        }

        public async Task<IScraperResult> GetInfoByID(params int[] id)
        {
            var response = await TgdbApi.GetGamesByGameID(String.Join(",", Array.ConvertAll(id, i => i.ToString())));
            return await ApiToResult(response);
        }

        // Scraper doesn't support these
        public Task<IScraperResult> GetInfoByCRC(byte[] crc) => throw new NotImplementedException();
        public Task<IScraperResult> GetInfoByMD5(byte[] md5) => throw new NotImplementedException();
        public Task<IScraperResult> GetInfoBySHA1(byte[] sha1) => throw new NotImplementedException();

        // ToString
        public override string ToString() => ProviderName;
    }
}
