using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursovaPoSAA
{
    public class Node
    {
        public string Data { get; set; }
        public List<Node> Children { get; set; }
        public List<Property> Properties { get; set; }

        public Node(string Data)
        {
            this.Data = Data;
            Children = new List<Node>();
        }
        public Node(List<Property> Properties)
        {
            this.Properties = Properties;
        }
        public Node()
        {
            Data = string.Empty;
            Children = new List<Node>();
        }

        public void AddChild(string child)
        {
            Children.Add(new Node(child));
        }
        public void AddChild(List<Property> properties)
        {
            Children.Add(new Node(properties));
        }

        public Node? Find(string stringToFind)
        {
            // find the string, starting with the current instance
            return Find(this, stringToFind);
        }
        public Node? Find(Node node, string stringToFind)
        {
            if (node.Data.Contains(stringToFind))
                return node;

            foreach (var child in node.Children)
            {
                var result = Find(child, stringToFind);
                if (result is not null)
                    return result;
            }

            return null;
        }

    }
}
