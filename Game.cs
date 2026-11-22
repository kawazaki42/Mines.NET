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
            if (!HandleKey(key))
                return;
        }
    }

    bool HandleKey(ConsoleKeyInfo key)
    {
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
                    return false;

                break;
        }

        SetCursor();
        return true;
    }
}
