using System.Text;

namespace Books
{
    public class ReversedIndex
    {
        List<string> fileNames;
        private Dictionary<string, Dictionary<string, int>> storage = new Dictionary<string, Dictionary<string, int>>();
        public BinarySearchTree trigramIndex = new BinarySearchTree();

        public ReversedIndex(List<string> fileNames)
        {
            this.fileNames = fileNames;
        }

        public void BuildIndex()
        {
            var tasks = new List<Task<IndexResult>>();
            foreach (string name in fileNames)
            {
                tasks.Add(AddFile(name));
            }
            Task.WaitAll(tasks.ToArray());

            foreach (var t in tasks)
            {
                var r = t.Result;
                foreach (var fileCount in r.occurrences)
                {
                    if (storage.ContainsKey(fileCount.Key))
                    {
                        storage[fileCount.Key].Add(r.fileName, fileCount.Value);
                    }
                    else
                    {
                        storage.Add(fileCount.Key, new Dictionary<string, int>() { [r.fileName] = fileCount.Value });
                    }
                }

                foreach (var wordTrigrams in r.trigrams)
                {
                    foreach (var trigram in wordTrigrams.Value)
                    {
                        var n = trigramIndex.Add(trigram);
                        n.words.Add(wordTrigrams.Key);
                    }
                }
            }
        }

        private List<SearchResult> searchWord(string word, bool includes)
        {
            Dictionary<string, int> wordInFiles = null;

            if (!storage.TryGetValue(word, out wordInFiles))
            {
                wordInFiles = new Dictionary<string, int>();
            }

            foreach (var name in fileNames)
            {
                wordInFiles.TryAdd(name, 0);
            }

            var res = new List<SearchResult>();
            foreach (var sr in wordInFiles)
            {
                if ((sr.Value != 0) == includes)
                {
                    res.Add(new SearchResult(sr.Key, sr.Value));
                }
            }
            return res;
        }

        private List<SearchResult> searchTrigrams(string pattern, bool includes)
        {
            var trigrams = toTrigrams(pattern).Where(t => !t.Contains('*'));
            var words = trigrams
                .Select(t => trigramIndex.Find(t))
                .Where(n => n != null)
                .SelectMany(n => n.words)
                .Distinct()
                .Where(w => wordMatchesPattern(w, pattern));
            Console.WriteLine(string.Join(",", words));

            return words.SelectMany(w => searchWord(w, includes)).ToList();
        }

        public List<SearchResult> Find(string word, bool includes)
        {
            return word.Contains('*') ? searchTrigrams(word, includes) : searchWord(word, includes);
        }

        public async Task<IndexResult> AddFile(string path)
        {
            Console.WriteLine("Indexing file: {0}", path);

            string rawText = await File.ReadAllTextAsync(path, Encoding.GetEncoding(1251));
            string fullText = rawText.ToLower();

            string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя-";

            var words = new Dictionary<string, int>();

            var isWord = false;
            int startIndex = 0;
            for (int endIndex = startIndex; endIndex < fullText.Length; endIndex++)
            {
                var isAlpha = alphabet.Contains(fullText[endIndex]);
                if (isAlpha && !isWord)
                {
                    startIndex = endIndex;
                    isWord = true;
                }
                if (isWord && (!isAlpha || endIndex == fullText.Length - 1))
                {
                    var word = fullText.Substring(startIndex, endIndex - startIndex);
                    word = Porter.Stem(word);
                    if (!words.ContainsKey(word)) words[word] = 0;
                    words[word] += 1;
                    isWord = false;
                }
            }

            var trigrams = new Dictionary<string, HashSet<string>>();
            foreach (var pair in words)
            {
                trigrams[pair.Key] = toTrigrams(pair.Key);
            }

            Console.WriteLine("Indexed: {0}", path);
            return new IndexResult(path, words, trigrams);
        }

        private HashSet<string> toTrigrams(string word)
        {
            var withPadding = $"$${word}$$";

            var trigrams = new HashSet<string>();
            for (int i = 0; i < withPadding.Length - 2; i++)
            {
                trigrams.Add(withPadding.Substring(i, 3));
            }
            return trigrams;
        }

        public static bool wordMatchesPattern(string word, string pattern)
        {
            var parts = pattern.Split('*', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return false;

            if (!pattern.StartsWith('*') && !word.StartsWith(parts.First())) return false;
            if (!pattern.EndsWith('*') && !word.EndsWith(parts.Last())) return false;

            int prev = 0;
            foreach (var p in parts)
            {
                int i = word.IndexOf(p, prev);
                if (i < 0) return false;
                prev = i + p.Length;
            }
            return true;
        }
    }
}
