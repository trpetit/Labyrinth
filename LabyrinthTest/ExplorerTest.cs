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

    [Test]
    public void GetOutRaisesDirectionChangedAndPositionChangedWithCurrentState()
    {
        var crawler = new TestCrawler();
        var explorer = new Explorer(crawler, new SequenceRandom(0, 1, 2));
        var directionChanges = new List<CrawlingEventArgs>();
        var positionChanges = new List<CrawlingEventArgs>();
        explorer.DirectionChanged += (_, args) => directionChanges.Add(args);
        explorer.PositionChanged += (_, args) => positionChanges.Add(args);

        explorer.GetOut(3);

        using var all = Assert.EnterMultipleScope();
        Assert.That(directionChanges, Has.Count.EqualTo(2));
        Assert.That(directionChanges[0].Direction, Is.EqualTo(Direction.West));
        Assert.That(directionChanges[0].X, Is.EqualTo(2));
        Assert.That(directionChanges[0].Y, Is.EqualTo(3));
        Assert.That(directionChanges[1].Direction, Is.EqualTo(Direction.North));
        Assert.That(positionChanges, Has.Count.EqualTo(1));
        Assert.That(positionChanges[0].X, Is.EqualTo(2));
        Assert.That(positionChanges[0].Y, Is.EqualTo(2));
        Assert.That(positionChanges[0].Direction, Is.EqualTo(Direction.North));
    }

    private sealed class TestCrawler : ICrawler
    {
        public int X { get; private set; } = 2;
        public int Y { get; private set; } = 3;
        public Direction Direction { get; } = Direction.North;
        public Tile FacingTile { get; set; } = new Room();
        public int WalkCount { get; private set; }

        public Inventory Walk()
        {
            WalkCount++;
            X += Direction.DeltaX;
            Y += Direction.DeltaY;
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
