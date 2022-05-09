namespace Tetris.Aboba.Core.Pieces;

public sealed class PieceO : Tetromino
{
    private static Point[] OShape => new Point[]
    {
        new(0, 0),
        new(1, 0),
        new(0, 1),
        new(1, 1),
    };

    private static Dictionary<Rotation, Point[]> PointsByDirection => new()
    {
        [Rotation.Rot0] = OShape,
        [Rotation.Rot90] = OShape,
        [Rotation.Rot180] = OShape,
        [Rotation.Rot270] = OShape
    };

    private static Point Origin => new(0, 0);

    public PieceO() : base(PointsByDirection, Origin, Color.Yellow, KickTable.OKickTable)
    {
    }

    public override void SetAtStartingPosition(int width, int height)
    {
        Position = new(width / 2, 0);
    }
}
