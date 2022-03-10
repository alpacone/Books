namespace Books
{
    enum OpType
    {
        Or,
        And,
        Leaf
    }

    public class Query
    {
        OpType opType;
        string term;
        bool isNegated = false;
        List<Query> clauses = new List<Query>();

        public Query() { opType = OpType.And; }

        public Query(string term, bool isNegated)
        {
            opType = OpType.Leaf;
            this.term = Porter.Stem(term);
            this.isNegated = isNegated;
        }

        public Query(string query)
        {
            opType = OpType.Or;
            string[] parts = splitString(query, new string[] { " OR " });
            foreach (string part in parts)
            {
                clauses.Add(Query.and(part));
            }
        }

        public static Query and(string query)
        {
            var and = new Query();

            string[] tokens = splitString(query, new string[] { " " });
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "NOT")
                {
                    and.clauses.Add(new Query(tokens[i + 1], true));
                    i++;
                }
                else if (tokens[i] != "AND")
                {
                    and.clauses.Add(new Query(tokens[i], false));
                }
            }
            return and;
        }

        static string[] splitString(string str, string[] separators)
        {
            return str.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public override string ToString()
        {
            if (opType == OpType.Or || opType == OpType.And)
            {
                return string.Format(opType == OpType.Or ? "OR({0})" : "AND({0})", string.Join(", ", clauses));
            }
            else if (opType == OpType.Leaf)
            {
                return string.Format(isNegated ? "NOT({0})" : "{0}", term);
            }
            else
            {
                throw new Exception("Unsupported opType");
            }
        }

        public List<SearchResult> evaluate(ReversedIndex reversedIndex)
        {
            if (opType == OpType.Or)
            {
                var listOfLists = clauses.Select(c => c.evaluate(reversedIndex));
                return UnionAll(listOfLists);
            }
            else if (opType == OpType.And)
            {
                var listOfLists = clauses.Select(c => c.evaluate(reversedIndex));
                return IntersectAll(listOfLists);
            }
            else if (opType == OpType.Leaf)
            {
                return reversedIndex.searchWord(term, !isNegated);
            }
            else
            {
                throw new Exception("Unsupported opType");
            }
        }

        public List<SearchResult> UnionAll(IEnumerable<List<SearchResult>> listOfLists)
        {
            var docs = new Dictionary<string, int>();
            foreach (var list in listOfLists)
            {
                foreach (var sr in list)
                {
                    if (!docs.ContainsKey(sr.fileName)) docs[sr.fileName] = 0;
                    docs[sr.fileName] += sr.occurrences;
                }
            }
            return docs.Select(x => new SearchResult(x.Key, x.Value)).ToList();
        }

        public List<SearchResult> IntersectAll(IEnumerable<List<SearchResult>> listOfLists)
        {
            listOfLists.OrderBy(x => x.Count);

            var firstList = listOfLists.First();
            var docs = new Dictionary<string, int>();
            foreach (var sr in firstList)
            {
                docs[sr.fileName] = sr.occurrences;
            }

            foreach (var list in listOfLists.Skip(1))
            {
                foreach (var sr in list)
                {
                    if (docs.ContainsKey(sr.fileName)) docs[sr.fileName] += sr.occurrences;
                }
                foreach (var sr in firstList)
                {
                    if (list.All(x => x.fileName != sr.fileName)) docs.Remove(sr.fileName);
                }
            }

            return docs.Select(x => new SearchResult(x.Key, x.Value)).ToList();
        }
    }
}
