using System.Runtime.InteropServices;
using NoParamlessCtor.Shared.Attributes;

namespace MathStuff.GraphTheory
{
    public sealed partial class Graph
    {
        private readonly Vertex[] _vertices;

        private readonly Edge[] _edges;

        private readonly VertexData[] _vertexData;

        internal readonly bool IsDirectedGraph;

        [NoParamlessCtor]
        public readonly partial struct VertexData(uint[] edgeIndices)
        {
            private readonly uint[] _edgeIndices = edgeIndices;

            public uint Degree => unchecked((uint) _edgeIndices.Length);
        }

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

            var vertexToEdgeIndicesMap = new Dictionary<Vertex, List<uint>>();

            uint currentEdgeIndex = 0;

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

                var leftVertex = edge.From;

                var rightVertex = edge.To;

                var leftVertexEdgeIndices = GetOrCreateEdgeIndices(leftVertex, vertexToEdgeIndicesMap);

                var rightVertexEdgeIndices = GetOrCreateEdgeIndices(rightVertex, vertexToEdgeIndicesMap);

                leftVertexEdgeIndices.Add(currentEdgeIndex);

                rightVertexEdgeIndices.Add(currentEdgeIndex);

                currentEdgeIndex++;

                continue;

                static List<uint> GetOrCreateEdgeIndices(Vertex vertex, Dictionary<Vertex, List<uint>> map)
                {
                    ref var slot = ref CollectionsMarshal.GetValueRefOrAddDefault(map, vertex, out var exists);

                    if (!exists)
                    {
                        slot = [];
                    }

                    return slot!;
                }
            }

            _vertices = vertices;

            _edges = edges;

            IsDirectedGraph = isDirectedGraph;

            var vertexDataList = new List<VertexData>();

            foreach (var vertex in vertices)
            {
                var edgeIndices = vertexToEdgeIndicesMap[vertex].ToArray();

                vertexDataList.Add(new(edgeIndices));
            }

            _vertexData = vertexDataList.ToArray();
        }

        public override string ToString()
        {
            var verticesText = string.Join(", ", _vertices);

            var edgesText = string.Join(", ", _edges.Select(x => x.GetLabel(this)));

            return
            $$"""
            Vertices: { {{verticesText}} }
            
            Edges: { {{edgesText}} }
            
            Is Directed: {{IsDirectedGraph}}
            """;
        }
    }
}