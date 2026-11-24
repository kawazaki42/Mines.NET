record struct Coordinate(int X, int Y);

// System.Collections.Specialized.

abstract class Grid<T>
{
    abstract public int Height { get; }
    abstract public int Width { get; }

    abstract public T this[int x, int y] { get; }

    protected int Linearize(int x, int y) => y * Width + x;

    public bool IsValidCoordinate(int x, int y) =>
        (0 <= x && x < Width) && (0 <= y && y < Height);

    public const string CellSeparator = "";

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
            let fmtRow =
                from item in row
                select Repr(item)
            // select string.Join(CellSeparator, row.Select(it => Repr(it)));
            select string.Join(CellSeparator, fmtRow);

        return string.Join('\n', lines);
    }
}
