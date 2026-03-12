namespace MathStuff.GraphTheory
{
    public readonly record struct Edge(Vertex From, Vertex To)
    {
        public static implicit operator Edge((Vertex From, Vertex To) edge) => new(edge.From, edge.To);

        public void Deconstruct(out Vertex from, out Vertex to)
        {
            from = From;
            to = To;
        }
    }
}