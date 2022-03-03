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
            this.term = term;
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
                return clauses.SelectMany(c => c.evaluate(reversedIndex)).Distinct().ToList();
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

        public List<SearchResult> IntersectAll(IEnumerable<List<SearchResult>> lists)
        {
            var results = new List<SearchResult>(lists.First());
            var files = new HashSet<string>(results.Select(res => res.fileName));

            foreach (var list in lists.Skip(1))
            {
                foreach (var sr in list)
                {
                    if (files.Contains(sr.fileName))
                    {
                        results.Add(sr);
                    }
                }
            }
            return results.Distinct().ToList();
        }
    }
}
