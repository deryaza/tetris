using Tetris.Aboba.Core.Pieces;

namespace Tetris.Aboba.Core;

public enum GameAction
{
    L, R,
    DBG_UP,
    LockingHardDrop,
    NonLockingSoftDrop,
    RotateCounterclockwise,
    RotateClockwise,
    Hold,
}

public class TetrominoPool
{
    private readonly Random seed;

    public TetrominoPool(Random seed)
    {
        this.seed = seed;
        Next = generateCore();
    }

    public Tetromino Next { get; private set; }

    private Tetromino generateCore()
    {
        return seed.Next(0, 7) switch
        {
            0 => new PieceI(),
            1 => new PieceO(),
            2 => new PieceT(),
            3 => new PieceS(),
            4 => new PieceZ(),
            5 => new PieceJ(),
            6 => new PieceL(),
            _ => throw new("idk"),
        };
    }

    public Tetromino GenerateNew()
    {
        Tetromino toReturn = Next;
        Next = generateCore();
        return toReturn;
    }
}

public class GameField
{
    private const int width = 10;
    private const int height = 22;
    private const int scoreMultiplier = 5;
    private const int tetrisBonus = 50;

    private readonly Tetromino[,] field;

    private TimeSpan lastTime;
    private readonly TimeSpan HardLockTime = TimeSpan.FromSeconds(0.5);

    private readonly (int, TimeSpan)[] clearedLinesToUpdateSpeed = new[]
    {
        (100, TimeSpan.FromSeconds(0.02)),
        (90, TimeSpan.FromSeconds(0.05)),
        (80, TimeSpan.FromSeconds(0.1)),
        (70, TimeSpan.FromSeconds(0.15)),
        (60, TimeSpan.FromSeconds(0.2)),
        (50, TimeSpan.FromSeconds(0.25)),
        (40, TimeSpan.FromSeconds(0.4)),
        (30, TimeSpan.FromSeconds(0.5)),
        (20, TimeSpan.FromSeconds(0.6)),
        (15, TimeSpan.FromSeconds(0.65)),
        (10, TimeSpan.FromSeconds(0.7)),
        (5, TimeSpan.FromSeconds(0.75)),
        (0, TimeSpan.FromSeconds(1)),
    };
    private readonly TetrominoPool pool;
    private Tetromino? onHoldTetromino;
    private Tetromino currentTetromino = null!;

    public bool IsGameOver = false;

    public int Score { get; private set; }

    public int ClearedLines { get; private set; }

    public int Speed => (int)((2 - GetUpdateRate().TotalSeconds) * 100);

    public GameField(TetrominoPool pool)
    {
        this.pool = pool;
        field = new Tetromino[width, height];
        SpawnAndAssignRandom();
    }

    public void SpawnAndAssignRandom()
    {
        Tetromino? tetrominoSpawnCandidate = pool.GenerateNew();
        tetrominoSpawnCandidate.SetAtStartingPosition(width, height);
        if (IsValidTetromino(tetrominoSpawnCandidate))
        {
            currentTetromino = tetrominoSpawnCandidate;
        }
        else
        {
            IsGameOver = true;
        }
    }

