using Microsoft.Extensions.Caching.Memory;

internal class Program // (CoveredField state)
{
    // private CoveredField State;

    private static void Main(string[] args)
    {
        var f = new Field(8, 8);

        f.Randomize(10);

        // foreach (var row in f.Rows())
        // {
        //     var chars = row.Select(it => it ? '*' : ' ');
        //     var line = string.Join('|', chars);
        //     Console.WriteLine(line);
        // }

        var view = new FieldView(f);
        var covered = new CoveredField(view);

        Console.Clear();

        // Console.WriteLine(f);
        // Console.WriteLine();
        // Console.WriteLine(view);
        // Console.WriteLine();
        Console.WriteLine(covered);

        Console.SetCursorPosition(0, 0);

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            // Console.WriteLine(((byte)key.KeyChar));

            // Console.MoveBufferArea

            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    Console.CursorLeft++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (Console.CursorLeft > 0)
                        Console.CursorLeft--;
                    break;
                case ConsoleKey.DownArrow:
                    Console.CursorTop++;
                    break;
                case ConsoleKey.UpArrow:
                    // Console.Write(Console.CursorTop--);
                    Console.CursorTop--;
                    break;
                case ConsoleKey.Spacebar:
                    var y = Console.CursorTop;

                    if (Console.CursorLeft % 2 != 0)
                        break;

                    var x = Console.CursorLeft / 2;
                    var mine = !covered.TryOpen(x, y);
                    Console.Clear();
                    Console.WriteLine(covered);

                    if (mine)
                        return;

                    Console.SetCursorPosition(x * 2, y);
                    break;
            }
        }

        // while (true)
        // {
        //     try
        //     {
        //         if (Interact(covered))
        //             break;
        //     }
        //     catch (Exception e)
        //     {
        //         if (e is not FormatException and not ArgumentOutOfRangeException)
        //             throw;

        //         Console.WriteLine($"lox {e.Message}");
        //     }
        // }
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


// var (xs, ys) = view.Bounds;

// foreach (var y in ys)
//     foreach (var x in xs)

// var rows =
//     from y in ys
//     select
//         from x in xs
//         select view[x, y];

// foreach (var row in rows)
// {
//     // var chars = row.Select(it => it ? '*' : ' ');
//     var line = string.Join('|', row);
//     Console.WriteLine(line);
// }

abstract class Grid<T>
{
    abstract public int Height { get; }
    abstract public int Width { get; }

    abstract public T this[int x, int y] { get; }

    protected int Linearize(int x, int y) => y * Width + x;

    public bool IsValidCoordinate(int x, int y) =>
        (0 <= x && x < Width) && (0 <= y && y < Height);

    public IEnumerable<IEnumerable<T>> Rows()
    {
        return
            from y in Enumerable.Range(0, Height)
            select
                from x in Enumerable.Range(0, Width)
                select this[x, y];
    }

    // protected virtual string Repr(int x, int y) => this[x, y].ToString();

    protected virtual string Repr(T item) => item?.ToString() ?? " ";

    public override string ToString()
    {
        var lines =
            from row in Rows()
                // let fmtRow =
                //     from item in row
            select string.Join('.', row.Select(it => Repr(it)));

        return string.Join('\n', lines);
    }
}

class Field : Grid<bool>
{
    readonly bool[] mines;

    public override int Width { get; }
    public override int Height { get; }

    internal Field(int width, int height)
    {
        mines = new bool[width * height];
        Width = width;
        Height = height;
    }

    protected override string Repr(bool item) => item ? "*" : " ";

    public void Randomize(int count)
    {
        // while (count != 0)
        // {
        //     // System.Random.Shared.NextInteger();
        // }
        // mines[..count] = true;
        Array.Fill(mines, true, 0, count);
        Array.Fill(mines, false, count, Height * Width - count);
        // mines.Shuffle();

        Random.Shared.Shuffle(mines);
    }

    // public IEnumerable<IEnumerable<bool>> Rows()
    // {
    //     foreach (var y in Enumerable.Range(0, Height))
    //     {
    //         var start = y * Width;
    //         var end = start + Width;
    //         yield return mines[start..end];
    //     }
    // }

