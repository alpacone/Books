using System.Text;

namespace Books
{
    public class ReversedIndex
    {
        List<string> fileNames;
        public BinarySearchTree storage = new BinarySearchTree();

        public ReversedIndex(List<string> fileNames)
        {
            this.fileNames = fileNames;
        }

        public void BuildIndex()
        {
            var tasks = new List<Task<Dictionary<string, int>>>();
            foreach (string name in fileNames)
            {
                tasks.Add(AddFile(name));
            }
            Task.WaitAll(tasks.ToArray());

            for (int i = 0; i < tasks.Count; i++)
            {
                var t = tasks[i];
                var name = fileNames[i];
                foreach (var item in t.Result)
                {
                    var n = AddFilesRow(item.Key);
                    n.books[name] = item.Value;
                }
            }
        }

        public List<SearchResult> searchWord(string word, bool includes)
        {
            var results = new Dictionary<string, int>();

            var node = storage.Find(word.ToLower());
            if (node != null) results = node.books;

            foreach (var name in fileNames)
            {
                if (!results.ContainsKey(name)) results.Add(name, 0);
            }

            var res = new List<SearchResult>();
            foreach (var sr in results)
            {
                if ((sr.Value != 0) == includes)
                {
                    res.Add(new SearchResult(sr.Key, sr.Value));
                }
            }
            return res;
        }

        Node AddFilesRow(string word)
        {
            var n = storage.Find(word);
            if (n == null) n = storage.Add(word);
            return n;
        }

        public async Task<Dictionary<string, int>> AddFile(string path)
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

            Console.WriteLine("Indexed: {0}", path);
            return words;
        }
    }
}
