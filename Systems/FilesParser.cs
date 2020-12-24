namespace BatchReplacer.Systems
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Data;

    public class FilesParser
    {
        private readonly (Regex pattern, string replace) fileNameSearch;
        private readonly (Regex pattern, string replace)[] contentSearches;

        public FilesParser(ReplaceInfo fileNameReplacement, ReplaceInfo[] contentReplacements)
        {
            this.contentSearches = new (Regex, string)[contentReplacements.Length];
            this.fileNameSearch = (new Regex(fileNameReplacement.Search), fileNameReplacement.Replace);

            for (var i = 0; i < contentReplacements.Length; i++)
            {
                var (search, replace) = contentReplacements[i];
                this.contentSearches[i] = (new Regex(search), replace);
            }
        }

        public IEnumerable<string> GetFiles(string pattern, IEnumerable<string> paths)
        {
            return paths.SelectMany(path => Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
        }

        public async IAsyncEnumerable<(string file, (bool changed, string content))> ParseFilesAsync(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var result = await ReplaceContent(file);
                yield return (file, result);
            }
        }

        public async Task SaveContent(string file, string content)
        {
            await File.WriteAllTextAsync(file, content);
        }

        public bool ParseFileName(string file, out string newName)
        {
            var (pattern, replace) = this.fileNameSearch;
            newName = pattern.Replace(file, replace);

            if (file == newName) return false;
            
            File.Move(file, newName);
            return true;
        }

        private async Task<(bool, string)> ReplaceContent(string file)
        {
            var content = await File.ReadAllTextAsync(file);
            var result = content;
            result = contentSearches.Aggregate(result, ReplacePattern);
            return (content != result, result);
        }

        private static string ReplacePattern(string current, (Regex pattern, string replace) search)
        {
            return search.pattern.Replace(current, search.replace);
        }
    }
}