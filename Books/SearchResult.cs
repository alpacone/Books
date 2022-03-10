namespace Books
{
    public struct SearchResult
    {
        public string fileName;
        public int occurrences;

        public SearchResult(string fileName, int occurrences)
        {
            this.fileName = fileName;
            this.occurrences = occurrences;
        }
    }
}
