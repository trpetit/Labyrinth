using Labyrinth.Crawl;
using Labyrinth.Items;
using Labyrinth.Tiles;

namespace LabyrinthTest.Crawl;

[TestFixture]
public class ExplorerTest
{
    [Test]
    public void GetOutStopsAfterMaximumNumberOfActions()
    {
        var crawler = new TestCrawler();
        var explorer = new Explorer(crawler, new SequenceRandom(0, 1, 0));

        explorer.GetOut(3);

        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
        Assert.That(crawler.WalkCount, Is.Zero);
    }

    [Test]
    public void GetOutStopsWhenCrawlerFindsOutside()
    {
        var crawler = new TestCrawler();
        var explorer = new Explorer(crawler, new SequenceRandom(2, 0));

        explorer.GetOut(10);

        Assert.That(crawler.WalkCount, Is.EqualTo(1));
        Assert.That(crawler.Direction, Is.EqualTo(Direction.North));
    }

    [Test]
    public void GetOutTurnsInsteadOfWalkingIntoNonTraversableTile()
    {
        var crawler = new TestCrawler { FacingTile = Wall.Singleton };
        var explorer = new Explorer(crawler, new SequenceRandom(2, 0));

        explorer.GetOut(1);

        Assert.That(crawler.WalkCount, Is.Zero);
        Assert.That(crawler.Direction, Is.EqualTo(Direction.West));
    }

    [Test]
    public void GetOutDoesNothingWhenAlreadyFacingOutside()
    {
        var crawler = new TestCrawler { FacingTile = Outside.Singleton };
        var explorer = new Explorer(crawler, new SequenceRandom());

        explorer.GetOut(10);

        Assert.That(crawler.WalkCount, Is.Zero);
        Assert.That(crawler.Direction, Is.EqualTo(Direction.North));
    }

    private sealed class TestCrawler : ICrawler
    {
        public int X => 0;
        public int Y => 0;
        public Direction Direction { get; } = Direction.North;
        public Tile FacingTile { get; set; } = new Room();
        public int WalkCount { get; private set; }

        public Inventory Walk()
        {
            WalkCount++;
            FacingTile = Outside.Singleton;
            return new MyInventory();
        }
    }

    private sealed class SequenceRandom(params int[] values) : Random
    {
        private int index;

        public override int Next(int maxValue) => values[index++] % maxValue;
    }
}
