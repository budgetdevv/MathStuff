using MathStuff.GraphTheory;

namespace MathStuff
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var graph = new Graph(vertices: [ 1, 2 ], edges: [ (1, 2) ], isDirectedGraph: false);
        }
    }
}