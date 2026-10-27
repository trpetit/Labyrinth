using Labyrinth.Items;
using Labyrinth.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Labyrinth.Crawl
{
    public class Crawler : ICrawler
    {
        public int X { get;}

        public int Y { get; }

        public Direction Direction { get; }

        public Tile FacingTile { get; }

        public Inventory Walk()
        {
            throw new NotImplementedException();
        }

        public Crawler((int,int) startPosition)
        {
            this.X = startPosition.Item1; this.Y = startPosition.Item2;
            this.Direction = Direction.North;
            this.FacingTile = Wall.Singleton;
        }
    }
}
