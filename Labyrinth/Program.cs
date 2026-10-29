using Labyrinth.Build;
using Labyrinth.Crawl;
using Labyrinth.Tiles;

var labyrinth = new Labyrinth.Labyrinth(new AsciiParser("""
    +--+--------+
    |  /        |
    |  +--+--+  |
    |     |k    |
    +--+  |  +--+
       |k  x    |
    +  +-------/|
    |           |
    +-----------+
    """));
var crawler = labyrinth.NewCrawler();
var explorer = new Explorer(crawler);
var previousX = crawler.X;
var previousY = crawler.Y;

Console.Write(labyrinth);
DrawCrawler(crawler.X, crawler.Y, crawler.Direction);

explorer.PositionChanged += (_, args) =>
{
    Console.SetCursorPosition(previousX, previousY);
    Console.Write(GetTileCharacter(labyrinth.GetTile(previousX, previousY)));
    previousX = args.X;
    previousY = args.Y;
    DrawCrawler(args.X, args.Y, args.Direction);
    Thread.Sleep(50);
};
explorer.DirectionChanged += (_, args) =>
{
    DrawCrawler(args.X, args.Y, args.Direction);
    Thread.Sleep(50);
};

explorer.GetOut(100);
Console.SetCursorPosition(0, labyrinth.Height + 1);

void DrawCrawler(int x, int y, Direction direction)
{
    Console.SetCursorPosition(x, y);
    Console.Write((direction.DeltaX, direction.DeltaY) switch
    {
        (0, -1) => '^',
        (1, 0) => '>',
        (0, 1) => 'v',
        (-1, 0) => '<',
        _ => throw new ArgumentException("Invalid crawler direction.", nameof(direction))
    });
}

char GetTileCharacter(Tile tile) => tile switch
{
    Room => ' ',
    Wall => '#',
    Door => '/',
    _ => throw new NotSupportedException("Unknown tile type")
};