    private bool IsValidTetrominoShape<T>(T shape) where T : IEnumerable<Point>
    {
        (int wl, int hl) = (width, height);
        if (shape is Point[] shapeArray)
        {
            for (int i = 0; i < shapeArray.Length; i++)
            {
                ref Point point = ref shapeArray[i];
                (int x, int y) = point;
                if (x < 0 || y < 0
                    || x >= wl || y >= hl
                    || field[x, y] is not null)
                {
                    return false;
                }

                point.Y++;
            }
        }
        else
        {
            foreach (Point point in shape)
            {
                (int x, int y) = point;
                if (x < 0 || y < 0
                    || x >= wl || y >= hl
                    || field[x, y] is not null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void HardDrop(Tetromino tetromino)
    {
        Point[] shapeCopy = tetromino.GetCurrentShapeCopy();
        int addToYPosition = 0;
        while (IsValidTetrominoShape(shapeCopy))
        {
            addToYPosition++;
        }

        tetromino.Position.Y += addToYPosition - 1;
    }

    private bool RotateClockwise(Tetromino tetromino)
    {
        (Rotation from, Rotation to) = currentTetromino.RotateClockwise();
        if (IsValidTetromino(currentTetromino))
        {
            return true;
        }

        var table = tetromino.KickTable.GetTable(from, to);
        var state = tetromino.SaveState();
        foreach (Point offset in table)
        {
            tetromino.Position += offset;
            if (IsValidTetromino(tetromino))
            {
                return true;
            }
            tetromino.RestoreState(state);
        }

        return false;
    }

    private bool RotateCounterclockwise(Tetromino tetromino)
    {
        (Rotation from, Rotation to) = currentTetromino.RotateCounterclockwise();
        if (IsValidTetromino(currentTetromino))
        {
            return true;
        }

        var table = tetromino.KickTable.GetTable(from, to);
        var state = tetromino.SaveState();
        foreach (Point offset in table)
        {
            tetromino.Position += offset;
            if (IsValidTetromino(tetromino))
            {
                return true;
            }
            tetromino.RestoreState(state);
        }

        return false;
    }

    private bool hitOnce = false;
    private TimeSpan lastLockCheck;

    private TimeSpan GetUpdateRate()
    {
        foreach ((int lines, TimeSpan upr) in clearedLinesToUpdateSpeed)
        {
            if (ClearedLines >= lines)
            {
                return upr;
            }
        }

        return clearedLinesToUpdateSpeed.First().Item2;
    }

    public void Update(TimeSpan elapsed)
    {
        TimeSpan delta = elapsed - lastTime;
        TimeSpan upr = GetUpdateRate();
        if (delta < upr)
        {
            return;
        }

        lastTime = elapsed;
        for (int i = 0; i < (int)(delta / upr); i++)
        {
            Move(GameAction.NonLockingSoftDrop, shouldHardDrop: true, elapsed);
        }
    }

    public void Move(GameAction move, bool shouldHardDrop = false, TimeSpan elapsed = default)
    {
        if (IsGameOver) return;

        bool shouldSpawnNewTetromino = false;
        bool isValidated = false;

        var state = currentTetromino.SaveState();
        switch (move)
        {
            #region !DONE! MOVING SECTION
            case GameAction.L:
                currentTetromino.Position.X--;
                break;
            case GameAction.R:
                currentTetromino.Position.X++;
                break;
            case GameAction.DBG_UP:
#if DEBUG
                currentTetromino.Position.Y--;
#endif
                break;
            #endregion

            #region !DONE! DROP SECTION
            case GameAction.NonLockingSoftDrop:
                currentTetromino.Position.Y++;
                break;
            case GameAction.LockingHardDrop:
                HardDrop(currentTetromino);
                shouldSpawnNewTetromino = true;
                break;
            #endregion

            #region !DONE: KICK TABLE! ROTATE SECTION
            case GameAction.RotateCounterclockwise:
                isValidated = RotateCounterclockwise(currentTetromino);
                break;
            case GameAction.RotateClockwise:
                isValidated = RotateClockwise(currentTetromino);
                break;
            #endregion

            #region META SECTION
            case GameAction.Hold:
                if (onHoldTetromino is null)
                {
                    onHoldTetromino = currentTetromino;
                    SpawnAndAssignRandom();
                }
                else
                {
                    onHoldTetromino.SetAtStartingPosition(width, height);
                    currentTetromino.SetAtStartingPosition(width, height);
                    currentTetromino.Rotation = Rotation.Rot0;
                    (currentTetromino, onHoldTetromino) = (onHoldTetromino, currentTetromino);
                }

                break;
            #endregion
            default:
                break;
        }

        if (isValidated || IsValidTetromino(currentTetromino))
        {
            if (shouldSpawnNewTetromino)
            {
                AddTetromino(currentTetromino);
                SpawnAndAssignRandom();
            }
        }
        else
        {
            currentTetromino.RestoreState(state);
            if (shouldHardDrop)
            {
                if (hitOnce)
                {
                    if (elapsed - lastLockCheck > HardLockTime)
                    {
                        hitOnce = false;
                        HardDrop(currentTetromino);
                        AddTetromino(currentTetromino);
                        SpawnAndAssignRandom();
                    }
                }
                else
                {
                    hitOnce = true;
                    lastLockCheck = elapsed;
                }
            }
        }
    }

    public bool IsValidTetromino(Tetromino tetromino)
    {
        return IsValidTetrominoShape(tetromino.EnumeratePoints());
    }

    private void RemoveLine(int line)
    {
        for (int j = line - 1; j >= 0; j--)
        {
            for (int i = 0; i < width; i++)
            {
                field[i, j + 1] = field[i, j];
                field[i, j] = null;
            }
        }
    }

    public void RemoveFilled()
    {
        int removedLines = 0;
    from_head:
        for (int j = 0; j < height; j++)
        {
            bool filled = true;
            for (int i = 0; i < width; i++)
            {
                filled &= field[i, j] is not null;
            }

            if (filled)
            {
                removedLines++;
                RemoveLine(j);
                goto from_head;
            }
        }

        ClearedLines += removedLines;
        Score = ClearedLines * scoreMultiplier;
        if (removedLines > 3)
        {
            Score += tetrisBonus;
        }
    }

    public bool AddTetromino(Tetromino tetromino)
    {
        if (!IsValidTetromino(tetromino))
        {
            return false;
        }

        foreach (Point point in tetromino.EnumeratePoints())
        {
            (int x, int y) = point;
            field[x, y] = tetromino;
        }

        RemoveFilled();
        return true;
    }

    public void DrawNextTetromino(DrawingContext dc)
    {
        var oht = pool.Next;
        var state = oht.SaveState();
        oht.Position = default;
        foreach ((int x, int y) in oht.EnumeratePointsPreview())
        {
            dc.DrawCell(x, y, oht.Colour);
        }
        oht.RestoreState(state);
    }

    public void DrawOnHoldTetromino(DrawingContext dc)
    {
        if (onHoldTetromino is not { } oht)
        {
            return;
        }
        var state = oht.SaveState();
        oht.Position = default;
        foreach ((int x, int y) in oht.EnumeratePointsPreview())
        {
            dc.DrawCell(x, y, oht.Colour);
        }
        oht.RestoreState(state);
    }

    public void Draw(DrawingContext dc)
    {
        for (int i = 0; i < field.GetLength(0); i++)
        {
            for (int j = 0; j < field.GetLength(1); j++)
            {
                if (!(field[i, j] is { } t))
                {
                    continue;
                }

                dc.DrawCell(i, j, t.Colour);
            }
        }

        foreach ((int x, int y) in currentTetromino.EnumeratePoints())
        {
            dc.DrawCell(x, y, currentTetromino.Colour);
        }

        var state = currentTetromino.SaveState();
        HardDrop(currentTetromino);
        foreach ((int x, int y) in currentTetromino.EnumeratePoints())
        {
            dc.DrawCell(x, y, currentTetromino.AlphaColour);
        }
        currentTetromino.RestoreState(state);
    }
}