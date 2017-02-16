using Microsoft.Xna.Framework;
using MonoGame.Extended.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    enum NodeType { Rail, Cabal, Road, Hell, Net };
    enum NodeMusic { Fear };

    class Node
    {
        // instance properties

        public List<Node> Children { get; set; }
        public NodeType Type { get; set; }
        public NodeMusic Music { get; set; }
        public Color Color { get; set; }
        public Point MenuPosition { get; set; }
        public string Company { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public bool Completed { get; set; } = false;

        Dictionary<string, string> nbre = new Dictionary<string, string>();

        // instance methods

        public Node()
        {
            this.Children = new List<Node>();
            HslColor _color = new HslColor();
            _color = new HslColor(KProject.Rnd.Next(0, 256) / 256f, KProject.Rnd.Next(0, 256) / 256f, KProject.Rnd.Next(20, 200) / 256f);
            this.Color = _color.ToRgb();
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
