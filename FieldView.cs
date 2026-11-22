class FieldView(Field _field) : Grid<byte>
{
    public override int Width => _field.Width;
    public override int Height => _field.Height;

    internal bool[] MinesRaw => _field.mines;
    public Field Mines => _field;

    // static sbyte OffMargin(int x, int length) => x switch
    // {
    //     < 0 => -1,
    //     var i when i >= length => 1,
    //     _ => 0,
    // };

    // sbyte OffMarginX(int x) => OffMargin(x, Width);
    // sbyte OffMarginY(int y) => OffMargin(y, Height);

    // (sbyte, sbyte) OffMarginXY(int x, int y) =>
    //     (OffMarginX(x), OffMarginY(y));

    // bool InBounds(int x, int y) => OffMarginXY(x, y) is (0, 0);

    internal (IEnumerable<int> X, IEnumerable<int> Y) Bounds => (
        Enumerable.Range(0, Width),
        Enumerable.Range(0, Height)
    );

    public override byte this[int x, int y]
    {
        get
        {
            if (!IsValidCoordinate(x, y))
                throw new ArgumentOutOfRangeException();

            if (_field[x, y])
                return 9;

            var mines =
                from offX in Enumerable.Range(x - 1, 3)
                from offY in Enumerable.Range(y - 1, 3)
                where IsValidCoordinate(offX, offY)
                let mine = _field[offX, offY]
                where mine
                select mine;

            return (byte)mines.Count();
        }
    }

    protected override string Repr(byte item) => item switch
    {
        0 => " ",
        9 => "*",
        var count => count.ToString(),
    };
}
