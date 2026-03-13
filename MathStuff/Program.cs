using MathStuff.GraphTheory;

namespace MathStuff
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var graph = new Graph(
                vertices: [ 1, 2, 3 ],
                edges: [ (1, 2), (2, 3) ],
                isDirectedGraph: false
            );

            Console.WriteLine(graph);
        }
    }
}