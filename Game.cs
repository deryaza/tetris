using System.Collections.Concurrent;
using System.Diagnostics;
using Tetris.Aboba.Core;

namespace Tetris.Aboba;

internal class Game
{
    private readonly GameField gameField;

    private readonly Stopwatch gameTimer;
    private TimeSpan previouslyElapsed;
    private TimeSpan previouslyElapsedOneTime;

    public Game(string seed, Font gameOverFont, Font uiFont)
    {
        gameField = new(new(string.IsNullOrWhiteSpace(seed) ? Random.Shared : new(seed.Sum(c => (int)c))));
        gameTimer = Stopwatch.StartNew();

        Dictionary<Keys, GameAction>.ValueCollection? allActions = inputMap.Values;
        ongoingActions = new(allActions.Count);
        foreach (GameAction action in allActions)
        {
            ongoingActions[action] = 0;
        }

        this.gameOverFont = gameOverFont;
        this.uiFont = uiFont;
    }

    private readonly ConcurrentQueue<GameAction> actions = new();
    private readonly ConcurrentQueue<GameAction> undoActions = new();

    private readonly Dictionary<GameAction, int> ongoingActions;

    private readonly Dictionary<Keys, GameAction> inputMap = new()
    {
        [Keys.Left] = GameAction.L,
        [Keys.Right] = GameAction.R,
        [Keys.Down] = GameAction.NonLockingSoftDrop,
        [Keys.Up] = GameAction.DBG_UP,
    };

    private readonly Dictionary<Keys, GameAction> oneClickActions = new()
    {
        [Keys.Space] = GameAction.LockingHardDrop,
        [Keys.X] = GameAction.RotateClockwise,
        [Keys.Z] = GameAction.RotateCounterclockwise,
        [Keys.C] = GameAction.Hold,
        [Keys.ShiftKey] = GameAction.Hold
    };

    public void KeyUp(Keys key)
    {
        if (inputMap.TryGetValue(key, out GameAction action))
        {
            undoActions.Enqueue(action);
        }
    }

    public void KeyDown(Keys key)
    {
        if (inputMap.TryGetValue(key, out GameAction action))
        {
            actions.Enqueue(action);
        }

        if (oneClickActions.TryGetValue(key, out action))
        {
            gameField.Move(action);
        }
    }

    static readonly TimeSpan constantUpdateRate = TimeSpan.FromSeconds(0.075);
    static readonly TimeSpan firstCountTimeout = constantUpdateRate * 5;

    public void UpdateGameField()
    {
        TimeSpan elapsed = gameTimer.Elapsed;
        gameField.Update(elapsed);

        TimeSpan deltaQuick = elapsed - previouslyElapsed;
        TimeSpan deltaOneTime = elapsed - previouslyElapsedOneTime;

        while (actions.TryDequeue(out GameAction action))
        {
            if (ongoingActions[action] == 0)
            {
                Debug.WriteLine($"action: {action}");
                ongoingActions[action] = 1;
            }
        }

        while (undoActions.TryDequeue(out GameAction action))
        {
            Debug.WriteLine($"off action: {action}");
            ongoingActions[action] = 0;
        }

        foreach ((GameAction action, int count) in ongoingActions)
        {
            if (count == 1)
            {
                gameField.Move(action);
                ongoingActions[action]++;
                previouslyElapsedOneTime = elapsed;
            }
            else if (count == 2 && deltaOneTime > firstCountTimeout)
            {
                ongoingActions[action]++;
            }
            else if (count > 2 && deltaQuick > constantUpdateRate)
            {
                gameField.Move(action);
                previouslyElapsed = elapsed;
            }
        }
    }

    private readonly Font gameOverFont;
    private readonly Font uiFont;

    public void RenderElements(Graphics g, Rectangle clientRectangle)
    {
        DrawingContext dc = new(g, clientRectangle.Width, clientRectangle.Height, 10, 22);
        gameField.Draw(dc);
        dc.DrawGrid(2);

        dc = new DrawingContext(g, clientRectangle.Width, clientRectangle.Height, 4, 22, dc.UISection);
        dc.DrawTextInCell(0, 0, $"Очков: {gameField.Score}", uiFont);
        int speed = gameField.Speed;
        dc.DrawTextInCell(0, 1, $"Скорость: {(speed == 198 ? "MAX" : speed)}%", uiFont);
        int nextCell = dc.DrawTextInCell(0, 2, "Запас:", uiFont);
        Core.Point onHoldOffset = dc.GetOffsetForCell(nextCell, 2);
        DrawingContext onHoldDc = new(g, dc.CellWidth * 2 + onHoldOffset.X, dc.CellHeight * 2 + onHoldOffset.Y, 4, 4, onHoldOffset);
        gameField.DrawOnHoldTetromino(onHoldDc);

        nextCell = dc.DrawTextInCell(0, 3, "Следующая:", uiFont);
        Core.Point nextPieceOffset = dc.GetOffsetForCell(nextCell, 3);
        DrawingContext nextDc = new(g, dc.CellWidth * 2 + nextPieceOffset.X, dc.CellHeight * 2 + nextPieceOffset.Y, 4, 4, nextPieceOffset);
        gameField.DrawNextTetromino(nextDc);

        if (gameField.IsGameOver)
        {
            g.DrawString("ВСЁ!", gameOverFont, Brushes.Black, dc.UISection.X, dc.UISection.Y);
        }
    }
}
