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

    private void Redraw()
    {
        Console.Clear();
        Console.WriteLine(_field);
    }

    private void MoveCursorIntoField()
    {
        Console.SetCursorPosition(X * CellWidth + 1, Y);
    }

    private void MoveCursorAfterField()
    {
        Console.SetCursorPosition(0, _field.Height);
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

            case K.Z:
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
