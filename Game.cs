using static System.ConsoleKey;
using static System.Console;

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
    const int CellWidth = 3;

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
        Clear();
        WriteLine(_field);
    }

    private void SetCursor()
    {
        Console.SetCursorPosition(X * CellWidth + 1, Y);
    }

    internal void Start()
    {
        Redraw();
        SetCursor();

        Console.CancelKeyPress += (_, _) =>
        {
            Console.SetCursorPosition(0, _field.Height);
        };

        // ConsoleCancelEventArgs

        // Console.TreatControlCAsInput

        while (true)
        {
            // prevent the key from printing itself?
            const bool intercept = true;

            var key = ReadKey(intercept);

            if (!HandleKey(key))
                break;

            if (_field.IsClear())
                break;
        }

        Console.SetCursorPosition(0, _field.Height);
    }

    bool HandleKey(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case RightArrow:
                X++;
                break;

            case LeftArrow:
                X--;
                break;

            case DownArrow:
                Y++;
                break;

            case UpArrow:
                Y--;
                break;

            case F:
                _field.ToggleFlagAt(X, Y);
                Redraw();
                break;

            // case Spacebar when key.Modifiers.HasFlag(ConsoleModifiers.Shift):
            //     _field.ToggleFlagAt(X, Y);
            //     Redraw();
            //     break;

            case Spacebar:
                // var mine = !_field.TryOpen(X, Y);
                _field.TryOpen(X, Y);

                Redraw();

                if (_field.Mines[X, Y])
                    return false;

                break;
        }

        SetCursor();
        return true;
    }
}
