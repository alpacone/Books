using System.Text;

namespace Books
{
    public class ReversedIndex
    {
        List<string> fileNames;
        Dictionary<string, List<List<int>>> storage = new Dictionary<string, List<List<int>>>();

        public ReversedIndex(List<string> fileNames)
        {
            this.fileNames = fileNames;
            foreach (string name in fileNames)
            {
                AddFile(name);
            }
        }

        public List<SearchResult> searchWord(string word, bool includes)
        {
            var wordInFiles = new List<List<int>>();
            var res = new List<SearchResult>();

            if (!storage.TryGetValue(word.ToLower(), out wordInFiles))
            {
                wordInFiles = Enumerable.Repeat<List<int>>(null, fileNames.Count).ToList();
            }

            for (int i = 0; i < wordInFiles.Count; i++)
            {
                if ((wordInFiles[i] != null) == includes)
                {
                    res.Add(new SearchResult(fileNames[i], word, wordInFiles[i]));
                }
            }
            return res;
        }

        List<List<int>> AddFilesRow(string word)
        {
            if (storage.ContainsKey(word)) return storage[word];
            List<List<int>> row = Enumerable.Repeat<List<int>>(null, fileNames.Count).ToList();
            storage.Add(word, row);
            return row;
        }

        public void AddFile(string path)
        {
            Console.WriteLine("Indexing file: {0}", path);

            string rawText = File.ReadAllText(path, Encoding.GetEncoding(1251));
            string fullText = rawText.ToLower();

            string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя-";

            var words = new Dictionary<string, List<int>>();

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
                    if (words.ContainsKey(word))
                    {
                        words[word].Add(startIndex);
                    }
                    else
                    {
                        words[word] = new List<int>() { startIndex };
                    }
                    isWord = false;
                }
            }

            var fileIndex = fileNames.IndexOf(path);
            foreach (var item in words)
            {
                var occurrences = AddFilesRow(item.Key);
                occurrences[fileIndex] = item.Value;
            }

            Console.WriteLine("Indexed: {0}", path);
        }
    }
}
