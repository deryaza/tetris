namespace Tetris.Aboba.Core.Pieces;

public sealed class PieceT : Tetromino
{
    private static Dictionary<Rotation, Point[]> PointsByDirection => new()
    {
        [Rotation.Rot0] = new Point[]
        {
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1),
            new Point(2, 1),
        },
        [Rotation.Rot90] = new Point[]
        {
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2),
            new Point(2, 1),
        },
        [Rotation.Rot180] = new Point[]
        {
            new Point(0, 1),
            new Point(1, 1),
            new Point(2, 1),
            new Point(1, 2),
        },
        [Rotation.Rot270] = new Point[]
        {
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2),
            new Point(0, 1),
        }
    };

    private static Point Origin => new(1, 1);

    public PieceT() : base(PointsByDirection, Origin, Color.Purple, KickTable.JLTSZKickTable)
    {
    }

    public override void SetAtStartingPosition(int width, int height)
    {
        Position = new(width / 2, 0);
    }
}
