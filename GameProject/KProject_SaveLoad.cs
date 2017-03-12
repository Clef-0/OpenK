using Microsoft.Xna.Framework;
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
        void SaveGame()
        {
            using (Stream stream = File.Open("save.dat", FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, rootNode);
            }
        }

        void LoadGame()
        {
            using (Stream stream = File.Open("save.dat", FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                rootNode = (Node)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
