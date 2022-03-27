using Books;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public void TestQueryParsing()
        {
            var q1 = new Query("a AND b OR c AND d");
            Assert.AreEqual(q1.ToString(), "OR(AND(a, b), AND(c, d))");

            var q2 = new Query("NOT a AND b OR NOT c AND d");
            Assert.AreEqual(q2.ToString(), "OR(AND(NOT(a), b), AND(NOT(c), d))");

            var q3 = new Query("NOT a");
            Assert.AreEqual(q3.ToString(), "OR(AND(NOT(a)))");

            var q4 = new Query("a AND b");
            Assert.AreEqual(q4.ToString(), "OR(AND(a, b))");

            var q5 = new Query("NOT a AND b");
            Assert.AreEqual(q5.ToString(), "OR(AND(NOT(a), b))");

            var q6 = new Query("NOT a AND NOT b");
            Assert.AreEqual(q6.ToString(), "OR(AND(NOT(a), NOT(b)))");

            var q7 = new Query("a AND NOT b");
            Assert.AreEqual(q7.ToString(), "OR(AND(a, NOT(b)))");

            var q8 = new Query("a b");
            Assert.AreEqual(q8.ToString(), "OR(AND(a, b))");

            var q9 = new Query("NOT a b");
            Assert.AreEqual(q9.ToString(), "OR(AND(NOT(a), b))");

            var q10 = new Query("NOT a NOT b");
            Assert.AreEqual(q10.ToString(), "OR(AND(NOT(a), NOT(b)))");

            var q11 = new Query("a NOT b");
            Assert.AreEqual(q11.ToString(), "OR(AND(a, NOT(b)))");

            var q12 = new Query("a OR b OR c OR d");
            Assert.AreEqual(q12.ToString(), "OR(AND(a), AND(b), AND(c), AND(d))");

            var q13 = new Query("NOT a AND b OR c AND NOT d");
            Assert.AreEqual(q13.ToString(), "OR(AND(NOT(a), b), AND(c, NOT(d)))");
        }

        [TestMethod]
        public void TestWordPattern()
        {
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("дом", "д*м"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("дым", "д*м"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("доход", "д*м"), false);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("доход", "х*д"), false);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("доход", "д*о"), false);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("доход", "д*д"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("весна", "ве*сна"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("весна", "вес*"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("весна", "*сна"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("лилипут", "ли*ут"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("лилипут", "ли*ли*ут"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("лилипут", "*ли*"), true);
            Assert.AreEqual(ReversedIndex.wordMatchesPattern("лилипутов", "ли*ут"), false);
        }
    }
}