namespace Tetris.Aboba.Core;

public class KickTable
{
    private readonly Dictionary<Rotation, Dictionary<Rotation, Point[]>> table;

    private static Dictionary<Rotation, Dictionary<Rotation, Point[]>> JLTSZ => new()
    {
        [Rotation.Rot0] = new()
        {
            [Rotation.Rot90] = new Point[]
            {
                new(-1, 0),
                new(-1, -1),
                new(0, 2),
                new(-1, 2),
            },
            [Rotation.Rot270] = new Point[]
            {
                new(1, 0),
                new(1, -1),
                new(0, 2),
                new(1, 2),
            }
        },
        [Rotation.Rot90] = new()
        {
            [Rotation.Rot0] = new Point[]
            {
                new(1, 0),
                new(1, 1),
                new(0, -2),
                new(1, -2),
            },
            [Rotation.Rot180] = new Point[]
            {
                new(1, 0),
                new(1, 1),
                new(0, -2),
                new(1, -2),
            }
        },
        [Rotation.Rot180] = new()
        {
            [Rotation.Rot90] = new Point[]
            {
                new(-1, 0),
                new(-1, -1),
                new(0, 2),
                new(-1, 2),
            },
            [Rotation.Rot270] = new Point[]
            {
                new(1, 0),
                new(1, -1),
                new(0, 2),
                new(1, 2),
            }
        },
        [Rotation.Rot270] = new()
        {
            [Rotation.Rot180] = new Point[]
            {
                new(-1, 0),
                new(-1, 1),
                new(0, -2),
                new(-1, -2),
            },
            [Rotation.Rot0] = new Point[]
            {
                new(-1, 0),
                new(-1, 1),
                new(0, -2),
                new(-1, -2),
            }
        }
    };

    private static Dictionary<Rotation, Dictionary<Rotation, Point[]>> I => new()
    {
        [Rotation.Rot0] = new()
        {
            [Rotation.Rot90] = new Point[]
            {
                new(-2, 0),
                new(1, 0),
                new(-2, 1),
                new(1, -2),
            },
            [Rotation.Rot270] = new Point[]
            {
                new(-1, 0),
                new(2, 0),
                new(-1, -2),
                new(2, 1),
            }
        },
        [Rotation.Rot90] = new()
        {
            [Rotation.Rot0] = new Point[]
            {
                new(2, 0),
                new(-1, 0),
                new(2, -1),
                new(-1, 2),
            },
            [Rotation.Rot180] = new Point[]
            {
                new(1, 0),
                new(-2, 0),
                new(1, -2),
                new(-2, 1),
            }
        },
        [Rotation.Rot180] = new()
        {
            [Rotation.Rot90] = new Point[]
            {
                new(1, 0),
                new(-2, 0),
                new(1, 2),
                new(-2, -1),
            },
            [Rotation.Rot270] = new Point[]
            {
                new(2, 0),
                new(-1, 0),
                new(2, -1),
                new(-1, 2),
            }
        },
        [Rotation.Rot270] = new()
        {
            [Rotation.Rot180] = new Point[]
            {
                new(-2, 0),
                new(1, 0),
                new(-2, 1),
                new(1, -2),
            },
            [Rotation.Rot0] = new Point[]
            {
                new(-1, 0),
                new(2, 0),
                new(-1, 2),
                new(2, -1),
            }
        }
    };

    public readonly static KickTable IKickTable = new(I);

    public readonly static KickTable OKickTable = new(new());

    public readonly static KickTable JLTSZKickTable = new(JLTSZ);

    public KickTable(Dictionary<Rotation, Dictionary<Rotation, Point[]>> table)
    {
        this.table = table;
    }

    public Point[] GetTable(Rotation from, Rotation to)
    {
        return table[from][to];
    }
}
