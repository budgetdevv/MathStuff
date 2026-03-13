namespace MathStuff.GraphTheory
{
    public sealed class Graph
    {
        private readonly Vertex[] Vertices;

        private readonly Edge[] Edges;

        internal readonly bool IsDirectedGraph;

        public Graph(Vertex[] vertices, Edge[] edges, bool isDirectedGraph)
        {
            var vertexSet = new HashSet<Vertex>();

            var edgeSet = new HashSet<Edge>();

            foreach (var vertex in vertices)
            {
                if (!vertexSet.Add(vertex))
                {
                    throw new ArgumentException($"Duplicate vertex: {vertex}");
                }
            }

            foreach (var edge in edges)
            {
                if (!vertexSet.Contains(edge.From))
                {
                    throw new ArgumentException($"Edge from vertex {edge.From} which is not in the vertex set");
                }

                if (!vertexSet.Contains(edge.To))
                {
                    throw new ArgumentException($"Edge to vertex {edge.To} which is not in the vertex set");
                }

                if (!edgeSet.Add(edge))
                {
                    throw new ArgumentException($"Duplicate edge: {edge}");
                }

                if (!isDirectedGraph)
                {
                    var reversedEdge = edge.Invert();

                    if (!edgeSet.Add(reversedEdge))
                    {
                        throw new ArgumentException($"Duplicate edge: {reversedEdge}");
                    }
                }
            }

            Vertices = vertices;

            Edges = edges;

            IsDirectedGraph = isDirectedGraph;
        }

        public override string ToString()
        {
            var verticesText = string.Join(", ", Vertices);

            var edgesText = string.Join(", ", Edges.Select(x => x.GetLabel(this)));

            return
            $$"""
            Vertices: { {{verticesText}} }
            
            Edges: { {{edgesText}} }
            
            Is Directed: {{IsDirectedGraph}}
            """;
        }
    }
}