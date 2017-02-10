using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject
{
    public partial class KProject : Game
    {
        private void CreateChildren(Node parent, int minChildren, int maxChildren, int level)
        {
            int numberOfChildren = Rnd.Next(minChildren, maxChildren + 1);
            for (int i = 0; i < numberOfChildren; i++)
            {
                bool nodeMade = false;
                do
                {
                    int random = Rnd.Next(1,2);
                    switch (random)
                    {
                        case 1: // rail node
                            if (railNodesMade != railNodesToMake)
                            {
                                parent.Children.Add(new Node
                                {
                                    Type = NodeType.Rail,
                                    Company = NameGenerate(1),
                                    Country = NameGenerate(2),
                                    Address = NameGenerate(3)
                                });
                                railNodesMade += 1;
                                nodeMade = true;
                            }
                            break;
                    }
                } while (!nodeMade);
            }

            foreach (Node child in parent.Children)
            {
                switch (level)
                {
                    case 0:
                        CreateChildren(child, 1, 3, level + 1);
                        break;
                    case 1:
                        CreateChildren(child, 1, 2, level + 1);
                        break;
                    default:
                        break;
                }
            }
        }

        static Random random = new Random();

        static string NameGenerate(int type)
        {
            switch (type)
            {
                case 1: //company
                    string[] company1 = { "Mac", "Sil", "A", "Cor", "Neo", "So", "Em", "Wi", "Ar", "Mo", "Nu", "Tu", "Da", "Dro", "Sim", "Pa", "Tor", "Wide", "Carl", "Free", "Jam", "Job", "Jo", "Meta", "Na", "Ro" };
                    string[] company2 = { "ro", "ver", "do", "pa", "o", "lu", "ber", "du", "no", "us", "ni", "sa", "clo", "rom", "som", "ril", "bo", "stro", "cka" };
                    string[] company3 = { "soft", "co", "com", "code", "dock", "studio", "graph", "cle", "node", "user", "hold", "pose", "rose", "to", "serve", "search", "role", "das", "tech", "worth", "year", "jo", "brier", "man", "on", "life", "beck", "son", "talk", "flow" };
                    string[] company4 = { "", "", "", "", " Corporation", " Association", " Incorporated", " Limited", " Ventures", " Capital", " Foundation", " Multimedia", " Productions", " Society", " Union", " Sciences", " Radio", " Syndicate", " Institute", " plc", " LLC", " Bank", " Group", " Partnership", " Cooperative", " Television", " Security", " Rail", " Project", " Brewery", " Healthcare", " Insurance", " Recovery", " Laboratories", " Partners", " Brands", " Systems", " Computers", " Telecommunications", " Technologies", " Broadcasting", " Reprographics", " Finance", " Power", " Energy", " Devices", " Petroleum", " Electrics", " Industries", " Automotives", " Aerospace", " International", " Pharmaceuticals", " Worldwide", " Studios" };
                    return (RandomStringFromArray(company1) + RandomStringFromArray(company2) + RandomStringFromArray(company3) + RandomStringFromArray(company4));
                case 2: //country
                    string[] nation1 = { "Me", "En", "Fra", "Ha", "Aus", "Chi", "Co", "Ge", "Gha", "Ir", "Is" };
                    string[] nation2 = { "ri", "ni", "iti", "dur", "ga", "ra", "or", "a", "e", "men", "ti", "dom", "rom", "lo", "bo", "rai", "" };
                    string[] nation3 = { "ne", "land", "stan", "thel", "ca", "ia", "tria", "ba", "ch", "lia", "ban", "pa", "ji", "ry", "dia", "el", "bia", "pal", "ria", "wan" };
                    return (RandomStringFromArray(nation1) + RandomStringFromArray(nation2) + RandomStringFromArray(nation3));
                case 3: //ip



                    int ip1, ip2, ip3, ip4, ip5;
                    do
                    {
                        ip1 = random.Next(11, 210);
                        ip2 = random.Next(8, 255);
                        ip3 = random.Next(8, 255);
                        ip4 = random.Next(11, 210);
                        ip5 = random.Next(1, 40000);
                    } while (ip1 == 192);
                    return (ip1 + "." + ip2 + "." + ip3 + "." + ip4 + ":" + ip5);
            }

            return "";
        }

        static string RandomStringFromArray(string[] strings)
        {
            return strings[random.Next(0, strings.Count())];
        }

        private void DrawTree(Node startingNode)
        {
            int treeX = 700;
            int treeY = 300;
            //spriteBatch.DrawString(menuFont, rootNode.Company, new Vector2(700, 300), Color.White);
            spriteBatch.Draw(linePixel, new Vector2(treeX, treeY), Color.White); // draw root node
            DrawNode(rootNode, new Vector2(treeX,treeY), MathHelper.ToRadians(0), MathHelper.ToRadians(360));
            //DrawLine(spriteBatch, new Vector2(700,300), new Vector2(1061,800));
        }

        private void DrawNode(Node parentNode, Vector2 centre, double angleStart, double angleRange)
        {
            int iterator = 0;
            foreach (Node node in parentNode.Children)
            {
                double angle = angleStart + (iterator * angleRange / rootNode.Children.Count);
                float nodeX = centre.X + (30f * (float)Math.Cos(angle));
                float nodeY = centre.Y + (30f * (float)Math.Sin(angle));
                spriteBatch.Draw(linePixel, new Vector2(nodeX, nodeY), Color.White);
                DrawLine(spriteBatch, new Vector2(centre.X, centre.Y), new Vector2(nodeX, nodeY));
                if (node.Children.Count > 0)
                {
                    double aO = (angleRange / rootNode.Children.Count) / 2;
                    DrawNode(node, new Vector2(nodeX, nodeY), angle - aO, angleRange / rootNode.Children.Count);
                }
                iterator++;
            }
        }

        void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            spriteBatch.Draw(linePixel, new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), 2), null, Color.White, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
