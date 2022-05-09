namespace Tetris.Aboba.Core;

public enum Rotation
{
    Rot0,
    Rot90,
    Rot180,
    Rot270,
}

public abstract class Tetromino
{
    private readonly Dictionary<Rotation, Point[]> PointsByDirection;

    public readonly Point Origin;

    public readonly KickTable KickTable;

    protected Tetromino(Dictionary<Rotation, Point[]> points, Point origin, Color colour, KickTable kickTable)
    {
        PointsByDirection = points;
        Origin = origin;
        Colour = colour;
        this.KickTable = kickTable;
    }

    public Rotation Rotation;

    public Point Position;

    public bool IsSolid { get; }

    public Color Colour { get; }

    public Color AlphaColour => Color.FromArgb(100, Colour.R, Colour.G, Colour.B);

    public (Rotation, Point) SaveState() => (Rotation, Position);

    public abstract void SetAtStartingPosition(int width, int height);

    public void RestoreState((Rotation, Point) state) => (Rotation, Position) = state;

    public (Rotation from, Rotation to) RotateCounterclockwise()
    {
        Rotation from = Rotation;
        if (Rotation == Rotation.Rot0)
        {
            Rotation = Rotation.Rot270;
        }
        else
        {
            Rotation--;
        }

        return (from, Rotation);
    }

    public (Rotation from, Rotation to) RotateClockwise()
    {
        Rotation from = Rotation;
        if (Rotation == Rotation.Rot270)
        {
            Rotation = Rotation.Rot0;
        }
        else
        {
            Rotation++;
        }
        return (from, Rotation);
    }

    public virtual IEnumerable<Point> EnumeratePointsPreview()
    {
        return EnumeratePoints();
    }

    public IEnumerable<Point> EnumeratePoints()
    {
        Point[] points = PointsByDirection[Rotation];
        foreach (Point point in points)
        {
            yield return Position + point;
        }
    }

    public Point[] GetCurrentShapeCopy()
    {
        Point[] points = PointsByDirection[Rotation];
        Point[] copy = new Point[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            copy[i] = points[i] + Position;
        }
        return copy;
    }
}
