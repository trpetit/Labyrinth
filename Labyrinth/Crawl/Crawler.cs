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

        private Labyrinth labyrinth;

        public Direction Direction { get; }

        public Tile FacingTile
        {
            get
            {
                if (this.Y == 0 && this.Direction.Equals(Direction.North))
                {
                    return Outside.Singleton;
                }
                if(this.X == 0 && this.Direction.Equals(Direction.West))
                { return Outside.Singleton;
                }
                if( this.X == this.labyrinth.Width-1 && this.Direction.Equals(Direction.East)){
                    return Outside.Singleton;
                }

                return Wall.Singleton;
            }
        }

        public Inventory Walk()
        {
            throw new NotImplementedException();
        }

        public Crawler((int,int) startPosition)
        {
            this.X = startPosition.Item1; this.Y = startPosition.Item2;
            this.Direction = Direction.North;
            //this.FacingTile = Wall.Singleton;
        }

        public Crawler(Labyrinth labyrinth, (int,int) startPosition)
        {
            this.X = startPosition.Item1; this.Y = startPosition.Item2;
            this.Direction = Direction.North;
            this.labyrinth = labyrinth;
        }
    }
}