    // public Span Row(int y) => mines.AsSpan(y * width, width);

    // public ref bool this[int x, int y] => ref mines[y * _width + x];
    // public override bool this[int x, int y] => mines[y * Width + x];
    public override bool this[int x, int y] => mines[Linearize(x, y)];

    // public override char this[int x, int y] => this[x, y] ? '*' : ' ';

    // public override string ToString()
    // {
    //     return (this as IGrid<bool>).ToString();
    // }
}

class FieldView(Field _field) : Grid<byte>
{
    // readonly Field _field;

    public override int Width => _field.Width;
    public override int Height => _field.Height;

    // FieldView(Field field)
    // {
    //     _field = field;
    // }

    // bool inBounds(int x, int y) =>

    static sbyte OffMargin(int x, int length) => x switch
    {
        // 0..Width => 0
        < 0 => -1,
        var i when i >= length => 1,
        _ => 0,
    };

    sbyte OffMarginX(int x) => OffMargin(x, Width);
    sbyte OffMarginY(int y) => OffMargin(y, Height);

    (sbyte, sbyte) OffMarginXY(int x, int y) =>
        (OffMarginX(x), OffMarginY(y));

    bool InBounds(int x, int y) => OffMarginXY(x, y) is (0, 0);

    internal (IEnumerable<int> X, IEnumerable<int> Y) Bounds => (
        Enumerable.Range(0, Width),
        Enumerable.Range(0, Height)
    );

    // public IEnumerable<IEnumerable<bool>> Rows()
    // {
    //     foreach (var y in Enumerable.Range(0, Height))
    //     {
    //         // var start = y * Width;
    //         // var end = start + Width;
    //         yield return this[..Width];
    //     }
    // }

    public override byte this[int x, int y]
    {
        get
        {
            if (!InBounds(x, y))
                throw new ArgumentOutOfRangeException();

            if (_field[x, y])
                return 9;

            var mines =
                from offX in Enumerable.Range(x - 1, 3)
                from offY in Enumerable.Range(y - 1, 3)
                where InBounds(offX, offY)
                let mine = _field[offX, offY]
                where mine
                select mine;

            return (byte)mines.Count();

            // foreach (var offX in Enumerable.Range(x - 1, 3))
            //     foreach (var offY in Enumerable.Range(y - 1, 3))
            //     {

            //     }

        }
    }

    protected override string Repr(byte item) => item switch
    {
        0 => " ",
        9 => "*",
        var count => count.ToString(),
    };
}

class CoveredField(FieldView _field) : Grid<char>
{
    // bool[] open = Array.CreateInstance(typeof(bool), _field.Width * _field.Height);
    readonly bool[] open = new bool[_field.Width * _field.Height];

    public override int Height => _field.Height;
    public override int Width => _field.Width;

    public override char this[int x, int y]
    {
        get
        {
            // var linearized = y * _field.Width + x;

            if (!open[Linearize(x, y)])
                return 'O';

            // return _field[x, y].ToString()[0];
            return _field[x, y] switch
            {
                0 => ' ',
                9 => '*',
                var count => count.ToString()[0],
            };
        }

        // private new set
        // {
        //     open[Linearize(x, y)] = value
        // }
    }

    // private new this[int x, int y]
    // {
    //     set => open[Linearize(x, y)] = value;
    // }

    public bool TryOpen(int x, int y)
    {
        open[Linearize(x, y)] = true;

        switch (_field[x, y])
        {
            case 9:
                return false;
            case 0:
                var coords =
                    from nx in Enumerable.Range(x - 1, 3)
                    from ny in Enumerable.Range(y - 1, 3)
                    where IsValidCoordinate(nx, ny)
                    where !open[Linearize(nx, ny)]
                    where (nx, ny) != (x, y)
                    select (nx, ny);
                
                foreach (var (nx, ny) in coords)
                    TryOpen(nx, ny);

                return true;
        }

        return true;
    }
}

// static class Util
// {

// }
