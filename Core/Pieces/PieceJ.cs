namespace Tetris.Aboba.Core.Pieces;

public sealed class PieceJ : Tetromino
{
    private static Dictionary<Rotation, Point[]> PointsByDirection => new()
    {
        [Rotation.Rot0] = new Point[]
        {
            new(0, 0),
            new(0, 1),
            new(1, 1),
            new(2, 1),
        },
        [Rotation.Rot90] = new Point[]
        {
            new(1, 0),
            new(2, 0),
            new(1, 1),
            new(1, 2),
        },
        [Rotation.Rot180] = new Point[]
        {
            new(0, 1),
            new(1, 1),
            new(2, 1),
            new(2, 2),
        },
        [Rotation.Rot270] = new Point[]
        {
            new(1, 0),
            new(1, 1),
            new(1, 2),
            new(0, 2),
        }
    };

    private static Point Origin => new(2, 0);

    public PieceJ() : base(PointsByDirection, Origin, Color.Blue, KickTable.JLTSZKickTable)
    {
    }

    public override void SetAtStartingPosition(int width, int height)
    {
        Position = new(width / 2, 0);
    }
}
