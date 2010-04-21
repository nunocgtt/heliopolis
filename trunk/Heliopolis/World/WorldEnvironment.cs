using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Heliopolis.Engine;

namespace Heliopolis.World
{
    public class WorldEnviroment : IIsometricTileProvider, IWorld
    {
        public int[,] LevelInfo { get; set; }
        public Point WorldSize { get; set; }
        Random blahRand = new Random(123123);

        public WorldEnviroment(Point worldSize)
        {
            this.WorldSize = worldSize;
            LevelInfo = new int[WorldSize.X, WorldSize.Y];
            Enumerable.Range(0, 
                WorldSize.X).ToList().ForEach(p =>
                Enumerable.Range(0, WorldSize.Y).ToList().ForEach(q => LevelInfo[p, q] = getRandomTile()));
        }

        private int getRandomTile()
        {
            int randAmount = blahRand.Next(10);
            if (randAmount == 9)
                return 2;
            else if (randAmount > 7)
                return 1;
            else
                return 0;
        }

        private int levelInfoByPoint(Point position)
        {
            return LevelInfo[position.X, position.Y];
        }

        public List<string> GetTexturesToDraw(Point position)
        {
            List<string> returnMe = new List<string>();
            if (levelInfoByPoint(position) == 0)
            {
                returnMe.Add("grass");
            }
            else if (levelInfoByPoint(position) == 1)
            {
                returnMe.Add("rock");
            }
            else
            {
                returnMe.Add("grass");
                returnMe.Add("tree1");
            }
            return returnMe;
        }
    }
}
