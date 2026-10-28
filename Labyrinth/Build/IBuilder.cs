using Labyrinth.Items;
using Labyrinth.Tiles;

namespace Labyrinth.Build
{
    public interface IBuilder
    {
        event EventHandler<StartEventArgs>? StartPositionFound;

        Tile[,] Build();
    }
}
