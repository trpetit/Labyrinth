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
                        crawler.Direction.TurnLeft();
                        break;
                    case 1:
                        crawler.Direction.TurnRight();
                        break;
                    default:
                        if (crawler.FacingTile.IsTraversable)
                        {
                            crawler.Walk();
                        }
                        else if (random.Next(2) == 0)
                        {
                            crawler.Direction.TurnLeft();
                        }
                        else
                        {
                            crawler.Direction.TurnRight();
                        }
                        break;
                }
            }
        }

        private readonly ICrawler crawler;
        private readonly Random random;
    }
}
