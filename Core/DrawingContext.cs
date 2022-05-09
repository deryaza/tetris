namespace Tetris.Aboba.Core;

public readonly struct DrawingContext
{
    private readonly Graphics g;
    private readonly int actualWidth;
    private readonly int actualHeight;
    private readonly int playfieldWidth;
    private readonly int playfieldHeight;

    public readonly int CellWidth;
    public readonly int CellHeight;

    public readonly Point UISection;

    public readonly Point Offset;

    public DrawingContext(Graphics g, int width, int height, int playfieldWidth, int playfieldHeight, Point offset = default)
    {
        this.g = g;
        actualWidth = width - offset.X;
        actualHeight = height - offset.Y;
        this.playfieldWidth = playfieldWidth;
        this.playfieldHeight = playfieldHeight;
        CellHeight = actualHeight / playfieldHeight;
        CellWidth = (int)(CellHeight * 1.16);
        Offset = offset;
        UISection = new(CellWidth * playfieldWidth + 10, 10);
    }

    public Point GetOffsetForCell(int x, int y)
    {
        return new(x * CellWidth + Offset.X, y * CellHeight + Offset.Y);
    }

    public int DrawTextInCell(int x, int y, string text, Font font)
    {
        (x, y) = (x * CellWidth + Offset.X, y * CellHeight + Offset.Y);
        SizeF size = g.MeasureString(text, font);
        g.DrawString(text, font, Brushes.Black, x, y);
        return (int)(size.Width / CellWidth) + 1;
    }

    public void DrawCell(int x, int y, Color color)
    {
        using SolidBrush? sb = new(color);
        g.FillRectangle(sb, x * CellWidth + Offset.X, y * CellHeight + Offset.Y, CellWidth, CellHeight);
    }

    internal void DrawGrid(int padding)
    {
        for (int x = 0; x < playfieldWidth; x++)
        {
            for (int y = 0; y < padding; y++)
            {
                g.DrawRectangle(Pens.Red, x * CellWidth + Offset.X, y * CellHeight + Offset.Y, CellWidth, CellHeight);
            }
        }

        for (int x = 0; x < playfieldWidth; x++)
        {
            for (int y = padding; y < playfieldHeight; y++)
            {
                g.DrawRectangle(Pens.Black, x * CellWidth + Offset.X, y * CellHeight + Offset.Y, CellWidth, CellHeight);
            }
        }
    }
}
