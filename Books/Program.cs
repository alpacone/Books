using System.Text;

namespace Books
{
    public static class Books
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            string[] files = Directory.GetFiles("data", "*.txt");
            var reversedIndex = new ReversedIndex(files.ToList());
            Console.Clear();

            while (true)
            {
                Console.Write("Запрос: ");
                var s = Console.ReadLine();
                if (s == "Q") break;

                var q = new Query(s);
                var results = q.evaluate(reversedIndex).Where(item => item.occurrences != null);
                if (results.Count() > 0)
                {
                    foreach (var res in results.OrderByDescending(item => item.occurrences.Count))
                    {
                        res.Print();
                    }
                }
                else
                {
                    Console.WriteLine("По запросу '{0}' ничего не найдено", s);
                }
            }
        }
    }
}