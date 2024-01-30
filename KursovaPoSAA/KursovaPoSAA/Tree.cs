using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovaPoSAA
{
    public class Tree
    {
        private CStack<string> commands = new CStack<string>(); // saving commands so we know the last command
        private CQueue<string> currentData = new CQueue<string>(); // saving data until a command is read
        private CQueue<string> bracketData = new CQueue<string>(); // saving dava if we found brackets
        private Table table;
        public Node? Root { get; set; }

        public Tree(Table table)
        {
            Root = null;
            this.table = table;
        }
         
        public Tree ConstructTree(CQueue<string> query)
        {
            while (query.Any())
            {
                string data = query.Dequeue();

                if (data.Contains('('))
                {
                    data = data.TrimStart('(');
                    bracketData.Enqueue(data);

                    while (!data.Contains(')'))
                    {
                        data = query.Dequeue();
                        bracketData.Enqueue(data.TrimEnd(')'));
                    }
                    continue;
                }

                if (data.ToLower() == "not")
                    data = ChangeSymbol(query.Dequeue());

                if (isCommand(data))
                {
                    if (Root is null)
                        Root = new Node(data);

                    else if (Root.Children.Count == 0)
                    {
                        Root.AddChild(data);
                        if (!bracketData.Any())
                            Root.Children[0].AddChild(currentData.Dequeue());
                    }
                    else // Root already has commands in it, so traverse the tree until the last command from the stack is found
                    {
                        Node? node = new Node();
                        foreach (Node child in Root.Children)
                        {
                            node = child.Find(commands.Peek());
                        }
                        if (node.Data == data) //if last command matches the current one
                        {
                            if (currentData.Any())
                                node.AddChild(currentData.Dequeue());
                        }
                        else
                        {
                            node.AddChild(data);
                            if (currentData.Any())
                                node.Children[node.Children.Count - 1].AddChild(currentData.Dequeue());
                        }
                    }
                    commands.Push(data);
                }
                else
                {
                    currentData.Enqueue(data);
                }
            }

            //if current data has any data, place it in the last command node
        
            Node? lastNode = new Node();
            if (Root is null)
                throw new InvalidOperationException("Tree is empty");

            if (Root.Data == commands.Peek())
                lastNode = Root;
            else
                foreach (Node child in Root.Children)
                    lastNode = child.Find(commands.Peek());

            if (currentData.Any())
                lastNode.AddChild(currentData.Dequeue());

            //if bracketData has any, repeat the process but without pushing the command into the stack

            while (bracketData.Any())
            {
                string word = bracketData.Dequeue();
                if (isCommand(word))
                {
                    if (word.ToLower() == "not")
                        word = ChangeSymbol(query.Dequeue());

                    if (!currentData.Any())
                        lastNode.AddChild(bracketData.Dequeue());
                    else
                    {
                        lastNode.AddChild(word);
                        int pos = lastNode.Children.Count - 1;


                        lastNode.Children[pos].AddChild(currentData.Dequeue());
                        lastNode.Children[pos].AddChild(bracketData.Dequeue());
                    }

                }
                else
                    currentData.Enqueue(word);

            }
            return this;
        }

        private static bool isCommand(string input)
        {
            string[] keywords = { "WHERE", "AND", "OR", "Where", "And", "Or" };
            return keywords.Contains(input);
        }
        private static string ChangeSymbol(string input)
        {
            string[] condition = input.Split();
            string param = condition[0];
            string op = condition[1];
            string comparator = condition[2];

            switch (op)
            {
                case "<":
                    {
                        op = ">";
                        break;
                    }
                case ">":
                    {
                        op = "<";
                        break;
                    }
                case "<>":
                    {
                        op = "=";
                        break;
                    }
                case "=":
                    {
                        op = "<>";
                        break;
                    }
                case ">=":
                    {
                        op = "<=";
                        break;
                    }
                case "<=":
                    {
                        op = ">=";
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Used an unsupported comparison sign!");
                    }
            }
            return $"{param} {op} {comparator}";
        }
        private List<object> CollectLeafData(Node node)
        {
            List<object> leafData = new List<object>();

            foreach (var child in node.Children)
            {
                if (child.Children is null || child.Children.Count == 0) // Leaf node
                {
                    if(child.Data is not null)
                        leafData.Add(child.Data);

                    if (child.Properties is not null)
                        leafData.Add(child.Properties);
                }
                else
                {
                    leafData.AddRange(CollectLeafData(child));
                }
            }

            return leafData;
        }
        private void ProcessNode(Node node, string operation)
        {
            foreach (var child in node.Children)
            {
                ProcessNode(child, operation);
            }

            if (node.Data.Equals("And", StringComparison.OrdinalIgnoreCase))
            {
                List<object> leafData = CollectLeafData(node);
                List<Property> prop = table.And(leafData);
                //Console.WriteLine($"Processing 'And' node with data: {string.Join(", ", leafData)}");
                node.Children.Clear();
                node.AddChild(prop);
            }
            else if (node.Data.Equals("Or", StringComparison.OrdinalIgnoreCase))
            {
                List<object> leafData = CollectLeafData(node);
                List<Property> prop = table.Or(leafData);
                //Console.WriteLine($"Processing 'Or' node with data: {string.Join(", ", leafData)}");
                node.Children.Clear();
                node.AddChild(prop);
            }
            else if (node.Data.Equals("Where", StringComparison.OrdinalIgnoreCase))
            {
                List<object> leafData = CollectLeafData(node);
                //Console.WriteLine($"Processing 'Or' node with data: {string.Join(", ", leafData)}");
                node.Children.Clear();
                table = table.Where(leafData[0], operation);
            }
            //node.Children.Clear();
        }
        public Table ProcessTree(string operation)
        {
            if (Root is null)
                throw new InvalidOperationException();

            ProcessNode(Root, operation);

            return table;
        }
    }
}
