namespace Books
{
    public enum Color
    {
        Red,
        Black
    }

    public class Node
    {
        public string key;
        public Color color;
        public Node parent;
        public Node left;
        public Node right;
        public HashSet<string> words = new HashSet<string>();

        public static Node CreateLeaf(Node parent)
        {
            var n = new Node(null)
            {
                color = Color.Black,
                parent = parent
            };
            return n;
        }

        public static Node CreateNode(string key)
        {
            var n = new Node(key);

            var leftNode = new Node(null);
            leftNode.color = Color.Black;
            leftNode.parent = n;

            var rightNode = new Node(null);
            rightNode.color = Color.Black;
            rightNode.parent = n;

            n.left = leftNode;
            n.right = rightNode;
            return n;
        }

        internal Node(string key)
        {
            this.key = key;
        }
    }

    public class BinarySearchTree
    {
        private Node root = null;

        private int cmpStr(string a, string b)
        {
            return String.Compare(a, b, StringComparison.InvariantCulture);
        }

        private bool isNilNode(Node n)
        {
            return n == null || (n.key == null && n.color == Color.Black && n.left == null && n.right == null);
        }

        private void PrintTree(Node node)
        {
            if (isNilNode(node)) return;

            if (!isNilNode(node.left))
            {
                Console.WriteLine($"{node.left.parent.key} --> {node.left.key}");
                PrintTree(node.left);
            }

            if (!isNilNode(node.right))
            {
                Console.WriteLine($"{node.right.parent.key} --> {node.right.key}");
                PrintTree(node.right);
            }
        }

        public void Print()
        {
            PrintTree(root);
        }

        public int Depth
        {
            get { return FindHeight(root); }
        }

        public int Count
        {
            get { return FindCount(root); }
        }

        private int FindCount(Node node)
        {
            if (isNilNode(node)) return 0;
            return 1 + FindCount(node.left) + FindCount(node.right);
        }

        private int FindHeight(Node node)
        {
            if (isNilNode(node)) return -1;
            var leftH = FindHeight(node.left);
            var rightH = FindHeight(node.right);

            return (leftH > rightH ? leftH : rightH) + 1;
        }

        public Node Find(string key)
        {
            var n = root;
            while (n != null)
            {
                int r = cmpStr(key, n.key);
                if (r < 0)
                {
                    n = n.left;
                }
                else if (r > 0)
                {
                    n = n.right;
                }
                else
                {
                    return n;
                }
            }
            return null;
        }

        private void RotateRight(Node node)
        {
            var y = node.left;

            if (isNilNode(y.right))
            {
                node.left = Node.CreateLeaf(node);
            }
            else
            {
                node.left = y.right;
            }

            if (!isNilNode(y.right))
            {
                y.right.parent = node;
            }
            y.parent = node.parent;
            if (isNilNode(node.parent))
            {
                root = y;
            }
            else
            {
                if (node == node.parent.right)
                {
                    node.parent.right = y;
                }
                else
                {
                    node.parent.left = y;
                }
            }
            y.right = node;
            node.parent = y;
        }

        private void RotateLeft(Node node)
        {
            var y = node.right;

            if (isNilNode(y.left))
            {
                node.right = Node.CreateLeaf(node);
            }
            else
            {
                node.right = y.left;
            }

            if (!isNilNode(y.left))
            {
                y.left.parent = node;
            }
            y.parent = node.parent;
            if (isNilNode(node.parent))
            {
                root = y;
            }
            else
            {
                if (node == node.parent.left)
                {
                    node.parent.left = y;
                }
                else
                {
                    node.parent.right = y;
                }
            }
            y.left = node;
            node.parent = y;
        }

        public Node Add(string key)
        {
            Node parent = null;
            var x = root;
            var newNode = Node.CreateNode(key);
            if (root == null)
            {
                root = newNode;
                newNode.color = Color.Black;
            }
            else
            {
                while (!isNilNode(x))
                {
                    parent = x;
                    int r = cmpStr(key, x.key);
                    if (r == 0)
                    {
                        return x;
                    }
                    else if (r < 0)
                    {
                        x = x.left;
                    }
                    else
                    {
                        x = x.right;
                    }
                }

                if (cmpStr(key, parent.key) < 0)
                {
                    parent.left = newNode;
                }
                else
                {
                    parent.right = newNode;
                }

                newNode.parent = parent;
                newNode.left = Node.CreateLeaf(newNode);
                newNode.right = Node.CreateLeaf(newNode);
                newNode.color = Color.Red;
                Balance(newNode);
            }

            return newNode;
        }

        private void Balance(Node node)
        {
            while (node.parent != null && node.parent.color == Color.Red)
            {
                if (node.parent == node.parent.parent.left)
                {
                    var uncle = node.parent.parent.right;

                    if (uncle != null && uncle.color == Color.Red)
                    {
                        node.parent.color = Color.Black;
                        uncle.color = Color.Black;
                        node.parent.parent.color = Color.Red;
                        node = node.parent.parent;
                        continue;
                    }
                    if (node == node.parent.right)
                    {
                        node = node.parent;
                        RotateLeft(node);
                    }
                    node.parent.color = Color.Black;
                    node.parent.parent.color = Color.Red;
                    RotateRight(node.parent.parent);
                }
                else
                {
                    var uncle = node.parent.parent.left;
                    if (uncle != null && uncle.color == Color.Red)
                    {
                        node.parent.color = Color.Black;
                        uncle.color = Color.Black;
                        node.parent.parent.color = Color.Red;
                        node = node.parent.parent;
                        continue;
                    }
                    if (node == node.parent.left)
                    {
                        node = node.parent;
                        RotateRight(node);
                    }
                    node.parent.color = Color.Black;
                    node.parent.parent.color = Color.Red;
                    RotateLeft(node.parent.parent);
                }
            }
            root.color = Color.Black;
        }
    }
}
