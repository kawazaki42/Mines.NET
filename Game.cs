using static System.ConsoleKey;
using static System.Console;

class Program // (CoveredField state)
{
    // private CoveredField State;

    static void Main()
    {
        var f = new Field(8, 8);
        f.Randomize(10);

        var view = new FieldView(f);
        var covered = new CoveredField(view);

        new Game(covered).Start();
    }

    static bool Interact(CoveredField state)
    {
        Console.Write("> ");
        var response = Console.ReadLine();

        if (response is null)
            return true;

        if (response.Split() is not [var yStr, var xStr])
            throw new FormatException();

        // {
        //     Console.WriteLine("lox");
        //     return false;
        // }

        var x = int.Parse(xStr);
        var y = int.Parse(yStr);

        if (!state.IsValidCoordinate(x, y))
            throw new ArgumentOutOfRangeException();

        // if (!int.TryParse(xStr, out x) || !int.TryParse(yStr, out y));

        state.TryOpen(x, y);
        Console.WriteLine(state);

        return false;
    }
}

class Game(CoveredField field)
{
    const int cellWidth = 3;

    int x = 0;
    int y = 0;

    private void Redraw()
    {
        Clear();
        WriteLine(field);
    }

    private void SetCursor()
    {
        SetCursorPosition(x * cellWidth + 1, y);
    }

    internal void Start()
    {
        Redraw();
        SetCursor();

        while (true)
        {
            var key = ReadKey(intercept: true);

            switch (key.Key)
            {
                case RightArrow:
                    x++;
                    break;
                case LeftArrow:
                    if (x > 0)
                        x--;
                    break;
                case DownArrow:
                    y++;
                    break;
                case UpArrow:
                    if (y > 0)
                        y--;
                    break;
                case Spacebar:
                    var mine = !field.TryOpen(x, y);

                    Redraw();

                    if (mine)
                        return;

                    break;
            }

            SetCursor();
        }
    }
}
