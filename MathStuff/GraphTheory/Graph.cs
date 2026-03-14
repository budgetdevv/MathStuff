using System.Collections.Frozen;
using System.Runtime.InteropServices;

namespace MathStuff.GraphTheory
{
    public sealed class Graph
    {
        private readonly Vertex[] _vertices;

        private readonly Edge[] _edges;

        internal readonly bool IsDirectedGraph;

        public readonly struct VertexData(uint[] edgeIndices)
        {
            private readonly uint[] _edgeIndices = edgeIndices;

            public uint Degree => unchecked((uint) _edgeIndices.Length);
        }

        private readonly FrozenDictionary<Vertex, VertexData> _vertexToDataMap;

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

            var vertexToDataMap = new Dictionary<Vertex, VertexData>(vertexToEdgeIndicesMap.Count);

            foreach (var (vertex, edgeIndices) in vertexToEdgeIndicesMap)
            {
                vertexToDataMap.Add(vertex, new(edgeIndices.ToArray()));
            }

            _vertexToDataMap = vertexToDataMap.ToFrozenDictionary();
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