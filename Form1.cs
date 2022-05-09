
using System.Diagnostics;
using Timer = System.Threading.Timer;

namespace Tetris.Aboba
{
    public partial class Form1 : Form
    {
        private readonly Timer timer;
        private readonly SynchronizationContext sc;

        private Game? game;

        public Form1()
        {
            sc = SynchronizationContext.Current ?? throw new Exception();
            InitializeComponent();

            Width = 860;
            Height = 830;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;

            timer = new Timer(Callback, null, Timeout.InfiniteTimeSpan, TimeSpan.FromMilliseconds(16));
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            game?.KeyUp(e.KeyCode);
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            game?.KeyDown(e.KeyCode);
        }

        private void Callback(object? state)
        {
            sc.Post(_ =>
            {
                Invalidate();
            }, null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (game is { } gm)
            {
                SuspendLayout();
                Graphics? g = e.Graphics;
                g.Clear(Color.White);
                gm.UpdateGameField();
                gm.RenderElements(g, ClientRectangle);
                ResumeLayout(false);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game = new(textBox1.Text, new Font(Font.FontFamily, 100), new Font(Font.FontFamily, 18));
            timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
            this.Controls.Clear();
            this.Focus();
        }
    }
}