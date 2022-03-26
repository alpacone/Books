using System.Text;

namespace Books
{
    public static class Books
    {
        static string Pluralize(int num, string word)
        {
            var forms = word.Split("_");
            return (num % 10 == 1 && num % 100 != 11) ? forms[0] : (num % 10 >= 2 && num % 10 <= 4 && (num % 100 < 10 || num % 100 >= 20)) ? forms[1] : forms[2];
        }

        static void PrintDocument(string name, int occurrences)
        {
            if (occurrences == 0)
            {
                Console.Write("Запрос в файле ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(name);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(" не найден");
            }
            else
            {
                Console.Write("Запрос в файле");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" {0} ", name);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("найден");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(" {0} ", occurrences);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(Pluralize(occurrences, "раз_раза_раз"));
            }
            Console.ResetColor();
        }

        static async Task Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            string[] files = Directory.GetFiles("data", "*.txt");
            var reversedIndex = new ReversedIndex(files.ToList());
            reversedIndex.BuildIndex();
            Console.Clear();
            var tree = reversedIndex.trigramIndex;
            Console.WriteLine($"trigrams={tree.Count} depth={tree.Depth} [log2({tree.Count})={Math.Log2(tree.Count)}]");

            while (true)
            {
                Console.Write("Запрос: ");
                var s = Console.ReadLine();
                if (s == "Q") break;

                var q = new Query(s);
                Console.WriteLine(q);
                var docs = q.evaluate(reversedIndex);

                // use case и architecture

                var sortedResults = docs.OrderByDescending(item => item.occurrences);
                foreach (var res in sortedResults)
                {
                    PrintDocument(res.fileName, res.occurrences);
                }
            }
        }
    }
}