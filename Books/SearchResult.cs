namespace Books
{
    public struct SearchResult : IEquatable<SearchResult>
    {
        public string fileName;
        public string term;
        public List<int> occurrences;

        public SearchResult(string fileName, string term, List<int> occurrences)
        {
            this.fileName = fileName;
            this.term = term;
            this.occurrences = occurrences;
        }

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Слово '{0}' в файле '{1}'", term, fileName);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("На позициях: {0}", string.Join(",", occurrences));
            Console.ResetColor();
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is SearchResult)) return false;

            var sr = (SearchResult)obj;
            return fileName == sr.fileName && term == sr.term;
        }

        public override int GetHashCode()
        {
            int hashFileName = fileName.GetHashCode();
            int hashTerm = term.GetHashCode();

            return hashFileName ^ hashTerm;
        }

        public bool Equals(SearchResult other)
        {
            return fileName == other.fileName && term == other.term;
        }
    }
}
