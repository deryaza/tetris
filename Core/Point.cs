namespace Tetris.Aboba.Core;

public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

    public static Point operator +(Point left, Point right)
    {
        return new(left.X + right.X, left.Y + right.Y);
    }

    public static Point operator -(Point left, Point right)
    {
        return new(left.X - right.X, left.Y - right.Y);
    }

    public override string ToString()
    {
        return $"{X}, {Y}";
    }
}
