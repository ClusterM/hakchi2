using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.clusterrr.hakchi_gui
{
    
    public struct HmodReadme
    {
        public static readonly string[] readmeFiles = new string[] { "readme.md", "readme.txt", "readme" };
        public readonly Dictionary<string, string> frontMatter;
        public readonly string readme;
        public readonly string rawReadme;
        public readonly bool isMarkdown;
        public readonly string[] headingLines;
        public HmodReadme(string readme, bool markdown = false)
        {
            this.rawReadme = readme;
            Dictionary<string, string> output = new Dictionary<string, string>();
            Match match = Regex.Match(readme, "^(?:-{3,}[\\r\\n]+(.*?)[\\r\\n]*-{3,})?[\\r\\n\\t\\s]*(.*)[\\r\\n\\t\\s]*$", RegexOptions.Singleline);
            this.readme = match.Groups[2].Value.Trim();
            MatchCollection matches = Regex.Matches(match.Groups[1].Value, "^[\\s\\t]*([^:]+)[\\s\\t]*:[\\s\\t]*(.*?)[\\s\\t]*$", RegexOptions.Multiline);
            foreach (Match fmMatch in matches)
            {
                if (!output.ContainsKey(fmMatch.Groups[1].Value))
                {
                    output.Add(fmMatch.Groups[1].Value, fmMatch.Groups[2].Value);
                }
            }

            frontMatter = output;
            isMarkdown = markdown;

            string[] headingFields = { "Creator", "Version" };
            List<string> headingLines = new List<string>();

            foreach (string heading in headingFields)
            {
                string keyValue;
                if (frontMatter.TryGetValue(heading, out keyValue))
                {
                    headingLines.Add($"**{heading}:** {keyValue}");
                }
            }

            foreach (string keyName in frontMatter.Keys)
            {
                if (!headingFields.Contains(keyName) && keyName != "Name")
                {
                    headingLines.Add($"**{keyName}:** {frontMatter[keyName]}");
                }
            }

            this.headingLines = headingLines.ToArray();
        }
    }
}
