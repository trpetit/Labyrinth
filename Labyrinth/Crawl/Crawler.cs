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

        public Crawler()
        {
            this.X = 2; this.Y = 1;
            this.Direction = Direction.North;
            this.FacingTile = Wall.Singleton;
        }
    }
}
