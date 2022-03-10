﻿using System.Collections.Concurrent;
using System.Text;

namespace Books
{
    public class ReversedIndex
    {
        List<string> fileNames;
        ConcurrentDictionary<string, ConcurrentDictionary<string, List<int>>> storage = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<int>>>();

        public ReversedIndex(List<string> fileNames)
        {
            this.fileNames = fileNames;
        }

        public async Task BuildIndex()
        {
            var tasks = new List<Task>();
            foreach (string name in fileNames)
            {
                tasks.Add(AddFile(name));
            }
            await Task.WhenAll(tasks);
        }

        public List<SearchResult> searchWord(string word, bool includes)
        {
            ConcurrentDictionary<string, List<int>> wordInFiles = null;
            var res = new List<SearchResult>();

            if (!storage.TryGetValue(word.ToLower(), out wordInFiles))
            {
                wordInFiles = new ConcurrentDictionary<string, List<int>>();
                foreach (var name in fileNames)
                {
                    wordInFiles.TryAdd(name, null);
                }
            }

            foreach (var fileOcc in wordInFiles)
            {
                if ((fileOcc.Value != null) == includes)
                {
                    res.Add(new SearchResult(fileOcc.Key, word, fileOcc.Value));
                }
            }
            return res;
        }

        ConcurrentDictionary<string, List<int>> AddFilesRow(string word)
        {
            return storage.GetOrAdd(word, key =>
            {
                Dictionary<string, List<int>> dict = fileNames.ToDictionary(x => x, x => (List<int>)null);
                return new ConcurrentDictionary<string, List<int>>(dict);
            });
        }

        public async Task AddFile(string path)
        {
            Console.WriteLine("Indexing file: {0}", path);

            string rawText = await File.ReadAllTextAsync(path, Encoding.GetEncoding(1251));
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
                    word = Porter.Stem(word);
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
                var occurrences = AddFilesRow(item.Key);
                occurrences.AddOrUpdate(path, item.Value, (key, oldValue) => item.Value);
            }
            Console.WriteLine("Indexed: {0}", path);
        }
    }
}
