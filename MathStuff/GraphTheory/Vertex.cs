namespace MathStuff.GraphTheory
{
    public readonly record struct Vertex(int Value)
    {
        public static implicit operator Vertex(int value) => new(value);

        public static implicit operator int(Vertex vertex) => vertex.Value;
    }
}