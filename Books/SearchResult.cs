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
