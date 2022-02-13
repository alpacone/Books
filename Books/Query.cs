namespace Books
{
    public class Query
    {
        OpType opType;
        string term;
        bool isNegated = false;
        List<Query> clauses = new List<Query>();

        public Query() { this.opType = OpType.And; }

        public Query(string term, bool isNegated)
        {
            this.opType = OpType.Leaf;
            this.term = term;
            this.isNegated = isNegated;
        }

        public Query(string query)
        {
            this.opType = OpType.Or;
            string[] parts = splitString(query, new string[] { " OR " });
            foreach (string part in parts)
            {
                clauses.Add(Query.and(part));
            }
        }

        public static Query and(string query)
        {
            var and = new Query();

            bool prevWasOp = false;
            string[] tokens = splitString(query, new string[] { " " });
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "NOT")
                {
                    and.clauses.Add(new Query(tokens[i + 1], true));
                    i++;
                    prevWasOp = true;
                }
                else if (tokens[i] == "AND")
                {
                    prevWasOp = true;
                }
                else if (prevWasOp || i == 0)
                {
                    and.clauses.Add(new Query(tokens[i], false));
                    prevWasOp = false;
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
            if (this.opType == OpType.Or || this.opType == OpType.And)
            {
                return string.Format(this.opType == OpType.Or ? "OR({0})" : "AND({0})", string.Join(", ", this.clauses));
            }
            else if (this.opType == OpType.Leaf)
            {
                return string.Format(isNegated ? "NOT({0})" : "{0}", this.term);
            }
            else
            {
                throw new Exception("Not supported opType");
            }

        }
    }
}
