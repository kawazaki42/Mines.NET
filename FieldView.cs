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
