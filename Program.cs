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
    internal void Start()
    {

        Clear();

        WriteLine(field);

        SetCursorPosition(1, 0);

        while (true)
        {
            var key = ReadKey(intercept: true);

            // Console.WriteLine(((byte)key.KeyChar));

            // Console.MoveBufferArea

            // using static ConsoleKey;

            switch (key.Key)
            {
                case RightArrow:
                    CursorLeft += 3;
                    break;
                case LeftArrow:
                    if (CursorLeft - 3 >= 0)
                        CursorLeft -= 3;
                    break;
                case DownArrow:
                    CursorTop++;
                    break;
                case UpArrow:
                    // Console.Write(Console.CursorTop--);
                    if (CursorTop > 0)
                        CursorTop--;
                    break;
                case Spacebar:
                    var y = CursorTop;

                    if (CursorLeft % 3 == 2)
                        break;

                    var x = CursorLeft / 3;
                    var mine = !field.TryOpen(x, y);
                    Clear();
                    WriteLine(field);

                    if (mine)
                        return;

                    SetCursorPosition(x * 3 + 1, y);
                    break;
            }
        }
    }
}
