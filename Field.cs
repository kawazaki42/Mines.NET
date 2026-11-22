class Field : Grid<bool>
{
    internal readonly bool[] mines;

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
        Array.Fill(mines, true, 0, count);
        Array.Fill(mines, false, count, Height * Width - count);

        Random.Shared.Shuffle(mines);
    }

    public override bool this[int x, int y] => mines[Linearize(x, y)];

    // internal bool AtLinear(int index) => mines[index];
}
