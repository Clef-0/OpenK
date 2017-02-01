using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    class Node
    {
        // static properties

        public enum NodeType { Rail, Cabal, Road, Hell, Net };


        // instance properties

        public List<Node> Children { get; set; }
        public NodeType Type { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        private bool Completed = false;

        Dictionary<string, string> nbre = new Dictionary<string, string>();

        // instance methods

        public Node()
        {
            this.Children = new List<Node>();

            Console.WriteLine("new node instance");
        }

        public void Complete()
        {
            this.Completed = true;
        }

        public void CreateChild(NodeType Type, string Company, string Country, string Address)
        {
            Node newChild = new Node();
            newChild.Type = Type;
            newChild.Company = Company;
            newChild.Country = Country;
            newChild.Address = Address;
            Children.Add(newChild);
        }
    }
}
