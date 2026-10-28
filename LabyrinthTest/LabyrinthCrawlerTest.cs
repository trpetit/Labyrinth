using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Items;
using Labyrinth.Tiles;

namespace LabyrinthTest.Crawl;

[TestFixture(Description = "Integration test for the crawler implementation in the labyrinth")]
public class LabyrinthCrawlerTest
{
    #region Initialization
    [Test]
    public void InitWithCenteredX()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                | x|
                +--+
                """));
        var test = laby.NewCrawler();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(1));
        Assert.That(test.Direction, Is.EqualTo(Direction.North));
        Assert.That(test.FacingTile, Is.TypeOf<Wall>());
    }

    [Test]
    public void InitWithMultipleXUsesLastOne()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +---+
                |x  |
                |  x|
                +---+
                """));
        var test = laby.NewCrawler();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(3));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.North));
    }

    [Test]
    public void ParserRaisesStartPositionFoundForEachX()
    {
        var parser = new AsciiParser("""
                +---+
                |x  |
                |  x|
                +---+
                """);
        var positions = new List<(int X, int Y)>();
        parser.StartPositionFound += (_, args) => positions.Add((args.X, args.Y));

        parser.Build();

        Assert.That(positions, Is.EqualTo(new[] { (1, 1), (3, 2) }));
    }

    [Test]
    public void InitWithNoXThrowsArgumentException()
    {
        var parser = new AsciiParser("""
                +---+
                |   |
                |   |
                +---+
                """);

        Assert.Throws<ArgumentException>(() => new Labyrinth.Labyrinth(parser));
    }
    #endregion
     
    #region Labyrinth borders
    [Test]
    public void FacingNorthOnUpperTileReturnsOutside()
    {
        var parser = new AsciiParser("""
                x--+
                |  |
                +--+
                """);
        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        Assert.That(crawler.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingWestOnFarLeftTileReturnsOutside()
    {
        var parser = new AsciiParser("""
                +---+
                x   |
                +---+
                """);
        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        crawler.Direction.TurnLeft();

        Assert.That(crawler.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingEastOnFarRightTileReturnsOutside()
    {
        var parser = new AsciiParser("""
                +---+
                |   x
                +---+
                """);
        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        crawler.Direction.TurnRight();

        Assert.That(crawler.FacingTile, Is.TypeOf<Outside>());
    }

    [Test]
    public void FacingSouthOnBottomTileReturnsOutside()
    {
        var parser = new AsciiParser("""
                +---+
                |   |
                +--x+
                """);
        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        crawler.Direction.TurnRight();
        crawler.Direction.TurnRight();

        Assert.That(crawler.FacingTile, Is.TypeOf<Outside>());
    }
    #endregion

    #region Moves
    [Test]
    public void TurnLeftFacesWestTile()
    {
        var parser = new AsciiParser("""
                +---+
                |  x|
                +---+
                """);
        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        crawler.Direction.TurnLeft();

        Assert.That(crawler.FacingTile, Is.TypeOf<Room>());
    }

    [Test]
    public void WalkReturnsInventoryAndChangesPositionAndFacingTile()
    {
        // On place le point de départ avec de l'espace au Nord
        var parser = new AsciiParser("""
        +---+
        |   |
        | x |
        +---+
        """);

        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        // Par défaut, le Crawler est en (2, 2) et regarde vers le Nord.
        // La case au Nord (2, 1) est une Room. La case (2, 0) est un Wall (+).
        var inventory = crawler.Walk();

        Assert.That(inventory, Is.Not.Null); // On vérifie qu'un inventaire est bien retourné
        Assert.That(inventory.HasItem, Is.False);
        Assert.That(crawler.X, Is.EqualTo(2));
        Assert.That(crawler.Y, Is.EqualTo(1)); // La position Y a diminué de 1 (marche vers le Nord)
        Assert.That(crawler.FacingTile, Is.TypeOf<Wall>()); // Il fait maintenant face au mur du haut
    }

    [Test]
    public void TurnAndWalkReturnsInventoryChangesPositionAndFacingTile()
    {
        // On place le point de départ complètement à gauche
        var parser = new AsciiParser("""
        +---+
        |x  |
        +---+
        """);

        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        // Rotation vers l'Est (adapte l'appel selon ta logique exacte de rotation)
        crawler.Direction.TurnRight();

        // Initialement en (1, 1), il marche vers l'Est pour aller en (2, 1)
        var inventory = crawler.Walk();

        Assert.That(inventory, Is.Not.Null);
        Assert.That(inventory.HasItem, Is.False);
        Assert.That(crawler.X, Is.EqualTo(2)); // La position X a augmenté de 1
        Assert.That(crawler.Y, Is.EqualTo(1));
        Assert.That(crawler.FacingTile, Is.TypeOf<Room>()); // Il fait face à la prochaine case vide en (3, 1)
    }

    [Test]
    public void WalkOnNonTraversableTileThrowsInvalidOperationException()
    {
        // On place le point de départ collé au mur du haut
        var parser = new AsciiParser("""
        +---+
        | x |
        +---+
        """);

        var laby = new Labyrinth.Labyrinth(parser);
        var crawler = laby.NewCrawler();

        // Le Crawler regarde vers le Nord par défaut, il fait donc face au Wall.
        // Essayer d'avancer doit déclencher une InvalidOperationException.
        Assert.Throws<InvalidOperationException>(() => crawler.Walk());
    }
    #endregion

    #region Items and doors
 
    [Test]
    public void WalkInARoomWithAnItem()
    {
        // On place un point de départ 'x' et un item 'k' (clé) juste à sa droite
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +---+
                |xk |
                |  /|
                +---+
                """));
        var crawler = laby.NewCrawler();

        // On tourne vers l'Est pour faire face à la clé
        crawler.Direction.TurnRight();

        // Le Crawler avance sur la case contenant l'item
        var inventory = crawler.Walk();

        using var all = Assert.EnterMultipleScope();
        Assert.That(crawler.X, Is.EqualTo(2));
        Assert.That(crawler.Y, Is.EqualTo(1));
        // L'inventaire retourné ne doit pas être null et devrait logiquement contenir la clé ramassée
        Assert.That(inventory, Is.Not.Null);
        Assert.That(inventory.HasItem, Is.True);
        Assert.That(crawler.FacingTile, Is.TypeOf<Room>());
    }

    [Test]
    public void WalkUseAWrongKeyToOpenADoor()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
            +---+
            |xk/|
            | /k|
            +---+
            """));
        var test = laby.NewCrawler();

        test.Direction.TurnRight();
        var inventory = test.Walk();

        test.Direction.TurnRight();
        var opened = ((Door)test.FacingTile).Open(inventory);

        using var all = Assert.EnterMultipleScope();

        Assert.That(opened, Is.False);
        Assert.That(inventory.HasItem, Is.True);
        Assert.Throws<InvalidOperationException>(() => test.Walk());
    }

    [Test]
    public void WalkUseKeyToOpenADoorAndPass()
    {
        var laby = new Labyrinth.Labyrinth(new AsciiParser("""
                +--+
                |xk|
                +-/|
                """));
        var test = laby.NewCrawler();

        test.Direction.TurnRight();

        var inventory = test.Walk();

        test.Direction.TurnRight();
        ((Door)test.FacingTile).Open(inventory);

        test.Walk();

        using var all = Assert.EnterMultipleScope();

        Assert.That(test.X, Is.EqualTo(2));
        Assert.That(test.Y, Is.EqualTo(2));
        Assert.That(test.Direction, Is.EqualTo(Direction.South));
        Assert.That(test.FacingTile, Is.TypeOf<Outside>());
    }
    #endregion
}