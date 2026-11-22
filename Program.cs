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

        Console.Clear();

        Console.WriteLine(field);

        Console.SetCursorPosition(1, 0);

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            // Console.WriteLine(((byte)key.KeyChar));

            // Console.MoveBufferArea

            // using static ConsoleKey;

            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    Console.CursorLeft += 3;
                    break;
                case ConsoleKey.LeftArrow:
                    if (Console.CursorLeft - 3 >= 0)
                        Console.CursorLeft -= 3;
                    break;
                case ConsoleKey.DownArrow:
                    Console.CursorTop++;
                    break;
                case ConsoleKey.UpArrow:
                    // Console.Write(Console.CursorTop--);
                    if (Console.CursorTop > 0)
                        Console.CursorTop--;
                    break;
                case ConsoleKey.Spacebar:
                    var y = Console.CursorTop;

                    if (Console.CursorLeft % 3 == 2)
                        break;

                    var x = Console.CursorLeft / 3;
                    var mine = !field.TryOpen(x, y);
                    Console.Clear();
                    Console.WriteLine(field);

                    if (mine)
                        return;

                    Console.SetCursorPosition(x * 3 + 1, y);
                    break;
            }
        }
    }
}
