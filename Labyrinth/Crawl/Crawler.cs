using Labyrinth.Items;
using Labyrinth.Tiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Labyrinth.Crawl
{
    public class Crawler : ICrawler
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        private Labyrinth labyrinth;

        public Direction Direction { get; }

public Tile FacingTile
{
    get
    {
        if ((this.Y == 0 && this.Direction == Direction.North) ||
            (this.X == 0 && this.Direction == Direction.West) ||
            (this.X == this.labyrinth.Width - 1 && this.Direction == Direction.East) ||
            (this.Y == this.labyrinth.Height - 1 && this.Direction == Direction.South))
        {
            return Outside.Singleton;
        }
                int targetX = this.X;
                int targetY = this.Y;

                if (this.Direction == Direction.North) targetY--;
                if (this.Direction == Direction.South) targetY++;
                if (this.Direction == Direction.East) targetX++;
                if (this.Direction == Direction.West) targetX--;
                Tile taleFaced = this.labyrinth.GetTile(targetX,targetY);

        return taleFaced;
    }
}

        public Inventory Walk()
        {
            if (!this.FacingTile.IsTraversable)
            {
                throw new InvalidOperationException("Cannot walk into a non-traversable tile");
            }
            if (this.Direction == Direction.North) this.Y--;
            if (this.Direction == Direction.South) this.Y++;
            if (this.Direction == Direction.East) this.X++;
            if (this.Direction == Direction.West) this.X--;
            return new MyInventory();
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
