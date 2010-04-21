using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Heliopolis.Utilities
{
    class Serialization
    {
        //public static void SaveWorldToDiskBinary(GameWorld gameWorld, string filename)
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new FileStream("savegame.bin",
        //                 FileMode.Create,
        //                 FileAccess.Write, FileShare.None);
        //    formatter.Serialize(stream, gameWorld);
        //    stream.Close();
        //}

        //public static void LoadWorldFromDiskBinary(string filename)
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new FileStream("savegame.bin",
        //                              FileMode.Open,
        //                              FileAccess.Read,
        //                              FileShare.Read);
        //    GameWorld gameWorld = (GameWorld)formatter.Deserialize(stream);
        //    stream.Close();
        //    GameWorld.LoadNewGameWorld(gameWorld);
        //    Game.renderer.GameWorld = gameWorld;
        //}        
    }
}
