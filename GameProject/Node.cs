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

    [Serializable()]
    class Node
    {
        // instance properties

        public List<Node> Children { get; set; }
        public NodeType Type { get; set; }
        public NodeMusic Music { get; set; }
        private int ColourR;
        private int ColourG;
        private int ColourB;
        private int ColourA;
        private int MenuPositionX;
        private int MenuPositionY;
        public string Company { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public bool Completed { get; set; }
        public decimal Percentage { get; set; }

        // instance methods

        public Node()
        {
            this.Completed = false;
            this.Children = new List<Node>();
            HslColor _color = new HslColor();
            _color = new HslColor(KProject.Rnd.Next(0, 256) / 256f, KProject.Rnd.Next(0, 256) / 256f, KProject.Rnd.Next(20, 200) / 256f);
            this.Colour = _color.ToRgb();
            Console.WriteLine("new node instance");
        }

        public Color Colour
        {
            get {
                return new Color(ColourR,ColourG,ColourB,ColourA);
            }
            set
            {
                ColourR = value.R;
                ColourG = value.G;
                ColourB = value.B;
                ColourA = value.A;
            }
        }

        public Point MenuPosition
        {
            get
            {
                return new Point(MenuPositionX,MenuPositionY);
            }
            set
            {
                MenuPositionX = value.X;
                MenuPositionY = value.Y;
            }
        }
    }
}
