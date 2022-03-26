namespace Books
{
    public struct IndexResult
    {
        public string fileName;
        public Dictionary<string, int> occurrences;
        public Dictionary<string, HashSet<string>> trigrams;

        public IndexResult(string fileName, Dictionary<string, int> occurrences, Dictionary<string, HashSet<string>> trigrams)
        {
            this.fileName = fileName;
            this.occurrences = occurrences;
            this.trigrams = trigrams;
        }
    }
}
