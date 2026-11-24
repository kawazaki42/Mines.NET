using K = System.ConsoleKey;
// using static System.Console;

class Program
{
    static void Main()
    {
        var f = new Field(8, 8);
        f.Randomize(10);

        var view = new FieldView(f);
        var covered = new CoveredField(view);

        new Game(covered).Start();
    }
}

class Game(CoveredField _field)
{
    // const int CellWidth = 3;
    static int CellWidth => CoveredField.CellWidth + CoveredField.CellSeparator.Length;

    int X
    {
        get;
        set => field = Math.Clamp(value, 0, _field.Width - 1);
    } = 0;

    int Y
    {
        get;
        set => field = Math.Clamp(value, 0, _field.Height - 1);
    } = 0;

    Coordinate ScreenCenter => new(
        (Console.WindowWidth - _field.Width * CellWidth) / 2,
        (Console.WindowHeight - _field.Height) / 2
    );

    private void Redraw()
    {
        Console.Clear();

        var (x, y) = ScreenCenter;

        Console.SetCursorPosition(x, y);

        foreach (var line in _field.ToString().Split('\n'))
        {
            Console.Write(line);
            Console.CursorTop++;
            Console.CursorLeft = x;
        }

    }

    private void MoveCursorIntoField()
    {
        var (x, y) = ScreenCenter;
        Console.SetCursorPosition(x + X * CellWidth + 1, y + Y);
    }

    private void MoveCursorAfterField()
    {
        // Console.SetCursorPosition(0, _field.Height);
        Console.SetCursorPosition(0, 0);
    }

    internal void Start()
    {
        Redraw();
        MoveCursorIntoField();

        Console.CancelKeyPress += (_, _) =>
        {
            MoveCursorAfterField();
        };

        // ConsoleCancelEventArgs

        // Console.TreatControlCAsInput

        while (true)
        {
            // prevent the key from printing itself?
            const bool intercept = true;

            var key = Console.ReadKey(intercept);
            HandleKey(key);

            // if (!HandleKey(key))
            // break;

            if (_field.GameLostPos is not null)
            {
                MoveCursorAfterField();
                Console.WriteLine("You lost");
                return;
            }

            if (_field.IsClear())
            {
                _field.OpenAll();
                Redraw();
                MoveCursorAfterField();
                Console.WriteLine("You win");
                return;
            }
        }

        // Console.SetCursorPosition(0, _field.Height);
    }

    void HandleKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case K.RightArrow or K.D:
                X++;
                break;

            case K.LeftArrow or K.A:
                X--;
                break;

            case K.DownArrow or K.S:
                Y++;
                break;

            case K.UpArrow or K.W:
                Y--;
                break;

            case K.Z or K.OemComma:
                _field.ToggleFlagAt(X, Y);
                Redraw();
                break;

            // case Spacebar when key.Modifiers.HasFlag(ConsoleModifiers.Shift):
            //     _field.ToggleFlagAt(X, Y);
            //     Redraw();
            //     break;

            case K.Spacebar:
                // var mine = !_field.TryOpen(X, Y);
                _field.TryOpen(X, Y);

                Redraw();

                // if (_field.Mines[X, Y])
                //     return false;

                break;
        }

        MoveCursorIntoField();
        // return true;
    }
}
