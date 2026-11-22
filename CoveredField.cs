record Closed(bool flag);
record Open(byte mines);
// record Mine;

union CellState(Open, Closed);

static class Extension
{
    internal static void Toggle(ref this bool self) => self = !self;
}

class CoveredField(FieldView _field) : Grid<CellState>
{
    readonly bool[] open = new bool[_field.Width * _field.Height];
    public readonly bool[] flags = new bool[_field.Width * _field.Height];

    public override int Height => _field.Height;
    public override int Width => _field.Width;

    public Field Mines => _field.Mines;
    internal (int X, int Y)? GameLostPos = null;

    public static int CellWidth => 2;

    public override CellState this[int x, int y]
    {
        get
        {
            if (open[Linearize(x, y)])
                // return 'O';
                return new Open(_field[x, y]);

            return new Closed(flags[Linearize(x, y)]);

            // return _field[x, y] switch
            // {
            //     0 => ' ',
            //     9 => '*',
            //     var count => count.ToString()[0],
            // };
        }
    }

    protected override string Repr(CellState item)
    {
        var result = item switch
        {
            // 'O' => "[]",
            // var other => " " + other,
            Closed(false) => "()",
            Closed(true) => "##",
            Open(0) => " ",
            Open(9) => "*",
            Open(var mines) => mines.ToString(),
        };

        if (result.Length > CellWidth)
            return "!!";

        return result.PadLeft(CellWidth);
    }

    public bool IsClear()
    {
        var safe = _field.MinesRaw.Select(it => !it);
        return open.SequenceEqual(safe);
    }

    public void ToggleFlagAt(int x, int y)
    {
        flags[Linearize(x, y)].Toggle();
    }

    void OpenAround(int x, int y)
    {
        var coords =
            from nx in Enumerable.Range(x - 1, 3)
            from ny in Enumerable.Range(y - 1, 3)
            where IsValidCoordinate(nx, ny)
            where !open[Linearize(nx, ny)]
            where !flags[Linearize(nx, ny)]
            select (nx, ny);

        foreach (var (nx, ny) in coords)
            TryOpen(nx, ny);
    }

    public void TryOpen(int x, int y)
    {
        var already = open[Linearize(x, y)];

        if (already)
        {
            OpenAround(x, y);
            return;
        }

        open[Linearize(x, y)] = true;

        if (_field[x, y] == 0)
            OpenAround(x, y);

        if (_field[x, y] == 9)
            GameLostPos ??= (x, y);

        // switch (_field[x, y])
        // {
        //     case 9:
        //         return false;
        //     case 0:
        //         var coords =
        //             from nx in Enumerable.Range(x - 1, 3)
        //             from ny in Enumerable.Range(y - 1, 3)
        //             where IsValidCoordinate(nx, ny)
        //             where !open[Linearize(nx, ny)]
        //             select (nx, ny);

        //         foreach (var (nx, ny) in coords)
        //             TryOpen(nx, ny);

        //         return true;
        //     default:
        //         return true;
        // }
    }
}
