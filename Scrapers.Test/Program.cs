using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TeamShinkansen.Scrapers.TheGamesDB;
using TeamShinkansen.Scrapers.TheGamesDB.ApiModels;

namespace ScrapersTest
{
    class Program
    {
        static string WordWrap(string input, int characterLimit)
        {
            StringBuilder output = new StringBuilder();

            foreach (var line in input.Replace("\r", "").Split("\n"))
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.Length <= characterLimit)
                {
                    output.AppendLine(trimmedLine);
                    continue;
                }

                string[] words = trimmedLine.Split(' ');

                string wrappedLine = "";
                foreach (string word in words)
                {
                    if ((wrappedLine + word).Length > characterLimit)
                    {
                        output.AppendLine(wrappedLine.Trim());
                        wrappedLine = "";
                    }

                    wrappedLine += string.Format("{0} ", word);
                }

                if (wrappedLine.Length > 0)
                {
                    output.AppendLine(wrappedLine.Trim());
                }
            }

            return output.ToString();
        }

        static string LineLimit(string sentence, int lineLimit)
        {
            var lines = sentence.Replace("\r", "").Split("\n");

            if (lines.Length <= lineLimit)
            {
                return string.Join("\n", lines);
            }
            else
            {
                return string.Join("\n", lines.Take(lineLimit).ToArray());
            }
        }

        static void HandleResult(GamesResponse response, Stream stream)
        {
            foreach (var game in response.Data.Games)
            {
                stream.Write(Encoding.UTF8.GetBytes($"{game.Title}\thttps://thegamesdb.net/game.php?id={game.ID}\n"));
            }
        }
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            API.TraceURLs = true;
            var scraper = new API()
            {
                Key = "TMP_TEST_KEY"
            };

            using (var output = File.OpenWrite("6.txt"))
            {
                var test = scraper.GetGamesByPlatformID("6");
                GamesResponse result;
                test.Wait();
                result = test.Result as GamesResponse;

                HandleResult(result, output);

                while (result.Pages.Next != null)
                {
                    var response = result.Pages.GetNextPage();
                    response.Wait();
                    result = response.Result;
                    HandleResult(result, output);
                }
            }

            using (var output = File.OpenWrite("7.txt"))
            {
                var test = scraper.GetGamesByPlatformID("7");
                GamesResponse result;
                test.Wait();
                result = test.Result as GamesResponse;

                HandleResult(result, output);

                while (result.Pages.Next != null)
                {
                    var response = result.Pages.GetNextPage();
                    response.Wait();
                    result = response.Result;
                    HandleResult(result, output);
                }
            }
        }
    }
}
