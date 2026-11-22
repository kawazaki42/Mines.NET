class CoveredField(FieldView _field) : Grid<char>
{
    readonly bool[] open = new bool[_field.Width * _field.Height];

    public override int Height => _field.Height;
    public override int Width => _field.Width;

    public override char this[int x, int y]
    {
        get
        {
            if (!open[Linearize(x, y)])
                return 'O';

            return _field[x, y] switch
            {
                0 => ' ',
                9 => '*',
                var count => count.ToString()[0],
            };
        }
    }

    protected override string Repr(char item) => item switch
    {
        'O' => "[]",
        var other => " " + other,
    };

    public bool IsClear()
    {
        // var bad =
        //     from pair in Enumerable.Zip(_field.Mines, open)
        //     let isMine = pair.First
        //     let isOpen = pair.Second
        //     // where pair is (var isMine, var isOpen)
        //     select isMine == isOpen;

        // // Enumerable.Zip(_field.Mines, open).Where(pair =>
        // // {
        // //     var (isMine, isOpen) = pair;


        // // })

        // return bad.Any(it => it);

        // var pairs = Enumerable.Zip(_field.Mines, open)

        // foreach ((var isMine, var isOpen) in pairs)
        // {

        // }

        var safe = _field.Mines.Select(it => !it).ToArray();
        return open.SequenceEqual(safe);
        // new List<int>().SequenceEqual
    }

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
                    // where (nx, ny) != (x, y)
                    select (nx, ny);

                foreach (var (nx, ny) in coords)
                    TryOpen(nx, ny);

                return true;
        }

        return true;
    }
}
