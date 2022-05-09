namespace Tetris.Aboba.Core.Pieces;

public sealed class PieceI : Tetromino
{
    private static Dictionary<Rotation, Point[]> PointsByDirection => new()
    {
        [Rotation.Rot0] = new Point[]
        {
            new(-2, 0),
            new(-1, 0),
            new(0, 0),
            new(1, 0),
        },
        [Rotation.Rot90] = new Point[]
        {
            new(0, -2),
            new(0, -1),
            new(0, 0),
            new(0, 1),
        },
        [Rotation.Rot180] = new Point[]
        {
            new(-1, 0),
            new(0, 0),
            new(1, 0),
            new(2, 0),
        },
        [Rotation.Rot270] = new Point[]
        {
            new(0, -1),
            new(0, 0),
            new(0, 1),
            new(0, 2),
        }
    };

    private static Point Origin => new(2, 0);

    public PieceI() : base(PointsByDirection, Origin, Color.Cyan, KickTable.IKickTable)
    {
    }

    public override IEnumerable<Point> EnumeratePointsPreview()
    {
        Point previewOrigin = Origin + new Point(0, 1);
        foreach (Point point in base.EnumeratePointsPreview())
        {
            yield return point + previewOrigin;
        }
    }

    public override void SetAtStartingPosition(int width, int height)
    {
        Position = new(width / 2, 0);
    }
}
