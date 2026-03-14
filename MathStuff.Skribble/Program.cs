using MathStuff.GraphTheory;

namespace MathStuff.Skribble
{
    internal static class Program
    {
        private static readonly Vertex VERTEX_2 = 2;

        private static void Main(string[] args)
        {
            var vertex2 = VERTEX_2;

            var graph = new Graph(
                vertices: [ 1, vertex2, 3 ],
                edges: [ (1, vertex2), (vertex2, 3) ],
                isDirectedGraph: false
            );

            var graphPrintText =
            $"""
            Graph data:
            
            {graph}

            """;

            Console.WriteLine(graphPrintText);

            Console.WriteLine($"Degree of vertex {vertex2} - {graph.GetVertexData(vertex2).Degree}");
        }
    }
}