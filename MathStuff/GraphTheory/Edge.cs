using NoParamlessCtor.Shared.Attributes;

namespace MathStuff.GraphTheory
{
    [NoParamlessCtor]
    public readonly partial record struct Edge(Vertex From, Vertex To)
    {
        public static implicit operator Edge((Vertex From, Vertex To) edge) => new(edge.From, edge.To);

        public void Deconstruct(out Vertex from, out Vertex to)
        {
            from = From;
            to = To;
        }

        public Edge Invert() => new(To, From);

        public string GetLabel(Graph graph)
        {
            var fromToText = $"{From}, {To}";

            var isDirectedGraph = graph.IsDirectedGraph;

            return !isDirectedGraph ?
                $$"""{ {{fromToText}} }""" :
                $"( {fromToText} )";
        }
    }
}