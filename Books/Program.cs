using System.Collections.Concurrent;
using System.Text;

namespace Books
{
    enum OpType
    {
        Or,
        And,
        Leaf
    }

    struct SearchResult
    {
        public string fileName;
        public List<int> occurrences;
    }

    public static class Books
    {
        static ConcurrentDictionary<string, ConcurrentBag<SearchResult>> reversedIndex = new ConcurrentDictionary<string, ConcurrentBag<SearchResult>>();

        static async Task AddFileToIndex(string fileName)
        {
            if (!fileName.EndsWith(".txt")) return;
            Console.WriteLine("Indexing file: {0}", fileName);

            string rawText = await File.ReadAllTextAsync(fileName, Encoding.GetEncoding(1251));
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

            foreach (var item in words)
            {
                var res = new SearchResult { fileName = fileName, occurrences = item.Value };
                ConcurrentBag<SearchResult> searchResults = null;

                if (reversedIndex.TryGetValue(item.Key, out searchResults))
                {
                    searchResults.Add(res);
                }
                else
                {
                    reversedIndex.TryAdd(item.Key, new ConcurrentBag<SearchResult>() { res });
                }
            }

            Console.WriteLine("Finished: {0}", fileName);
        }

        static async Task Main(string[] args)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //var tasks = new List<Task>();
            //string[] files = Directory.GetFiles("data");
            //foreach (string file in files)
            //{
            //    tasks.Add(AddFileToIndex(file));
            //}
            //await Task.WhenAll(tasks);

            //Console.WriteLine(reversedIndex["хозяин"]);

            var q1 = new Query("a b");
            Console.WriteLine(q1);
        }
    }
}