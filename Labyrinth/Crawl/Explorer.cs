using Labyrinth.Tiles;

namespace Labyrinth.Crawl
{
    /// <summary>
    /// Randomly explores a labyrinth using a crawler.
    /// </summary>
    public class Explorer
    {
        public Explorer(ICrawler crawler, Random? random = null)
        {
            ArgumentNullException.ThrowIfNull(crawler);
            this.crawler = crawler;
            this.random = random ?? Random.Shared;
        }

        public event EventHandler<CrawlingEventArgs>? PositionChanged;

        public event EventHandler<CrawlingEventArgs>? DirectionChanged;

        /// <summary>
        /// Performs up to <paramref name="n"/> random actions, stopping when an exit is found.
        /// </summary>
        public void GetOut(int n)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(n);

            for (var action = 0; action < n && crawler.FacingTile is not Outside; action++)
            {
                switch (random.Next(3))
                {
                    case 0:
                        TurnLeft();
                        break;
                    case 1:
                        TurnRight();
                        break;
                    default:
                        if (crawler.FacingTile.IsTraversable)
                        {
                            crawler.Walk();
                            PositionChanged?.Invoke(this, GetCurrentPosition());
                        }
                        else if (random.Next(2) == 0)
                        {
                            TurnLeft();
                        }
                        else
                        {
                            TurnRight();
                        }
                        break;
                }
            }
        }

        private void TurnLeft()
        {
            crawler.Direction.TurnLeft();
            DirectionChanged?.Invoke(this, GetCurrentPosition());
        }

        private void TurnRight()
        {
            crawler.Direction.TurnRight();
            DirectionChanged?.Invoke(this, GetCurrentPosition());
        }

        private CrawlingEventArgs GetCurrentPosition() =>
            new(crawler.X, crawler.Y, crawler.Direction);

        private readonly ICrawler crawler;
        private readonly Random random;
    }
}
