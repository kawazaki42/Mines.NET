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

    protected override string Repr(char item) => item switch
    {
        'O' => "[]",
        var other => " " + other,
    };

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
